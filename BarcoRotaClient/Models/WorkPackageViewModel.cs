using BarcoRota.Models;
using System;
using System.ComponentModel;
using System.Linq;

namespace BarcoRota.Client.Models
{
    public class WorkPackageViewModel
    {
        public int Id { get; }
        public string Name { get; }
        public BarcoMemberViewModel Manager { get; }
        [DisplayName("Total Activities")]
        public int JobsTotal { get; }
        [DisplayName("Unworked Activities")]
        public int JobsFailed { get; }
        [DisplayName("Outstanding Activities")]
        public int JobsOutstanding { get; }

        public WorkPackageViewModel(WorkPackage workPackage)
        {
            Id = workPackage.Id;
            Name = workPackage.Name;
            Manager = new BarcoMemberViewModel(workPackage.Manager);
            JobsTotal = workPackage.Jobs.Count;
            JobsOutstanding = workPackage.Jobs.Count(j => j.EndDateTime > DateTime.Now);
            JobsFailed = workPackage.Jobs
                .Count(j => 
                    j.EndDateTime < DateTime.Now &&
                    j.Shifts.Count(s=>s.ShiftStatus == ShiftStatus.Worked) == 0
                );
        }
    }
}
