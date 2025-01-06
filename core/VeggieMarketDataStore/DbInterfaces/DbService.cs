using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Reflection;
using VeggieMarketDataStore.Models;
using VeggieMarketLogger;

namespace VeggieMarketDataStore.DbInterfaces
{
    public abstract class DbService
    {
        public abstract DbConnection OpenConnection();
        public abstract DbCommand CreateCommand(string query, DbConnection connection);

        public static string MARKETS_TABLE = "Markets";
        public static string PRODUCTS_TABLE = "Products";
        public static string PRODUCT_TYPES_TABLE = "ProductTypes";
        public static string PRODUCT_PRICES_TABLE = "ProductPrices";
        public static string PROCESSED_PRODUCT_PRICES_TABLE = "ProcessedProductPrices";

        protected Logger logger;

        public DbService() 
        {
            logger = Logger.GetInstance();
        }

        public void CreateDatabase()
        {
            DbConnection connection = null;
            try
            {
                connection = OpenConnection();
                string[] queries = File.ReadAllLines("veggiedb.txt");
                foreach (string query in queries)
                {
                    var command = CreateCommand(query, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, ex.StackTrace, Logger.LogType.Exception);
            }
            finally
            {
                connection?.Close();
            }
        }

        public bool Insert(string query, Dictionary<string, object> parameters)
        {
            bool inserted = false;
            DbConnection connection = null;

            try
            {
                connection = OpenConnection();
                var command = CreateCommand(query, connection);
                command = AppendCommandParameters(command, parameters);
                int rowsAffected = command.ExecuteNonQuery();
                inserted = rowsAffected > 0;
            }
            catch (Exception ex)
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, ex.StackTrace, Logger.LogType.Exception);
            }
            finally
            {
                CloseConnection(connection, null);
            }

            return inserted;
        }

        public DbDataReader Select(DbConnection connection, string query, Dictionary<string, object> parameters = null)
        {
            DbDataReader reader = null;

            try
            {
                var command = CreateCommand(query, connection);
                if (parameters != null && parameters.Count > 0)
                {
                    command = AppendCommandParameters(command, parameters);
                }
                reader = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, ex.StackTrace, Logger.LogType.Exception);
            }

            return reader;
        }

        public void CloseConnection(DbConnection connection, DbDataReader reader)
        {
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
            }
            connection.Close();
        }

        public Market CreateMarket(DbDataReader reader)
        {
            int marketId = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("MarketId")));
            string marketName = Convert.ToString(reader.GetValue(reader.GetOrdinal("MarketName")));
            Market market = new Market(marketId, marketName);
            return market;
        }

        public Product CreateProduct(DbDataReader reader)
        {
            int productId = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ProductId")));
            string productName = Convert.ToString(reader.GetValue(reader.GetOrdinal("ProductName")));
            string productTypeName = Convert.ToString(reader.GetValue(reader.GetOrdinal("ProductTypeName")));
            int productTypeId = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ProductTypeId")));

            ProductType productType = new ProductType(productTypeId, productTypeName);
            Product product = new Product(productId, productName, productType);
            return product;
        }

        public double? ConvertDbValueToNullableDouble(object value)
        {
            double? retVal = null;
            string stringValue = Convert.ToString(value);
            if (!string.IsNullOrEmpty(stringValue))
            {
                retVal = Convert.ToDouble(stringValue);
            }
            return retVal;
        }

        public string MergeFilters(List<string> filters)
        {
            string retVal = "";
            List<string> nonEmptyFilters = new List<string>();
            for (int i = 0; i < filters.Count; i++)
            {
                if (!string.IsNullOrEmpty(filters[i]) && filters[i].Trim().Length > 0)
                {
                    nonEmptyFilters.Add(filters[i]);
                }
            }
            for (int i = 0; i < nonEmptyFilters.Count; i++)
            {
                retVal += (i != nonEmptyFilters.Count - 1) ? nonEmptyFilters[i] + " AND " : nonEmptyFilters[i];
            }
            return retVal;
        }

        private DbCommand AppendCommandParameters(DbCommand command, Dictionary<string, object> parameters)
        {
            foreach (var parameter in parameters)
            {
                var commandParameter = command.CreateParameter();
                commandParameter.ParameterName = parameter.Key;
                commandParameter.Value = parameter.Value;
                command.Parameters.Add(commandParameter);
            }
            return command;
        }
    }
}
