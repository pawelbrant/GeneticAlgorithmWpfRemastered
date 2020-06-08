using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;

namespace GeneticAlgorithm
{
    /// <summary>
    /// Logika interakcji dla klasy FittingDetailsWindow.xaml
    /// </summary>
    public partial class FittingDetailsWindow : Window, INotifyPropertyChanged
    {
        private GA gA;
        public GA GA
        {
            get
            {
                return gA;
            }
            set
            {
                gA = value;
                OnPropertyChanged("GA");
            }
        }

        public FittingDetailsWindow(GA GAInstance)
        {
            InitializeComponent();
            GA = GAInstance;
            Summary();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GA = (Owner as MainWindow).GetGAs()[0];
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GA = (Owner as MainWindow).GetGAs()[1];
        }
        private void Summary()
        {
            SeriesCollection series = new SeriesCollection();
            LineSeries bestValues = new LineSeries
            {
                Title = "Best Values",
                Values = new ChartValues<double>(),

            };

            LineSeries medianValues = new LineSeries()
            {
                Title = "Median Values",
                Values = new ChartValues<double>(),

            };
            LineSeries meanValues = new LineSeries() {
                Title = "Mean Values",
                Values = new ChartValues<double>(),

            };
            series.Add(bestValues);
            series.Add(medianValues);
            series.Add(meanValues);
            for(int i=0;i<GA.algorithmParameters.Generations;i++)
            {
                series[0].Values.Add(GA.BestValues[i]);
                series[1].Values.Add(GA.MedianValues[i]);
                series[2].Values.Add(GA.MeanValues[i]);
            }
            summaryChart.Series.Add(series[0]);
            summaryChart.Series.Add(series[1]);
            summaryChart.Series.Add(series[2]);
        }
    }
}
