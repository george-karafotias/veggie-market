using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using VeggieMarketDataProcessor;
using VeggieMarketDataReader;
using VeggieMarketDataStore;
using VeggieMarketDataStore.Models;
using VeggieMarketUi.Models;

namespace VeggieMarketUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataStorageService dataStorageService;
        private Dictionary<string, MarketDataReader> marketMap;
        private TextBoxLogger textBoxLogger;

        public MainWindow()
        {
            InitializeComponent();

            textBoxLogger = new TextBoxLogger(LogTextBox);
            InitializeData();
        }

        private void InitializeData()
        {
            dataStorageService = DataStorageService.GetInstance(new SqliteDbService(textBoxLogger), textBoxLogger);
            SetupMarkets();
        }

        private void SetupMarkets()
        {
            marketMap = new Dictionary<string, MarketDataReader>();
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

            foreach (string marketName in marketNames)
            {
                if (marketName.Contains("ΡΕΝΤΗ"))
                {
                    marketMap.Add(marketName, new RenthVeggieMarketDataReader(dataStorageService));
                }
                else if (marketName.Contains("ΘΕΣΣΑΛΟΝΙΚΗ"))
                {
                    marketMap.Add(marketName, new ThessVeggieMarketDataReader(dataStorageService));
                }
            }

            MarketsComboBox.ItemsSource = marketNames;
            MarketsComboBox.SelectedIndex = 0;
        }

        private MarketDataReader GetSelectedMarketDataReader()
        {
            if (MarketsComboBox.SelectedIndex < 0) return null;

            string selectedMarketName = MarketsComboBox.SelectedItem.ToString();
            foreach (KeyValuePair<string, MarketDataReader> marketEntry in marketMap)
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
            MarketDataReader marketDataReader = GetSelectedMarketDataReader();
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
            MarketDataReader marketDataReader = GetSelectedMarketDataReader();
            if (marketDataReader == null) return;
            LogTextBox.Text = "";
            string folderPath = @OpenFolderTextBox.Text;
            string selectedMarketName = MarketsComboBox.SelectedItem.ToString();
            DataProcessor dataProcessor = new DataProcessor(dataStorageService);

            Task.Run(() =>
            {    
                string[] priceFiles = Directory.GetFiles(folderPath, "*.xls", SearchOption.AllDirectories);
                DateTime[] days = marketDataReader.ReadMultipleDays(priceFiles);
                dataProcessor.ProcessProductPrices(selectedMarketName, days);
            });
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                TabItem selectedTab = (sender as TabControl).SelectedItem as TabItem;
                if (selectedTab.Header.ToString() == "Available Data")
                {
                    GetAvailableData();
                }
            }
        }

        private void GetAvailableData()
        {
            MarketAvailableData[] availableData = dataStorageService.MetadataDbService.GetAvailablePrices();
            if (availableData == null || availableData.Length == 0) return;

            ObservableCollection<PricePeriod> pricePeriods = new ObservableCollection<PricePeriod>();
            foreach (MarketAvailableData marketData in availableData)
            {
                foreach (DatePeriod datePeriod in marketData.DatePeriods)
                {
                    PricePeriod pricePeriod = new PricePeriod(marketData.Market, datePeriod);
                    pricePeriods.Add(pricePeriod);
                }
            }

            AvailablePricesDataGrid.ItemsSource = pricePeriods;
        }
    }
}
