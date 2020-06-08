using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using org.mariuszgromada.math.mxparser;
using Expression = org.mariuszgromada.math.mxparser.Expression;

namespace GeneticAlgorithm
{
    public class EvaluatedFunction
    {
        public string Function
        {
            get
            {
                return function.getFunctionExpressionString();
            }
        }
        public Vector xDomain { get; set; }
        public Vector yDomain { get; set; }
        public Function function { get; set; }
        public string Expression()
        {
            Expression e1 = new Expression("f(x,y)", function);
            return e1.getExpressionString() + "=" + e1.calculate();
        }
        public EvaluatedFunction(string function, double xFirstValue, double xLastValue, double yFirstValue, double yLastValue)
        {
            this.function = new Function(function);
            this.xDomain = new Vector(xFirstValue, xLastValue);
            this.yDomain = new Vector(yFirstValue, yLastValue);
        }
        public void setxDomain(double valueX, double valueY)
        {
            xDomain = new Vector(valueX, valueY);
        }
        public void setyDomain(double valueX, double valueY)
        {
            yDomain = new Vector(valueX, valueY);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
