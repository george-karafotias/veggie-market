using System;
using System.Reflection;
using VeggieMarketLogger;

namespace VeggieMarketDataStore
{
    public class ProductNormalization
    {
        private ILogger logger;
        private string[] normalizedProducts;

        public ProductNormalization(ILogger logger)
        {
            this.logger = logger;
            ReadNormalizedProducts();
        }

        public void ReadNormalizedProducts()
        {
            try
            {
                normalizedProducts = System.IO.File.ReadAllLines(@"products-normalizations.txt");
            }
            catch (Exception ex)
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, ex.StackTrace, LogType.Exception);
            }
        }

        public string NormalizeProductName(string productName)
        {
            if (normalizedProducts == null) return productName;

            foreach (string normalizedProduct in normalizedProducts)
            {
                string[] normalizedProductParts = normalizedProduct.Split(new char[] { ':' });
                if (normalizedProductParts == null || normalizedProductParts.Length != 2) continue;
                string allProductNamesString = normalizedProductParts[0];
                string normalizedProductName = normalizedProductParts[1];
                string[] allProductNames = allProductNamesString.Split(new char[] { ',' });
                if (allProductNames == null || allProductNames.Length == 0) continue;
                foreach (string oneProductName in allProductNames)
                {
                    if (productName.Contains(oneProductName))
                    {
                        return normalizedProductName;
                    }
                }
            }

            return productName;
        }
    }
}
