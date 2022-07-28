using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
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
            dbcontext = new InternshipTaskEntities();

            JobScheduleList = new ObservableCollection<JobSchedule>(dbcontext.JobSchedules);

            SelectedItems = new List<object>();

            SearchCommand = new DelegateCommand(Search, CanSearch);

            Init();

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

            RaisePropertyChanged(nameof(Results));
        }

        public DelegateCommand SearchCommand { get; set; }

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

    }
}
