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
    
    public partial class JobScheduleLogItem
    {
        public int JobScheduleLogItemId { get; set; }
        public int JobScheduleLogId { get; set; }
        public string Info { get; set; }
        public short LogSeverityId { get; set; }
        public System.DateTime Created { get; set; }
    
        public virtual JobScheduleLog JobScheduleLog { get; set; }
    }
}
