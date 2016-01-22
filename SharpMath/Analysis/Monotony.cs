using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMath.Analysis
{
    public class Monotony
    {
        public Dictionary<Interval, MonotonyType> MonotonyIntervals { get; set; } = new Dictionary<Interval, MonotonyType>();

        //public Monotony Determine(string term)
        //{
        //    Determine(new Function(term));
        //}

        //public Monotony Determine(Function function)
        //{
        //    var monotony = new Monotony();
        //    var derivation = function.Derive();
        //    double[] roots = derivation.Roots;
        //    double[] poles = derivation.Poles;

        //    var specialValues = roots.ToList().Concat(poles).ToList();
        //    double beginElement = double.NegativeInfinity;
        //    if ((derivation.Domain.SignAreas & Signs.Negative) != Signs.Negative)
        //    {
        //        beginElement = specialValues.Min();
        //        specialValues.Remove(beginElement);
        //    }

        //    // If the value is a root it can neither be a pole, nor infinity and must be included.
        //    var determineIntervalValueOption = new Func<double, IntervalValueOption>(d => roots.Contains(d) ? IntervalValueOption.Include : IntervalValueOption.Exclude);
        //    foreach (var value in specialValues)
        //    {
        //        var interval =
        //            new Interval(new IntervalValue(beginElement, determineIntervalValueOption(beginElement)),
        //                new IntervalValue(value, determineIntervalValueOption(value)));
        //        double difference = 0;

        //        beginElement = value;
        //    }
        //}
    }
}