using System.Collections.Generic;
using System.Web.Http;
using VeggieMarketDataStore.DbServices;
using VeggieMarketDataStore.Models;

namespace VeggieMarketApi.Controllers
{
    public class ProductsController : ApiController
    {
        private readonly ProductDbService productDbService;
        private const string ROUTE_URL = "products/";

        public ProductsController()
        {
            productDbService = DbServiceProvider.GetDataStorageService().ProductDbService;
        }

        [HttpGet]
        [Route(ROUTE_URL)]
        public IEnumerable<Product> GetAllProducts()
        {
            return productDbService.GetProducts();
        }

        [HttpGet]
        [Route(ROUTE_URL + "{id}")]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = productDbService.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}
