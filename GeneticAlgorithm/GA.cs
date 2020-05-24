using org.mariuszgromada.math.mxparser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class GA
    {
        public GA(AlgorithmParameters algorithmParameters, EvaluatedFunction evaluatedFunction)
        {
            this.algorithmParameters = algorithmParameters;
            this.evaluatedFunction = evaluatedFunction;
            this.Chromosomes = new BitArray[algorithmParameters.Generations, 2, algorithmParameters.Population];
            this.ChromosomeValues = new double[algorithmParameters.Generations, 3, algorithmParameters.Population];

            this.BestValues = new double[algorithmParameters.Generations, 3];
            this.MeanValues = new double[algorithmParameters.Generations, 3];
            this.MedianValues = new double[algorithmParameters.Generations, 3];

            // generate first population

            for(int individualIndex=0; individualIndex<algorithmParameters.Population; individualIndex++)
            {
                int x = generateRandomValue();
                int y = generateRandomValue();
                BitArray x_genome = int2Binary(x);
                BitArray y_genome = int2Binary(y);
                this.Chromosomes[0, 0, individualIndex] = x_genome;
                this.Chromosomes[0, 1, individualIndex] = y_genome;
            }

        }

        public BitArray[][][] Chromosomes { get; set; } // generation, {x,y}, population
        public double[][][] ChromosomeValues { get; set; } // generation, {x,y,, f(x,y)}, value
        public double[] BestValues { get; set; } // generation
        public double[] MeanValues { get; set; } // generation
        public double[] MedianValues { get; set; } // generation

        public AlgorithmParameters algorithmParameters { get; }
        public EvaluatedFunction evaluatedFunction { get; }


        public static int binary2Int(BitArray bitArray)
        {
            int result = 0;
            for (int index = 0; index < bitArray.Length; index++)
            {
                int value = bitArray[index] ? 1 : 0;
                result += (int)Math.Pow(2, bitArray.Length - 1 - index) * value;
            }
            return result;
        }

        public static BitArray int2Binary(int value)
        {
            return new BitArray(new int[] { value });
        }

        public int generateRandomValue()
        {
            Random random = new Random();
            return random.Next(0, (int)Math.Pow(2, algorithmParameters.Precision));
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
            Argument x = new Argument("x = " + xValue.ToString());
            Argument y = new Argument("y = " + yValue.ToString());
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
            Random random = new Random();
            for(int parentIndex = 0; parentIndex<algorithmParameters.Population; parentIndex++)
            {
                double drawnValue = random.NextDouble() * expFitFunc.Sum();
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
            for(int generation = 0; generation<algorithmParameters.Generations; generation++)
            {
                //For each element of population
                for(int populationIndex = 0; populationIndex<algorithmParameters.Population; populationIndex++)
                {
                    //Convert binary to int
                    int xInt = binary2Int(Chromosomes[generation, 0, populationIndex]);
                    int yInt = binary2Int(Chromosomes[generation, 1, populationIndex]);

                    //Relax int to double
                    double[] relaxedValues = relaxationFuncion(xInt, yInt);

                    //Calculate function
                    double fxy = fitnessFunction(relaxedValues[0], relaxedValues[1]);

                    ChromosomeValues[generation][0][populationIndex] = relaxedValues[0];
                    ChromosomeValues[generation][1][populationIndex] = relaxedValues[1];
                    ChromosomeValues[generation][2][populationIndex] = fxy;

                    //Get best, mean, median value
                }
                ChromosomeValues[generation][2]
            }
        }
    }
}
