using System.Collections.Generic;
using System.Web.Http;
using VeggieMarketDataStore.DbServices;
using VeggieMarketDataStore.Models;

namespace VeggieMarketApi.Controllers
{
    public class MetadataController : ApiController
    {
        private readonly MetadataDbService metadataDbService;
        private const string ROUTE_URL = "meta/";

        public MetadataController()
        {
            metadataDbService = DbServiceProvider.GetDataStorageService().MetadataDbService;
        }

        [HttpGet]
        [Route(ROUTE_URL + "availablePrices")]
        public IEnumerable<MarketAvailableData> GetAvailablePrices()
        {
            return metadataDbService.GetAvailablePrices();
        }
    }
}
