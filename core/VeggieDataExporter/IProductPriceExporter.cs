using System.Collections.Generic;
using VeggieMarketDataStore.Models;

namespace VeggieDataExporter
{
    public interface IProductPriceExporter
    {
        bool ExportProductPrices(string fileName, IEnumerable<ProductPrice> productPrices, IEnumerable<string> priceTypes);
    }
}
