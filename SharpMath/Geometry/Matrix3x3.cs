// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a 3x3 matrix and provides functions to transform 2-dimensional vectors.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    [Serializable]
    public struct Matrix3x3 : IEnumerable<double>, IEquatable<Matrix3x3>, ISquareMatrix<Matrix3x3>
    {
        /// <summary>
        ///     Initializes a <see cref="Matrix3x3" /> struct.
        /// </summary>
        /// <param name="m11">The value at row 1 and column 1.</param>
        /// <param name="m12">The value at row 1 and column 2.</param>
        /// <param name="m13">The value at row 1 and column 3.</param>
        /// <param name="m21">The value at row 2 and column 1.</param>
        /// <param name="m22">The value at row 2 and column 2</param>
        /// <param name="m23">The value at row 2 and column 3.</param>
        /// <param name="m31">The value at row 3 and column 1.</param>
        /// <param name="m32">The value at row 3 and column 2.</param>
        /// <param name="m33">The value at row 3 and column 3.</param>
        public Matrix3x3(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32,
            double m33)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        /// <summary>
        ///     Initializes a <see cref="Matrix3x3" /> struct.
        /// </summary>
        /// <param name="row1">The first row <see cref="Vector3" />.</param>
        /// <param name="row2">The second row <see cref="Vector3" />.</param>
        /// <param name="row3">The third row <see cref="Vector3" />.</param>
        public Matrix3x3(Vector3 row1, Vector3 row2, Vector3 row3)
            : this(row1.X, row2.X, row3.X, row1.Y, row2.Y, row3.Y, row1.Z, row2.Z, row3.Z)
        {
        }

        /// <summary>
        ///     Gets the zero <see cref="Matrix3x3" />.
        /// </summary>
        public static Matrix3x3 Zero => new Matrix3x3();

        /// <summary>
        ///     Gets the identity <see cref="Matrix3x3" />.
        /// </summary>
        public static Matrix3x3 Identity => MatrixUtils.GetIdentity<Matrix3x3>();

        /// <summary>
        ///     Gets or sets the value at row 1 and column 1.
        /// </summary>
        public double M11 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 1 and column 2.
        /// </summary>
        public double M12 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 1 and column 3.
        /// </summary>
        public double M13 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 2 and column 1.
        /// </summary>
        public double M21 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 2 and column 2.
        /// </summary>
        public double M22 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 2 and column 3.
        /// </summary>
        public double M23 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 3 and column 1.
        /// </summary>
        public double M31 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 3 and column 2.
        /// </summary>
        public double M32 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 3 and column 3.
        /// </summary>
        public double M33 { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix3x3" /> is singular, or not. If <c>true</c>, this
        ///     <see cref="Matrix3x3" /> doesn't have an inverse.
        /// </summary>
        public bool IsSingular => Determinant.IsApproximatelyEqualTo(0);

        /// <summary>
        ///     Gets the inverse of the <see cref="Matrix3x3" />.
        /// </summary>
        public Matrix3x3 Inverse => MatrixUtils.GaussJordan(this, Identity);

        /// <summary>
        ///     Gets the cofactor <see cref="Matrix3x3" /> of the <see cref="Matrix3x3" />.
        /// </summary>
        public Matrix3x3 CofactorMatrix => this.BuildCofactorMatrix();

        /// <summary>
        ///     Gets the adjugate of the <see cref="Matrix3x3" />.
        /// </summary>
        public Matrix3x3 Adjugate => CofactorMatrix.GetTranspose();

        /// <summary>
        ///     Gets the negated <see cref="Matrix3x3" /> of the <see cref="Matrix3x3" />.
        /// </summary>
        public Matrix3x3 Negate => this.GetNegate();

        /// <summary>
        ///     Gets the transpose of the <see cref="Matrix3x3" />.
        /// </summary>
        public Matrix3x3 Transpose => this.GetTranspose();

        public IEnumerator<double> GetEnumerator()
        {
            return new MatrixEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MatrixEnumerator(this);
        }

        public bool Equals(Matrix3x3 other)
        {
            return this == other;
        }

        /// <summary>
        ///     Gets or sets the value at the specified index. The values are accessed as: (row*3) + column.
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
                        return M13;
                    case 3:
                        return M21;
                    case 4:
                        return M22;
                    case 5:
                        return M23;
                    case 6:
                        return M31;
                    case 7:
                        return M32;
                    case 8:
                        return M33;
                    default:
                        throw new IndexOutOfRangeException("The index must be between 0 and 8.");
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
                        M13 = value;
                        break;
                    case 3:
                        M21 = value;
                        break;
                    case 4:
                        M22 = value;
                        break;
                    case 5:
                        M23 = value;
                        break;
                    case 6:
                        M31 = value;
                        break;
                    case 7:
                        M32 = value;
                        break;
                    case 8:
                        M33 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("The index must be between 0 and 8.");
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
            get { return this[row*3 + column]; }
            set { this[row*3 + column] = value; }
        }

        /// <summary>
        ///     Gets the dimension of the <see cref="Matrix3x3" />.
        /// </summary>
        public uint Dimension => 3;

        /// <summary>
        ///     Gets the row count of the <see cref="Matrix3x3" />.
        /// </summary>
        public uint RowCount => 3;

        /// <summary>
        ///     Gets the column count of the <see cref="Matrix3x3" />.
        /// </summary>
        public uint ColumnCount => 3;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix3x3" /> is orthogonal, or not.
        /// </summary>
        public bool IsOrthogonal => (this*Transpose) == Identity;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix3x3" /> is the identity <see cref="Matrix3x3" />, or not.
        /// </summary>
        public bool IsIdentity
            => M11.IsApproximatelyEqualTo(1) && M22.IsApproximatelyEqualTo(1) && M33.IsApproximatelyEqualTo(1);

        /// <summary>
        ///     Gets the determinant of the <see cref="Matrix3x3" />.
        /// </summary>
        public double Determinant => this.GetDeterminant();

        /// <summary>
        ///     Gets the trace of the <see cref="Matrix3x3" />.
        /// </summary>
        public double Trace => M11 + M22 + M33;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix3x3" /> is symmetric, or not.
        /// </summary>
        public bool IsSymmetric => this == Transpose;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix3x3" /> is skew symmetric, or not.
        /// </summary>
        public bool IsSkewSymmetric => Negate == Transpose;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix3x3" /> is antisymmetric, or not.
        /// </summary>
        public bool IsAntiSymmetric => this == Transpose.Negate;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix3x3" /> is a diagonal matrix, or not.
        /// </summary>
        public bool IsDiagonal => this.GetIsDiagonal();

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix3x3" /> is a triangle matrix, or not.
        /// </summary>
        public bool IsTriangle => this.GetIsTriangle();

        /// <summary>
        ///     Creates a <see cref="Matrix3x3" /> from a <see cref="IMatrix" /> object.
        /// </summary>
        /// <param name="matrix">The <see cref="IMatrix" /> to convert.</param>
        /// <returns>The <see cref="Matrix3x3" /> that has been created.</returns>
        public static Matrix3x3 FromMatrix(IMatrix matrix)
        {
            if (matrix.RowCount != 3 || matrix.ColumnCount != 3)
                throw new ArgumentException("The square matrix cannot be converted into a Matrix3x3");

            var resultMatrix = new Matrix3x3();
            for (uint y = 0; y < resultMatrix.Dimension; ++y)
                for (uint x = 0; x < resultMatrix.Dimension; ++x)
                    resultMatrix[y, x] = matrix[y, x];
            return resultMatrix;
        }

        /// <summary>
        ///     Creates a <see cref="Matrix3x3" /> that represents a rotation.
        /// </summary>
        /// <param name="angle">The angle to rotate around in radians.</param>
        /// <returns>A <see cref="Matrix3x3" /> that represents a rotation using the specified angle.</returns>
        public static Matrix3x3 Rotation(double angle)
        {
            // the is actually the same as Matrix4x4.RotationZ as we are rotating around the Z-axis that is 0 in 2D-space.

            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            var matrix = Identity;
            matrix[0, 0] = cos;
            matrix[1, 0] = sin;
            matrix[0, 1] = -sin;
            matrix[1, 1] = cos;

            return matrix;
        }

        /// <summary>
        ///     Creates a <see cref="Matrix3x3" /> that represents a scalation.
        /// </summary>
        /// <param name="scaleX">The scalation factor of the X-component.</param>
        /// <param name="scaleY">The scalation factor of the Y-component.</param>
        /// <returns>A <see cref="Matrix3x3" /> that represents a scalation using the specified factors.</returns>
        public static Matrix3x3 Scalation(double scaleX, double scaleY)
        {
            var matrix = Identity;
            matrix[0, 0] = scaleX;
            matrix[1, 1] = scaleY;

            return matrix;
        }

        /// <summary>
        ///     Creates a <see cref="Matrix3x3" /> that represents a translation.
        /// </summary>
        /// <param name="x">The translation of the X-component.</param>
        /// <param name="y">The translation of the Y-component.</param>
        /// <returns>A <see cref="Matrix3x3" /> that represents a translation using the specified addends.</returns>
        public static Matrix3x3 Translation(double x, double y)
        {
            var matrix = Identity;
            matrix[2, 0] = x;
            matrix[2, 1] = y;

            return matrix;
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix3x3" />.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix3x3" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix3x3 operator +(Matrix3x3 firstMatrix, Matrix3x3 secondMatrix)
        {
            return MatrixUtils.Add(firstMatrix, secondMatrix);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix3x3" />.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix3x3" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix3x3 operator -(Matrix3x3 firstMatrix, Matrix3x3 secondMatrix)
        {
            return MatrixUtils.Subtract(firstMatrix, secondMatrix);
        }

        /// <summary>
        ///     Implements the operator * to multiply a <see cref="Matrix3x3" /> with the specified scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="matrix">The <see cref="Matrix3x3" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix3x3 operator *(double scalar, Matrix3x3 matrix)
        {
            return MatrixUtils.Multiply(matrix, scalar);
        }

        /// <summary>
        ///     Implements the operator * to multiply a <see cref="Matrix3x3" /> with the specified scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix3x3" />.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix3x3 operator *(Matrix3x3 matrix, double scalar)
        {
            return MatrixUtils.Multiply(matrix, scalar);
        }

        /// <summary>
        ///     Implements the operator * to transform a <see cref="Vector2" /> with a <see cref="Matrix3x3" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix3x3" />.</param>
        /// <param name="vector">The <see cref="Vector2" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector2 operator *(Matrix3x3 matrix, Vector2 vector)
        {
            var resultMatrix = MatrixUtils.Multiply<Matrix3x1>(matrix,
                new Vector3(vector.X, vector.Y, 1).AsVerticalMatrix<Matrix3x1>());
            return new Vector2(resultMatrix[0], resultMatrix[1]);
        }

        /// <summary>
        ///     Implements the operator * to transform a <see cref="Vector2" /> with a <see cref="Matrix3x3" />.
        /// </summary>
        /// <param name="vector">The <see cref="Vector2" />.</param>
        /// <param name="matrix">The <see cref="Matrix3x3" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector2 operator *(Vector2 vector, Matrix3x3 matrix)
        {
            return matrix*vector;
        }

        /// <summary>
        ///     Implements the operator * to transform a <see cref="Vector3" /> with a <see cref="Matrix3x3" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix3x3" />.</param>
        /// <param name="vector">The <see cref="Vector3" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector3 operator *(Matrix3x3 matrix, Vector3 vector)
        {
            var resultMatrix = MatrixUtils.Multiply<Matrix3x1>(matrix, vector.AsVerticalMatrix<Matrix3x1>());
            return new Vector3(resultMatrix[0], resultMatrix[1], resultMatrix[2]);
        }

        /// <summary>
        ///     Implements the operator * to transform a <see cref="Vector3" /> with a <see cref="Matrix3x3" />.
        /// </summary>
        /// <param name="vector">The <see cref="Vector3" />.</param>
        /// <param name="matrix">The <see cref="Matrix3x3" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector3 operator *(Vector3 vector, Matrix3x3 matrix)
        {
            return matrix*vector;
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix3x3" />.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix3x3" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix3x3 operator *(Matrix3x3 firstMatrix, Matrix3x3 secondMatrix)
        {
            return MatrixUtils.Multiply<Matrix3x3>(firstMatrix, secondMatrix);
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
            if (!(obj is Matrix3x3))
                return false;
            return this == (Matrix3x3) obj;
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

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left <see cref="Matrix3x3" />.</param>
        /// <param name="right">The right <see cref="Matrix3x3" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Matrix3x3 left, Matrix3x3 right)
        {
            return left.SequenceEqual(right);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Matrix3x3" />.</param>
        /// <param name="right">The right <see cref="Matrix3x3" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Matrix3x3 left, Matrix3x3 right)
        {
            return !left.SequenceEqual(right);
        }
    }
}