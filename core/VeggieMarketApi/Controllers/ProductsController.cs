using System.Collections.Generic;
using System.Web.Http;
using VeggieMarketDataStore;
using VeggieMarketDataStore.Models;

namespace VeggieMarketApi.Controllers
{
    public class ProductsController : ApiController
    {
        private readonly DataStorageService dataStorageService;
        private readonly ProductDbService productDbService;
        private const string ROUTE_URL = "products/"; 

        public ProductsController()
        {
            dataStorageService = DataStorageService.GetInstance(new SqliteDbService());
            productDbService = dataStorageService.ProductDbService;
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
