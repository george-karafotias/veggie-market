﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        public JsonProductPriceExporter(ILogger logger = null)
        {
            this.logger = logger;
        }

        public bool ExportProductPrices(string fileName, IEnumerable<ProductPrice> productPrices, IEnumerable<string> priceTypes)
        {
            if (productPrices == null)
            {
                Log("No product prices to export", LogType.Error);
                return false;
            }

            Dictionary<int, List<ProductPrice>> productPricesPerYear = ProductPriceGrouping.GroupByYear(productPrices);

            List<string> selectedPriceTypes = GetSelectedPriceTypes(priceTypes);
            if (selectedPriceTypes.Count == 0)
            {
                Log("The selected price types are not valid", LogType.Error);
                return false;
            }

            StringBuilder output = new StringBuilder();
            output.Append("{");
            int numberOfYears = productPricesPerYear.Count;
            int yearIndex = 0;

            foreach (KeyValuePair<int, List<ProductPrice>> productPriceYearEntry in productPricesPerYear)
            {
                int year = productPriceYearEntry.Key;
                Log("Exporting year " + year + "...", LogType.Info);
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
            try
            {
                string fullPath = Path.GetFullPath(fileName + ".json");
                System.IO.File.WriteAllText(fileName + ".json", BeautifyJson(outputJson));
                Log("Successfully exported " + fullPath, LogType.Info);
                return true;
            }
            catch (Exception ex)
            {
                Log("An exception has occurred while exporting the data. Ex: " + ex.StackTrace, LogType.Error);
                return false;
            }
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

        private void Log(string message, LogType logType)
        {
            if (this.logger == null) return;
            logger.Log(GetType().Name, MethodBase.GetCurrentMethod().Name, message, logType);
        }
    }
}
