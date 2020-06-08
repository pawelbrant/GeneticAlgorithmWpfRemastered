using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class Individual
    {
        public int Id { get; }
        public double X { get; }
        public double Y { get; }
        public double F_value { get; }
        public Individual(int id, double x, double y, double f_value)
        {
            Id = id;
            X = x;
            Y = y;
            F_value = f_value;
        }
    }
}
