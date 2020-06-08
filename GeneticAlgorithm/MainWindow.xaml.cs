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

        private void Fitting_click(object sender, RoutedEventArgs e)
        {
            genericAlgorithmsList.Clear();
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
            Parallel.ForEach(genericAlgorithmsList, (ga) =>
            {
                ga.Fit();
            });

            progress.Value += 10;
        }

        private void ShowDetails_Click(object sender, RoutedEventArgs e)
        {

            FittingDetailsWindow dlg = new FittingDetailsWindow(genericAlgorithmsList[0]);
            dlg.Owner = this;
            if (dlg.ShowDialog() == true)
            {

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

                        //Exit
                        Close();
                    }
                    catch (Win32Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                else
                    Close();
                #endregion
            }
        }

        private void Edit_Function(object sender, RoutedEventArgs e)
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
    }
}
