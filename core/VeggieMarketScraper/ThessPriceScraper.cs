using System;

namespace VeggieMarketScraper
{
    public class ThessPriceScraper : PriceScraper
    {
        private const string BASE_URL = "https://www.kath.gr/uploadimages/";

        public ThessPriceScraper() : base(BASE_URL) 
        { 
        }

        protected override string ConstructDayUrl(DateTime day)
        {
            string folder = day.ToString("yyyy-MM-dd");
            string file = day.ToString("dd-MM-yy");
            return BASE_URL + folder + @"/" + file + ".xls";
        }
    }
}
