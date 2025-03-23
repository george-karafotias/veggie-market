using System.Collections.Generic;
using System.Data.Common;
using VeggieMarketDataStore.DbInterfaces;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore.DbServices
{
    public class ProductDbService : IProductDbService
    {
        private readonly DbService dbService;

        public ProductDbService(DbService dbService)
        {
            this.dbService = dbService;
        }

        public Product GetProduct(int productId)
        {
            Product product = null;
            
            string query = "SELECT * FROM Products INNER JOIN ProductTypes ON Products.ProductType = ProductTypes.ProductTypeId WHERE ProductId = @productId";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@productId", productId }
            };
            DbConnection connection = dbService.OpenConnection();
            DbDataReader reader = dbService.Select(connection, query, parameters);
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    product = dbService.CreateProduct(reader);
                    break;
                }
            }

            dbService.CloseConnection(connection, reader);
            return product;
        }

        public Product GetProduct(string productName)
        {
            Product product = null;

            string query = "SELECT * FROM Products INNER JOIN ProductTypes ON Products.ProductType = ProductTypes.ProductTypeId WHERE ProductName = @productName";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@productName", productName }
            };
            DbConnection connection = dbService.OpenConnection();
            DbDataReader reader = dbService.Select(connection, query, parameters);
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    product = dbService.CreateProduct(reader);
                    break;
                }
            }

            dbService.CloseConnection(connection, reader);
            return product;
        }

        public IEnumerable<Product> GetProducts()
        {
            List<Product> products = new List<Product>();

            string query = "SELECT * FROM Products INNER JOIN ProductTypes ON Products.ProductType = ProductTypes.ProductTypeId ORDER BY Products.ProductName";
            DbConnection connection = dbService.OpenConnection();
            DbDataReader reader = dbService.Select(connection, query, new Dictionary<string, object>());
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    products.Add(dbService.CreateProduct(reader));
                }
            }

            dbService.CloseConnection(connection, reader);
            return products;
        }

        public bool InsertProduct(Product product)
        {
            string query = "INSERT INTO Products(ProductName, ProductType) VALUES(@productName, @productType)";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@productName", product.ProductName },
                { "@productType", product.ProductType.ProductTypeId }
            };
            return dbService.Insert(query, parameters);
        }
    }
}
