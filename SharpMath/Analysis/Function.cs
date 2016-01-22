using System.Globalization;
using SharpMath.Expressions;

namespace SharpMath.Analysis
{
    /// <summary>
    ///     Represents a mathematical function.
    /// </summary>
    public class Function
    {
        public Function(string term)
        {
            
        }

        public string Term { get; set; }

        /// <summary>
        ///     Gets the poles of this <see cref="Function"/>.
        /// </summary>
        public double[] Poles { get; }

        /// <summary>
        ///     Gets the roots of this <see cref="Function"/>.
        /// </summary>
        public double[] Roots { get; }

        /// <summary>
        ///     Gets the <see cref="Domain"/> of this <see cref="Function"/>.
        /// </summary>
        public Domain Domain { get; }

        /// <summary>
        ///     Calculates the derivation of this <see cref="Function"/>.
        /// </summary>
        /// <returns>The derivation of this <see cref="Function"/>.</returns>
        public Function Derive()
        {
            return null;
        }

        /// <summary>
        ///     Gets the slope of the tangent that touches the <see cref="Point"/> with the specified x-coordinate.
        /// </summary>
        /// <param name="xValue">The x-coordinate value of the <see cref="Point"/> the tangent touches.</param>
        /// <returns>The slope of the <see cref="Point"/>.</returns>
        public double Derive(double xValue)
        {
            var parser = new Parser(Derive().Term.Replace("x", xValue.ToString(CultureInfo.InvariantCulture)));
            return parser.Evaluate();
        }
    }
}