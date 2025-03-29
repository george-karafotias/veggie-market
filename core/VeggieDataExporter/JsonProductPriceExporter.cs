using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using VeggieMarketDataStore.Models;
using VeggieMarketLogger;

namespace VeggieDataExporter
{
    public class JsonProductPriceExporter : IProductPriceExporter
    {
        private ILogger logger;

        public JsonProductPriceExporter(ILogger logger)
        {
            this.logger = logger;
        }

        public void ExportProductPrices(IEnumerable<ProductPrice> productPrices, IEnumerable<string> priceTypes)
        {
            if (productPrices == null)
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "No product prices to export", LogType.Error);
                return;
            }

            Dictionary<int, List<ProductPrice>> productPricesPerYear = ProductPriceGrouping.GroupByYear(productPrices);

            List<string> selectedPriceTypes = GetSelectedPriceTypes(priceTypes);
            if (selectedPriceTypes.Count == 0)
            {
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "The selected price types are not valid", LogType.Error);
                return;
            }

            StringBuilder output = new StringBuilder();
            output.Append("{");
            int numberOfYears = productPricesPerYear.Count;
            int yearIndex = 0;

            foreach (KeyValuePair<int, List<ProductPrice>> productPriceYearEntry in productPricesPerYear)
            {
                int year = productPriceYearEntry.Key;
                logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, "Exporting year " + year, LogType.Info);
                output.Append(CreateJsonKey(year.ToString()));
                output.Append("[");

                List<ProductPrice> yearPrices = productPriceYearEntry.Value;
                for (int i = 0; i < yearPrices.Count; i++)
                {
                    output.Append("{");

                    for (int j = 0; j < selectedPriceTypes.Count; j++)
                    {
                        output.Append(CreateJsonKey(selectedPriceTypes[j]));
                        double? priceTypeValue = yearPrices[i].GetPriceType(selectedPriceTypes[j]);
                        AppendNullableDouble(ref output, priceTypeValue);
                        
                        if (j != selectedPriceTypes.Count - 1)
                        {
                            output.Append(",");
                        }
                    }

                    AppendWithComma(ref output, "}", i != yearPrices.Count - 1);
                }

                AppendWithComma(ref output, "]", yearIndex != numberOfYears - 1);
                yearIndex++;
            }
            
            output.Append("}");
            string outputJson = output.ToString();
            System.IO.File.WriteAllText("aggouria_thess.json", BeautifyJson(outputJson));
        }

        private string BeautifyJson(string json)
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
            string prettyJson = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
            return prettyJson;
        }

        private string CreateJsonKey(string key)
        {
            StringBuilder output = new StringBuilder();
            output.Append("\"");
            output.Append(key);
            output.Append("\"");
            output.Append(":");
            return output.ToString();
        }

        private void AppendWithComma(ref StringBuilder output, string text, bool includeComma)
        {
            if (includeComma)
            {
                output.Append(text + ",");
            }
            else
            {
                output.Append(text);
            }
        }

        private void AppendNullableDouble(ref StringBuilder output, double? value)
        {
            if (value.HasValue)
            {
                output.Append(value.Value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                output.Append("\"\"");
            }
        }

        private List<string> GetSelectedPriceTypes(IEnumerable<string> priceTypes)
        {
            Dictionary<string, string> priceTypesDictionary = ProductPrice.GetPriceTypes();
            List<string> selectedPriceTypes = new List<string>();
            foreach (string priceType in priceTypes)
            {
                if (priceTypesDictionary.ContainsKey(priceType))
                {
                    selectedPriceTypes.Add(priceTypesDictionary[priceType]);
                }
            }
            return selectedPriceTypes;
        }
    }
}
