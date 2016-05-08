// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using System.Linq;
//using SharpMath.Equations;
using SharpMath.Equations.Exceptions;

namespace SharpMath.Geometry
{
    public static class MatrixUtils
    {
        /// <summary>
        ///     Adds two <see cref="IMatrix" /> instances.
        /// </summary>
        /// <param name="firstMatrix">The first < see cref="IMatrix" />.</param>
        /// <param name="secondMatrix">The second <see cref="IMatrix" />.</param>
        /// <returns>The resulting <see cref="IMatrix" />.</returns>
        public static T Add<T>(T firstMatrix, T secondMatrix) where T : IMatrix
        {
            for (uint y = 0; y < firstMatrix.RowCount; ++y)
                for (uint x = 0; x < firstMatrix.ColumnCount; ++x)
                    firstMatrix[y, x] += secondMatrix[y, x];
            return firstMatrix;
        }

        /// <summary>
        ///     Calculates the cofactor <see cref="ISquareMatrix" /> of the specified <see cref="ISquareMatrix" />.
        /// </summary>
        /// <param name="matrix">The <see cref="ISquareMatrix" /> whose cofactor <see cref="ISquareMatrix" /> should be calculated.</param>
        /// <returns>The calculated cofactor <see cref="ISquareMatrix" />.</returns>
        public static T BuildCofactorMatrix<T>(this T matrix) where T : ISquareMatrix, new()
        {
            var resultMatrix = new T();
            for (uint y = 0; y < matrix.RowCount; ++y)
            {
                for (uint x = 0; x < matrix.ColumnCount; ++x)
                {
                    resultMatrix[y, x] = matrix.GetCofactor(y, x);
                }
            }

            return resultMatrix;
        }

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public static T Clone<T>(this T matrix) where T : IMatrix, new()
        {
            var cloneMatrix = new T();
            for (uint y = 0; y < matrix.RowCount; ++y)
            {
                for (uint x = 0; x < matrix.ColumnCount; ++x)
                {
                    cloneMatrix[y, x] = matrix[y, x];
                }
            }
            return cloneMatrix;
        }

        /// <summary>
        ///     Implements the Gauss-Jordan-algorithm.
        /// </summary>
        /// <param name="leftSide">The left side <see cref="IMatrix" />.</param>
        /// <param name="rightSide">The right side <see cref="IMatrix" />.</param>
        /// <returns>The resulting <see cref="IMatrix" />.</returns>
        /// <exception cref="EquationNotSolvableException">The linear equation system cannot be solved clearly.</exception>
        public static T GaussJordan<T>(T leftSide, T rightSide) where T : IMatrix
        {
            for (uint x = 0; x < leftSide.ColumnCount; x++)
            {
                uint nextX = x;
                while (leftSide[x, x].IsApproximatelyEqualTo(0))
                {
                    nextX++;

                    if (nextX >= leftSide.ColumnCount)
                        throw new EquationNotSolvableException("The linear equation system cannot be solved clearly.");

                    if (Math.Abs(leftSide[x, nextX]) < FloatingNumber.Epsilon)
                        continue;

                    leftSide.InterchangeRows(nextX, x);
                    rightSide.InterchangeRows(nextX, x);
                }

                for (uint y = 0; y < leftSide.RowCount; y++)
                {
                    if (y != x && Math.Abs(leftSide[y, x]) >= FloatingNumber.Epsilon)
                    {
                        double factor = leftSide[y, x]/leftSide[x, x];
                        leftSide.SubtractRows(y, x, factor);
                        rightSide.SubtractRows(y, x, factor);
                    }
                }
            }

            for (uint i = 0; i < leftSide.ColumnCount; i++)
            {
                double factor = 1/leftSide[i, i];
                leftSide.MultiplyRow(i, factor);
                rightSide.MultiplyRow(i, factor);
            }

            return rightSide;
        }

