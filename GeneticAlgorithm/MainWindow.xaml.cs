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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeneticAlgorithm
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddFunctionBtn_Click(object sender, RoutedEventArgs e)
        {
            DlgAddFunction dlg = new DlgAddFunction();
            if (dlg.ShowDialog() == true)
            {
                string functionExpression = dlg.functionExpression.Text;
                double xFirstValue = (double)dlg.xFirstValue.Value;
                double xFLastValue = (double)dlg.xLastValue.Value;
                double yFirstValue = (double)dlg.yFirstValue.Value;
                double yLastValue = (double)dlg.yLastValue.Value;


            }
        }

        private void AddParametersBtn_Click(object sender, RoutedEventArgs e)
        {
            DlgAddParameters dlg = new DlgAddParameters();
            if (dlg.ShowDialog() == true)
            {
                double crossoverProbability = (double)dlg.crossoverProbability.Value;
                double mutationProbability = (double)dlg.mutationProbability.Value;
                int population = (int)dlg.individualsNumber.Value;
                int generationsNumber = (int)dlg.generationsNumber.Value;
            }
        }
    }
}
