using System;
using System.Linq;
using System.Data;
using VeggieMarketDataStore.Models;
using System.Globalization;

namespace VeggieMarketDataReader
{
    public class ThessVeggieMarketDataReader : MarketDataExcelReader
    {
        private const string MARKET = "ΚΕΝΤΡΙΚΗ ΑΓΟΡΑ ΘΕΣΣΑΛΟΝΙΚΗΣ";
        private const string VEGETABLES = "ΛΑΧΑΝΙΚΑ";
        private const string FRUITS = "ΦΡΟΥΤΑ";
        private const int DATE_ROW_INDEX = 9;
        private const int DATE_COLUMN_INDEX = 0;
        private const int PRODUCT_AA_COLUMN_INDEX = 0;
        private const int PRODUCT_TYPE_COLUMN_INDEX = 1;
        private const int PRODUCT_NAME_COLUMN_INDEX = 1;
        private const int EXTRA_CATEGORY_PRICE_COLUMN_INDEX = 2;
        private const int CATEGORY1_PRICE_COLUMN_INDEX = 3;
        private const int CATEGORY2_PRICE_COLUMN_INDEX = 5;
        private const int QUANTITY_TO_SUPPLY_COLUMN_INDEX = 7;
        private const int DOMINANT_PRICE_COLUMN_INDEX = 8;
        private const int PREVIOUS_WEEK_DOMINANT_PRICE_COLUMN_INDEX = 9;
        private const int PREVIOUS_YEAR_DOMINANT_PRICE_COLUMN_INDEX = 10;
        private const int PREVIOUS_WEEK_PRICE_DIFFERENCE_COLUMN_INDEX = 11;
        private const int PREVIOUS_YEAR_PRICE_DIFFERENCE_COLUMN_INDEX = 12;
        private const int SOLID_QUANTITY_PERCENTAGE_COLUMN_INDEX = 13;

        public ThessVeggieMarketDataReader(VeggieMarketDataStore.DataStorageService dataStorageService) : base(dataStorageService)
        {
            GetMarket(MARKET);
        }

        protected override bool IsDateRow(int rowNumber)
        {
            return rowNumber == DATE_ROW_INDEX;
        }

        protected override DateTime? ExtractDate(DataRow row)
        {
            const int numberOfLanguages = 2;
            const int numberOfDateParts = 4;
            string inputDate = Convert.ToString(row[DATE_COLUMN_INDEX]);
            string[] dateArray = inputDate.Split(new char[] { '/' });
            if (dateArray == null || dateArray.Length != numberOfLanguages) return null;
            string dateInEnglish = dateArray[1].Trim();
            string[] dateParts = dateInEnglish.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (dateParts == null || dateParts.Length != numberOfDateParts) return null;
            string[] adjustedDateParts = dateParts.Skip(1).Take(dateParts.Length - 1).ToArray();
            string formattedDate = string.Join(" ", adjustedDateParts);
            DateTime date = DateTime.Parse(formattedDate);
            return date;
        }

        protected override bool IsProductTypeRow(DataRow row)
        {
            string secondColumn = Convert.ToString(row[PRODUCT_TYPE_COLUMN_INDEX]);
            if (string.IsNullOrEmpty(secondColumn)) return false;
            secondColumn = secondColumn.ToUpper();
            if (secondColumn.Contains(VEGETABLES) || secondColumn.Contains(FRUITS)) return true;
            return false;
        }

        protected override bool IsProductRow(DataRow row)
        {
            string firstColumn = Convert.ToString(row[PRODUCT_AA_COLUMN_INDEX]);
            return int.TryParse(firstColumn, out int productSerialNumber);
        }

        protected override string ExtractProductTypeName(DataRow row)
        {
            string productTypeName = Convert.ToString(row[PRODUCT_TYPE_COLUMN_INDEX]);
            if (productTypeName.Contains(VEGETABLES)) productTypeName = VEGETABLES;
            if (productTypeName.Contains(FRUITS)) productTypeName = FRUITS;
            return productTypeName;
        }

        protected override string ExtractProductName(DataRow row)
        {
            return Convert.ToString(row[PRODUCT_NAME_COLUMN_INDEX]);
        }

