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
using MahApps.Metro.Controls;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.ComponentModel;
using System.Drawing;
using Syncfusion.Pdf.Tables;
using System.Data;
using System.Security.AccessControl;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GeneticAlgorithm
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
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
        private void AddFunction(object sender, RoutedEventArgs e)
        {
            EvaluatedFunction evaluatedFunction;
            DlgAddFunction dlg = new DlgAddFunction();
            if (dlg.ShowDialog() == true)
            {
                string functionExpression = dlg.functionExpression.Text;
                double xFirstValue = (double)dlg.xFirst.Value;
                double xLastValue = (double)dlg.xLast.Value;
                double yFirstValue = (double)dlg.yFirst.Value;
                double yLastValue = (double)dlg.yLast.Value;
                evaluatedFunction = new EvaluatedFunction(functionExpression, xFirstValue, xLastValue, yFirstValue, yLastValue);
                evaluatedFunctionsList.Add(evaluatedFunction);
                FunctionGrid.Items.Refresh();
                if (FunctionGrid.Columns.Count == 4) FunctionGrid.Columns[3].Visibility = Visibility.Hidden;
            }
        }

        private void AddParameters(object sender, RoutedEventArgs e)
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
                algorithmParameters = new AlgorithmParameters(crossoverProbability, mutationProbability, population, generationsNumber, precision, isMaximumSearching);
                algorithmParametersList.Add(algorithmParameters);
                ICollectionView AlgParams = CollectionViewSource.GetDefaultView(AlgorithmGrid.ItemsSource);
                if (AlgParams != null && AlgParams.CanGroup == true)
                {
                    AlgParams.GroupDescriptions.Clear();
                }
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

        private void CanExecuteRemoveItems(object sender, CanExecuteRoutedEventArgs e)
        {
            if (AlgorithmGrid != null && FunctionGrid != null)
            {
                e.CanExecute = AlgorithmGrid.SelectedItems.Count != 0 || FunctionGrid.SelectedItems.Count != 0;
            }
        }

        private void Edit_Algorithm(object sender, RoutedEventArgs e)
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
        private void CanExecuteEditAlgorithm(object sender, CanExecuteRoutedEventArgs e)
        {
            if (AlgorithmGrid != null)
            {
                e.CanExecute = AlgorithmGrid.SelectedItems.Count != 0;
            }
        }


        private void Fitting(object sender, RoutedEventArgs e)
        {
            genericAlgorithmsList.Clear();
            progress.Value = 0;
            progress.Maximum = FunctionGrid.SelectedItems.Count * AlgorithmGrid.SelectedItems.Count+10;
            foreach (EvaluatedFunction evaluatedFunction in FunctionGrid.SelectedItems)
            {              
                foreach (AlgorithmParameters algorithmParameters in AlgorithmGrid.SelectedItems)
                {
                    progress.Value++;
                    genericAlgorithmsList.Add(new GA(algorithmParameters, evaluatedFunction));
                }
            }
            genericAlgorithmsList.ForEach((ga) =>
            {
                try
                {
                    ga.Fit();
                }
                catch (Exception error)
                {
                    if (MessageBox.Show(error.ToString(), "Fitting function error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                    {
                    }
                }
            });
            resultsList.ItemsSource = genericAlgorithmsList;
            resultsList.Items.Refresh();
            progress.Value += 10;
        }
        private void CanExecuteFit(object sender, CanExecuteRoutedEventArgs e)
        {
            if (AlgorithmGrid != null && FunctionGrid !=null)
            {
                e.CanExecute = AlgorithmGrid.SelectedItems.Count != 0 && FunctionGrid.SelectedItems.Count != 0;
            }
        }

        private void ShowDetails(object sender, RoutedEventArgs e)
        {
            int index = resultsList.SelectedIndex;
            if (index == -1)
                return;
            FittingDetailsWindow details = new FittingDetailsWindow(genericAlgorithmsList[index]);
            details.Owner = this;
            details.ShowDialog();
        }
        private void CanExecuteShowDetails(object sender, CanExecuteRoutedEventArgs e)
        {
            if (resultsList != null)
            {
                e.CanExecute = resultsList.SelectedItems.Count != 0;
            }
        }

        public List<GA> GetGAs()
        {
            return genericAlgorithmsList;
        }

        private void ExportToPDF(object sender, RoutedEventArgs e)
        {
            if (!evaluatedFunctionsList.Any() || !algorithmParametersList.Any())
            {
                if (MessageBox.Show("Add parameters first", "No parameters error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    return;
                }
            }
            using (PdfDocument document = new PdfDocument())
            {
                foreach(var ga in genericAlgorithmsList)
                {
                    
                    //Add a page to the document
                    PdfPage page = document.Pages.Add();

                    ////Create PDF graphics for a page
                    PdfGraphics graphics = page.Graphics;

                    //Set the standard font
                    PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 13);

                    //Draw the text
                    String functionString = "Function: " + 
                        ga.evaluatedFunction.function.getFunctionExpressionString() +
                        " x:<" + ga.evaluatedFunction.xDomain.X.ToString() + 
                        "," + ga.evaluatedFunction.xDomain.Y.ToString() +
                        " > y:<" + ga.evaluatedFunction.yDomain.X.ToString() + 
                        "," + ga.evaluatedFunction.yDomain.Y.ToString()+">";

                    graphics.DrawString(functionString, font, PdfBrushes.Black, new PointF(0, 0));


                    //Draw the text
                    graphics.DrawString("Parameters", font, PdfBrushes.Black, new PointF(0, 20));


                    // Create a PdfLightTable.

                    PdfLightTable pdfLightTable = new PdfLightTable();

                    // Initialize DataTable to assign as DateSource to the light table.

                    DataTable table = new DataTable();

                    //Include columns to the DataTable.

                    table.Columns.Add("Crossover probability");
                    table.Columns.Add("Mutation probability");
                    table.Columns.Add("Population");
                    table.Columns.Add("Generations");
                    table.Columns.Add("Precision");
                    table.Columns.Add("IsMaxSearching");
                    //Include rows to the DataTable.

                    table.Rows.Add(new string[] {
                        ga.algorithmParameters.CrossoverProbability.ToString(),
                        ga.algorithmParameters.MutationProbability.ToString(), 
                        ga.algorithmParameters.Population.ToString(), 
                        ga.algorithmParameters.Generations.ToString(), 
                        ga.algorithmParameters.Precision.ToString(),
                        ga.algorithmParameters.isMaxSearching.ToString()
                    });

                    //Assign data source.

                    pdfLightTable.DataSource = table;
                    pdfLightTable.Style.ShowHeader = true;

                    //Draw PdfLightTable.

                    pdfLightTable.Draw(page, new PointF(0, 40));

                        //Draw the text
                    graphics.DrawString("Summary", font, PdfBrushes.Black, new PointF(0, 60));

                    // Create a PdfLightTable.

                    PdfLightTable summary = new PdfLightTable();

                    // Initialize DataTable to assign as DateSource to the light table.

                    DataTable summaryTable = new DataTable();

                    //Include columns to the DataTable.

                    summaryTable.Columns.Add("Generation");
                    summaryTable.Columns.Add("Best solution");
                    summaryTable.Columns.Add("Median solution");
                    summaryTable.Columns.Add("Mean solution");
                    
                    //Include rows to the DataTable.
                    for(int generation = 0;  generation<ga.algorithmParameters.Generations; generation++)
                    {
                        summaryTable.Rows.Add(new string[] {
                        (generation+1).ToString(),
                        ga.BestValues[generation].ToString(),
                        ga.MedianValues[generation].ToString(),
                        ga.MeanValues[generation].ToString(),
                   });
                    }
                    

                    //Assign data source.

                    summary.DataSource = summaryTable;
                    summary.Style.ShowHeader = true;

                    //Draw PdfLightTable.

                    summary.Draw(page, new PointF(0, 80));
                }

                //Save the document
                document.Save("Output.pdf");

                #region View the Workbook
                //Message box confirmation to view the created document.
                if (MessageBox.Show("Do you want to view the PDF?", "PDF has been created",
                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    try
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start("Output.pdf");
                    }
                    catch (Win32Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                #endregion
            }
        }

        private void CanExecuteExportToPDF(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = genericAlgorithmsList.Any();
        }

        private void Edit_Function(object sender, RoutedEventArgs e)
        {
            DlgAddFunction dlg = new DlgAddFunction();
            EvaluatedFunction evaluatedFunction = FunctionGrid.SelectedItem as EvaluatedFunction;
            int indexOfFunction = evaluatedFunctionsList.IndexOf(evaluatedFunction);
            dlg.functionExpression.Text = evaluatedFunction.Function;
            dlg.xFirst.Value = evaluatedFunction.xDomain.X;
            dlg.xLast.Value = evaluatedFunction.xDomain.Y;
            dlg.yFirst.Value = evaluatedFunction.yDomain.X;
            dlg.yLast.Value = evaluatedFunction.yDomain.Y;
            if (dlg.ShowDialog() == true)
            {
                evaluatedFunction.setxDomain((double)dlg.xFirst.Value, (double)dlg.xLast.Value);
                evaluatedFunction.setyDomain((double)dlg.yFirst.Value, (double)dlg.yLast.Value);
                evaluatedFunctionsList[indexOfFunction] = evaluatedFunction;
                FunctionGrid.Items.Refresh();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
                formatter.Serialize(stream, new Tuple<List<SimpleFunction>, List<AlgorithmParameters>>(SimpleFunction.ListFromEvaluatedFunction(evaluatedFunctionsList), algorithmParametersList));
                stream.Close();
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = null;
                try
                {
                    stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                    Tuple<List<SimpleFunction>, List<AlgorithmParameters>> lists = (Tuple<List<SimpleFunction>, List<AlgorithmParameters>>)formatter.Deserialize(stream);
                    evaluatedFunctionsList.AddRange(SimpleFunction.ConvertToListOfEvaluatedFunction(lists.Item1));
                    algorithmParametersList.AddRange(lists.Item2);
                    stream.Close();
                }
                catch (FileNotFoundException)
                {
                    Debug.WriteLine("FileNotFound");
                }
                catch (SerializationException)
                {
                    Debug.WriteLine("SerializationException");
                    stream.Close();
                }
                AlgorithmGrid.Items.Refresh();
                FunctionGrid.Items.Refresh();
            }
        }

        private void CanExecuteEditFunction(object sender, CanExecuteRoutedEventArgs e)
        {
            if (FunctionGrid != null)
            {
                e.CanExecute = FunctionGrid.SelectedItems.Count != 0;
            }
        }

        private void GroupParameters(object sender, RoutedEventArgs e)
        {
            ICollectionView AlgParams = CollectionViewSource.GetDefaultView(AlgorithmGrid.ItemsSource);
            if (AlgParams != null && AlgParams.CanGroup == true)
            {
                AlgParams.GroupDescriptions.Clear();
                AlgParams.GroupDescriptions.Add(new PropertyGroupDescription("isMaxSearching"));
            }
        }
        private void ClearGroupParameters(object sender, RoutedEventArgs e)
        {
            ICollectionView AlgParams = CollectionViewSource.GetDefaultView(AlgorithmGrid.ItemsSource);
            if (AlgParams != null && AlgParams.CanGroup == true)
            {
                AlgParams.GroupDescriptions.Clear();
            }
        }

        private void FilterParameters(object sender, RoutedEventArgs e)
        {
            DlgFilterParameters dlg = new DlgFilterParameters();
            if (dlg.ShowDialog() == true)
            {
                ListCollectionView collectionView = new ListCollectionView(algorithmParametersList);
                collectionView.Filter = (o) =>
                {
                    AlgorithmParameters algorithmParameters = o as AlgorithmParameters;
                    if(dlg.greaterCrossover.IsChecked!= null)
                    {
                        if ((bool)dlg.greaterCrossover.IsChecked)
                        {
                            if (algorithmParameters.CrossoverProbability <= dlg.crossoverProbability.Value)
                                return false;
                        }
                    }
                    if (dlg.lesserCrossover.IsChecked != null)
                    {
                        if ((bool)dlg.lesserCrossover.IsChecked)
                        {
                            if (algorithmParameters.CrossoverProbability >= dlg.crossoverProbability.Value)
                                return false;
                        }
                    }
                    if (dlg.greaterMutation.IsChecked != null)
                    {
                        if ((bool)dlg.greaterMutation.IsChecked)
                        {
                            if (algorithmParameters.MutationProbability <= dlg.mutationProbability.Value)
                                return false;
                        }
                    }
                    if (dlg.lesserMutation.IsChecked != null)
                    {
                        if ((bool)dlg.lesserMutation.IsChecked)
                        {
                            if (algorithmParameters.MutationProbability >= dlg.mutationProbability.Value)
                                return false;
                        }
                    }
                    if (dlg.greaterPrecision.IsChecked != null)
                    {
                        if ((bool)dlg.greaterPrecision.IsChecked)
                        {
                            if (algorithmParameters.Precision <= dlg.precision.Value)
                                return false;
                        }
                    }
                    if (dlg.lesserPrecision.IsChecked != null)
                    {
                        if ((bool)dlg.lesserPrecision.IsChecked)
                        {
                            if (algorithmParameters.Precision >= dlg.precision.Value)
                                return false;
                        }
                    }
                    if (dlg.greaterPopulation.IsChecked != null)
                    {
                        if ((bool)dlg.greaterPopulation.IsChecked)
                        {
                            if (algorithmParameters.Population <= dlg.individualsNumber.Value)
                                return false;
                        }
                    }
                    if (dlg.lesserPopulation.IsChecked != null)
                    {
                        if ((bool)dlg.lesserPopulation.IsChecked)
                        {
                            if (algorithmParameters.Population >= dlg.individualsNumber.Value)
                                return false;
                        }
                    }
                    if (dlg.greaterGeneration.IsChecked != null)
                    {
                        if ((bool)dlg.greaterGeneration.IsChecked)
                        {
                            if (algorithmParameters.Generations <= dlg.generationsNumber.Value)
                                return false;
                        }
                    }
                    if (dlg.lesserGeneration.IsChecked != null)
                    {
                        if ((bool)dlg.lesserGeneration.IsChecked)
                        {
                            if (algorithmParameters.Generations >= dlg.generationsNumber.Value)
                                return false;
                        }
                    }
                    if (dlg.minimumSearch.IsChecked != null)
                    {
                        if ((bool)dlg.minimumSearch.IsChecked)
                        {
                            if (algorithmParameters.isMaxSearching)
                                return false;
                        }
                    }
                    if (dlg.maximumSearch.IsChecked != null)
                    {
                        if ((bool)dlg.maximumSearch.IsChecked)
                        {
                            if (!algorithmParameters.isMaxSearching)
                                return false;
                        }
                    }
                    return true;
                };
                AlgorithmGrid.ItemsSource = collectionView;
            }
        }

        private void ClearFilterParameters(object sender, RoutedEventArgs e)
        {
            AlgorithmGrid.ItemsSource = algorithmParametersList;
        }
    }
    [ValueConversion(typeof(Boolean), typeof(String))]
    public class CompleteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isMaxSearching = (bool)value;
            if (isMaxSearching)
                return "Maximum search";
            else
                return "Minimum search";
                
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strIsMaxSearching = (string)value;
            if (strIsMaxSearching == "Maximum search")
                return false;
            else
                return true;
        }
    }

}
