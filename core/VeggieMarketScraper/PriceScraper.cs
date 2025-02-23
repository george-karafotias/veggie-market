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
        private ILogger logger;

        public PriceScraper(string url, ILogger logger)
        {
            this.url = url;
            this.logger = logger;
            requestDelayInMs = 1000;
            chunkSize = 31;
            chunkAdditionalDelayInMs = 60000;
        }

        private string DownloadDay(DateTime day, string downloadFolder)
        {
            string dayUrl = ConstructDayUrl(day);
            return DownloadFile(dayUrl, downloadFolder);
        }

        public string[] DownloadPeriod(DateTime startDate, DateTime endDate, string downloadFolder)
        {
            List<string> files = new List<string>();
            int chunkIndex = 0;
            int chunkDelay = chunkAdditionalDelayInMs;
            int totalNumberOfDays = (endDate - startDate).Days + 1;
            int dayIndex = 1;

            foreach (DateTime day in EachDay(startDate, endDate))
            {
                string file = DownloadDay(day, downloadFolder);
                if (!string.IsNullOrEmpty(file))
                {
                    files.Add(file);
                }

                Thread.Sleep(requestDelayInMs);
                chunkIndex++;
                if (chunkIndex == chunkSize)
                {
                    chunkIndex = 0;
                    if (dayIndex != totalNumberOfDays)
                    {
                        Thread.Sleep(chunkDelay);
                        chunkDelay += chunkAdditionalDelayInMs;
                    }
                }

                dayIndex++;
            }

            return files.ToArray();
        }

        public string[] DownloadYear(int year, string downloadFolder)
        {
            DateTime firstDay = new DateTime(year, 1, 1);
            DateTime lastDay = new DateTime(year, 12, 31);
            return DownloadPeriod(firstDay, lastDay, downloadFolder);
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        private string DownloadFile(string url, string downloadFolder)
        {
            logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Downloading " + url, LogType.Info);

            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " +
                                  "Windows NT 5.2; .NET CLR 1.0.3705;)");
                    if (!string.IsNullOrEmpty(downloadFolder))
                    {
                        char lastCharacter = downloadFolder[downloadFolder.Length - 1];
                        if (lastCharacter != '\\')
                        {
                            downloadFolder = downloadFolder + "\\";
                        }
                    }
                    string filename = System.IO.Path.GetFileName(url);
                    string filePath = downloadFolder + filename;
                    client.DownloadFile(url, filePath);

                    logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Downloaded " + url, LogType.Info);
                    return filePath;
                }
            }
            catch (Exception ex)
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, ex.StackTrace, LogType.Exception);
                return null;
            }
        }
    }
}
