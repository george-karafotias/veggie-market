using System;

namespace VeggieMarketScraper
{
    public class RenthPriceScraper : PriceScraper
    {
        private const string BASE_URL = "https://www.okaa.gr/files/1/%CE%9F%CE%9A%CE%91%CE%91/%CE%A4%CE%99%CE%9C%CE%95%CE%A3%20%CE%A7%CE%9F%CE%9D%CE%94%CE%A1%CE%99%CE%9A%CE%97%CE%A3/";

        public RenthPriceScraper() : base(BASE_URL)
        {
        }

        protected override string ConstructDayUrl(DateTime day)
        {
            return BASE_URL + day.ToString("yyyyMMdd") + ".xls";
        }
    }
}