        /// <summary>
        ///     Calculates the cofactor of an element at the specified position in the <see cref="ISquareMatrix" />.
        /// </summary>
        /// <param name="matrix">The <see cref="ISquareMatrix" />.</param>
        /// <param name="row">The row of the element.</param>
        /// <param name="column">The column of the element.</param>
        /// <returns>The cofactor of the element in this <see cref="ISquareMatrix" />.</returns>
        public static double GetCofactor(this ISquareMatrix matrix, uint row, uint column)
        {
            return Math.Pow(-1, row + column)*matrix.GetSubMatrix(row, column).GetDeterminant();
        }

        //public static TOut GetCore<TOut>(this ISquareMatrix matrix) where TOut : IVector, new()
        //{
        //    if (!matrix.Determinant.IsApproximatelyEqualTo(0))
        //        throw new InvalidOperationException($"Cannot calculate core of {nameof(matrix)} as its determinant is not 0.");

        //    var vector = new TOut();
        //    if (matrix.Dimension != vector.Dimension)
        //        throw new InvalidOperationException($"Type parameter TOut is not an adequate IVector - type as its dimension does not fit the one of the resulting vector. The dimension must be {matrix.Dimension}.");

        //    var equations = new List<LinearEquation>((int)matrix.ColumnCount);
        //    for (uint y = 0; y < matrix.RowCount; ++y)
        //    {
        //        var coefficients = new List<double>((int)matrix.ColumnCount);
        //        for (uint x = 0; x < matrix.ColumnCount; ++x)
        //            coefficients.Add(matrix[y, x]);

        //        equations.Add(new LinearEquation(coefficients.ToArray(), 0));
        //    }
            
        //    var equationSystem = new LinearEquationSystem(equations);
        //    double[] solutions = equationSystem.Solve();
        //    for (uint i = 0; i < matrix.ColumnCount; ++i)
        //        vector[i] = solutions[i];

        //    return vector;
        //}

        /// <summary>
        ///     Calculates the determinant of the <see cref="ISquareMatrix" />.
        /// </summary>
        /// <param name="matrix">The <see cref="ISquareMatrix" /> whose determinant should be calculated.</param>
        /// <returns>The determinant of the <see cref="ISquareMatrix" />.</returns>
        public static double GetDeterminant(this ISquareMatrix matrix)
        {
            switch (matrix.Dimension)
            {
                case 1:
                    return matrix[0, 0];
                case 2:
                    return matrix[0, 0]*matrix[1, 1] - matrix[0, 1]*matrix[1, 0];
                default:
                    return LaplaceExpansion(matrix);
            }
        }

        /// <summary>
        ///     Calculates the identity <see cref="ISquareMatrix" /> of the specified <see cref="ISquareMatrix" /> type.
        /// </summary>
        /// <typeparam name="T">
        ///     The <see cref="ISquareMatrix" /> type whose identity <see cref="ISquareMatrix" /> should be
        ///     calculated.
        /// </typeparam>
        /// <returns>The identity <see cref="ISquareMatrix" />.</returns>
        public static T GetIdentity<T>() where T : ISquareMatrix, new()
        {
            var resultMatrix = new T();
            for (uint i = 0; i < resultMatrix.Dimension; ++i)
                resultMatrix[i, i] = 1;
            return resultMatrix;
        }

        //public static T GetInverse<T>(this ISquareMatrix matrix) where T : ISquareMatrix, new()
        //{
        //    if (matrix.Determinant.IsApproximatelyEqualTo(0))
        //        throw new InvalidOperationException("The specified matrix does not have an inverse.");
        //    return GaussJordan(matrix, GetIdentity())
        //}

        /// <summary>
        ///     Determines whether the <see cref="ISquareMatrix" /> is a diagonal <see cref="ISquareMatrix" />, or not.
        /// </summary>
        /// <param name="matrix">The <see cref="ISquareMatrix" />.</param>
        /// <returns>
        ///     <c>true</c>, if the <see cref="ISquareMatrix" /> is a diagonal <see cref="ISquareMatrix" />, otherwise
        ///     <c>false</c>.
        /// </returns>
        public static bool GetIsDiagonal(this ISquareMatrix matrix)
        {
            if (matrix.Dimension == 1)
                return false;

            for (uint y = 0; y < matrix.RowCount; ++y)
                for (uint x = 0; x < matrix.ColumnCount; ++x)
                {
                    if ((y == x && FloatingNumber.AreApproximatelyEqual(matrix[y, x], 0)) ||
                        (y != x && !FloatingNumber.AreApproximatelyEqual(matrix[y, x], 0)))
                        return false;
                }
            return true;
        }

