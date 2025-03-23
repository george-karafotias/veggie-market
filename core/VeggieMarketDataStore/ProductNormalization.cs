using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
            if (normalizedProducts == null) 
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "no normalized products found", LogType.Error);
                return productName;
            }
            productName = TrimAndCapitalizeProductName(productName);
            productName = RemoveTones(productName);

            foreach (string normalizedProduct in normalizedProducts)
            {
                string[] normalizedProductParts = normalizedProduct.Split(new char[] { ':' });
                if (normalizedProductParts == null || normalizedProductParts.Length != 2) continue;
                string productNamesString = normalizedProductParts[0];
                string normalizedProductName = normalizedProductParts[1];
                string[] productNames = productNamesString.Split(new char[] { ',' });
                if (productNames == null || productNames.Length == 0) continue;
                for (int i = 0; i < productNames.Length; i++)
                {
                    if (productName.Contains(productNames[i]))
                    {
                        logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "product " + productName + " is normalized to " + normalizedProductName, LogType.Info);
                        return normalizedProductName;
                    }
                }
            }

            return productName;
        }

        private string TrimAndCapitalizeProductName(string productName)
        {
            productName = productName.Trim();
            productName = productName.ToUpper();
            string pattern = @"\s+";
            string result = Regex.Replace(productName, pattern, " ");
            return result;
        }

        private string RemoveTones(string text)
        {
            string normalizedText = text.Normalize(NormalizationForm.FormD);
            StringBuilder resultBuilder = new StringBuilder();

            foreach (char c in normalizedText)
            {
                // Include only characters that are not diacritical marks
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    resultBuilder.Append(c);
                }
            }

            // Final result without tones
            string finalResult = resultBuilder.ToString().Normalize(NormalizationForm.FormC);
            return finalResult;
        }
    }
}
