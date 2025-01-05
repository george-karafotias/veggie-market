using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VeggieMarketDataStore;
using VeggieMarketDataStore.Models;

namespace VeggieMarketApi.Controllers
{
    public class ProductTypesController : ApiController
    {
        private readonly DataStorageService dataStorageService;

        public ProductTypesController()
        {
            dataStorageService = DataStorageService.GetInstance(new SqliteDbService());
        }

        public IEnumerable<ProductType> GetAllProductTypes()
        {
            return dataStorageService.ProductTypeDbService.GetProductTypes();
        }
    }
}
