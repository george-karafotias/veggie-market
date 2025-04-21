using LiveCharts.Wpf.Charts.Base;

namespace VeggieMarketUi
{
    public class VeggieChart
    {
        private string title;
        private Chart chart;

        public VeggieChart(string title, Chart chart) 
        {
            this.title = title;
            this.chart = chart;
        }

        public string Title { get { return title; } }
        public Chart Chart { get { return chart; } }
    }
}
