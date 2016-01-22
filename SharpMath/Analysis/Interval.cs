namespace SharpMath.Analysis
{
    public struct Interval
    {
        public Interval(IntervalValue leftSide, IntervalValue rightSide)
        {
            LeftSide = leftSide;
            RightSide = rightSide;
        }

        public IntervalValue LeftSide { get; set; }
        public IntervalValue RightSide { get; set; }

        public override string ToString()
        {
            return $"{LeftSide.ValueOption.GetEnumDescription()}{LeftSide.Value};{RightSide.Value}{RightSide.ValueOption.GetEnumDescription()}";
        }
    }
}