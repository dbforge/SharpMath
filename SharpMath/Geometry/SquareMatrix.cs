// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a <see cref="Matrix" /> whose row and column count are equal. (n*n)
    /// </summary>
    public class SquareMatrix : Matrix
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="SquareMatrix" /> class.
        /// </summary>
        /// <param name="dimension">The dimension of the <see cref="SquareMatrix" />.</param>
        public SquareMatrix(uint dimension)
            : base(dimension, dimension)
        {
            Dimension = dimension;
        }

        /// <summary>
        ///     Gets the dimension (column and row count) of this <see cref="SquareMatrix" />.
        /// </summary>
        public uint Dimension { get; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="SquareMatrix" /> is singular, or not. If <c>true</c>, this
        ///     <see cref="SquareMatrix" /> doesn't have an inverse.
        /// </summary>
        public bool IsSingular => Math.Abs(Determinant) < FloatingNumber.Epsilon;

        /// <summary>
        ///     Gets a value indicating whether this <see cref="SquareMatrix" /> is orthogonal, or not.
        /// </summary>
        public bool IsOrthogonal => Multiply(this, Transpose) == GetIdentity(Dimension);

        /// <summary>
        ///     Gets the inverse of this <see cref="SquareMatrix" />.
        /// </summary>
        public SquareMatrix Inverse => FromMatrix(Algorithm.GaussJordan(this, GetIdentity(ColumnCount)));

        /// <summary>
        ///     Gets the determinant of this <see cref="SquareMatrix" />.
        /// </summary>
        public double Determinant
        {
            get
            {
                switch (ColumnCount)
                {
                    case 1:
                        return this[0, 0];
                    case 2:
                        return this[0, 0]*this[1, 1] - this[0, 1]*this[1, 0];
                    default:
                        return LaplaceExpansion();
                }
            }
        }

        /// <summary>
        ///     Gets the trace of this <see cref="SquareMatrix" />.
        /// </summary>
        public double Trace
        {
            get
            {
                double result = 0d;
                for (uint i = 0; i < ColumnCount; ++i)
                    result += this[i, i];
                return result;
            }
        }

        /// <summary>
        ///     Gets the cofactor <see cref="SquareMatrix" /> of this <see cref="SquareMatrix" />.
        /// </summary>
        public SquareMatrix CofactorMatrix => BuildCofactorMatrix(this);

        /// <summary>
        ///     Gets the adjugate of this <see cref="SquareMatrix" />.
        /// </summary>
        public SquareMatrix Adjugate => FromMatrix(CofactorMatrix.Transpose);

        /// <summary>
        ///     Creates a <see cref="SquareMatrix" /> from an abstract <see cref="Matrix" /> object.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix" /> to convert.</param>
        /// <returns>The <see cref="SquareMatrix" /> that has been created.</returns>
        public static SquareMatrix FromMatrix(Matrix matrix)
        {
            if (matrix.ColumnCount != matrix.RowCount)
                throw new InvalidOperationException(
                    "Cannot create a square matrix as the row count does not match the column count.");

            var squareMatrix = new SquareMatrix(matrix.ColumnCount);
            for (uint y = 0; y < squareMatrix.Dimension; y++)
                for (uint x = 0; x < squareMatrix.Dimension; x++)
                    squareMatrix[y, x] = matrix[y, x];

            return squareMatrix;
        }

        /// <summary>
        ///     Creates an identity <see cref="SquareMatrix" /> with the specified dimension.
        /// </summary>
        public static SquareMatrix GetIdentity(uint dimension)
        {
            var resultMatrix = new SquareMatrix(dimension);
            for (uint i = 0; i < resultMatrix.ColumnCount; ++i)
                resultMatrix[i, i] = 1;
            return resultMatrix;
        }

        internal double LaplaceExpansion()
        {
            double determinant = 0;
            for (uint y = 0; y < Dimension; ++y)
                determinant += this[y, 0]*GetCofactor(y, 0); // The sigma sign is equal to a for-loop with recursion.

            return determinant;
        }

        /// <summary>
        ///     Calculates a cofactor of an element at the specified position in this <see cref="SquareMatrix" />.
        /// </summary>
        /// <param name="row">The row of the element.</param>
        /// <param name="column">The column of the element.</param>
        /// <returns>The cofactor of the element in this <see cref="SquareMatrix" />.</returns>
        public double GetCofactor(uint row, uint column)
        {
            return Math.Pow(-1, row + column)*FromMatrix(GetSubMatrix(row, column)).Determinant;
        }

        /// <summary>
        ///     Calculates the cofactor <see cref="SquareMatrix" /> of the specified <see cref="SquareMatrix" />.
        /// </summary>
        /// <param name="matrix">The <see cref="SquareMatrix" /> whose cofactor <see cref="SquareMatrix" /> should be calculated.</param>
        /// <returns>The calculated cofactor <see cref="SquareMatrix" />.</returns>
        public static SquareMatrix BuildCofactorMatrix(SquareMatrix matrix)
        {
            var resultMatrix = new SquareMatrix(matrix.Dimension);
            for (uint y = 0; y < matrix.RowCount; ++y)
            {
                for (uint x = 0; x < matrix.ColumnCount; ++x)
                {
                    resultMatrix[y, x] = matrix.GetCofactor(y, x);
                }
            }

            return resultMatrix;
        }
    }
}