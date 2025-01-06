using System;
using System.Collections.Generic;
using System.Data.Common;
using VeggieMarketDataStore.DbInterfaces;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataStore.DbServices
{
    public class ProductTypeDbService : IProductTypeDbService
    {
        private readonly DbService dbService;

        public ProductTypeDbService(DbService dbService)
        {
            this.dbService = dbService;
        }

        public bool InsertProductType(ProductType productType)
        {
            string query = "INSERT INTO ProductTypes(ProductTypeName) VALUES(@productTypeName)";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@productTypeName", productType.ProductTypeName }
            };
            return dbService.Insert(query, parameters);
        }

        public IEnumerable<ProductType> GetProductTypes()
        {
            List<ProductType> productTypes = new List<ProductType>();

            string sql = "SELECT * FROM ProductTypes;";
            DbConnection connection = dbService.OpenConnection();
            DbDataReader reader = dbService.Select(connection, sql, new Dictionary<string, object>());
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    productTypes.Add(CreateProductType(reader));
                }
            }

            dbService.CloseConnection(connection, reader);
            return productTypes;
        }

        public ProductType GetProductType(string productTypeName, bool partialMatch = false)
        {
            ProductType productType = null;

            string query = "SELECT * FROM ProductTypes WHERE ProductTypeName ";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            if (partialMatch)
            {
                query = query + "LIKE @productTypeName";
                parameters.Add("@productTypeName", "%" + productTypeName + "%");
            }
            else
            {
                query = query + "= @productTypeName";
                parameters.Add("@productTypeName", productTypeName);
            }
            DbConnection connection = dbService.OpenConnection();
            DbDataReader reader = dbService.Select(connection, query, parameters);
            if (reader != null && reader.HasRows)
            {
                while (reader.Read())
                {
                    productType = CreateProductType(reader);
                    break;
                }
            }

            dbService.CloseConnection(connection, reader);
            return productType;
        }

        private ProductType CreateProductType(DbDataReader reader)
        {
            int productTypeId = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ProductTypeId")));
            string productTypeName = Convert.ToString(reader.GetValue(reader.GetOrdinal("ProductTypeName")));
            ProductType productType = new ProductType(productTypeId, productTypeName);
            return productType;
        }
    }
}
