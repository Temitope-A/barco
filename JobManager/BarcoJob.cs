using System;
using System.Collections.Generic;

namespace BarcoRota.Models
{
    public class BarcoJob
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public JobType JobType { get; set; }
        public int JobCapacity { get; set; }
        public int WorkPackageId { get; set; }
        public WorkPackage WorkPackage { get; set; }
        public virtual ICollection<BarcoShift> Shifts { get; set; }
    }

    public enum JobType
    {
        Standard = 0,
        FormalNight = 1,
        InternalEvent = 2,
        ExternalEvent = 3,
        Bop = 4
    }
}
