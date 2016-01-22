namespace SharpMath.Analysis
{
    public struct IntervalValue
    {
        public IntervalValue(double value, IntervalValueOption valueOption)
        {
            Value = value;
            ValueOption = valueOption;
        }

       public double Value { get; set; }
       public IntervalValueOption ValueOption { get; set; } 
    }
}
