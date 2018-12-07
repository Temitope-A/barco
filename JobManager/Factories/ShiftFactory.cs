using System;

namespace BarcoRota.Models.Factories
{
    public class ShiftFactory
    {
        public static BarcoShift AddShift(BarcoJob job, BarcoMember member) {

            if (job.Shifts.Count < job.JobCapacity)
            {
                var shift = new BarcoShift
                {
                    BarcoMember = member,
                    ShiftStatus = ShiftStatus.Planned,
                    BarcoJob = job
                };
                job.Shifts.Add(shift);

                return shift;
            }
            else
            {
                return null;
            }
        }
    }
}
