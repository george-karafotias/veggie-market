using System;

namespace VeggieMarketDataStore.Models
{
    public class DatePeriod
    {
        private DateTime startDate;
        private DateTime endDate;

        public DatePeriod(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public DateTime StartDate { get { return startDate; } }
        public DateTime EndDate { get { return endDate; } }
    }
}
