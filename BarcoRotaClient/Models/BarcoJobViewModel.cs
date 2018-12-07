using BarcoRota.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BarcoRota.Client.Models
{
    public class BarcoJobViewModel
    {
        public int Id { get; }
        [DisplayName("Date")]
        public int Date { get; }
        [DisplayName("Month")]
        public string MonthCode { get; }
        public List<BarcoShiftViewModel> Shifts { get; }
        [DisplayName("Activity Type")]
        public JobType JobType { get; }
        [DisplayName("Filled Shifts")]
        public int NumberOfFilledShifts { get; }
        [DisplayName("Capacity")]
        public int JobCapacity { get; }
        [DisplayName("Opening Time")]
        public string OpeningTime { get; }
        [DisplayName("Closing Time")]
        public string ClosingTime { get; }

        public BarcoJobViewModel(BarcoJob job) {
            Id = job.Id;
            Date = job.StartDateTime.Day;
            MonthCode = job.StartDateTime.ToString("MMM");
            Shifts = job.Shifts.Where(s=>s.ShiftStatus != ShiftStatus.Cancelled).Select(s => new BarcoShiftViewModel(s)).ToList();
            NumberOfFilledShifts = Shifts.Count;
            JobCapacity = job.JobCapacity;
            JobType = job.JobType;
            OpeningTime = job.StartDateTime.ToString("HH:mm");
            ClosingTime = job.EndDateTime.ToString("HH:mm");
        }
    }
}
