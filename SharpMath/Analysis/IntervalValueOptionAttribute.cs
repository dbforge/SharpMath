using System;

namespace SharpMath.Analysis
{
    public class IntervalValueOptionAttribute : Attribute
    {
        public IntervalValueOption ValueOption { get; set; }

        public IntervalValueOptionAttribute(IntervalValueOption valueOption)
        {
            ValueOption = valueOption;
        }
    }
}
