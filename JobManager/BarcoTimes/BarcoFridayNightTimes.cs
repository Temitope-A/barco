using System;
using System.Collections.Generic;
using System.Text;

namespace BarcoRota.Models.BarcoTimes
{
    public class BarcoFridayNightTimes : BarcoTimesBase
    {
        protected override string OpeningTime => "20:30";
        protected override string SoundsOffTime => "1.1:30";
        protected override string TillsClosedTime => "1.1:30";
        protected override string ClosingTime => "1.2:00";

        public BarcoFridayNightTimes(DateTime date) : base(date) { }
    }
}
