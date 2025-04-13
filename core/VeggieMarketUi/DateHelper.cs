using System;
using System.Collections.Generic;

namespace VeggieMarketUi
{
    public static class DateHelper
    {
        public static List<int> CalculateFullYearsList(DateTime? start, DateTime? end)
        {
            List<int> fullYearsList = new List<int>();
            if (start == null || !start.HasValue || end == null || !end.HasValue) return fullYearsList;

            DateTime startDate = start.Value;
            DateTime endDate = end.Value;
            int years = endDate.Year - startDate.Year;
            if (endDate.Month < startDate.Month || (endDate.Month == startDate.Month && endDate.Day < startDate.Day))
            {
                years--;
            }

            fullYearsList.Add(years);

            return fullYearsList;
        }
    }
}
