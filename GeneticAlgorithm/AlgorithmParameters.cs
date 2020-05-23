using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class AlgorithmParameters
    {
        public AlgorithmParameters(float crossoverProbability, float mutationProbability, int population, int generations, bool isMaxSearching)
        {
            CrossoverProbability = crossoverProbability;
            MutationProbability = mutationProbability;
            Population = population;
            Generations = generations;
            this.isMaxSearching = isMaxSearching;
        }

        public double CrossoverProbability { get; set; }
        public double MutationProbability { get; set; }
        public int Population { get; set; }
        public int Generations { get; set; }

        public bool isMaxSearching { get; set; }
        
    }
}