        protected override ProductPrice ExtractProductPrice(DataRow row, Product product, long productDate)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;

            string extraCategory = NormalizePrice(Convert.ToString(row[EXTRA_CATEGORY_PRICE_COLUMN_INDEX]));
            string category1 = NormalizePrice(Convert.ToString(row[CATEGORY1_PRICE_COLUMN_INDEX]));
            double[] category1MinMaxPrice = ExtractMinMaxPrice(category1);
            string category2 = NormalizePrice(Convert.ToString(row[CATEGORY2_PRICE_COLUMN_INDEX]));
            double[] category2MinMaxPrice = ExtractMinMaxPrice(category2);
            string quantityToSupply = Convert.ToString(row[QUANTITY_TO_SUPPLY_COLUMN_INDEX]);
            string dominantPrice = NormalizePrice(Convert.ToString(row[DOMINANT_PRICE_COLUMN_INDEX]));
            string previousWeekDominantPrice = NormalizePrice(Convert.ToString(row[PREVIOUS_WEEK_DOMINANT_PRICE_COLUMN_INDEX]));
            string previousYearDominantPrice = NormalizePrice(Convert.ToString(row[PREVIOUS_YEAR_DOMINANT_PRICE_COLUMN_INDEX]));
            string previousWeekPriceDifference = NormalizePrice(Convert.ToString(row[PREVIOUS_WEEK_PRICE_DIFFERENCE_COLUMN_INDEX]));
            string previousYearPriceDifference = NormalizePrice(Convert.ToString(row[PREVIOUS_YEAR_PRICE_DIFFERENCE_COLUMN_INDEX]));
            string soldQuantityPercentage = NormalizePrice(Convert.ToString(row[SOLID_QUANTITY_PERCENTAGE_COLUMN_INDEX]));

            ProductPrice productPrice = new ProductPrice(product, productDate, market);
            if (!string.IsNullOrEmpty(extraCategory) && double.TryParse(extraCategory, NumberStyles.Any, culture, out double extraCategoryDouble))
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
            if (!string.IsNullOrEmpty(quantityToSupply) && double.TryParse(quantityToSupply, NumberStyles.Any, culture, out double quantityToSupplyDouble))
            {
                productPrice.QuantityToSupply = quantityToSupplyDouble;
            }
            if (!string.IsNullOrEmpty(dominantPrice) && double.TryParse(dominantPrice, NumberStyles.Any, culture, out double dominantPriceDouble))
            {
                productPrice.DominantPrice = dominantPriceDouble;
            }
            if (!string.IsNullOrEmpty(previousWeekDominantPrice) && double.TryParse(previousWeekDominantPrice, NumberStyles.Any, culture, out double previousWeekDominantPriceDouble))
            {
                productPrice.PreviousWeekDominantPrice = previousWeekDominantPriceDouble;
            }
            if (!string.IsNullOrEmpty(previousYearDominantPrice) && double.TryParse(previousYearDominantPrice, NumberStyles.Any, culture, out double previousYearDominantPriceDouble))
            {
                productPrice.PreviousYearDominantPrice = previousYearDominantPriceDouble;
            }
            if (!string.IsNullOrEmpty(previousWeekPriceDifference) && double.TryParse(previousWeekPriceDifference, NumberStyles.Any, culture, out double previousWeekPriceDifferenceDouble))
            {
                productPrice.PreviousWeekPriceDifference = previousWeekPriceDifferenceDouble;
            }
            if (!string.IsNullOrEmpty(previousYearPriceDifference) && double.TryParse(previousYearPriceDifference, NumberStyles.Any, culture, out double previousYearPriceDifferenceDouble))
            {
                productPrice.PreviousYearPriceDifference = previousYearPriceDifferenceDouble;
            }
            if (!string.IsNullOrEmpty(soldQuantityPercentage) && double.TryParse(soldQuantityPercentage, NumberStyles.Any, culture, out double soldQuantityPercentageDouble))
            {
                productPrice.SoldQuantityPercentage = soldQuantityPercentageDouble;
            }

            return productPrice;
        }
    }
}
