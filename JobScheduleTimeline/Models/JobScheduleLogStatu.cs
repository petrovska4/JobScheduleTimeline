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
    
    public partial class JobScheduleLogStatu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public JobScheduleLogStatu()
        {
            this.JobScheduleLogs = new HashSet<JobScheduleLog>();
        }
    
        public short JobScheduleLogStatusId { get; set; }
        public string JobScheduleLogStatusCode { get; set; }
        public string Description { get; set; }
        public string DescriptionEng { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobScheduleLog> JobScheduleLogs { get; set; }
    }
}
