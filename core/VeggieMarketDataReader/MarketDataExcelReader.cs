using System;
using System.Data;
using System.Reflection;
using VeggieMarketDataStore;
using VeggieMarketDataStore.Models;
using VeggieMarketLogger;

namespace VeggieMarketDataReader
{
    public abstract class MarketDataExcelReader : MarketDataReader
    {
        protected abstract ProductPrice ExtractProductPrice(DataRow row, Product product, long productDate);
        protected abstract bool IsDateRow(int rowNumber);
        protected abstract DateTime? ExtractDate(DataRow row);
        protected abstract bool IsProductTypeRow(DataRow row);
        protected abstract bool IsProductRow(DataRow row);
        protected abstract string ExtractProductTypeName(DataRow row);
        protected abstract string ExtractProductName(DataRow row);

        private ProductNormalization productNormalization;

        public MarketDataExcelReader(VeggieMarketDataStore.DataStorageService dataStorageService) : base(dataStorageService)
        {
            this.productNormalization = new ProductNormalization(logger);
        }

        public override bool ReadSingleDay(string file)
        {
            DataTable dataTable = ExcelDataReader.ReadFile(file);
            if (dataTable == null || dataTable.Rows == null) return false;

            int rowNumber = 1;
            ProductType currentProductType = null;
            long? currentProductDate = null;

            foreach (DataRow row in dataTable.Rows)
            {
                if (IsDateRow(rowNumber))
                {
                    DateTime? date = ExtractDate(row);
                    if (!date.HasValue)
                    {
                        logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Cannot extract date", Logger.LogType.Error);
                        return false;
                    }
                    currentProductDate = date.Value.Ticks;
                }
                else
                {
                    if (IsProductTypeRow(row))
                    {
                        currentProductType = ParseProductType(ExtractProductTypeName(row));
                    }
                    else if (IsProductRow(row))
                    {
                        if (currentProductType == null)
                        {
                            logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Identified a product row but the current product type is null, so continuing..", Logger.LogType.Warning);
                        }
                        else
                        {
                            ParseProduct(row, currentProductType, currentProductDate.Value);
                        }
                    }
                }

                rowNumber++;
            }

            return true;
        }

        protected void ParseProduct(DataRow row, ProductType currentProductType, long currentProductDate)
        {
            string productName = ExtractProductName(row);
            string normalizedProductName = productNormalization.NormalizeProductName(productName);
            if (normalizedProductName != productName)
            {
                productName = normalizedProductName;
            }
            Product product = dataStorageService.ProductDbService.GetProduct(productName);

            ProductPrice productPrice = null;
            bool productPriceIsAlreadyStored = false;

            if (product == null)
            {
                product = new Product(productName, currentProductType);
                bool productInserted = dataStorageService.ProductDbService.InsertProduct(product);
                if (productInserted)
                {
                    product = dataStorageService.ProductDbService.GetProduct(productName);
                    productPrice = ExtractProductPrice(row, product, currentProductDate);
                }
            }
            else
            {
                productPriceIsAlreadyStored = dataStorageService.ProductPriceDbService.ProductHasPrice(product.ProductId, currentProductDate);
                if (!productPriceIsAlreadyStored)
                {
                    productPrice = ExtractProductPrice(row, product, currentProductDate);
                }
            }

            if (!productPriceIsAlreadyStored)
            {
                if (productPrice != null)
                {
                    bool insertedProductPrice = dataStorageService.ProductPriceDbService.InsertPrice(productPrice);
                    if (!insertedProductPrice)
                    {
                        logger.Log(
                            GetType().Name, MethodBase.GetCurrentMethod().Name,
                            "Cannot store product price for product " + product.ProductId + " and date " + currentProductDate.ToString(),
                            Logger.LogType.Error);
                    }
                }
                else
                {
                    logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Product price is null and cannot be stored", Logger.LogType.Error);
                }
            }
            else
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Product " + product.ProductId + " and date " + currentProductDate.ToString() + " is already stored", Logger.LogType.Warning);
            }
        }
    }
}
