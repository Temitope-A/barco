using System;
using System.Collections.Generic;
using System.Text;

namespace BarcoRota.Models.BarcoTimes
{
    public class BarcoSaturdayNightTimes : BarcoTimesBase
    {
        protected override string OpeningTime => "20:30";
        protected override string SoundsOffTime => "1.0:30";
        protected override string TillsClosedTime => "1.0:30";
        protected override string ClosingTime => "1.1:00";

        public BarcoSaturdayNightTimes(DateTime date) : base(date) { }
    }
}
