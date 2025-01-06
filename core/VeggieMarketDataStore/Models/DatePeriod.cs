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

        public DatePeriod(DateTime date)
        {
            this.startDate = date;
            this.endDate = date;
        }

        public string StartDate { get { return FormatDate(startDate); } }
        public string EndDate { get { return FormatDate(endDate); } }
    
        private string FormatDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }
    }
}