        /// <summary>
        ///     Determines whether the <see cref="ISquareMatrix" /> is a triangle <see cref="ISquareMatrix" />, or not.
        /// </summary>
        /// <param name="matrix">The <see cref="ISquareMatrix" />.</param>
        /// <returns>
        ///     <c>true</c>, if the <see cref="ISquareMatrix" /> is a triangle <see cref="ISquareMatrix" />, otherwise
        ///     <c>false</c>.
        /// </returns>
        public static bool GetIsTriangle(this ISquareMatrix matrix)
        {
            if (matrix.Dimension == 1)
                return false;

            var upperTriangle = new List<double>();
            var lowerTriangle = new List<double>();
            for (uint y = 0; y < matrix.RowCount; ++y)
            {
                var isRightSide = false;
                for (uint x = 0; x < matrix.ColumnCount; ++x)
                {
                    if (y == x)
                    {
                        isRightSide = true;
                        continue;
                    }

                    if (isRightSide)
                        upperTriangle.Add(matrix[y, x]);
                    else
                        lowerTriangle.Add(matrix[y, x]);
                }
            }

            if (upperTriangle.All(val => val.IsApproximatelyEqualTo(0)) ||
                lowerTriangle.All(val => val.IsApproximatelyEqualTo(0)))
                return true;
            return false;
        }

        /// <summary>
        ///     Calculates the negated <see cref="IMatrix" /> of the <see cref="IMatrix" />.
        /// </summary>
        /// <param name="matrix">The <see cref="IMatrix" /> whose negated <see cref="IMatrix" /> should be calculated.</param>
        /// <returns>The negated <see cref="IMatrix" />.</returns>
        public static T GetNegate<T>(this T matrix) where T : IMatrix, new()
        {
            var resultMatrix = new T();
            for (uint y = 0; y < matrix.RowCount; ++y)
                for (uint x = 0; x < matrix.ColumnCount; ++x)
                    resultMatrix[y, x] = -matrix[y, x];
            return resultMatrix;
        }

        /// <summary>
        ///     Calculates the sub <see cref="IMatrix" /> of the current <see cref="IMatrix" /> by removing the specified column
        ///     and row.
        /// </summary>
        /// <param name="matrix">The <see cref="ISquareMatrix" />.</param>
        /// <param name="row">The row that should be removed.</param>
        /// <param name="column">The column that should be removed.</param>
        /// <returns>The calculated sub <see cref="ISquareMatrix" />.</returns>
        public static ISquareMatrix GetSubMatrix(this ISquareMatrix matrix, uint row, uint column)
        {
            ISquareMatrix resultMatrix;
            switch (matrix.Dimension)
            {
                case 2:
                    resultMatrix = new Matrix1x1();
                    break;
                case 3:
                    resultMatrix = new Matrix2x2();
                    break;
                case 4:
                    resultMatrix = new Matrix3x3();
                    break;
                default:
                    throw new InvalidOperationException("Cannot build sub matrix of " + nameof(matrix) +
                                                        " as there is no lower dimension defined for it.");
            }

            uint y = 0;
            for (uint cy = 0; cy < matrix.RowCount; cy++)
            {
                if (cy != row)
                {
                    uint x = 0;
                    for (uint cx = 0; cx < matrix.ColumnCount; ++cx)
                        if (cx != column)
                        {
                            resultMatrix[y, x] = matrix[cy, cx];
                            x++;
                        }
                    y++;
                }
            }
            return resultMatrix;
        }

        /// <summary>
        ///     Calculates the transpose of the <see cref="IMatrix" />.
        /// </summary>
        /// <param name="matrix">The <see cref="IMatrix" /> whose transpose should be calculated.</param>
        /// <returns>The transpose <see cref="IMatrix" />.</returns>
        public static T GetTranspose<T>(this T matrix) where T : ISquareMatrix, new()
        {
            var resultMatrix = new T();
            for (uint y = 0; y < matrix.Dimension; ++y)
                for (uint x = 0; x < matrix.ColumnCount; ++x)
                    resultMatrix[y, x] = matrix[x, y];
            return resultMatrix;
        }

