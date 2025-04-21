using LiveCharts;

namespace VeggieMarketUi.Models
{
    public class VeggieChartData
    {
        private string title;
        private SeriesCollection seriesCollection;

        public VeggieChartData(string title, SeriesCollection seriesCollection)
        {
            this.title = title;
            this.seriesCollection = seriesCollection;
        }

        public string Title { get { return title; } }
        public SeriesCollection SeriesCollection { get { return seriesCollection; } }
    }
}
