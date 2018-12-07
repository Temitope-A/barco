using System;
using System.Collections.Generic;
using System.Text;

namespace BarcoRota.Models.BarcoTimes
{
    public class BarcoWeekNightsTimes: BarcoTimesBase
    {
        protected override string OpeningTime => "20:30";
        protected override string SoundsOffTime => "23:15" ;
        protected override string TillsClosedTime => "23:15";
        protected override string ClosingTime => "23:30";

        public BarcoWeekNightsTimes(DateTime date) : base(date) { }
    }
}
