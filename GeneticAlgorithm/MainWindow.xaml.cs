using org.mariuszgromada.math.mxparser;
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
            FunctionGrid.ItemsSource = evaluatedFunctionsList;
            AlgorithmGrid.ItemsSource = algorithmParametersList;
        }
        private List<EvaluatedFunction> evaluatedFunctionsList = new List<EvaluatedFunction>();
        private List<AlgorithmParameters> algorithmParametersList = new List<AlgorithmParameters>();
        private List<GA> genericAlgorithmsList = new List<GA>();
        private void AddFunctionBtn_Click(object sender, RoutedEventArgs e)
        {
            EvaluatedFunction evaluatedFunction;
            DlgAddFunction dlg = new DlgAddFunction();
            if (dlg.ShowDialog() == true)
            {
                string functionExpression = dlg.functionExpression.Text;
                double xFirstValue = (double)dlg.xFirstValue.Value;
                double xLastValue = (double)dlg.xLastValue.Value;
                double yFirstValue = (double)dlg.yFirstValue.Value;
                double yLastValue = (double)dlg.yLastValue.Value;
                evaluatedFunction = new EvaluatedFunction(functionExpression, xFirstValue, xLastValue, yFirstValue, yLastValue);
                evaluatedFunctionsList.Add(evaluatedFunction);
                FunctionGrid.Items.Refresh();
                
            }
        }

        private void AddParametersBtn_Click(object sender, RoutedEventArgs e)
        {
            DlgAddParameters dlg = new DlgAddParameters();
            AlgorithmParameters algorithmParameters;
            if (dlg.ShowDialog() == true)
            {
                double crossoverProbability = (double)dlg.crossoverProbability.Value;
                double mutationProbability = (double)dlg.mutationProbability.Value;
                int population = (int)dlg.individualsNumber.Value;
                int generationsNumber = (int)dlg.generationsNumber.Value;
                int precision = (int)dlg.precision.Value;
                bool isMaximumSearching = (bool)dlg.maximumSearch.IsChecked;
                algorithmParameters = new AlgorithmParameters(crossoverProbability, mutationProbability, population, generationsNumber, precision,isMaximumSearching);
                algorithmParametersList.Add(algorithmParameters);
                AlgorithmGrid.Items.Refresh();
            }
        }

        private void Remove_Items(object sender, RoutedEventArgs e)
        {
            foreach(EvaluatedFunction evaluatedFunction in FunctionGrid.SelectedItems)
            {
                evaluatedFunctionsList.Remove(evaluatedFunction);
            }
            foreach(AlgorithmParameters algorithmParameters in AlgorithmGrid.SelectedItems)
            {
                algorithmParametersList.Remove(algorithmParameters);
            }
            FunctionGrid.Items.Refresh();
            AlgorithmGrid.Items.Refresh();
        }
        private void Edit_Function(object sender, MouseButtonEventArgs e)
        {
            DlgAddFunction dlg = new DlgAddFunction();
            EvaluatedFunction evaluatedFunction = FunctionGrid.SelectedItem as EvaluatedFunction;
            int indexOfFunction = evaluatedFunctionsList.IndexOf(evaluatedFunction);
            dlg.functionExpression.Text = evaluatedFunction.function.ToString();
            dlg.xFirstValue.Value = evaluatedFunction.xDomain.X;
            dlg.xLastValue.Value = evaluatedFunction.xDomain.Y;
            dlg.yFirstValue.Value = evaluatedFunction.yDomain.X;
            dlg.yLastValue.Value = evaluatedFunction.yDomain.Y;
            if (dlg.ShowDialog() == true)
            {
                evaluatedFunction.setxDomain((double)dlg.xFirstValue.Value, (double)dlg.xLastValue.Value);
                evaluatedFunction.setyDomain((double)dlg.yFirstValue.Value, (double)dlg.yLastValue.Value);
                evaluatedFunctionsList[indexOfFunction] = evaluatedFunction;
                FunctionGrid.Items.Refresh();
            }
        }
        private void Edit_Algorithm(object sender, MouseButtonEventArgs e)
        {
            DlgAddParameters dlg = new DlgAddParameters();
            AlgorithmParameters algorithmParameters = AlgorithmGrid.SelectedItem as AlgorithmParameters;
            int indexOfAlgorithm = algorithmParametersList.IndexOf(algorithmParameters);
            dlg.crossoverProbability.Value = algorithmParameters.CrossoverProbability;
            dlg.mutationProbability.Value = algorithmParameters.MutationProbability;
            dlg.individualsNumber.Value = algorithmParameters.Population;
            dlg.generationsNumber.Value = algorithmParameters.Generations;
            dlg.precision.Value = algorithmParameters.Precision;
            dlg.maximumSearch.IsChecked = algorithmParameters.isMaxSearching;
            if (dlg.ShowDialog() == true)
            {
                algorithmParameters.CrossoverProbability = (double)dlg.crossoverProbability.Value;
                algorithmParameters.MutationProbability = (double)dlg.mutationProbability.Value;
                algorithmParameters.Population = (int)dlg.individualsNumber.Value;
                algorithmParameters.Generations = (int)dlg.generationsNumber.Value;
                algorithmParameters.Precision = (int)dlg.precision.Value;
                algorithmParameters.isMaxSearching = (bool)dlg.maximumSearch.IsChecked;
                algorithmParametersList[indexOfAlgorithm] = algorithmParameters;
                AlgorithmGrid.Items.Refresh();
            }
        }

        private void Fitting_click(object sender, RoutedEventArgs e)
        {
            progress.Value = 0;
            progress.Maximum = evaluatedFunctionsList.Count * AlgorithmGrid.SelectedItems.Count+10;
            foreach (EvaluatedFunction evaluatedFunction in evaluatedFunctionsList)
            {              
                foreach (AlgorithmParameters algorithmParameters in AlgorithmGrid.SelectedItems)
                {
                    progress.Value++;
                    genericAlgorithmsList.Add(new GA(algorithmParameters, evaluatedFunction));
                }
            }
            foreach(GA ga in genericAlgorithmsList)
            {
                ga.Fit();
            }
            progress.Value += 10;
        }

        private void ShowDetails_Click(object sender, RoutedEventArgs e)
        {

            FittingDetailsWindow dlg = new FittingDetailsWindow(genericAlgorithmsList[0]);
            if (dlg.ShowDialog() == true)
            {

            }
        }
    }
}
