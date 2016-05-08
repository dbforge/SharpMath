// Author: Dominic Beger (Trade/ProgTrade) 2016

namespace SharpMath.Equations
{
    /// <summary>
    ///     Represents a linear equation.
    /// </summary>
    public class LinearEquation
    {
        public LinearEquation() : this(new double[] {}, 0)
        {
        }

        public LinearEquation(double[] coefficients, double result)
        {
            Coefficients = coefficients;
            Result = result;
        }
        public LinearEquation(double result, params double[] coefficients)
        {
            Coefficients = coefficients;
            Result = result;
        }

        public double[] Coefficients { get; set; }
        public double Result { get; set; }
    }
}