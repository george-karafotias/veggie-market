using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Media;
using VeggieMarketDataStore.Models;
using VeggieMarketUi.Models;

namespace VeggieMarketUi
{
    public class ChartCreator
    {
        public enum ChartGroup
        {
            Market,
            Product,
            Year,
            Price,
            NoGroup
        }

        private List<VeggieChartData> GroupChartsByMarket(List<string> selectedPriceTypes, PriceRetrievalParameters priceRetrievalParameters, List<ProductPrice> retrievedPrices)
        {
            List<int> selectedYears = DateHelper.CalculateFullYearsList(priceRetrievalParameters.FromDate, priceRetrievalParameters.ToDate);
            List<VeggieChartData> charts = new List<VeggieChartData>();

            foreach (Market market in priceRetrievalParameters.Markets)
            {
                SeriesCollection seriesCollection = new SeriesCollection();

                foreach (int year in selectedYears)
                {
                    foreach (Product product in priceRetrievalParameters.Products)
                    {
                        foreach (string priceType in selectedPriceTypes)
                        {
                            string seriesTitle = product.ProductName + " " + year + " - " + priceType;
                            List<ProductPrice> filteredPrices = FilterPrices(retrievedPrices, market, product, year);

                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                seriesCollection.Add(CreateLineSeries(seriesTitle, priceType, filteredPrices));
                            });
                        }
                    }
                }

                charts.Add(new VeggieChartData(market.MarketName, seriesCollection));
            }

