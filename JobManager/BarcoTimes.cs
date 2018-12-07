using System;
using System.Collections.Generic;
using System.Text;

namespace BarcoRota.Models
{
    public abstract class BarcoTimesBase
    {
        protected abstract string OpeningTime { get; }
        protected abstract string TillsClosedTime { get; }
        protected abstract string SoundsOffTime { get; }
        protected abstract string ClosingTime { get; }

        public readonly DateTime OpeningDateTime;
        public readonly DateTime TillsClosedDateTime;
        public readonly DateTime SoundsOffDateTime;
        public readonly DateTime ClosingDateTime;

        public BarcoTimesBase(DateTime date)
        {
            OpeningDateTime = date + TimeSpan.Parse(OpeningTime);
            SoundsOffDateTime = date + TimeSpan.Parse(SoundsOffTime);
            TillsClosedDateTime = date + TimeSpan.Parse(TillsClosedTime);
            ClosingDateTime = date + TimeSpan.Parse(ClosingTime);
        }
    }
}
