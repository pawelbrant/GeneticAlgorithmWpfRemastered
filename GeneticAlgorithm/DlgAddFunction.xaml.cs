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
    /// Logika interakcji dla klasy DlgAddFunction.xaml
    /// </summary>
    public partial class DlgAddFunction : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        virtual protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private double xFirstValue;
        private double xLastValue;

        public double FirstValue
        {
            get { return xFirstValue; }
            set
            {
                xFirstValue = value;
                OnPropertyChanged("FirstValue");
            }
        }

        public double LastValue
        {
            get { return xLastValue; }
            set
            {
                if(value < FirstValue)
                {
                    throw new ArgumentException("End of x domain should be greater than a beginning ");
                }
                xLastValue = value;
                OnPropertyChanged("LastValue");
            }
        }

        public DlgAddFunction()
        {
            InitializeComponent();
        }

        private void AddFunction(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
