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
        [Route(ROUTE_URL)]
        public IEnumerable<ProductPriceType> GetProductPrices()
        {
            List<ProductPriceType> priceTypes = new List<ProductPriceType>();
            foreach (KeyValuePair<string, string> priceType in ProductPrice.GetPriceTypes())
            {
                priceTypes.Add(new ProductPriceType(priceType.Value, priceType.Key));
            }
            return priceTypes;
        }

        [HttpGet]
        [Route(ROUTE_URL + "{productId}/{fromDate}/{toDate}")]
        public IEnumerable<ProductPrice> GetProductPrices(int productId, string fromDate, string toDate)
        {
            IEnumerable<ProductPrice> productPrices = dataStorageService.ProcessedProductPriceDbService.GetProcessedProductPrices(productId, ConvertFormattedDate(fromDate), ConvertFormattedDate(toDate));
            return productPrices;
        }

        [HttpGet]
        [Route(ROUTE_URL + "{productId}/{marketId}/{fromDate}/{toDate}")]
        public IEnumerable<ProductPrice> GetProductPrices(int productId, int marketId, string fromDate, string toDate)
        {
            IEnumerable<ProductPrice> productPrices = dataStorageService.ProcessedProductPriceDbService.GetProcessedProductMarketPrices(productId, marketId, ConvertFormattedDate(fromDate), ConvertFormattedDate(toDate));
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
