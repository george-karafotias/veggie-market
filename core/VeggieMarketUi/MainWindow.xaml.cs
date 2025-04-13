using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using LiveCharts.Wpf;
using LiveCharts;
using Microsoft.Win32;
using VeggieMarketDataProcessor;
using VeggieMarketDataReader;
using VeggieMarketDataStore;
using VeggieMarketDataStore.Models;
using VeggieMarketLogger;
using VeggieMarketScraper;
using VeggieMarketUi.Models;
using VeggieDataExporter;
using System.Windows.Media;
using LiveCharts.Wpf.Charts.Base;
using LiveCharts.Definitions.Charts;

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
        private TextBoxLogger dataAnalysisTextBoxLogger;
        private PriceRetrievalParameters priceRetrievalParameters;
        private IEnumerable<ProductPrice> retrievedPrices;

        public MainWindow()
        {
            InitializeComponent();

            importDataTextBoxLogger = new TextBoxLogger(LogTextBox);
            downloadDataTextBoxLogger = new TextBoxLogger(DownloadLogTextBox);
            dataAnalysisTextBoxLogger = new TextBoxLogger(DataAnalysisLogTextBox);
            InitializeData();
        }

        private void InitializeData()
        {
            dataStorageService = DataStorageService.GetInstance(new SqliteDbService(importDataTextBoxLogger), importDataTextBoxLogger);

            Market[] markets = dataStorageService.MarketDbService.GetMarkets();
            List<string> marketNames = RetrieveMarketNames(markets);
            PopulateMarketReaderMap(marketNames);
            PopulateMarketPriceScraperMap(marketNames);

            ImportDataMarketsComboBox.ItemsSource = marketNames;
            ImportDataMarketsComboBox.SelectedIndex = 0;
            DownloadDataMarketsComboBox.ItemsSource = marketNames;
            DownloadDataMarketsComboBox.SelectedIndex = 0;

            PopulateMarketsComboBox(AvailableMarketsListBox, markets);
            InitMarketsComboBox(SelectedMarketsListBox);

            IEnumerable<Product> products = dataStorageService.ProductDbService.GetProducts();
            PopulateProductsComboBox(AvailableProductsListBox, products);
            InitProductsComboBox(SelectedProductsListBox);

            List<string> priceTypes = ProductPrice.GetPriceTypes().Keys.ToList();
            AvailablePricesListBox.ItemsSource = new ObservableCollection<object>(priceTypes);
        }

        private void InitMarketsComboBox(ListBox marketsComboBox)
        {
            marketsComboBox.DisplayMemberPath = "MarketName";
            marketsComboBox.SelectedValuePath = "MarketId";
        }

        private void InitProductsComboBox(ListBox productsComboBox)
        {
            productsComboBox.DisplayMemberPath = "ProductName";
            productsComboBox.SelectedValuePath = "ProductId";
        }

        private void PopulateMarketsComboBox(ListBox marketsComboBox, Market[] markets)
        {
            InitMarketsComboBox(marketsComboBox);
            marketsComboBox.ItemsSource = new ObservableCollection<object>(markets);
        }

        private void PopulateProductsComboBox(ListBox productsComboBox, IEnumerable<Product> products)
        {
            InitProductsComboBox(productsComboBox);
            productsComboBox.ItemsSource = new ObservableCollection<object>(products);
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

        private List<string> RetrieveMarketNames(Market[] markets)
        {
            List<string> marketNames = new List<string>();
            if (markets != null)
            {
                foreach (Market market in markets)
                {
                    marketNames.Add(market.MarketName);
                }
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

        private PriceScraper GetSelectedMarketPriceScraper(string selectedMarketName)
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
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                OpenFileTextBox.Text = openFileDialog.FileName;
            }
        }

        private void ImportFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (ImportDataMarketsComboBox.SelectedIndex < 0) return;
            string selectedMarketName = ImportDataMarketsComboBox.SelectedItem.ToString();
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
            if (ImportDataMarketsComboBox.SelectedIndex < 0) return;
            string selectedMarketName = ImportDataMarketsComboBox.SelectedItem.ToString();

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

            if (DownloadDataMarketsComboBox.SelectedIndex < 0) return;
            string selectedMarketName = DownloadDataMarketsComboBox.SelectedItem.ToString();

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
            PriceScraper priceScraper = GetSelectedMarketPriceScraper(marketName);
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
            PriceScraper priceScraper = GetSelectedMarketPriceScraper(marketName);
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

        private async void RetrievePricesButton_Click(object sender, RoutedEventArgs e)
        {
            List<Market> selectedMarkets = GetSelectedMarkets();
            if (selectedMarkets.Count == 0)
            {
                ShowErrorMessage("Please select the market.");
                return;
            }

            List<Product> selectedProducts = GetSelectedProducts();
            if (selectedProducts.Count == 0)
            {
                ShowErrorMessage("Please select a product.");
                return;
            }

            DateTime? fromDate = DataAnalysisFromDatePicker.SelectedDate;
            if (fromDate == null)
            {
                ShowErrorMessage("Please select from date.");
                return;
            }

            DateTime? toDate = DataAnalysisToDatePicker.SelectedDate;
            if (toDate == null)
            {
                ShowErrorMessage("Please select to date.");
                return;
            }

            priceRetrievalParameters = new PriceRetrievalParameters();
            priceRetrievalParameters.Products = selectedProducts;
            priceRetrievalParameters.Markets = selectedMarkets;
            priceRetrievalParameters.FromDate = fromDate.Value;
            priceRetrievalParameters.ToDate = toDate.Value;

            ShowPleaseWaitForDataAnalysis();
            await Task.Run(() => RetrievePrices());

            if (retrievedPrices == null)
            {
                ShowErrorMessage("No prices found for the selected time period.");
                return;
            }

            HidePleaseWaitForDataAnalysis();
            PriceAnalysisStackPanel.Visibility = Visibility.Visible;
        }

        private void ShowPleaseWaitForDataAnalysis()
        {
            DataAnalysiInputPanel.Visibility = Visibility.Collapsed;
            DataAnalysisContainer.VerticalAlignment = VerticalAlignment.Center;
            DataAnalysisPleaseWaitPanel.Visibility = Visibility.Visible;
        }

        private void HidePleaseWaitForDataAnalysis()
        {
            DataAnalysiInputPanel.Visibility = Visibility.Visible;
            DataAnalysisPleaseWaitPanel.Visibility = Visibility.Collapsed;
            DataAnalysisContainer.VerticalAlignment = VerticalAlignment.Top;
        }

        private void RetrievePrices()
        {
            retrievedPrices = dataStorageService.ProcessedProductPriceDbService.GetProcessedProductMarketPrices(priceRetrievalParameters.Products[0].ProductId, priceRetrievalParameters.Markets[0].MarketId, priceRetrievalParameters.FromDate, priceRetrievalParameters.ToDate);
        }

        private void Plot(List<string> selectedPriceTypes)
        {
            List<Chart> charts = new ChartCreator().CreateCharts(selectedPriceTypes, priceRetrievalParameters, retrievedPrices);

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Chart chart in charts)
                {
                    GraphContainer.Children.Clear();
                    GraphContainer.Children.Add(chart);
                }
            });
        }

        private async void PlotButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedPriceTypes = GetSelectedPriceTypes();
            if (selectedPriceTypes == null || selectedPriceTypes.Count == 0)
            {
                ShowErrorMessage("Please select at least one price indicator.");
                return;
            }

            ShowPleaseWaitForDataAnalysis();
            await Task.Run(() => Plot(selectedPriceTypes));

            HidePleaseWaitForDataAnalysis();
            GraphsContainer.Height = this.Height;
            GraphsContainer.Visibility = Visibility.Visible;
        }

        private List<string> GetSelectedPriceTypes()
        {
            ObservableCollection<object> selectedPriceTypesCollection = SelectedPricesListBox.ItemsSource as ObservableCollection<object>;
            if (selectedPriceTypesCollection == null || selectedPriceTypesCollection.Count() == 0) return null;
            List<string> selectedPriceTypes = new List<string>();
            foreach (object price in selectedPriceTypesCollection)
            {
                selectedPriceTypes.Add(price.ToString());
            }
            return selectedPriceTypes;
        }

        private List<Market> GetSelectedMarkets()
        {
            List<Market> marketList = new List<Market>();
            ObservableCollection<object> markets = SelectedMarketsListBox.ItemsSource as ObservableCollection<object>;
            if (markets == null || markets.Count() == 0) return marketList;
            foreach (Market market in markets)
            {
                marketList.Add(market);
            }
            return marketList;
        }

        private List<Product> GetSelectedProducts()
        {
            List<Product> productList = new List<Product>();
            ObservableCollection<object> products = SelectedProductsListBox.ItemsSource as ObservableCollection<object>;
            if (products == null || products.Count() == 0) return productList;
            foreach (Product product in products)
            {
                productList.Add(product);
            }
            return productList;
        }

        private void ExportPricesButton_Click(object sender, RoutedEventArgs e)
        {
            if (retrievedPrices == null)
            {
                ShowErrorMessage("No prices retrieved.");
                return;
            }

            List<Product> selectedProducts = GetSelectedProducts();
            if (selectedProducts.Count == 0)
            {
                ShowErrorMessage("Please select a product.");
                return;
            }

            JsonProductPriceExporter jsonProductPriceExporter = new JsonProductPriceExporter(dataAnalysisTextBoxLogger);
            jsonProductPriceExporter.ExportProductPrices(selectedProducts[0].ProductName, retrievedPrices, GetSelectedPriceTypes());
        }

        private void GetAvailableDataButton_Click(object sender, RoutedEventArgs e)
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

        private void AddToSelectedMarketsButton_Click(object sender, RoutedEventArgs e)
        {
            MoveSelectedItems(AvailableMarketsListBox, SelectedMarketsListBox);
        }

        private void RemoveFromSelectedMarketsButton_Click(object sender, RoutedEventArgs e)
        {
            MoveSelectedItems(SelectedMarketsListBox, AvailableMarketsListBox);
        }

        private void AddToSelectedProductsButton_Click(object sender, RoutedEventArgs e)
        {
            MoveSelectedItems(AvailableProductsListBox, SelectedProductsListBox);
        }

        private void RemoveFromSelectedProductsButton_Click(object sender, RoutedEventArgs e)
        {
            MoveSelectedItems(SelectedProductsListBox, AvailableProductsListBox);
        }

        private void AddToSelectedPricesButton_Click(object sender, RoutedEventArgs e)
        {
            MoveSelectedItems(AvailablePricesListBox, SelectedPricesListBox);
        }

        private void RemoveFromSelectedPricesButton_Click(object sender, RoutedEventArgs e)
        {
            MoveSelectedItems(SelectedPricesListBox, AvailablePricesListBox);
        }

        private void MoveSelectedItems(ListBox source, ListBox destination)
        {
            IEnumerable<object> selectedItems = source.SelectedItems as IEnumerable<object>;
            if (selectedItems == null) return;
            ObservableCollection<object> itemsSource = source.ItemsSource as ObservableCollection<object>;
            ObservableCollection<object> itemsDestination = destination.ItemsSource as ObservableCollection<object>;
            if (itemsDestination == null)
            {
                itemsDestination = new ObservableCollection<object>();
            }

            List<int> itemsToRemove = new List<int>();
            foreach (var selectedItem in selectedItems)
            {
                itemsDestination.Add(selectedItem);
                for (int i = 0; i < itemsSource.Count(); i++)
                {
                    if (itemsSource[i] == selectedItem)
                    {
                        itemsToRemove.Add(i);
                    }
                }
            }
            destination.ItemsSource = itemsDestination;

            ObservableCollection<object> newItemsSource = new ObservableCollection<object>();
            for (int i = 0; i < itemsSource.Count(); i++)
            {
                if (!itemsToRemove.Contains(i))
                {
                    newItemsSource.Add(itemsSource[i]);
                }
            }
            source.ItemsSource = newItemsSource;
        }
    }
}
