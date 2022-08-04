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

namespace JobScheduleTimeline.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Appointments = new ObservableCollection<Appointment>();

            dbcontext = new InternshipTaskEntities();

            JobScheduleList = new ObservableCollection<JobSchedule>(dbcontext.JobSchedules);

            SelectedItems = new List<object>();

            SearchCommand = new DelegateCommand(Search, CanSearch);

            Init();

            FrqTypeList = new List<KeyValuePair<int, string>>();

            FrqTypeList.Add(new KeyValuePair<int, string>(0, "Unknown"));
            FrqTypeList.Add(new KeyValuePair<int, string>(1, "PerDay"));
            FrqTypeList.Add(new KeyValuePair<int, string>(2, "Minutes"));
            FrqTypeList.Add(new KeyValuePair<int, string>(3, "PerWeek"));
            FrqTypeList.Add(new KeyValuePair<int, string>(4, "PerMonth"));


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

            Results = dbcontext.Database.SqlQuery<JobScheduleTimeline_Result>("EXEC [dbo].[JobScheduleTimeline] @StartDate , @EndDate , @ResourceIdsCSV", parameters).ToList();

            Appointments.Clear();
            int i = 0;
            foreach (var item in Results)
            {
                var appt = CreateAppt(ref i, item);

                Appointment apt = CreateAppointment(AppointmentType.Pattern);
                apt.Id = i++;
                apt.StartTime = item.Started;
                apt.EndTime = item.Ended;
                apt.PatientName = JobScheduleList.FirstOrDefault(js => js.JobScheduleId == item.JobScheduleId)?.Name;
                apt.DoctorId = item.JobScheduleId;
                apt.Recurrence = item.Reccurance;
                apt.FrequencyType = item.FrequencyTypeId;
                apt.FrequencyInterval = item.FrequencyInterval;

                if (appt.Recurrence == 1)
                {
                    appt.AppointmentType = AppointmentType.Pattern;

                    appt.RecurrenceInfo.Type = RecurrenceType.Minutely;
                    apt.RecurrenceInfo= new RecurrenceInfo();

                }

                Appointments.Add(appt);
            }

            RaisePropertyChanged(nameof(Results));
            RaisePropertyChanged(nameof(Appointments));
        }

        private Appointment CreateAppointment(AppointmentType pattern)
        {
            return new Appointment();
        }

        private Appointment CreateAppt(ref int i, JobScheduleTimeline_Result item)
        {
            return new Appointment()
            {
                Id = i++,
                StartTime = item.Started,
                EndTime = item.Ended,
                PatientName = JobScheduleList.FirstOrDefault(js => js.JobScheduleId == item.JobScheduleId)?.Name,
                DoctorId = item.JobScheduleId,
                Recurrence = item.Reccurance,
                FrequencyType = item.FrequencyTypeId,
                FrequencyInterval = item.FrequencyInterval
            };
        }


        public DelegateCommand SearchCommand { get; set; }
        public ObservableCollection<Appointment> Appointments { get; set; }
        public ObservableCollection<Appointment> SelectedAppointments { get; set; }
        public InternshipTaskEntities dbcontext { get; private set; }
        public ObservableCollection<JobSchedule> JobScheduleList { get; private set; }
        public List<object> SelectedItems { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public List<JobScheduleTimeline_Result> Results { get; set; }

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
        public string RecurrenceRule { get; internal set; }
        public AppointmentType AppointmentType { get; internal set; }
        public object RecurrenceInfo { get; internal set; }
    }

}
