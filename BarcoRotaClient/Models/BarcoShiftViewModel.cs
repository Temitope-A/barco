using BarcoRota.Models;
using System.ComponentModel;

namespace BarcoRota.Client.Models
{
    public class BarcoShiftViewModel
    {
        [DisplayName("Date")]
        public int Date { get; }
        [DisplayName("Month")]
        public string MonthCode { get; }
        [DisplayName("Host")]
        public BarcoMemberViewModel BarcoMember { get; }
        [DisplayName("Status")]
        public ShiftStatus ShiftStatus { get; }
        public BarcoShiftViewModel(BarcoShift shift)
        {
            ShiftStatus = shift.ShiftStatus;
            BarcoMember = new BarcoMemberViewModel(shift.BarcoMember);
        }

        public BarcoShiftViewModel(BarcoShift shift, BarcoJob job) : this(shift)
        {
            Date = job.StartDateTime.Day;
            MonthCode = job.StartDateTime.ToString("MMM");
        }
    }
}
