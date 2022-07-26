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
    using System.Collections.Generic;
    
    public partial class JobSchedule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public JobSchedule()
        {
            this.JobScheduleStamps = new HashSet<JobScheduleStamp>();
            this.JobScheduleDependencies = new HashSet<JobScheduleDependency>();
            this.JobScheduleDependencies1 = new HashSet<JobScheduleDependency>();
            this.JobScheduleLogs = new HashSet<JobScheduleLog>();
            this.JobScheduleParameters = new HashSet<JobScheduleParameter>();
        }
    
        public int JobScheduleId { get; set; }
        public int HostId { get; set; }
        public System.DateTime DateFrom { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }
        public Nullable<System.DateTime> TimeFrom { get; set; }
        public Nullable<System.DateTime> TimeTo { get; set; }
        public Nullable<int> FrequencyTypeId { get; set; }
        public Nullable<int> FrequencyInterval { get; set; }
        public Nullable<System.DateTime> LastRun { get; set; }
        public Nullable<int> DateId { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string NameEng { get; set; }
        public int JobScheduleMethodId { get; set; }
        public Nullable<short> JobScheduleChainId { get; set; }
        public Nullable<short> Ordinal { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobScheduleStamp> JobScheduleStamps { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobScheduleDependency> JobScheduleDependencies { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobScheduleDependency> JobScheduleDependencies1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobScheduleLog> JobScheduleLogs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobScheduleParameter> JobScheduleParameters { get; set; }
        public virtual JobScheduleChain JobScheduleChain { get; set; }
        public virtual JobScheduleMethod JobScheduleMethod { get; set; }
    }
}
