using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeggieMarketDataProcessor;
using VeggieMarketDataReader;
using VeggieMarketDataStore;
using VeggieMarketDataStore.Models;
using VeggieMarketLogger;
using VeggieMarketScraper;

namespace VeggieMarketConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //System.Data.DataTable dataTable = excelDataReader.ReadExcelFiles(path);
            //if (dataTable != null)
            //{
            //    foreach (DataRow row in dataTable.Rows)
            //    {
            //        Console.Write($"{row[1]}, {row[2]}, {row[3]} ");
            //        Console.WriteLine();
            //    }
            //}



            //PricesWebScraper thessMarketWebScraper = new PricesWebScraper("https://www.kath.gr/uploadimages/");
            //DateTime startDate = new DateTime(2024, 10, 1);
            //DateTime endDate = new DateTime(2024, 10, 11);
            //thessMarketWebScraper.DownloadPeriod(startDate, endDate);
            Logger logger = Logger.GetInstance();
            DataStorageService dataStorageService = DataStorageService.GetInstance(new SqliteDbService(logger), logger);
            ThessVeggieMarketDataReader thessVeggieMarketDataReader = new ThessVeggieMarketDataReader(dataStorageService);
            RenthVeggieMarketDataReader renthVeggieMarketDataReader = new RenthVeggieMarketDataReader(dataStorageService);

            //string thessPath = @"C:\Stuff\Projects\Λαχαναγορά_Θεσσαλονίκη_2021\Θεσσαλονίκη_01_2021\04-01-21.xls";
            //thessVeggieMarketDataReader.ReadSingleDay(thessPath);

            //string path = @"C:\Stuff\Projects\Λαχαναγορά_Θεσσαλονίκη_2021\";
            //veggieMarketDataReader.ReadDirectory(path);

            //string renthPath = @"C:\Users\gk250151\Downloads\20241010.xls";
            //string renthPath = @"C:\Users\gk250151\source\repos\VeggieMarket\VeggieMarketConsole\bin\Debug\20241001.xls";
            //renthVeggieMarketDataReader.ReadSingleDay(renthPath);
            string renthPath = @"C:\Users\ketam\OneDrive\Υπολογιστής\20160114.xlsx";
            renthVeggieMarketDataReader.ReadSingleDay(renthPath);

            //ThessPriceScraper thessPriceScraper = new ThessPriceScraper();
            //RenthPriceScraper renthPriceScraper = new RenthPriceScraper();

            //DateTime startDay = new DateTime(2023, 1, 1);
            //DateTime endDay = new DateTime(2023, 12, 31);
            //string[] thessPrices = thessPriceScraper.DownloadPeriod(startDay, endDay);
            //string[] thessPrices = thessPriceScraper.DownloadYear(2023);
            //string[] renthPrices = renthPriceScraper.DownloadPeriod(startDay, endDay);

            //renthVeggieMarketDataReader.ReadMultipleDays(renthPrices);
            //thessVeggieMarketDataReader.ReadMultipleDays(thessPrices);

            //string[] renthPrices = Directory.GetFiles(@"C:\Stuff\Projects\veggie\wetransfer_data-files-for-athens-and-thessaloniki_2024-12-04_1526\Athens data sent for proccesing to George (2016-2022)\Λαχαναγορά_Αθήνα_2022", "*.xls", SearchOption.AllDirectories);
            //renthVeggieMarketDataReader.ReadMultipleDays(renthPrices);

            //IEnumerable<ProductPrice> productPrices = dataStorageService.ProductPriceDbService.GetProductMarketPrices(29, 1, 2022);
            //DataProcessor dataProcessor = new DataProcessor();
            //IEnumerable<ProductPrice> processedProductPrices = dataProcessor.ProcessProductPrices(productPrices, 2022);

            //foreach (ProductPrice processedProductPrice in processedProductPrices)
            //{
            //    dataStorageService.ProcessedProductPriceDbService.InsertProcessedPrice(processedProductPrice);
            //}

            //double?[] test = new double?[7];
            //test[0] = 0.25;
            //test[5] = 1.7;
            //test[6] = 1.8;
            //Interpolation.LinSpaceArray(ref test);

            //ProcessAllData();
        }

        private static void ProcessAllData()
        {
            //Logger logger = Logger.GetInstance();
            //DataStorageService dataStorageService = DataStorageService.GetInstance(new SqliteDbService(logger), logger);
            //IEnumerable<Product> products = dataStorageService.ProductDbService.GetProducts();
            //IEnumerable<Market> markets = dataStorageService.MarketDbService.GetMarkets();
            //int year = 2022;
            //DataProcessor dataProcessor = new DataProcessor();
            //foreach (Market market in markets)
            //{
            //    foreach (Product product in products)
            //    {
            //        IEnumerable<ProductPrice> productPrices = dataStorageService.ProductPriceDbService.GetProductMarketPrices(product.ProductId, market.MarketId, year);
            //        if (productPrices != null && productPrices.Count() > 0)
            //        {
            //            IEnumerable<ProductPrice> processedProductPrices = dataProcessor.ProcessProductPrices(productPrices, year);
            //            foreach (ProductPrice processedProductPrice in processedProductPrices)
            //            {
            //                dataStorageService.ProcessedProductPriceDbService.InsertProcessedPrice(processedProductPrice);
            //            }
            //        }
            //    }
            //}
        }
    }
}
