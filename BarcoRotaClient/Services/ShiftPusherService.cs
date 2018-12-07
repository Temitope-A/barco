using BarcoRota.Client.Models;
using BarcoRota.Models;
using BarcoRota.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace BarcoRota.Client.Services
{
    public class ShiftPusherService : BarcoRotaService
    {
        public ShiftPusherService(IServiceProvider services,ILogger<BarcoRotaService> logger, IEmailSender emailSender) : base(services, logger, emailSender)
        {
        }

        protected override TimeSpan TimeInterval => TimeSpan.FromMinutes(1);

        protected override void DoWork(object state)
        {
            using (var scope = Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BarcoContext>();
                var liveShifts = context.BarcoShifts.Where(s =>
                    s.ShiftStatus != ShiftStatus.Cancelled &&
                    s.ShiftStatus != ShiftStatus.Worked &&
                    s.ShiftStatus != ShiftStatus.UnWorked)
                    .Include(s=>s.BarcoMember);

                foreach (var shift in liveShifts.Where(s=> s.IsWaitingOnRota))
                {
                    PushShift(shift, shift.BarcoJob);
                }

                context.SaveChanges();
            }
        }

        private void PushShift(BarcoShift shift, BarcoJob job)
        {
            var dueInterval = TimeSpan.FromHours(10);
            if (shift.ShiftStatus == ShiftStatus.Planned && DateTime.Now > job.StartDateTime - dueInterval)
            {
                //notify
                NotifyDueShifts(shift);
                shift.ShiftStatus = ShiftStatus.Notified;
            }
            else if (shift.ShiftStatus == ShiftStatus.Started && job.EndDateTime < DateTime.Now)
            {
                //close it
                shift.ShiftStatus = ShiftStatus.Worked;
            }
            else if (shift.ShiftStatus == ShiftStatus.Planned && job.EndDateTime < DateTime.Now)
            {
                //fail it
                shift.ShiftStatus = ShiftStatus.UnWorked;
            }
        }

        private void NotifyDueShifts(BarcoShift shift)
        {
            Notify(shift.BarcoMember, "Barco Shift Notification", "dd");
        }
    }
}
