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
    public partial class DlgAddFunction : Window, IDataErrorInfo
    {

        public double XFirstValue { get; set; }
        public double XLastValue { get; set; }
        public double YFirstValue { get; set; }
        public double YLastValue { get; set; }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                switch(columnName)
                {
                    case "XFirstValue":
                        if (this.XFirstValue < 0)
                            return "Value less than 0";
                        break;
                }
                return string.Empty;
            }
        }

        public DlgAddFunction()
        {
            InitializeComponent();
        }

        private void AddFunction(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(XFirstValue);
            System.Diagnostics.Debug.WriteLine(XLastValue);
            if (XLastValue < XFirstValue || YLastValue < YFirstValue)
            {
                if (MessageBox.Show("End of domain should be greater than a beginning of a domain", "Invalid function domain error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
            }
            DialogResult = true;
        }
    }
}
