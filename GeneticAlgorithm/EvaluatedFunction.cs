using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;

namespace GeneticAlgorithm
{
    public class EvaluatedFunction
    {
        public Function function { get; set; }

        public EvaluatedFunction(string function)
        {
            this.function = new Function(function);
        }
    }
}
