// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a 1x1 matrix.
    /// </summary>
    [Serializable]
    public struct Matrix1x1 : IEnumerable<double>, ISquareMatrix<Matrix1x1>, IEquatable<Matrix1x1>
    {
        /// <summary>
        ///     Initializes a <see cref="Matrix1x1" /> struct.
        /// </summary>
        /// <param name="m11">The value at row 1 and column 1.</param>
        public Matrix1x1(double m11)
        {
            M11 = m11;
        }

        /// <summary>
        ///     Gets the zero <see cref="Matrix1x1" />.
        /// </summary>
        public static Matrix1x1 Zero => new Matrix1x1();

        /// <summary>
        ///     Gets the identity <see cref="Matrix1x1" />.
        /// </summary>
        public static Matrix1x1 Identity => MatrixUtils.GetIdentity<Matrix1x1>();

        /// <summary>
        ///     Gets or sets the value at row 1 and column 1.
        /// </summary>
        public double M11 { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix1x1" /> is singular, or not. If <c>true</c>, this
        ///     <see cref="Matrix1x1" /> doesn't have an inverse.
        /// </summary>
        public bool IsSingular => Determinant.IsApproximatelyEqualTo(0);

        /// <summary>
        ///     Gets the inverse of the <see cref="Matrix1x1" />.
        /// </summary>
        public Matrix1x1 Inverse => MatrixUtils.GaussJordan(this, Identity);

        /// <summary>
        ///     Gets the cofactor <see cref="Matrix1x1" /> of the <see cref="Matrix1x1" />.
        /// </summary>
        public Matrix1x1 CofactorMatrix => this.BuildCofactorMatrix();

        /// <summary>
        ///     Gets the adjugate of the <see cref="Matrix1x1" />.
        /// </summary>
        public Matrix1x1 Adjugate => CofactorMatrix.Transpose;

        /// <summary>
        ///     Gets the negated <see cref="Matrix1x1" /> of the <see cref="Matrix1x1" />.
        /// </summary>
        public Matrix1x1 Negate => this.GetNegate();

        /// <summary>
        ///     Gets the transpose of the <see cref="Matrix1x1" />.
        /// </summary>
        public Matrix1x1 Transpose => this.GetTranspose();

        public IEnumerator<double> GetEnumerator()
        {
            return new MatrixEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MatrixEnumerator(this);
        }

        public bool Equals(Matrix1x1 other)
        {
            return this == other;
        }

        /// <summary>
        ///     Gets or sets the value at the specified index. The values are accessed as: row + column.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The value at the specified index.</returns>
        public double this[uint index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return M11;
                    default:
                        throw new IndexOutOfRangeException("The index must be 0.");
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        M11 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("The index must be 0.");
                }
            }
        }

        /// <summary>
        ///     Gets or sets the value at the specified row and column.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns>The value at the specified row and column.</returns>
        public double this[uint row, uint column]
        {
            get { return this[row + column]; }
            set { this[row + column] = value; }
        }

        /// <summary>
        ///     Gets the dimension of the <see cref="Matrix1x1" />.
        /// </summary>
        public uint Dimension => 1;

        /// <summary>
        ///     Gets the row count of the <see cref="Matrix1x1" />.
        /// </summary>
        public uint RowCount => 1;

        /// <summary>
        ///     Gets the column count of the <see cref="Matrix1x1" />.
        /// </summary>
        public uint ColumnCount => 1;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix1x1" /> is orthogonal, or not.
        /// </summary>
        public bool IsOrthogonal => (this*Transpose) == Identity;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix1x1" /> is the identity <see cref="Matrix1x1" />, or not.
        /// </summary>
        public bool IsIdentity => M11.IsApproximatelyEqualTo(1);

        /// <summary>
        ///     Gets the determinant of the <see cref="Matrix1x1" />.
        /// </summary>
        public double Determinant => this.GetDeterminant(); // Determinant == M11

        /// <summary>
        ///     Gets the trace of the <see cref="Matrix1x1" />.
        /// </summary>
        public double Trace => M11;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix1x1" /> is symmetric, or not.
        /// </summary>
        public bool IsSymmetric => this == Transpose;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix1x1" /> is skew symmetric, or not.
        /// </summary>
        public bool IsSkewSymmetric => Negate == Transpose;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix1x1" /> is antisymmetric, or not.
        /// </summary>
        public bool IsAntiSymmetric => this == Transpose.Negate;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix1x1" /> is a diagonal matrix, or not.
        /// </summary>
        public bool IsDiagonal => false;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix1x1" /> is a triangle matrix, or not.
        /// </summary>
        public bool IsTriangle => false;

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix1x1" />.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix1x1" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix1x1 operator +(Matrix1x1 firstMatrix, Matrix1x1 secondMatrix)
        {
            return MatrixUtils.Add(firstMatrix, secondMatrix);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix1x1" />.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix1x1" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix1x1 operator -(Matrix1x1 firstMatrix, Matrix1x1 secondMatrix)
        {
            return MatrixUtils.Subtract(firstMatrix, secondMatrix);
        }

        /// <summary>
        ///     Implements the operator * to multiply a <see cref="Matrix1x1" /> with the specified scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="matrix">The <see cref="Matrix1x1" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix1x1 operator *(double scalar, Matrix1x1 matrix)
        {
            return MatrixUtils.Multiply(matrix, scalar);
        }

        /// <summary>
        ///     Implements the operator * to multiply a <see cref="Matrix1x1" /> with the specified scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix1x1" />.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix1x1 operator *(Matrix1x1 matrix, double scalar)
        {
            return MatrixUtils.Multiply(matrix, scalar);
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix1x1" />.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix1x1" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix1x1 operator *(Matrix1x1 firstMatrix, Matrix1x1 secondMatrix)
        {
            return MatrixUtils.Multiply<Matrix1x1>(firstMatrix, secondMatrix);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="object" />, is equal to the instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with the instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to the instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Matrix1x1))
                return false;
            return this == (Matrix1x1) obj;
        }

        /// <summary>
        ///     Returns a hash code for the instance.
        /// </summary>
        /// <returns>
        ///     A hash code for the instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return 17*23 + this[0, 0].GetHashCode();
            }
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Matrix1x1" />.</param>
        /// <param name="right">The right <see cref="Matrix1x1" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Matrix1x1 left, Matrix1x1 right)
        {
            return FloatingNumber.AreApproximatelyEqual(left[0, 0], right[0, 0]);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Matrix1x1" />.</param>
        /// <param name="right">The right <see cref="Matrix1x1" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Matrix1x1 left, Matrix1x1 right)
        {
            return !FloatingNumber.AreApproximatelyEqual(left[0, 0], right[0, 0]);
        }
    }
}