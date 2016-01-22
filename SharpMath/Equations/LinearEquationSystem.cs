using SharpMath.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMath.Equations
{
    /// <summary>
    ///     Represents a linear equation system.
    /// </summary>
    public class LinearEquationSystem
    {
        /// <summary>
        ///     Gets or sets the <see cref="LinearEquation"/>s of this <see cref="LinearEquationSystem"/>.
        /// </summary>
        public List<LinearEquation> Equations { get; set; } = new List<LinearEquation>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearEquationSystem"/> class.
        /// </summary>
        public LinearEquationSystem()
        { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearEquationSystem"/> class.
        /// </summary>
        /// <param name="equations">The <see cref="LinearEquation"/>s that this <see cref="LinearEquationSystem"/> should contain.</param>
        public LinearEquationSystem(IEnumerable<LinearEquation> equations)
        {
            Equations.AddRange(equations);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinearEquationSystem"/> class.
        /// </summary>
        /// <param name="equations">The <see cref="LinearEquation"/>s that this <see cref="LinearEquationSystem"/> should contain.</param>
        public LinearEquationSystem(params LinearEquation[] equations)
        {
            Equations.AddRange(equations);
        }

        /// <summary>
        ///     Solves this <see cref="LinearEquationSystem"/> using the Gauss-Jordan algorithm.
        /// </summary>
        /// <returns>The result of each variable in chronological order.</returns>
        public double[] Solve()
        {
            uint count = (uint)Equations.Count;
            if (count == 0)
                throw new ArgumentException("There must be at least one equation to solve.");

            var coefficientsCount = Equations.Select(item => item.Coefficients).Count();
            if (coefficientsCount == 0 || coefficientsCount != count)
                throw new EquationNotSolvableException("This linear equation system cannot be solved.");

            var leftSide = new Matrix(count, count);
            var rightSide = new Matrix(count, 1);

            for (uint i = 0; i < count; ++i)
            {
                var currentEquation = Equations[(int)i];
                double[] coefficients = currentEquation.Coefficients;
                for (uint c = 0; c < coefficients.Count(); ++c)
                    leftSide[i, c] = coefficients[c];
                rightSide[i, 0] = currentEquation.Result;
            }

            var resultMatrix = Algorithm.GaussJordan(leftSide, rightSide);
            double[] result = new double[rightSide.RowCount];
            for (uint i = 0; i < resultMatrix.RowCount; ++i)
                result[i] = resultMatrix[i, 0];

            return result;
        }
    }
}