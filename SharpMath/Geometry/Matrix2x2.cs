// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a 2x2 matrix.
    /// </summary>
    [Serializable]
    public struct Matrix2x2 : IEnumerable<double>, ISquareMatrix<Matrix2x2>
    {
        /// <summary>
        ///     Initializes a <see cref="Matrix2x2" /> struct.
        /// </summary>
        /// <param name="m11">The value at row 1 and column 1.</param>
        /// <param name="m12">The value at row 1 and column 2.</param>
        /// <param name="m21">The value at row 2 and column 1.</param>
        /// <param name="m22">The value at row 2 and column 2.</param>
        public Matrix2x2(double m11, double m12, double m21, double m22)
        {
            M11 = m11;
            M12 = m12;
            M21 = m21;
            M22 = m22;
        }

        /// <summary>
        ///     Gets the zero <see cref="Matrix2x2" />.
        /// </summary>
        public static Matrix2x2 Zero => new Matrix2x2();

        /// <summary>
        ///     Gets the identity <see cref="Matrix2x2" />.
        /// </summary>
        public static Matrix2x2 Identity => MatrixUtils.GetIdentity<Matrix2x2>();

        /// <summary>
        ///     Gets or sets the value at row 1 and column 1.
        /// </summary>
        public double M11 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 1 and column 2.
        /// </summary>
        public double M12 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 2 and column 1.
        /// </summary>
        public double M21 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 2 and column 2.
        /// </summary>
        public double M22 { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix2x2" /> is singular, or not. If <c>true</c>, this
        ///     <see cref="Matrix2x2" /> doesn't have an inverse.
        /// </summary>
        public bool IsSingular => Determinant.IsApproximatelyEqualTo(0);

        /// <summary>
        ///     Gets the inverse of the <see cref="Matrix2x2" />.
        /// </summary>
        public Matrix2x2 Inverse => MatrixUtils.GaussJordan(this, Identity);

        /// <summary>
        ///     Gets the cofactor <see cref="Matrix2x2" /> of the <see cref="Matrix2x2" />.
        /// </summary>
        public Matrix2x2 CofactorMatrix => this.BuildCofactorMatrix();

        /// <summary>
        ///     Gets the adjugate of the <see cref="Matrix2x2" />.
        /// </summary>
        public Matrix2x2 Adjugate => CofactorMatrix.Transpose;

        /// <summary>
        ///     Gets the negated <see cref="Matrix2x2" /> of the <see cref="Matrix2x2" />.
        /// </summary>
        public Matrix2x2 Negate => this.GetNegate();

        /// <summary>
        ///     Gets the transpose of the <see cref="Matrix2x2" />.
        /// </summary>
        public Matrix2x2 Transpose => this.GetTranspose();

        public IEnumerator<double> GetEnumerator()
        {
            return new MatrixEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MatrixEnumerator(this);
        }

        /// <summary>
        ///     Gets or sets the value at the specified index. The values are accessed as: (row*2) + column.
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
                    case 1:
                        return M12;
                    case 2:
                        return M21;
                    case 3:
                        return M22;
                    default:
                        throw new IndexOutOfRangeException("The index must be between 0 and 3.");
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        M11 = value;
                        break;
                    case 1:
                        M12 = value;
                        break;
                    case 2:
                        M21 = value;
                        break;
                    case 3:
                        M22 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("The index must be between 0 and 3.");
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
            get { return this[row*2 + column]; }
            set { this[row*2 + column] = value; }
        }

        /// <summary>
        ///     Gets the dimension of the <see cref="Matrix2x2" />.
        /// </summary>
        public uint Dimension => 2;

        /// <summary>
        ///     Gets the row count of the <see cref="Matrix2x2" />.
        /// </summary>
        public uint RowCount => 2;

        /// <summary>
        ///     Gets the column count of the <see cref="Matrix2x2" />.
        /// </summary>
        public uint ColumnCount => 2;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix2x2" /> is orthogonal, or not.
        /// </summary>
        public bool IsOrthogonal => (this*Transpose) == Identity;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix2x2" /> is the identity <see cref="Matrix2x2" />, or not.
        /// </summary>
        public bool IsIdentity => M11.IsApproximatelyEqualTo(1) && M22.IsApproximatelyEqualTo(0);

        /// <summary>
        ///     Gets the determinant of the <see cref="Matrix2x2" />.
        /// </summary>
        public double Determinant => this.GetDeterminant();

        /// <summary>
        ///     Gets the trace of the <see cref="Matrix2x2" />.
        /// </summary>
        public double Trace => M11 + M22;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix2x2" /> is symmetric, or not.
        /// </summary>
        public bool IsSymmetric => this == Transpose;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix2x2" /> is skew symmetric, or not.
        /// </summary>
        public bool IsSkewSymmetric => Negate == Transpose;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix2x2" /> is antisymmetric, or not.
        /// </summary>
        public bool IsAntiSymmetric => this == Transpose.Negate;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix2x2" /> is a diagonal matrix, or not.
        /// </summary>
        public bool IsDiagonal => this.GetIsDiagonal();

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix2x2" /> is a triangle matrix, or not.
        /// </summary>
        public bool IsTriangle => this.GetIsTriangle();

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix2x2" />.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix2x2" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix2x2 operator +(Matrix2x2 firstMatrix, Matrix2x2 secondMatrix)
        {
            return MatrixUtils.Add(firstMatrix, secondMatrix);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix2x2" />.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix2x2" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix2x2 operator -(Matrix2x2 firstMatrix, Matrix2x2 secondMatrix)
        {
            return MatrixUtils.Subtract(firstMatrix, secondMatrix);
        }

        /// <summary>
        ///     Implements the operator * to multiply a <see cref="Matrix2x2" /> with the specified scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="matrix">The <see cref="Matrix2x2" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix2x2 operator *(double scalar, Matrix2x2 matrix)
        {
            return MatrixUtils.Multiply(matrix, scalar);
        }

        /// <summary>
        ///     Implements the operator * to multiply a <see cref="Matrix2x2" /> with the specified scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix2x2" />.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix2x2 operator *(Matrix2x2 matrix, double scalar)
        {
            return MatrixUtils.Multiply(matrix, scalar);
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix2x2" />.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix2x2" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix2x2 operator *(Matrix2x2 firstMatrix, Matrix2x2 secondMatrix)
        {
            return MatrixUtils.Multiply<Matrix2x2>(firstMatrix, secondMatrix);
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
            if (!(obj is Matrix2x2))
                return false;
            return this == (Matrix2x2) obj;
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
                int hash = 17;
                for (uint y = 0; y < RowCount; ++y)
                {
                    for (uint x = 0; x < ColumnCount; ++x)
                    {
                        hash = hash*23 + this[y, x].GetHashCode();
                    }
                }
                return hash;
            }
        }

        public bool Equals(Matrix2x2 other)
        {
            return this == other;
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Matrix2x2" />.</param>
        /// <param name="right">The right <see cref="Matrix2x2" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Matrix2x2 left, Matrix2x2 right)
        {
            return left.SequenceEqual(right);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Matrix2x2" />.</param>
        /// <param name="right">The right <see cref="Matrix2x2" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Matrix2x2 left, Matrix2x2 right)
        {
            return !left.SequenceEqual(right);
        }
    }
}