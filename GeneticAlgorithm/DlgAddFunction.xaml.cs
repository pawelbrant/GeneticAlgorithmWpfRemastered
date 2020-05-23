using System;
using System.Collections.Generic;
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
    public partial class DlgAddFunction : Window
    {
        public DlgAddFunction()
        {
            InitializeComponent();
        }

        private void AddFunction(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
