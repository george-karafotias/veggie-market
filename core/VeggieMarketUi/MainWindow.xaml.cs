using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using VeggieMarketDataProcessor;
using VeggieMarketDataReader;
using VeggieMarketDataStore;
using VeggieMarketDataStore.Models;
using VeggieMarketLogger;
using VeggieMarketScraper;
using VeggieMarketUi.Models;

namespace VeggieMarketUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataStorageService dataStorageService;
        private Dictionary<string, MarketDataReader> marketReaderMap;
        private Dictionary<string, PriceScraper> marketPriceScraperMap;
        private TextBoxLogger importDataTextBoxLogger;
        private TextBoxLogger downloadDataTextBoxLogger;

        public MainWindow()
        {
            InitializeComponent();

            importDataTextBoxLogger = new TextBoxLogger(LogTextBox);
            downloadDataTextBoxLogger = new TextBoxLogger(DownloadLogTextBox);
            InitializeData();
        }

        private void InitializeData()
        {
            dataStorageService = DataStorageService.GetInstance(new SqliteDbService(importDataTextBoxLogger), importDataTextBoxLogger);
            List<string> marketNames = RetrieveMarketNames();
            PopulateMarketReaderMap(marketNames);
            PopulateMarketPriceScraperMap(marketNames);

            MarketsComboBox.ItemsSource = marketNames;
            MarketsComboBox.SelectedIndex = 0;

            DownloadMarketsComboBox.ItemsSource = marketNames;
            DownloadMarketsComboBox.SelectedIndex = 0;
        }

        private void PopulateMarketReaderMap(List<string> marketNames)
        {
            marketReaderMap = new Dictionary<string, MarketDataReader>();
            foreach (string marketName in marketNames)
            {
                if (IsRenth(marketName))
                {
                    marketReaderMap.Add(marketName, new RenthVeggieMarketDataReader(dataStorageService));
                }
                else if (IsThessaloniki(marketName))
                {
                    marketReaderMap.Add(marketName, new ThessVeggieMarketDataReader(dataStorageService));
                }
            }
        }

        private void PopulateMarketPriceScraperMap(List<string> marketNames)
        {
            marketPriceScraperMap = new Dictionary<string, PriceScraper>();
            foreach (string marketName in marketNames)
            {
                if (IsRenth(marketName))
                {
                    marketPriceScraperMap.Add(marketName, new RenthPriceScraper(downloadDataTextBoxLogger));
                }
                else if (IsThessaloniki(marketName))
                {
                    marketPriceScraperMap.Add(marketName, new ThessPriceScraper(downloadDataTextBoxLogger));
                }
            }
        }

        private List<string> RetrieveMarketNames()
        {
            Market[] markets = dataStorageService.MarketDbService.GetMarkets();

            List<string> marketNames = new List<string>();
            foreach (Market market in markets)
            {
                marketNames.Add(market.MarketName);
            }

            if (marketNames.Count == 0)
            {
                marketNames.Add("ΚΕΝΤΡΙΚΗ ΑΓΟΡΑ ΘΕΣΣΑΛΟΝΙΚΗΣ");
                marketNames.Add("ΛΑΧΑΝΑΓΟΡΑ ΡΕΝΤΗ");
            }

            return marketNames;
        }

        private bool IsRenth(string marketName)
        {
            return marketName.Contains("ΡΕΝΤΗ");
        }

        private bool IsThessaloniki(string marketName)
        {
            return marketName.Contains("ΘΕΣΣΑΛΟΝΙΚΗ");
        }

        private MarketDataReader GetSelectedMarketDataReader(string selectedMarketName)
        {
            foreach (KeyValuePair<string, MarketDataReader> marketEntry in marketReaderMap)
            {
                if (marketEntry.Key == selectedMarketName)
                {
                    return marketEntry.Value;
                }
            }
            return null;
        }

        private PriceScraper GetSelectedPriceScraper(string selectedMarketName)
        {
            foreach (KeyValuePair<string, PriceScraper> marketEntry in marketPriceScraperMap)
            {
                if (marketEntry.Key == selectedMarketName)
                {
                    return marketEntry.Value;
                }
            }
            return null;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel files (*.xls)|*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                OpenFileTextBox.Text = openFileDialog.FileName;
            }
        }

        private void ImportFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (MarketsComboBox.SelectedIndex < 0) return;
            string selectedMarketName = MarketsComboBox.SelectedItem.ToString();
            MarketDataReader marketDataReader = GetSelectedMarketDataReader(selectedMarketName);
            if (marketDataReader == null) return;

            LogTextBox.Text = "";
            marketDataReader.ReadSingleDay(OpenFileTextBox.Text);
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    OpenFolderTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void ImportFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (MarketsComboBox.SelectedIndex < 0) return;
            string selectedMarketName = MarketsComboBox.SelectedItem.ToString();

            LogTextBox.Text = "";
            string[] priceFiles = Directory.GetFiles(@OpenFolderTextBox.Text, "*.xls", SearchOption.AllDirectories);
            bool processData = ProcessDataAfterInsertCheckBox.IsChecked.HasValue && ProcessDataAfterInsertCheckBox.IsChecked.Value;
            InsertPricesToDatabase(selectedMarketName, priceFiles, processData, importDataTextBoxLogger);
        }

        private void DownloadFolderButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    DownloadFolderTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                TabItem selectedTab = (sender as TabControl).SelectedItem as TabItem;
                if (selectedTab.Header.ToString() == "Available Data")
                {
                    PopulateAvailablePrices();
                }
            }
        }

        private void DownloadMarketsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedMarketName = (sender as ComboBox).SelectedItem as string;
            MarketAvailableData marketPrices = GetMarketAvailablePrices(selectedMarketName);
        }

        private void PopulateAvailablePrices()
        {
            MarketAvailableData[] availablePrices = dataStorageService.MetadataDbService.GetAvailablePrices();
            if (availablePrices == null || availablePrices.Length == 0) return;

            ObservableCollection<PricePeriod> pricePeriods = new ObservableCollection<PricePeriod>();
            foreach (MarketAvailableData marketPrices in availablePrices)
            {
                foreach (DatePeriod datePeriod in marketPrices.DatePeriods)
                {
                    PricePeriod pricePeriod = new PricePeriod(marketPrices.Market, datePeriod);
                    pricePeriods.Add(pricePeriod);
                }
            }

            AvailablePricesDataGrid.ItemsSource = pricePeriods;
        }

        private MarketAvailableData GetMarketAvailablePrices(string marketName)
        {
            MarketAvailableData[] availablePrices = dataStorageService.MetadataDbService.GetAvailablePrices();
            if (availablePrices == null || availablePrices.Length == 0) return null;

            foreach (MarketAvailableData marketPrices in availablePrices)
            {
                if (marketPrices.Market.MarketName == marketName) return marketPrices;
            }

            return null;
        }

        private void DownloadDataButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? fromDate = FromDatePicker.SelectedDate;
            if (fromDate == null)
            {
                ShowErrorMessage("Please select from date.");
                return;
            }

            DateTime? toDate = ToDatePicker.SelectedDate;
            if (toDate == null)
            {
                ShowErrorMessage("Please select to date.");
                return;
            }

            if (toDate.Value < fromDate.Value)
            {
                ShowErrorMessage("The to date must be greater or equal to the from date.");
                return;
            }

            DateTime today = DateTime.Now;
            if (fromDate.Value > today || toDate.Value > today)
            {
                ShowErrorMessage("The from and to date cannot be in the future.");
                return;
            }

            string downloadFolder = DownloadFolderTextBox.Text;
            if (string.IsNullOrEmpty(downloadFolder))
            {
                ShowErrorMessage("Please select a download folder.");
                return;
            }

            if (!Directory.Exists(downloadFolder))
            {
                ShowErrorMessage("The download folder does not exist.");
                return;
            }

            if (DownloadMarketsComboBox.SelectedIndex < 0) return;
            string selectedMarketName = DownloadMarketsComboBox.SelectedItem.ToString();

            DownloadLogTextBox.Text = "";
            if (ImportToDbAfterDownloadCheckBox.IsChecked.HasValue && ImportToDbAfterDownloadCheckBox.IsChecked.Value)
            {
                bool processData = ProcessDataAfterDownloadAndInsertCheckBox.IsChecked.HasValue && ProcessDataAfterDownloadAndInsertCheckBox.IsChecked.Value;
                DownloadPricesAndInsertToDatabase(selectedMarketName, fromDate.Value, toDate.Value, downloadFolder, processData, downloadDataTextBoxLogger);
            }
            else
            {
                DownloadPrices(selectedMarketName, fromDate.Value, toDate.Value, downloadFolder);
            }
        }

        private void ShowErrorMessage(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowNoPriceScraperError()
        {
            ShowErrorMessage("The selected market prices cannot be downloaded.");
        }

        private void ShowNoMarketReaderError()
        {
            ShowErrorMessage("There is no way to insert the selected prices to the database.");
        }

        private void InsertPricesToDatabase(string marketName, string[] priceFiles, bool processPrices, ILogger logger)
        {
            MarketDataReader marketDataReader = GetSelectedMarketDataReader(marketName);
            if (marketDataReader == null)
            {
                ShowNoMarketReaderError();
                return;
            }

            dataStorageService.Logger = logger;
            DataProcessor dataProcessor = new DataProcessor(dataStorageService);

            Task.Run(() =>
            {
                DateTime[] days = marketDataReader.ReadMultipleDays(priceFiles);
                if (processPrices)
                {
                    dataProcessor.ProcessProductPrices(marketName, days);
                }
            });
        }

        private void DownloadPrices(string marketName, DateTime fromDate, DateTime toDate, string downloadFolder)
        {
            PriceScraper priceScraper = GetSelectedPriceScraper(marketName);
            if (priceScraper == null)
            {
                ShowNoPriceScraperError();
                return;
            }

            Task.Run(() =>
            {
                string[] priceFiles = priceScraper.DownloadPeriod(fromDate, toDate, downloadFolder);
            });
        }

        private void DownloadPricesAndInsertToDatabase(
            string marketName, 
            DateTime fromDate, 
            DateTime toDate, 
            string downloadFolder,
            bool processPrices,
            ILogger logger)
        {
            PriceScraper priceScraper = GetSelectedPriceScraper(marketName);
            if (priceScraper == null)
            {
                ShowNoPriceScraperError();
                return;
            }

            MarketDataReader marketDataReader = GetSelectedMarketDataReader(marketName);
            if (marketDataReader == null)
            {
                ShowNoMarketReaderError();
                return;
            }

            dataStorageService.Logger = logger;
            marketDataReader.Logger = logger;
            DataProcessor dataProcessor = new DataProcessor(dataStorageService);

            Task.Run(() =>
            {
                string[] priceFiles = priceScraper.DownloadPeriod(fromDate, toDate, downloadFolder);
                DateTime[] days = marketDataReader.ReadMultipleDays(priceFiles);
                if (processPrices)
                {
                    dataProcessor.ProcessProductPrices(marketName, days);
                }
            });
        }
    }
}
