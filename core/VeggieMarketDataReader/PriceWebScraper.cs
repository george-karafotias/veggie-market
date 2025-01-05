using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeggieMarketDataReader
{
    abstract class PriceWebScraper
    {
        abstract void DownloadPriceFile(DateTime date);

        protected string DownloadFile(string url)
        {

        }
    }
}
