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
using LiveCharts.Defaults;
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
        public int Generation { get; set; } = 0;
        private SeriesCollection series { get; set; }
        private SeriesCollection heatseries { get; set; }
        private List<Individual> Individuals { get; set; }
        private List<Individual> plotIndividuals { get; set; }
        public FittingDetailsWindow(GA GAInstance)
        {
            InitializeComponent();
            
            GA = GAInstance;
            series = new SeriesCollection();
            heatseries = new SeriesCollection();
            Summary();
            
            Reload_Individuals();
            DrawIndividuals();
            DrawHeatMap();
        }

        public event PropertyChangedEventHandler PropertyChanged;
       

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void Reload_Individuals()
        {
            Individuals = new List<Individual>();
            int id = 1;
            for (int i = 0; i < GA.algorithmParameters.Population; i++)
            {
                Individuals.Add(new Individual(
                    id,
                    GA.ChromosomeValues[Generation, 0, i],
                    GA.ChromosomeValues[Generation, 1, i],
                    GA.ChromosomeValues[Generation, 2, i]
                    ));
                id++;
            }
            GenerationsGrid.ItemsSource = Individuals;
        }
        private void Reload_PlotIndividuals(int generation)
        {
            plotIndividuals = new List<Individual>();
            int id = 1;
            for (int i = 0; i < GA.algorithmParameters.Population; i++)
            {
                plotIndividuals.Add(new Individual(
                    id,
                    GA.ChromosomeValues[generation, 0, i],
                    GA.ChromosomeValues[generation, 1, i],
                    GA.ChromosomeValues[generation, 2, i]
                    ));
                id++;
            }
           
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
        private void DrawHeatMap()
        {
            
            HeatSeries heatSeries = new HeatSeries();
            ChartValues<HeatPoint> Values = new ChartValues<HeatPoint>();
            int xStart=(int)GA.evaluatedFunction.xDomain.X, 
                xEnd= (int)GA.evaluatedFunction.xDomain.Y, 
                yStart= (int)GA.evaluatedFunction.yDomain.X,
                yEnd= (int)GA.evaluatedFunction.yDomain.Y;

            for(int i =xStart;i<xEnd;i++)
            {
                for(int j=yStart;j<yEnd;j++)
                {
                    Values.Add(new HeatPoint(i,j,GA.fitnessFunction(i,j)));
                }
            }
            heatSeries.Values = Values;
            heatSeries.Title = "f(x,y) = ";
            heatseries.Add(heatSeries);

            HeatMap.Series.Add(heatseries[0]);
        }
        public void DrawIndividuals()
        {
            
            SeriesCollection series = new SeriesCollection();
            int generation = (int)GenerationSlider2.Value;
            LineSeries individuals = new LineSeries()
            {
                Title = "Func. Value=",
                Values = new ChartValues<double>()
            };
            Reload_PlotIndividuals(generation-1);
            series.Add(individuals);
            for(int i=0;i<plotIndividuals.Count;i++)
            {
                series[0].Values.Add(plotIndividuals[i].Function_value);
            }         
            if (IndividualsPlot.Series.Count >0)
            {
                IndividualsPlot.Series.RemoveAt(0);              
            }
            series.Add(individuals);
            IndividualsPlot.Series.Add(series[0]);
        }
        private void GenerationSlider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GA is null)
                return;
            Generation = (int)e.NewValue - 1;
            Reload_Individuals();
            
        }

        private void GenerationSlider_DrawPlot(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GA is null)
                return;
            DrawIndividuals();
        }
    }
}
