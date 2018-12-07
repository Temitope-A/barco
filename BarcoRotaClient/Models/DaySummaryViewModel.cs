using BarcoRota.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BarcoRota.Client.Models
{
    public class DaySummaryViewModel
    {
        [DisplayName("Date")]
        public int Date { get; }
        [DisplayName("Month")]
        public string MonthCode { get; }
        public bool IsOpen { get; }
        [DisplayName("Activity Type")]
        public JobType JobType { get; }
        public List<BarcoShiftViewModel> Shifts { get; }

        public DaySummaryViewModel(DateTime date, BarcoJob job)
        {
            Date = date.Day;
            MonthCode = date.ToString("MMM");

            if (job == null)
            {
                IsOpen = false;
            }
            else
            {
                IsOpen = true;
                JobType = job.JobType;
                Shifts = job.Shifts.Select(s=>new BarcoShiftViewModel(s)).ToList();
            }
        }
    }
}
