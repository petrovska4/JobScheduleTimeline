//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JobScheduleTimeline.Models
{
    using System;
    
    public partial class JobScheduleTimeline1_Result
    {
        public int JobScheduleId { get; set; }
        public System.DateTime Started { get; set; }
        public Nullable<System.DateTime> Ended { get; set; }
        public Nullable<System.DateTime> TimeFrom { get; set; }
        public Nullable<System.DateTime> TimeTo { get; set; }
        public int Reccurance { get; set; }
        public Nullable<int> FrequencyTypeId { get; set; }
        public Nullable<int> FrequencyInterval { get; set; }
        public string OccuranceType { get; set; }
        public Nullable<int> JobScheduleLogStatusId { get; set; }
    }
}
