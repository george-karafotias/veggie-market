using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using VeggieMarketDataStore;
using VeggieMarketDataStore.Models;

namespace VeggieMarketApi.Controllers
{
    public class ProductPricesController : ApiController
    {
        private readonly DataStorageService dataStorageService;
        private const string ROUTE_URL = "productPrices/";

        public ProductPricesController()
        {
            dataStorageService = DbServiceProvider.GetDataStorageService();
        }

        [HttpGet]
        [Route(ROUTE_URL + "{productId}/{fromDate}/{toDate}")]
        public IEnumerable<ProductPrice> GetProductPrices(int productId, string fromDate, string toDate)
        {
            IEnumerable<ProductPrice> productPrices = dataStorageService.ProcessedProductPriceDbService.GetProcessedProductPrices(productId, ConvertFormattedDate(fromDate), ConvertFormattedDate(toDate));
            return productPrices;
        }

        private DateTime? ConvertFormattedDate(string formattedDate)
        {
            DateTime? date = null;
            if (!string.IsNullOrEmpty(formattedDate))
            {
                date = DateTime.ParseExact(formattedDate, "ddMMyyyy", CultureInfo.InvariantCulture);
            }
            return date;
        }
    }
}