        internal static double LaplaceExpansion(this ISquareMatrix matrix)
        {
            double determinant = 0;
            for (uint i = 0; i < matrix.Dimension; ++i)
                determinant += matrix[i, 0]*matrix.GetCofactor(i, 0);
                    // The sigma sign is equal to a for-loop with recursion.
            return determinant;
        }

        /// <summary>
        ///     Multiplies the <see cref="IMatrix" /> with a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="IMatrix" /> to include into the product.</param>
        /// <param name="scalar">The scalar factor that the <see cref="IMatrix" /> should be multiplied with.</param>
        /// <returns>Returns the <see cref="IMatrix" /> product.</returns>
        public static T Multiply<T>(this T matrix, double scalar) where T : IMatrix
        {
            for (uint y = 0; y < matrix.RowCount; ++y)
                for (uint x = 0; x < matrix.ColumnCount; ++x)
                    matrix[y, x] *= scalar;
            return matrix;
        }

        /// <summary>
        ///     Multiplies two <see cref="IMatrix" /> instances, if they are compatible to each other.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="IMatrix" /> to include into the product.</param>
        /// <param name="secondMatrix">The second <see cref="IMatrix" /> to include into the product.</param>
        /// <returns>The <see cref="IMatrix" /> product.</returns>
        public static TOut Multiply<TOut>(IMatrix firstMatrix, IMatrix secondMatrix) where TOut : IMatrix, new()
        {
            if (firstMatrix.ColumnCount != secondMatrix.RowCount)
                throw new ArgumentException(
                    "The column count of the first matrix does not match the row count of the second matrix.");

            var matrixProduct = new TOut();
            if (matrixProduct.RowCount != firstMatrix.RowCount || matrixProduct.ColumnCount != secondMatrix.ColumnCount)
                throw new ArgumentException(
                    $"Type parameter TOut is not an adequate IMatrix-type as its constraints do not fit those of the resulting matrix. The constraints must be {firstMatrix.RowCount}x{secondMatrix.ColumnCount}.");

            for (uint y = 0; y < matrixProduct.RowCount; ++y)
            {
                for (uint x = 0; x < matrixProduct.ColumnCount; ++x)
                {
                    for (uint i = 0; i < firstMatrix.ColumnCount; ++i)
                        matrixProduct[y, x] += firstMatrix[y, i]*secondMatrix[i, x];
                }
            }

            return matrixProduct;
        }

        internal static void MultiplyRow(this IMatrix matrix, uint rowIndex, double factor)
        {
            for (uint x = 0; x < matrix.ColumnCount; ++x)
                matrix[rowIndex, x] *= factor;
        }

        internal static void InterchangeRows(this IMatrix matrix, uint firstRowIndex, uint secondRowIndex)
        {
            for (uint x = 0; x < matrix.ColumnCount; ++x)
            {
                var firstValue = matrix[firstRowIndex, x];
                matrix[firstRowIndex, x] = matrix[secondRowIndex, x];
                matrix[secondRowIndex, x] = firstValue;
            }
        }

        /// <summary>
        ///     Adds two <see cref="IMatrix" /> instances, if they are compatible to each other.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="IMatrix" />.</param>
        /// <param name="secondMatrix">The second <see cref="IMatrix" />.</param>
        /// <returns>The resulting <see cref="IMatrix" />.</returns>
        public static T Subtract<T>(T firstMatrix, T secondMatrix) where T : IMatrix
        {
            for (uint y = 0; y < firstMatrix.RowCount; ++y)
                for (uint x = 0; x < firstMatrix.ColumnCount; ++x)
                    firstMatrix[y, x] -= secondMatrix[y, x];
            return firstMatrix;
        }

        internal static void SubtractRows(this IMatrix matrix, uint firstRowIndex, uint secondRowIndex, double factor)
        {
            for (uint x = 0; x < matrix.ColumnCount; x++)
                matrix[firstRowIndex, x] -= matrix[secondRowIndex, x]*factor;
        }
    }
}