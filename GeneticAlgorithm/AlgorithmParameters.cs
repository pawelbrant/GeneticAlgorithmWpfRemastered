using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class AlgorithmParameters
    {
        public AlgorithmParameters(double crossoverProbability, double mutationProbability, int population, int generations, int precision, bool isMaxSearching)
        {
            CrossoverProbability = crossoverProbability;
            MutationProbability = mutationProbability;
            Population = population;
            Generations = generations;
            Precision = precision;
            this.isMaxSearching = isMaxSearching;
        }

        public double CrossoverProbability { get; set; }
        public double MutationProbability { get; set; }
        public int Population { get; set; }
        public int Generations { get; set; }

        public int Precision { get; set; }
        public bool isMaxSearching { get; set; }
        
    }
}
