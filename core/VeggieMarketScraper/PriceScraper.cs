using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using VeggieMarketLogger;

namespace VeggieMarketScraper
{
    public abstract class PriceScraper
    {
        protected abstract string ConstructDayUrl(DateTime day);

        private int requestDelayInMs;
        private int chunkSize;
        private int chunkAdditionalDelayInMs;
        private string url;
        private Logger logger;

        public PriceScraper(string url)
        {
            this.url = url;
            logger = Logger.GetInstance();
            requestDelayInMs = 1000;
            chunkSize = 30;
            chunkAdditionalDelayInMs = 60000;
        }

        public string DownloadDay(DateTime day)
        {
            string dayUrl = ConstructDayUrl(day);
            return DownloadFile(dayUrl);
        }

        public string[] DownloadPeriod(DateTime startDate, DateTime endDate)
        {
            List<string> files = new List<string>();
            int chunkIndex = 0;
            int chunkDelay = chunkAdditionalDelayInMs;

            foreach (DateTime day in EachDay(startDate, endDate))
            {
                string file = DownloadDay(day);
                if (!string.IsNullOrEmpty(file))
                {
                    files.Add(file);
                }

                Thread.Sleep(requestDelayInMs);
                chunkIndex++;
                if (chunkIndex == chunkSize)
                {
                    chunkIndex = 0;
                    Thread.Sleep(chunkDelay);
                    chunkDelay += chunkAdditionalDelayInMs;
                }
            }

            return files.ToArray();
        }

        public string[] DownloadYear(int year)
        {
            DateTime firstDay = new DateTime(year, 1, 1);
            DateTime lastDay = new DateTime(year, 12, 31);
            return DownloadPeriod(firstDay, lastDay);
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        private string DownloadFile(string url)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " +
                                  "Windows NT 5.2; .NET CLR 1.0.3705;)");
                    string filename = System.IO.Path.GetFileName(url);
                    client.DownloadFile(url, filename);
                    return filename;
                }
            }
            catch (Exception ex)
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, ex.StackTrace, Logger.LogType.Exception);
                return null;
            }
        }
    }
}
