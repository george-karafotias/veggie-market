using System.Collections.Generic;
using System.Web.Http;
using VeggieMarketDataStore.DbServices;
using VeggieMarketDataStore.Models;

namespace VeggieMarketApi.Controllers
{
    public class ProductTypesController : ApiController
    {
        private readonly ProductTypeDbService productTypeDbService;

        public ProductTypesController()
        {
            productTypeDbService = DbServiceProvider.GetDataStorageService().ProductTypeDbService;
        }

        public IEnumerable<ProductType> GetAllProductTypes()
        {
            return productTypeDbService.GetProductTypes();
        }
    }
}
