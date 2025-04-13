using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Media;
using VeggieMarketDataStore.Models;
using VeggieMarketUi.Models;
using LiveCharts.Wpf.Charts.Base;

namespace VeggieMarketUi
{
    public class ChartCreator
    {
        public List<Chart> CreateCharts(List<string> selectedPriceTypes, PriceRetrievalParameters priceRetrievalParameters, IEnumerable<ProductPrice> retrievedPrices)
        {
            SeriesCollection seriesCollection = new SeriesCollection();
            Dictionary<string, string> priceTypeDictionary = ProductPrice.GetPriceTypes();
            foreach (string priceType in selectedPriceTypes)
            {
                double?[] priceValues = GetPriceValues(priceTypeDictionary[priceType], retrievedPrices);
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    LineSeries lineSeries = new LineSeries();
                    lineSeries.PointGeometry = null;
                    lineSeries.Title = priceType;
                    lineSeries.Values = PrepareDataForChart(priceValues);
                    seriesCollection.Add(lineSeries);
                });
            }

            List<string> labels = new List<string>();
            for (DateTime date = priceRetrievalParameters.FromDate.Value; date <= priceRetrievalParameters.ToDate.Value; date = date.AddDays(1))
            {
                labels.Add(date.ToString("dd/MM"));
            }
            Func<double, string> formatter = value => new DateTime((long)value).ToString("dd/MM");

            List<Chart> charts = new List<Chart>();
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
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
                cartesianChart.Series = seriesCollection;
                cartesianChart.LegendLocation = LegendLocation.Right;

                charts.Add(cartesianChart);
                return charts;
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
