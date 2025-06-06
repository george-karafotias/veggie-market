using System.Collections.Generic;
using System.Web.Http;
using VeggieMarketDataStore.DbServices;
using VeggieMarketDataStore.Models;

namespace VeggieMarketApi.Controllers
{
    public class MarketsController : ApiController
    {
        private readonly MarketDbService marketDbService;
        private const string ROUTE_URL = "markets/";

        public MarketsController()
        {
            marketDbService = DbServiceProvider.GetDataStorageService().MarketDbService;
        }

        [HttpGet]
        [Route(ROUTE_URL)]
        public IEnumerable<Market> GetAllMarkets()
        {
            return marketDbService.GetMarkets();
        }

        [HttpGet]
        [Route(ROUTE_URL + "{marketName}")]
        public IHttpActionResult GetMarket(string marketName)
        {
            Market market = marketDbService.GetMarket(marketName);
            if (market == null)
            {
                return NotFound();
            }
            return Ok(market);
        }
    }
}
