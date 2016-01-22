using System.ComponentModel;

namespace SharpMath.Analysis
{
    public enum IntervalValueOption
    {
        [Description("[")]
        Include,
        [Description("]")]
        Exclude,
    }
}