using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Scheduling;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using JobScheduleTimeline.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm.POCO;
using System.Windows.Media;

namespace JobScheduleTimeline.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Appointments = new ObservableCollection<Appointment>();

            dbcontext = new InternshipTaskEntities();

            JobScheduleList = new ObservableCollection<JobSchedule>(dbcontext.JobSchedules);

            //JobScheduleLogStatusList = new ObservableCollection<JobSchedule>((IEnumerable<JobSchedule>)dbcontext.JobScheduleLogStatus);

            LogStatus = new ObservableCollection<LogState>();

            SelectedItems = new List<object>();

            SearchCommand = new DelegateCommand(Search, CanSearch);

            Init();

            FrqTypeList = new List<KeyValuePair<int, string>>();

            FrqTypeList.Add(new KeyValuePair<int, string>(0, "Unknown"));
            FrqTypeList.Add(new KeyValuePair<int, string>(1, "PerDay"));
            FrqTypeList.Add(new KeyValuePair<int, string>(2, "Minutes"));
            FrqTypeList.Add(new KeyValuePair<int, string>(3, "PerWeek"));
            FrqTypeList.Add(new KeyValuePair<int, string>(4, "PerMonth"));

            LogStatus = CreateStatuses();

        }

        public class LogState
        {
            public static LogState Create()
            {
                return ViewModelSource.Create(() => new LogState());
            }

            protected LogState() { }
            public virtual int Id { get; set; }
            public virtual string Caption { get; set; }
            public virtual Brush Brush { get; set; }
        }

        public static string[] Description = {"", "Started", "Successful", "Warning", "Failed", "Acknowledged" };
        public static Brush[] BrushStates = { new SolidColorBrush(Colors.Blue),new SolidColorBrush(Colors.Blue), new SolidColorBrush(Colors.Green), new SolidColorBrush(Colors.Yellow), new SolidColorBrush(Colors.Red), new SolidColorBrush(Colors.Pink) };

        ObservableCollection<LogState> CreateStatuses()
        {
            ObservableCollection<LogState> result = new ObservableCollection<LogState>();

            for (int i = 1; i < 6; i++)
            {
                LogState logState = LogState.Create();
                logState.Id = i;
                logState.Brush = BrushStates[i];
                logState.Caption = Description[i];
                result.Add(logState);
            }
            return result;
        }

        private void SelectedItemss_ListChanged(object sender, ListChangedEventArgs e)
        {
            SearchCommand.RaiseCanExecuteChanged();
        }

        private bool CanSearch()
        {
            return true;
        }

        private void Search()
        {
            SqlParameter StartParam = new SqlParameter("StartDate", StartDateTime);
            SqlParameter EndParam = new SqlParameter("EndDate", EndDateTime);
            SqlParameter ResourceIdsParam = new SqlParameter("ResourceIdsCSV", (SelectedItems==null)? null : string.Join(",", SelectedItems));
            object[] parameters = new object[] { StartParam, EndParam, ResourceIdsParam };

            Results = dbcontext.Database.SqlQuery<JobScheduleTimeline1_Result>("EXEC [dbo].[JobScheduleTimeline] @StartDate , @EndDate , @ResourceIdsCSV", parameters).ToList();

            Appointments.Clear();
            
            foreach (var item in Results)
            {

                if (!item.FrequencyInterval.HasValue && item.Reccurance==1 && !item.TimeFrom.HasValue)
                    continue;

                Appointment pattern = new Appointment()
                {
                    StartTime = item.Started,
                    EndTime = item.Ended,
                    TimeFrom = item.TimeFrom,
                    TimeTo = item.TimeTo,
                    PatientName = JobScheduleList.FirstOrDefault(js => js.JobScheduleId == item.JobScheduleId)?.Name,
                    DoctorId = item.JobScheduleId,
                    Recurrence = item.Reccurance,
                    FrequencyType = item.FrequencyTypeId,
                    FrequencyInterval = item.FrequencyInterval,
                    JobScheduleLogStatusId = item.JobScheduleLogStatusId
                };

                if(pattern.Recurrence == 0)
                {
                    if (!item.TimeFrom.HasValue)
                        continue;

                    DateTime StartDate = new DateTime(pattern.StartTime.Year, pattern.StartTime.Month, pattern.StartTime.Day, pattern.TimeFrom.Value.Hour, pattern.TimeFrom.Value.Minute, pattern.TimeFrom.Value.Second);
                    pattern.StartTime = StartDate;

                }

                if (pattern.Recurrence == 1)
                {
                    if (!item.TimeFrom.HasValue)
                        continue;

                    DateTime StartDate = new DateTime(pattern.StartTime.Year,pattern.StartTime.Month,pattern.StartTime.Day, pattern.TimeFrom.Value.Hour, pattern.TimeFrom.Value.Minute, pattern.TimeFrom.Value.Second);

                    pattern.EndTime = StartDate.AddMinutes(30);

                    pattern.AppointmentType = AppointmentType.Pattern;
                    
                    if (pattern.FrequencyType == 0)
                    {
                        //Frequency unknown
                        continue;
                    }
                    if (pattern.FrequencyType == 1)
                    {
                        //Frequency perday
                        if(StartDate.Hour < DateTime.Now.Hour)
                        {
                            RecurrenceInfo info = (RecurrenceInfo)RecurrenceBuilder.Daily(StartDate.AddDays(1)).Build();
                            pattern.RecurrenceInfo = info.ToXml();
                        }
                        else
                        {
                            RecurrenceInfo info = (RecurrenceInfo)RecurrenceBuilder.Daily(StartDate).Build();
                            pattern.RecurrenceInfo = info.ToXml();
                        }                        
                    }
                    if (pattern.FrequencyType == 2)
                    {
                        //Frequency minutes

                        if(StartDate< DateTime.Now)
                        {
                            double pom1 = (double)((int)((DateTime.Now - StartDate).TotalMinutes / pattern.FrequencyInterval) + 1);

                            DateTime pom = StartDate.AddMinutes((double)pattern.FrequencyInterval *pom1);

                            RecurrenceInfo info = (RecurrenceInfo)RecurrenceBuilder.Minutely(pom).Build();
                            info.Periodicity = (int)pattern.FrequencyInterval;
                            pattern.RecurrenceInfo = info.ToXml();

                        }
                        else
                        {
                            RecurrenceInfo info = (RecurrenceInfo)RecurrenceBuilder.Minutely(StartDate).Build();
                            info.Periodicity = (int)pattern.FrequencyInterval;
                            pattern.RecurrenceInfo = info.ToXml();
                        }


                    }
                    if (pattern.FrequencyType == 3)
                    {
                        //Frequency perweek
                        if(StartDate.Hour < DateTime.Now.Hour)
                        {
                            RecurrenceInfo info = (RecurrenceInfo)RecurrenceBuilder.Weekly(StartDate.AddDays(7), (int)pattern.FrequencyInterval).Build();
                            pattern.RecurrenceInfo = info.ToXml();
                        }
                        else
                        {
                            RecurrenceInfo info = (RecurrenceInfo)RecurrenceBuilder.Weekly(StartDate, (int)pattern.FrequencyInterval).Build();
                            pattern.RecurrenceInfo = info.ToXml();
                        }
                        
                    }
                    if (pattern.FrequencyType == 4)
                    {
                        //Frequency permonth
                        if (StartDate.Hour < DateTime.Now.Hour)
                        {
                            RecurrenceInfo info = (RecurrenceInfo)RecurrenceBuilder.Monthly(StartDate.AddMonths(1), (int)pattern.FrequencyInterval).Build();
                            pattern.RecurrenceInfo = info.ToXml();
                        }
                        else
                        {
                            RecurrenceInfo info = (RecurrenceInfo)RecurrenceBuilder.Monthly(StartDate, (int)pattern.FrequencyInterval).Build();
                            pattern.RecurrenceInfo = info.ToXml();
                        }
                    }
                }

                Appointments.Add(pattern);
            }

            RaisePropertyChanged(nameof(Results));
            RaisePropertyChanged(nameof(Appointments));
        }

        public DelegateCommand SearchCommand { get; set; }
        public ObservableCollection<Appointment> Appointments { get; set; }
        public ObservableCollection<Appointment> SelectedAppointments { get; set; }
        public InternshipTaskEntities dbcontext { get; private set; }
        public ObservableCollection<JobSchedule> JobScheduleList { get; private set; }
        public ObservableCollection<JobSchedule> JobScheduleLogStatusList { get; private set; }
        public ObservableCollection<LogState> LogStatus { get; private set; }
        public List<object> SelectedItems { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public List<JobScheduleTimeline1_Result> Results { get; set; }

        public void Init()
        {
            StartDateTime = DateTime.Now.AddDays(-1);
            EndDateTime = DateTime.Now.AddDays(1);
        }

        public List<KeyValuePair<int,string>>FrqTypeList { get; set; }
        
    }

    public class KeyValuePair
    {
    }

    public class Appointment {
        public virtual int Id { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime? EndTime { get; set; }
        public virtual string PatientName { get; set; }
        public virtual int? DoctorId { get; set; }
        public virtual int Recurrence { get; set; }
        public int? FrequencyType { get; set; }
        public int? FrequencyInterval { get; set; }
        public AppointmentType AppointmentType { get; internal set; }
        public string RecurrenceInfo { get; internal set; }
        public DateTime PatternTime { get; internal set; }
        public DateTime? TimeFrom { get; internal set; }
        public DateTime? TimeTo { get; internal set; }
        public int? JobScheduleLogStatusId { get; internal set; }
    }

}