            return charts;
        }

        private List<VeggieChartData> GroupChartsByProduct(List<string> selectedPriceTypes, PriceRetrievalParameters priceRetrievalParameters, List<ProductPrice> retrievedPrices)
        {
            List<int> selectedYears = DateHelper.CalculateFullYearsList(priceRetrievalParameters.FromDate, priceRetrievalParameters.ToDate);
            List<VeggieChartData> charts = new List<VeggieChartData>();

            foreach (Product product in priceRetrievalParameters.Products)
            {
                SeriesCollection seriesCollection = new SeriesCollection();

                foreach (int year in selectedYears)
                {
                    foreach (Market market in priceRetrievalParameters.Markets)
                    {
                        foreach (string priceType in selectedPriceTypes)
                        {
                            string seriesTitle = market.MarketName + " " + year + " - " + priceType;
                            List<ProductPrice> filteredPrices = FilterPrices(retrievedPrices, market, product, year);

                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                seriesCollection.Add(CreateLineSeries(seriesTitle, priceType, filteredPrices));
                            });
                        }
                    }
                }

                charts.Add(new VeggieChartData(product.ProductName, seriesCollection));
            }

            return charts;
        }

        private List<VeggieChartData> GroupChartsByPrice(List<string> selectedPriceTypes, PriceRetrievalParameters priceRetrievalParameters, List<ProductPrice> retrievedPrices)
        {
            List<int> selectedYears = DateHelper.CalculateFullYearsList(priceRetrievalParameters.FromDate, priceRetrievalParameters.ToDate);
            List<VeggieChartData> charts = new List<VeggieChartData>();

            foreach (string priceType in selectedPriceTypes)
            {
                SeriesCollection seriesCollection = new SeriesCollection();

                foreach (int year in selectedYears)
                {
                    foreach (Market market in priceRetrievalParameters.Markets)
                    {
                        foreach (Product product in priceRetrievalParameters.Products)
                        {
                            string seriesTitle = market.MarketName + " " + product.ProductName + " " + year;
                            List<ProductPrice> filteredPrices = FilterPrices(retrievedPrices, market, product, year);

                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                seriesCollection.Add(CreateLineSeries(seriesTitle, priceType, filteredPrices));
                            });
                        }
                    }
                }

                charts.Add(new VeggieChartData(priceType, seriesCollection));
            }

            return charts;
        }

        private VeggieChartData CreateChart(List<string> selectedPriceTypes, PriceRetrievalParameters priceRetrievalParameters, List<ProductPrice> retrievedPrices)
        {
            SeriesCollection seriesCollection = new SeriesCollection();

            foreach (Market market in priceRetrievalParameters.Markets)
            {
                foreach (Product product in priceRetrievalParameters.Products)
                {
                    foreach (string priceType in selectedPriceTypes)
                    {
                        string seriesTitle = market.MarketName + " " + product.ProductName + " - " + priceType;
                        List<ProductPrice> filteredPrices = FilterPrices(retrievedPrices, market, product);

                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            seriesCollection.Add(CreateLineSeries(seriesTitle, priceType, filteredPrices));
                        });
                    }
                }
            }

            VeggieChartData chartData = new VeggieChartData("Chart", seriesCollection);
            return chartData;
        }

        private LineSeries CreateLineSeries(string title, string priceType, List<ProductPrice> prices)
        {
            Dictionary<string, string> priceTypeDictionary = ProductPrice.GetPriceTypes();
            double?[] priceValues = GetPriceValues(priceTypeDictionary[priceType], prices);
            LineSeries lineSeries = new LineSeries();
            lineSeries.PointGeometry = null;
            lineSeries.Title = title;
            lineSeries.Values = PrepareDataForChart(priceValues);

            return lineSeries;
        }

        private List<ProductPrice> FilterPrices(List<ProductPrice> retrievedPrices, Market market, Product product, int year)
        {
            List<ProductPrice> filteredPrices = new List<ProductPrice>();
            foreach (ProductPrice price in retrievedPrices)
            {
                int priceYear = new DateTime(price.ProductDate).Year;
                if (price.Market.MarketId == market.MarketId && price.Product.ProductId == product.ProductId && priceYear == year)
                {
                    filteredPrices.Add(price);
                }
            }
            return filteredPrices;
        }

        private List<ProductPrice> FilterPrices(List<ProductPrice> retrievedPrices, Market market, Product product)
        {
            List<ProductPrice> filteredPrices = new List<ProductPrice>();
            foreach (ProductPrice price in retrievedPrices)
            {
                if (price.Market.MarketId == market.MarketId && price.Product.ProductId == product.ProductId)
                {
                    filteredPrices.Add(price);
                }
            }
            return filteredPrices;
        }

        public List<VeggieChart> CreateCharts(List<string> selectedPriceTypes, PriceRetrievalParameters priceRetrievalParameters, List<ProductPrice> retrievedPrices, ChartGroup chartGroup)
        {
            List<VeggieChartData> allChartsData = new List<VeggieChartData>();
            if (chartGroup == ChartGroup.Market)
            {
                allChartsData = GroupChartsByMarket(selectedPriceTypes, priceRetrievalParameters, retrievedPrices);
            }
            else if (chartGroup == ChartGroup.Product)
            {
                allChartsData = GroupChartsByProduct(selectedPriceTypes, priceRetrievalParameters, retrievedPrices);
            }
            else if (chartGroup == ChartGroup.Price)
            {
                allChartsData = GroupChartsByPrice(selectedPriceTypes, priceRetrievalParameters, retrievedPrices);
            }
            else
            {
                allChartsData.Add(CreateChart(selectedPriceTypes, priceRetrievalParameters, retrievedPrices));
            }

            List<string> labels = new List<string>();
            for (DateTime date = priceRetrievalParameters.FromDate.Value; date <= priceRetrievalParameters.ToDate.Value; date = date.AddDays(1))
            {
                labels.Add(date.ToString("dd/MM"));
            }
            Func<double, string> formatter = value => new DateTime((long)value).ToString("dd/MM");

            List<VeggieChart> charts = new List<VeggieChart>();
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (VeggieChartData chartData in allChartsData)
                {
                    AxesCollection axisX = new AxesCollection
                    {
                        new Axis
                        {
                            Title = "Date",
                            Labels = labels,
                            LabelFormatter = formatter
                        }
                    };

                    AxesCollection axisY = new AxesCollection
                    {
                        new Axis
                        {
                            Title = "Price"
                        }
                    };

                    CartesianChart cartesianChart = new CartesianChart();
                    cartesianChart.Background = new SolidColorBrush(Colors.White);
                    cartesianChart.Height = 400;
                    cartesianChart.DisableAnimations = true;
                    cartesianChart.Hoverable = false;
                    cartesianChart.DataTooltip = null;
                    cartesianChart.AxisX = axisX;
                    cartesianChart.AxisY = axisY;
                    cartesianChart.Series = chartData.SeriesCollection;
                    cartesianChart.LegendLocation = LegendLocation.Right;

                    charts.Add(new VeggieChart(chartData.Title, cartesianChart));
                }
            });

            return charts;
        }

        private double?[] GetPriceValues(string priceType, IEnumerable<ProductPrice> retrievedPrices)
        {
            double?[] priceValues = new double?[retrievedPrices.Count()];
            int index = 0;
            foreach (ProductPrice productPrice in retrievedPrices)
            {
                priceValues[index] = productPrice.GetPriceType(priceType);
                index++;
            }

            return priceValues;
        }

        private ChartValues<double> PrepareDataForChart(double?[] data)
        {
            ChartValues<double> values = new ChartValues<double>();
            foreach (double? value in data)
            {
                if (!value.HasValue)
                {
                    return new ChartValues<double>();
                }
                values.Add(value.Value);
            }
            return values;
        }
    }
}
