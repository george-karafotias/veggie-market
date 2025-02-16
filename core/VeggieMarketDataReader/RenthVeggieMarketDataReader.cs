using System;
using System.Data;
using System.Text.RegularExpressions;
using VeggieMarketDataStore.Models;

namespace VeggieMarketDataReader
{
    public class RenthVeggieMarketDataReader : MarketDataExcelReader
    {
        private const string MARKET = "ΛΑΧΑΝΑΓΟΡΑ ΡΕΝΤΗ";
        private const string VEGETABLES = "ΛΑΧΑΝΙΚΑ";
        private const string FRUITS = "ΦΡΟΥΤΑ";
        private const int DATE_ROW_INDEX = 1;
        private const int DATE_COLUMN_INDEX = 2;
        private const int PRODUCT_AA_COLUMN_INDEX = 0;
        private const int PRODUCT_TYPE_COLUMN_INDEX = 1;
        private const int PRODUCT_NAME_COLUMN_INDEX = 1;
        private const int EXTRA_CATEGORY_PRICE_COLUMN_INDEX = 2;
        private const int CATEGORY1_PRICE_COLUMN_INDEX = 3;
        private const int CATEGORY2_PRICE_COLUMN_INDEX = 4;
        private const int DOMINANT_PRICE_COLUMN_INDEX = 5;
        private const int PREVIOUS_YEAR_DOMINANT_PRICE_COLUMN_INDEX = 6;
        private const int PREVIOUS_WEEK_DOMINANT_PRICE_COLUMN_INDEX = 7;

        public RenthVeggieMarketDataReader(VeggieMarketDataStore.DataStorageService dataStorageService) : base(dataStorageService)
        {
            GetMarket(MARKET);
        }

        protected override bool IsDateRow(int rowNumber)
        {
            return rowNumber == DATE_ROW_INDEX;
        }

        protected override DateTime? ExtractDate(DataRow row)
        {
            string cell = Convert.ToString(row[DATE_COLUMN_INDEX]);
            string pattern = @"\b\d{1,2}/\d{1,2}/\d{4}\b";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(cell);
            if (matches.Count == 1)
            {
                DateTime.TryParse(matches[0].Value, out DateTime date);
                return date;
            }
            return null;
        }

        protected override bool IsProductTypeRow(DataRow row)
        {
            string secondColumn = Convert.ToString(row[PRODUCT_TYPE_COLUMN_INDEX]);
            if (string.IsNullOrEmpty(secondColumn)) return false;
            secondColumn = secondColumn.ToUpper();
            if (secondColumn.Contains(VEGETABLES) || secondColumn.Contains("Λ Α Χ Α Ν Ι Κ Α") || secondColumn.Contains(FRUITS)) return true;
            return false;
        }

        protected override bool IsProductRow(DataRow row)
        {
            string firstColumn = Convert.ToString(row[PRODUCT_AA_COLUMN_INDEX]);
            return int.TryParse(firstColumn, out int productSerialNumber);
        }

        protected override string ExtractProductTypeName(DataRow row)
        {
            string productTypeName = Convert.ToString(row[PRODUCT_TYPE_COLUMN_INDEX]).Trim();
            if (productTypeName == "Λ Α Χ Α Ν Ι Κ Α") productTypeName = VEGETABLES;
            return productTypeName;
        }

        protected override string ExtractProductName(DataRow row)
        {
            return Convert.ToString(row[PRODUCT_NAME_COLUMN_INDEX]);
        }

        protected override ProductPrice ExtractProductPrice(DataRow row, Product product, long productDate)
        {
            string extraCategory = Convert.ToString(row[EXTRA_CATEGORY_PRICE_COLUMN_INDEX]);
            string category1 = Convert.ToString(row[CATEGORY1_PRICE_COLUMN_INDEX]);
            double[] category1MinMaxPrice = ExtractMinMaxPrice(category1);
            string category2 = Convert.ToString(row[CATEGORY2_PRICE_COLUMN_INDEX]);
            double[] category2MinMaxPrice = ExtractMinMaxPrice(category2);
            string dominantPrice = Convert.ToString(row[DOMINANT_PRICE_COLUMN_INDEX]);
            string previousWeekDominantPrice = Convert.ToString(row[PREVIOUS_WEEK_DOMINANT_PRICE_COLUMN_INDEX]);
            string previousYearDominantPrice = Convert.ToString(row[PREVIOUS_YEAR_DOMINANT_PRICE_COLUMN_INDEX]);

            ProductPrice productPrice = new ProductPrice(product, productDate, market);
            if (!string.IsNullOrEmpty(extraCategory) && double.TryParse(extraCategory, out double extraCategoryDouble))
            {
                productPrice.ExtraCategory = extraCategoryDouble;
            }
            if (category1MinMaxPrice != null)
            {
                productPrice.Category1MinPrice = category1MinMaxPrice[0];
                productPrice.Category1MaxPrice = category1MinMaxPrice[1];
            }
            if (category2MinMaxPrice != null)
            {
                productPrice.Category2MinPrice = category2MinMaxPrice[0];
                productPrice.Category2MaxPrice = category2MinMaxPrice[1];
            }
            if (!string.IsNullOrEmpty(dominantPrice) && double.TryParse(dominantPrice, out double dominantPriceDouble))
            {
                productPrice.DominantPrice = dominantPriceDouble;
            }
            if (!string.IsNullOrEmpty(previousWeekDominantPrice) && double.TryParse(previousWeekDominantPrice, out double previousWeekDominantPriceDouble))
            {
                productPrice.PreviousWeekDominantPrice = previousWeekDominantPriceDouble;
            }
            if (!string.IsNullOrEmpty(previousYearDominantPrice) && double.TryParse(previousYearDominantPrice, out double previousYearDominantPriceDouble))
            {
                productPrice.PreviousYearDominantPrice = previousYearDominantPriceDouble;
            }

            return productPrice;
        }
    }
}
