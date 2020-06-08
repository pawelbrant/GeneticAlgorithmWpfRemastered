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
        private List<Individual> Individuals { get; set; }

        public FittingDetailsWindow(GA GAInstance)
        {
            InitializeComponent();
            GA = GAInstance;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void Reload_Individuals()
        {

            GenerationsGrid.ItemsSource = Individuals;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GA = (Owner as MainWindow).GetGAs()[0];
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GA = (Owner as MainWindow).GetGAs()[1];
        }
    }
}
