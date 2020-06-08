using org.mariuszgromada.math.mxparser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class GA : INotifyPropertyChanged
    {
        public GA(AlgorithmParameters algorithmParameters, EvaluatedFunction evaluatedFunction)
        {
            this.algorithmParameters = algorithmParameters;
            this.evaluatedFunction = evaluatedFunction;
            this.Chromosomes = new BitArray[algorithmParameters.Generations, 2, algorithmParameters.Population];
            this.ChromosomeValues = new double[algorithmParameters.Generations, 3, algorithmParameters.Population];

            this.BestValues = new double[algorithmParameters.Generations];
            this.MeanValues = new double[algorithmParameters.Generations];
            this.MedianValues = new double[algorithmParameters.Generations];
            this.random = new Random();
            // generate first population

            for(int individualIndex=0; individualIndex<algorithmParameters.Population; individualIndex++)
            {
                int x = generateRandomValue();
                int y = generateRandomValue();
                System.Diagnostics.Debug.WriteLine("X: " + x.ToString());
                System.Diagnostics.Debug.WriteLine("Y: " + y.ToString());
                BitArray x_genome = int2Binary(x, this.algorithmParameters.Precision);
                BitArray y_genome = int2Binary(y, this.algorithmParameters.Precision);
                this.Chromosomes[0,0,individualIndex] = x_genome;
                this.Chromosomes[0,1,individualIndex] = y_genome;
            }

        }

        public BitArray[,,] Chromosomes { get; set; } // generation, {x,y}, population
        public double[,,] ChromosomeValues { get; set; } // generation, {x,y,, f(x,y)}, value
        public double[] BestValues { get; set; } // generation
        public double[] MeanValues { get; set; } // generation
        public double[] MedianValues { get; set; } // generation
        public Random random { get; set; }

        public AlgorithmParameters algorithmParameters { get; }
        public EvaluatedFunction evaluatedFunction { get; }


        public static int binary2Int(BitArray bitArray)
        {
            int result = 0;
            for (int index = 0; index < bitArray.Length; index++)
            {
                int value = bitArray[index] ? 1 : 0;
                result += (int)Math.Pow(2, index) * value;
            }
            return result;
        }

        public static BitArray int2Binary(int value, int precision)
        {
            BitArray result = new BitArray(precision);
            for(int i = 0; i<precision; i++)
            {
                if(value%2==1)
                {
                    result[i] = true;
                }
                else
                {
                    result[i] = false;
                }
                value = value / 2;
            }
            return result;
        }

        public int generateRandomValue()
        {
            return this.random.Next(0, (int)Math.Pow(2, algorithmParameters.Precision));
        }

        public double[] relaxationFuncion(int xValue, int yValue)
        {
            double[] results = new double[2];
            results[0] = (xValue * (evaluatedFunction.xDomain.Y - evaluatedFunction.xDomain.X) / Math.Pow(2, algorithmParameters.Precision)) + evaluatedFunction.xDomain.X;
            results[1] = (yValue * (evaluatedFunction.yDomain.Y - evaluatedFunction.yDomain.X) / Math.Pow(2, algorithmParameters.Precision)) + evaluatedFunction.yDomain.X;
            return results;
        }

        public double fitnessFunction(double xValue, double yValue)
        {
            String xAltered = xValue.ToString().Replace(',', '.');
            String yAltered = yValue.ToString().Replace(',', '.');
            Argument x = new Argument("x = " + xAltered );
            Argument y = new Argument("y = " + yAltered);
            Expression result = new Expression("f(x,y)", evaluatedFunction.function, x, y);
            return result.calculate();
        }

        public BitArray mutation(BitArray chromosome)
        {
            Random random = new Random();
            double drawnValue = random.NextDouble();

            if (drawnValue < algorithmParameters.MutationProbability)
            {
                int mutationIndex = random.Next(0, chromosome.Length - 1);
                chromosome[mutationIndex] = !chromosome[mutationIndex];
            }
            return chromosome;
        }

        public BitArray[] crossover(BitArray firstParent, BitArray secondParent)
        {
            Random random = new Random();
            double drawnValue = random.NextDouble();
            BitArray firstOffspring = new BitArray(firstParent.Length);
            BitArray secondOffspring = new BitArray(secondParent.Length);

            if (drawnValue < algorithmParameters.CrossoverProbability)
            {
                int crossoverIndex = random.Next(0, firstParent.Length - 1);

                firstOffspring = CopySlice(firstParent, 0, crossoverIndex);
                firstOffspring = CopySlice(secondParent, crossoverIndex, secondParent.Length - crossoverIndex);

                secondOffspring = CopySlice(secondParent, 0, crossoverIndex);
                secondOffspring = CopySlice(firstParent, crossoverIndex, firstParent.Length - crossoverIndex);
            }
            BitArray[] results = new BitArray[2];
            results[0] = firstOffspring;
            results[1] = secondOffspring;
            return results;
        }

        public static BitArray CopySlice(BitArray source, int offset, int size)
        {
            BitArray results = new BitArray(source.Length);
            for (int i = 0; i < size; i++)
            {
                results[offset + i] = source[offset + i];
            }
            return results;
        }

        public BitArray[,] SelectParents(BitArray[,]parents, double[] fitFunctionValue)
        {
            BitArray[,] selectedParents = new BitArray[2, algorithmParameters.Population];
            double[] expFitFunc = new double[fitFunctionValue.Length];
            for(int index = 0; index<fitFunctionValue.Length; index++)
            {
                expFitFunc[index] = Math.Exp(fitFunctionValue[index]);
            }
            for(int parentIndex = 0; parentIndex<algorithmParameters.Population; parentIndex++)
            {
                double drawnValue = this.random.NextDouble() * expFitFunc.Sum();
                for(int funcIndex=0; funcIndex<expFitFunc.Length; funcIndex++)
                {
                    drawnValue -= expFitFunc[funcIndex];
                    if(drawnValue < 0)
                    {
                        selectedParents[0, parentIndex] = parents[0, funcIndex];
                        selectedParents[1, parentIndex] = parents[1, funcIndex];
                        break;
                    }
                }
            }
            return selectedParents;
        }

        public void Fit()
        {
            for (int generation = 0; generation < algorithmParameters.Generations; generation++)
            {
                double[] fxvalues = new double[algorithmParameters.Population];
                //For each element of population
                for (int populationIndex = 0; populationIndex < algorithmParameters.Population; populationIndex++)
                {
                    //Convert binary to int
                    int xInt = binary2Int(Chromosomes[generation, 0, populationIndex]);
                    int yInt = binary2Int(Chromosomes[generation, 1, populationIndex]);

                    //Relax int to double
                    double[] relaxedValues = relaxationFuncion(xInt, yInt);

                    //Calculate function
                    double fxy = fitnessFunction(relaxedValues[0], relaxedValues[1]);

                    ChromosomeValues[generation, 0, populationIndex] = relaxedValues[0];
                    ChromosomeValues[generation, 1, populationIndex] = relaxedValues[1];
                    ChromosomeValues[generation, 2, populationIndex] = fxy;
                    fxvalues[populationIndex] = fxy;
                }
                //Get best, mean, median value
                if (algorithmParameters.isMaxSearching)
                {
                    BestValues[generation] = fxvalues.Max();
                }
                else
                {
                    BestValues[generation] = fxvalues.Min();
                }
                MeanValues[generation] = fxvalues.Average();
                MedianValues[generation] = getMedian(fxvalues);
                BitArray[,] parents = new BitArray[2, algorithmParameters.Population];
                
                if(generation != algorithmParameters.Generations-1)
                {
                    //Select parents for next generation
                    for (int populationIndex = 0; populationIndex < algorithmParameters.Population; populationIndex++)
                    {
                        parents[0, populationIndex] = Chromosomes[generation, 0, populationIndex];
                        parents[1, populationIndex] = Chromosomes[generation, 1, populationIndex];
                    }
                    if (!algorithmParameters.isMaxSearching)
                    {
                        fxvalues = fxvalues.Select(x => x * -1).ToArray();
                    }
                    parents = SelectParents(parents, fxvalues);
                    for (int populationIndex = 0; populationIndex < algorithmParameters.Population; populationIndex+=2)
                    {
                        BitArray[] selectedXPair = crossover(parents[0, populationIndex], parents[0, populationIndex + 1]);
                        BitArray[] selectedYPair = crossover(parents[1, populationIndex], parents[1, populationIndex + 1]);
                        selectedXPair[0] = mutation(selectedXPair[0]);
                        selectedXPair[1] = mutation(selectedXPair[1]);
                        selectedYPair[0] = mutation(selectedYPair[0]);
                        selectedYPair[1] = mutation(selectedYPair[1]);
                        Chromosomes[generation+1, 0, populationIndex] = selectedXPair[0];
                        Chromosomes[generation+1, 1, populationIndex] = selectedYPair[0];
                        Chromosomes[generation+1, 0, populationIndex+1] = selectedXPair[1];
                        Chromosomes[generation+1, 1, populationIndex+1] = selectedYPair[1];
                    }
                }
            }
        }
        public double getMedian(double[] values)
        {
            int numberCount = values.Count();
            int halfIndex = values.Count() / 2;
            var sortedNumbers = values.OrderBy(n => n);
            double median;
            if ((numberCount % 2) == 0)
            {
                median = (sortedNumbers.ElementAt(halfIndex) +
                    sortedNumbers.ElementAt(halfIndex+1)) / 2;
            }
            else
            {
                median = sortedNumbers.ElementAt(halfIndex);
            }
            return median;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,
                new PropertyChangedEventArgs(property));
        }

        public string geneticAlgorithmResults
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Results for function ");
                sb.Append(evaluatedFunction.function.getFunctionExpressionString());
                sb.Append(" x: <");
                sb.Append(evaluatedFunction.xDomain.X.ToString());
                sb.Append(",");
                sb.Append(evaluatedFunction.xDomain.Y.ToString());
                sb.Append("> y: <");
                sb.Append(evaluatedFunction.yDomain.X.ToString());
                sb.Append(",");
                sb.Append(evaluatedFunction.yDomain.Y.ToString());
                sb.Append("> CP: ");
                sb.Append(algorithmParameters.CrossoverProbability.ToString());
                sb.Append(" MP: ");
                sb.Append(algorithmParameters.MutationProbability.ToString());
                sb.Append(" Pop: ");
                sb.Append(algorithmParameters.Population.ToString());
                sb.Append(" Gen: ");
                sb.Append(algorithmParameters.Generations.ToString());
                sb.Append(" Prec: ");
                sb.Append(algorithmParameters.Precision.ToString());
                sb.Append(" Max: ");
                sb.Append(algorithmParameters.isMaxSearching.ToString());
                return sb.ToString();
            }
        }
    }
}
