using System;
using System.Globalization;
using System.Text;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace SharpMath.Geometry
{
    public class Matrix : IEquatable<Matrix>, ICloneable
    {
        private readonly double[,] _fields;

        public Matrix(uint rowCount, uint columnCount)
        {
            _fields = new double[rowCount, columnCount];
            RowCount = rowCount;
            ColumnCount = columnCount;
        }

        public double this[uint row, uint column] 
        {
            get { return _fields[row, column]; }
            set { _fields[row, column] = value; }
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder("{");

            for (uint y = 0; y < RowCount; y++)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("\t");

                for (uint x = 0; x < ColumnCount; x++)
                {
                    stringBuilder.Append(this[y, x].ToString(CultureInfo.InvariantCulture));

                    if (x != (ColumnCount - 1))
                        stringBuilder.Append(", ");
                }
            }

            return stringBuilder.Append("\n}").ToString();
        }

        /// <summary>
        ///     Gets the row count of this <see cref="Matrix"/>.
        /// </summary>
        public uint RowCount { get; }

        /// <summary>
        ///     Gets the column count of this <see cref="Matrix"/>.
        /// </summary>
        public uint ColumnCount { get; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Matrix"/> is symmetric, or not.
        /// </summary>
        public bool IsSymmetric => (this == Transpose);

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Matrix"/> is skew symmetric, or not.
        /// </summary>
        public bool IsSkewSymmetric => (Negate == Transpose);

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Matrix"/> is antisymmetric, or not.
        /// </summary>
        public bool IsAntiSymmetric => (this == Transpose.Negate);

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Matrix"/> is a square matrix, or not.
        /// </summary>
        public bool IsSquare => (RowCount == ColumnCount);

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Matrix"/> is a diagonal matrix, or not.
        /// </summary>
        public bool IsDiagonal
        {
            get
            {
                for (uint y = 0; y < RowCount; ++y)
                    for (uint x = 0; x < ColumnCount; ++x)
                    {
                        if ((y == x && this[y, x] == 0) || (y != x && this[y, x] != 0))
                            return false;
                    }
                return true;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Matrix"/> is a triangle matrix, or not.
        /// </summary>
        public bool IsTriangle
        {
            get
            {
                for (uint y = 0; y < RowCount; ++y)
                    for (uint x = 0; x < ColumnCount; ++x)
                    {
                        if ((y == x && this[y, x] == 0) || (y != x && this[y, x] != 0))
                            return false;
                    }
                return true;
            }
        }

        /// <summary>
        ///     Gets the transpose of this <see cref="Matrix"/>. 
        /// </summary>
        public Matrix Transpose
        {
            get
            {
                var resultMatrix = new Matrix(ColumnCount, RowCount);
                for (uint y = 0; y < RowCount; ++y)
                    for (uint x = 0; x < ColumnCount; ++x)
                        resultMatrix[y, x] = this[x, y];
                return resultMatrix;
            }
        }

        /// <summary>
        ///     Gets the column <see cref="Vector"/> at the specified index.
        /// </summary>
        /// <param name="index">The column index.</param>
        /// <returns>The column <see cref="Vector"/> at the specified index.</returns>
        public Vector GetColumnVector(uint index)
        {
            var resultVector = new Vector(ColumnCount);
            for (uint i = 0; i < ColumnCount; ++i)
                resultVector[i] = this[0, i];
            return resultVector;
        }

        /// <summary>
        ///     Gets the row <see cref="Vector"/> at the specified index.
        /// </summary>
        /// <param name="index">The column index.</param>
        /// <returns>The row <see cref="Vector"/> at the specified index.</returns>
        public Vector GetRowVector(uint index)
        {
            var resultVector = new Vector(RowCount);
            for (uint i = 0; i < RowCount; ++i)
                resultVector[i] = this[i, 0];
            return resultVector;
        }

        /// <summary>
        ///     Augments this <see cref="Matrix"/> with the specified <see cref="Matrix"/> attaching it to the right.
        /// </summary>
        /// <param name="other">The <see cref="Matrix"/> to augment this one with.</param>
        /// <returns>The augmented <see cref="Matrix"/>.</returns>
        public Matrix AugmentHorizontally(Matrix other)
        {
            if (RowCount != other.RowCount)
                throw new InvalidOperationException("Cannot the augment the first matrix as its row count doesn't match the row count of the second one.");

            var resultMatrix = new Matrix(RowCount, ColumnCount + other.ColumnCount);
            for (uint y = 0; y < resultMatrix.RowCount; ++y)
            {
                for (uint fx = 0; fx < ColumnCount; ++fx)
                    resultMatrix[y, fx] = this[y, fx];

                for (uint sx = 0; sx < other.ColumnCount; ++sx)
                    resultMatrix[y, ColumnCount + sx] = other[y, sx];
            }

            return resultMatrix;
        }

        /// <summary>
        ///     Augments a <see cref="Matrix"/> with an other <see cref="Matrix"/> attaching it to the right.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix"/>.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix"/> to augment the first one with.</param>
        /// <returns>The augmented <see cref="Matrix"/>.</returns>
        public static Matrix AugmentHorizontally(Matrix firstMatrix, Matrix secondMatrix)
        {
            return firstMatrix.AugmentHorizontally(secondMatrix);
        }

        /// <summary>
        ///     Augments this <see cref="Matrix"/> with the specified <see cref="Matrix"/> by attaching at to the bottom.
        /// </summary>
        /// <param name="other">The <see cref="Matrix"/> to augment this one with.</param>
        /// <returns>The augmented <see cref="Matrix"/>.</returns>
        public Matrix AugmentVertically(Matrix other)
        {
            if (ColumnCount != other.ColumnCount)
                throw new InvalidOperationException("Cannot the augment the first matrix as its column count doesn't match the column count of the second one.");

            var resultMatrix = new Matrix(RowCount + other.RowCount, ColumnCount);
            for (uint x = 0; x < resultMatrix.ColumnCount; ++x)
            {
                for (uint fy = 0; fy < RowCount; ++fy)
                    resultMatrix[fy, x] = this[fy, x];

                for (uint sy = 0; sy < other.RowCount; ++sy)
                    resultMatrix[RowCount + sy, x] = other[sy, x];
            }

            return resultMatrix;
        }

        /// <summary>
        ///     Augments a <see cref="Matrix"/> with an other <see cref="Matrix"/> by attaching it at the bottom.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix"/>.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix"/> to augment the first one with.</param>
        /// <returns>The augmented <see cref="Matrix"/>.</returns>
        public static Matrix AugmentVertically(Matrix firstMatrix, Matrix secondMatrix)
        {
            return firstMatrix.AugmentVertically(secondMatrix);
        }

        /// <summary>
        ///     Calculates a sub <see cref="Matrix"/> of this <see cref="Matrix"/> by removing the specified column and row.
        /// </summary>
        /// <param name="row">The row that should be removed.</param>
        /// <param name="column">The column that should be removed.</param>
        /// <returns>The calculated sub <see cref="Matrix"/>.</returns>
        public Matrix GetSubMatrix(uint row, uint column)
        {
            var resultMatrix = new Matrix(RowCount - 1, ColumnCount - 1);
            uint y = 0;
            for (uint cy = 0; cy < RowCount; cy++)
            {
                if (cy != row)
                {
                    uint x = 0;
                    for (uint cx = 0; cx < ColumnCount; ++cx)
                        if (cx != column)
                        {
                            resultMatrix[y, x] = this[cy, cx];
                            x++;
                        }
                    y++;
                }
            }
            return resultMatrix;
        }

        /// <summary>
        ///     Gets the negated <see cref="Matrix"/> of this <see cref="Matrix"/>.
        /// </summary>
        public Matrix Negate
        {
            get
            {
                var resultMatrix = new Matrix(RowCount, ColumnCount);
                for (uint y = 0; y < RowCount; ++y)
                    for (uint x = 0; x < ColumnCount; ++x)
                        resultMatrix[y, x] = -resultMatrix[y, x];
                return resultMatrix;
            }
        }

        /// <summary>
        ///     Gets a <see cref="Matrix"/> with all of its components set to zero. 
        /// </summary>
        public Matrix Zero => new Matrix(RowCount, ColumnCount);

        /// <summary>
        ///     Determines whether two <see cref="Matrix"/> instances are equal.
        /// </summary>
        /// <param name="firstMatrix">The first matrix.</param>
        /// <param name="secondMatrix">The second matrix.</param>
        /// <returns><c>true</c> if the <see cref="Matrix"/> instances are equal, otherwise <c>false</c>.</returns>
        public static bool AreEqual(Matrix firstMatrix, Matrix secondMatrix)
        {
            if (firstMatrix.ColumnCount != secondMatrix.ColumnCount || firstMatrix.RowCount != secondMatrix.RowCount)
                return false;

            for (uint y = 0; y < firstMatrix.RowCount; ++y)
            {
                for (uint x = 0; x < firstMatrix.ColumnCount; ++x)
                {
                    if (firstMatrix[y, x] != secondMatrix[y, x])
                        return false;
                }
            }

            return true;
        }

        internal void MultiplyRow(uint rowIndex, double factor)
        {
            for (uint x = 0; x < ColumnCount; ++x)
                this[rowIndex, x] *= factor;
        }

        internal void InterchangeRows(uint firstRowIndex, uint secondRowIndex)
        {
            for (uint x = 0; x < ColumnCount; ++x)
            {
                var firstValue = this[firstRowIndex, x];
                this[firstRowIndex, x] = this[secondRowIndex, x];
                this[secondRowIndex, x] = firstValue;
            }
        }

        internal void SubtractRows(uint firstRowIndex, uint secondRowIndex, double factor)
        {
            for (uint x = 0; x < ColumnCount; x++)
                this[firstRowIndex, x] -= this[secondRowIndex, x] * factor;
        }
        /// <summary>
        ///     Multiplies a <see cref="Matrix"/> with a scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix"/> to include into the product.</param>
        /// <param name="scalar">The scalar factor that the <see cref="Matrix"/> should be multiplied with.</param>
        /// <returns>Returns the <see cref="Matrix"/> product.</returns>
        public static Matrix Multiply(Matrix matrix, double scalar)
        {
            for (uint y = 0; y < matrix.RowCount; ++y)
                for (uint x = 0; x < matrix.ColumnCount; ++x)
                    matrix[y, x] *= scalar;
            return matrix;
        }

        /// <summary>
        ///     Multiplies two <see cref="Matrix"/> instances, if they are compatible to each other.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix"/> to include into the product.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix"/> to include into the product.</param>
        /// <returns>The <see cref="Matrix"/> product.</returns>
        public static Matrix Multiply(Matrix firstMatrix, Matrix secondMatrix)
        {
            if (firstMatrix.ColumnCount != secondMatrix.RowCount)
                throw new ArgumentException("Cannot multiply the specified matrices because the column count of the first one does not match the row count of the second one.");

            var matrixProduct = new Matrix(firstMatrix.RowCount, secondMatrix.ColumnCount);
            for (uint y = 0; y < matrixProduct.RowCount; ++y)
            {
                for (uint x = 0; x < matrixProduct.ColumnCount; ++x)
                {
                    // Iteration condition could be secondMatrix.RowCount, too, as they are equal (see above)
                    for (uint i = 0; i < firstMatrix.ColumnCount; ++i)
                        matrixProduct[y, x] += firstMatrix[y, i] * secondMatrix[i, x];
                }
            }

            return matrixProduct;
        }

        /// <summary>
        ///     Multiplies the specified <see cref="Matrix"/> with the reciprocal of the specified scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix"/>.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>The resulting <see cref="Matrix"/>.</returns>
        public static Matrix Divide(Matrix matrix, double scalar)
        {
            return Multiply(matrix, (1 / scalar));
        }

        public bool Equals(Matrix other)
        {
            if (ColumnCount != other.ColumnCount || RowCount != other.RowCount)
                return false;

            for (uint y = 0; y < RowCount; ++y)
            {
                for (uint x = 0; x < ColumnCount; ++x)
                {
                    if (this[y, x] != other[y, x])
                        return false;
                }
            }

            return true;
        }

        public object Clone()
        {
            var cloneMatrix = new Matrix(RowCount, ColumnCount);
            for (uint y = 0; y < RowCount; ++y)
            {
                for (uint x = 0; x < ColumnCount; ++x)
                {
                    cloneMatrix[y, x] = this[y, x];
                }
            }
            return cloneMatrix;
        }
    }
}