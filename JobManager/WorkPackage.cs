using System;
using System.Collections.Generic;

namespace BarcoRota.Models
{
    public class WorkPackage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public BarcoMember Manager { get; set; }
        public DateTime StartDate { get; set; }
        public WorkPackageStatus Status { get; set; }
        public virtual ICollection<BarcoJob> Jobs { get; set; }
    }
    public enum WorkPackageStatus
    {
        Planned = 0,
        Notified = 1,
        Filled = 2
    }
}
