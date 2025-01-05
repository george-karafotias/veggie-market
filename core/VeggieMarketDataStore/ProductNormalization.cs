using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VeggieMarketLogger;

namespace VeggieMarketDataStore
{
    public class ProductNormalization
    {
        private Logger logger;
        private string[] normalizedProducts;

        public ProductNormalization(VeggieMarketLogger.Logger logger)
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
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, ex.StackTrace, Logger.LogType.Exception);
            }
        }

        public string NormalizeProductName(string productName)
        {
            if (normalizedProducts == null) return productName;

            /*foreach (string normalizedProduct in normalizedProducts)
            {
                string[] normalizedProductParts = normalizedProduct.Split(new char[] { ':' });
                List<string> normalizedProductNames = new List<string>();
                for (int i = 0; i < normalizedProductParts.Length - 2; i++)
                {
                    normalizedProductNames.Add(normalizedProductParts[i]);
                }
                string normalizedProductName = normalizedProductParts[normalizedProductParts.Length - 2];
                string matchMode = normalizedProductParts[normalizedProductParts.Length - 1];

                if (matchMode == "ExactMatch")
                {
                    if (normalizedProductNames.Contains(productName))
                    {
                        return normalizedProductName;
                    }
                }
                else if (matchMode == "Contains")
                {
                    for (int i = 0; i < normalizedProductNames.Count; i++)
                    {
                        if (productName.Contains(normalizedProductNames[i]))
                        {
                            return normalizedProductName;
                        }
                    }
                }
            }*/

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
