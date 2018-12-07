using BarcoRota.Client.Models;
using BarcoRota.Models;
using BarcoRota.Models.Factories;
using BarcoRota.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarcoRota.Client.Services
{
    public class WorkPackageFillerService : BarcoRotaService
    {

        public WorkPackageFillerService(IServiceProvider services, ILogger<BarcoRotaService> logger, IEmailSender emailSender) : base(services, logger, emailSender)
        {
        }

        protected override TimeSpan TimeInterval => TimeSpan.FromDays(1);

        protected override void DoWork(object state)
        {
            using (var scope = Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BarcoContext>();
                var packages = context.WorkPackages.Include(w => w.Manager);

                foreach (var package in packages)
                {
                    PushPackage(package, context);
                }

                context.SaveChanges();
            }
        }

        private void PushPackage(WorkPackage package, BarcoContext context)
        {
            if (package.Status == WorkPackageStatus.Planned)
            {
                NotifyMembersOfNewPackage(package, context);
                package.Status = WorkPackageStatus.Notified;
                Notify(package.Manager, $"Package {package.Name} was notified", "TODO");
            }
            if (package.Status != WorkPackageStatus.Filled && DateTime.Now.Date > package.StartDate - TimeSpan.FromDays(3))
            {
                FillPackage(package, context);
                package.Status = WorkPackageStatus.Filled;
                Notify(package.Manager, $"Package {package.Name} was filled", "TODO");
            }
        }

        private void NotifyMembersOfNewPackage(WorkPackage package, BarcoContext context)
        {
            var members = context.BarcoMembers.Where(m => m.RotaStatus == RotaStatus.Active).ToList();
            foreach (var member in members)
            {
                Notify(member, "New Shifts Released", "TODO");
            }
        }

        private void FillPackage(WorkPackage package, BarcoContext context)
        {
            var members = context.BarcoMembers.Where(m => m.RotaStatus == RotaStatus.Active).ToList();
            package = context.WorkPackages
                .Where(p => p.Id == package.Id)
                .Include(p => p.Jobs)
                    .ThenInclude(j=>j.Shifts)
                .Single();
            var result = WorkPackageFactory.FillWorkPackage(package, members, false);
            while (result.UnfilledJobs > 0)
            {
                result = WorkPackageFactory.FillWorkPackage(package, members, true);
            }

            //notified concerned members

            var shiftsByMember = package.Jobs.SelectMany(j => j.Shifts).GroupBy(s => s.BarcoMember);
            foreach (var group in shiftsByMember)
            {
                foreach (var item in group.OrderBy(s=>s.BarcoJob.StartDateTime))
                {
                    //
                }
                Notify(group.Key, $"Your shifts for package {package.Name}", "");
            }
        }
    }
}
