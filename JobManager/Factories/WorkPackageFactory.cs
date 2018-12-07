using System;
using System.Collections.Generic;
using System.Linq;

namespace BarcoRota.Models.Factories
{
    public class WorkPackageFactory
    {
        public static WorkPackage CreateWorkPackage(string name, DateTime startDate, DateTime endDate, BarcoMember manager, JobType jobType,int jobCapacity) {
            var workPackage = new WorkPackage
            {
                Name = name,
                Manager = manager,
                StartDate = startDate,
                Status = WorkPackageStatus.Planned,
                Jobs = new HashSet<BarcoJob>()
            };

            for (DateTime date = startDate; date.Date <= endDate.Date; date = date.AddDays(1))
            {
                var barTimes = GetBarTimes(date);
                workPackage.Jobs.Add(new BarcoJob
                {
                    Created = DateTime.Now,
                    StartDateTime = barTimes.OpeningDateTime,
                    EndDateTime = barTimes.ClosingDateTime,
                    JobType = jobType,
                    JobCapacity = jobCapacity,
                    WorkPackage = workPackage
                });
            }

            return workPackage;
        }
        public static WorkPackageFIllResult FillWorkPackage(WorkPackage workPackage, List<BarcoMember> allMembersPool, bool useEngagedMembers)
        {
            var engagedMembers = useEngagedMembers ? new List<BarcoMember>(): workPackage.Jobs.SelectMany(j => j.Shifts).Select(s => s.BarcoMember).ToList();

            List<BarcoMember> membersPool = new List<BarcoMember>();

            foreach (var member in allMembersPool)
            {
                if (!engagedMembers.Contains(member))
                {
                    membersPool.Add(member);
                }
            }

            //shuffle
            var rand = new Random(DateTime.UtcNow.Millisecond);

            var n = membersPool.Count;

            while (n > 1)
            {
                n--;
                var k = rand.Next(n + 1);
                var member = membersPool[k];
                membersPool[k] = membersPool[n];
                membersPool[n] = member;
            }

            //assign
            var stats = new WorkPackageFIllResult(workPackage.Jobs.Count);

            foreach (var job in workPackage.Jobs)
            {
                var filledShifts = job.Shifts.Count;
                var attemptedShifts = 0;

                while (filledShifts + attemptedShifts < job.JobCapacity)
                {
                    if (membersPool.Count == 0)
                    {
                        return stats;
                    }

                    BarcoMember selected = GetMemberForJob(job, membersPool);

                    if (selected != null)
                    {
                        job.Shifts.Add(new BarcoShift
                        {
                            BarcoMember = selected,
                            BarcoJob = job,
                            ShiftStatus = ShiftStatus.Planned
                        });
                        filledShifts += 1;
                        stats.ShiftFilled();
                    }
                    else
                    {
                        attemptedShifts += 1;
                    }
                }

                stats.JobFilled();
            }

            return stats;
        }
        public static BarcoTimesBase GetBarTimes(DateTime date) {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                    return new BarcoTimes.BarcoWeekNightsTimes(date);
                case DayOfWeek.Friday:
                    return new BarcoTimes.BarcoFridayNightTimes(date);
                case DayOfWeek.Saturday:
                    return new BarcoTimes.BarcoSaturdayNightTimes(date);
                default:
                    throw new Exception("unreachable");
            }
        }

        public static BarcoMember GetMemberForJob(BarcoJob barcoJob, List<BarcoMember> membersPool)
        {
            BarcoMember selectedMember = null;
            foreach (var member in membersPool)
            {
                bool memberAlreadyInJob = false;
                foreach (var shift in barcoJob.Shifts)
                {
                    if (shift.BarcoMember.UserName == member.UserName)
                    {
                        memberAlreadyInJob = true;
                        break;
                    }
                }
                if (!memberAlreadyInJob)
                {
                    selectedMember = member;
                }
            }
            if (selectedMember != null)
            {
                membersPool.Remove(selectedMember);
            }
            return selectedMember;
        }
    }

    public class WorkPackageFIllResult
    {
        public WorkPackageFIllResult(int numberOfJobs)
        {
            UnfilledJobs = numberOfJobs;
            FilledShifts = 0;
        }
        public int UnfilledJobs { get; set; }
        public int FilledShifts { get; set; }
        public void JobFilled()
        {
            UnfilledJobs -= 1;
        }
        public void ShiftFilled()
        {
            FilledShifts += 1;
        }
    }
}
