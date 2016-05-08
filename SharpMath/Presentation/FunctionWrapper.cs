using System;
using System.Collections.Generic;

namespace SharpMath.Presentation
{
    public class FunctionWrapper
    {
        public FunctionWrapper(Func<double, double> function)
        {
            Values = new Dictionary<double, double>();
            BaseFunction = function;
        }

        public Func<double, double> BaseFunction { get; }
        public Dictionary<double, double> Values { get; }

        public double GetValue(double x)
        {
            if (Values.ContainsKey(x))
                return Values[x];
            var resultValue = BaseFunction(x);
            Values.Add(x, resultValue);
            return resultValue;
        }
    }
}