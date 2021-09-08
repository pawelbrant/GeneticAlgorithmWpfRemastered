using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeneticAlgorithm
{
    [Serializable]
    class SimpleFunction
    {
        private string expression;
        private Vector xDomain;
        private Vector yDomain;
        public SimpleFunction(string expression, Vector xDomain, Vector yDomain)
        {
            this.expression = expression;
            this.xDomain = xDomain;
            this.yDomain = yDomain;
        }
        public static List<SimpleFunction> ListFromEvaluatedFunction(List<EvaluatedFunction> evaluatedFunctions)
        {
            List<SimpleFunction> simpleFunctions = new List<SimpleFunction>();
            foreach (var item in evaluatedFunctions)
            {
                simpleFunctions.Add(new SimpleFunction(item.Function, item.xDomain, item.yDomain));
            }
            return simpleFunctions;
        }

        public static List<EvaluatedFunction> ConvertToListOfEvaluatedFunction(List<SimpleFunction> simpleFunctions)
        {
            List<EvaluatedFunction> evaluatedFunctions = new List<EvaluatedFunction>();
            foreach (var item in simpleFunctions)
            {
                evaluatedFunctions.Add(new EvaluatedFunction("f(x,y)=" + item.expression, item.xDomain.X, item.xDomain.Y, item.yDomain.X, item.yDomain.Y));
            }
            return evaluatedFunctions;
        }
    }
}
