namespace BarcoRota.Models
{
    public class BarcoShift
    {
        public int Id { get; set; }
        public BarcoJob BarcoJob { get; set; }
        public BarcoMember BarcoMember { get; set; }
        public ShiftStatus ShiftStatus { get; set; }

        public bool IsWaitingOnRota
        {
            get
            {
                switch (ShiftStatus)
                {
                    case ShiftStatus.Notified:
                    case ShiftStatus.Worked:
                    case ShiftStatus.Cancelled:
                        return false;
                    case ShiftStatus.Planned:
                    case ShiftStatus.Started:
                        return true;
                    default:
                        throw new System.Exception("Unreachable");
                }
            } }
    }

    public enum ShiftStatus
    {
        Planned = 0,
        Notified =1,
        Cancelled = 2,
        Started = 3,
        Worked = 4,
        UnWorked = 5
    }
}
