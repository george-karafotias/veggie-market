using System;
using System.Collections.Generic;
using System.Linq;

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
            int startYear = startDate.Year;
            int endYear = endDate.Year;
            List<int> allYears = GetYears(startYear, endYear);
            HashSet<int> years = new HashSet<int>();
            foreach (int year in allYears)
            {
                if (year == startYear)
                {
                    DateTime firstDayOfYear = new DateTime(year, 1, 1);
                    if (startDate == firstDayOfYear)
                    {
                        years.Add(year);
                    }
                }
                
                if (year == endYear)
                {
                    DateTime lastDayOfYear = new DateTime(year, 12, 31);
                    if (endDate == lastDayOfYear)
                    {
                        years.Add(year);
                    }
                }
                
                if (year != startYear && year != endYear)
                {
                    years.Add(year);
                }
            }

            return years.ToList();
        }

        public static List<int> GetYears(int startYear, int endYear)
        {
            List<int> years = new List<int>();
            for (int year = startYear; year <= endYear; year++)
            {
                years.Add(year);
            }
            return years;
        }
    }
}
