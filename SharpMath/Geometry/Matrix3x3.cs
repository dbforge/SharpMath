// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a 3x3 matrix and provides functions to transform 2-dimensional vectors.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class Matrix3x3 : SquareMatrix
    {
        /// <summary>
        ///     wa
        ///     Creates a new instance of the <see cref="Matrix3x3" /> class.
        /// </summary>
        public Matrix3x3() : base(3)
        {
        }

        /// <summary>
        ///     Gets the identity <see cref="Matrix4x4" />.
        /// </summary>
        public static Matrix3x3 Identity => FromMatrix(GetIdentity(3));

        /// <summary>
        ///     Creates a <see cref="Matrix3x3" /> from an abstract <see cref="Matrix" /> object.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix" /> to convert.</param>
        /// <returns>The <see cref="Matrix3x3" /> that has been created.</returns>
        public new static Matrix3x3 FromMatrix(Matrix matrix)
        {
            if (matrix.ColumnCount != 3 || matrix.RowCount != 3)
                throw new ArgumentException("The matrix cannot be converted into a Matrix3x3");

            var resultMatrix = new Matrix3x3();
            for (uint y = 0; y < 3; ++y)
                for (uint x = 0; x < 3; ++x)
                    resultMatrix[y, x] = matrix[y, x];
            return resultMatrix;
        }

        /// <summary>
        ///     Creates a <see cref="Matrix3x3" /> from a <see cref="SquareMatrix" /> object.
        /// </summary>
        /// <param name="matrix">The <see cref="SquareMatrix" /> to convert.</param>
        /// <returns>The <see cref="Matrix3x3" /> that has been created.</returns>
        public static Matrix3x3 FromMatrix(SquareMatrix matrix)
        {
            if (matrix.Dimension != 3)
                throw new ArgumentException("The square matrix cannot be converted into a Matrix4x4");

            var resultMatrix = new Matrix3x3();
            for (uint y = 0; y < 3; ++y)
                for (uint x = 0; x < 3; ++x)
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
            // This is actually the same as Matrix4x4.RotationZ as we are rotating around the Z-axis that is 0 in 2D-space.

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
            return FromMatrix(Add(firstMatrix, secondMatrix));
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
            return FromMatrix(Subtract(firstMatrix, secondMatrix));
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
            return FromMatrix(Multiply(matrix, scalar));
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
            return FromMatrix(Multiply(matrix, scalar));
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
            var resultMatrix = Multiply(matrix, new Vector3(vector.X, vector.Y, 1).AsVerticalMatrix());
            return resultMatrix.GetRowVector(0).Convert<Vector2>();
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
            var resultMatrix = Multiply(matrix, new Vector3(vector.X, vector.Y, 1).AsVerticalMatrix());
            return resultMatrix.GetRowVector(0).Convert<Vector2>();
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
            var resultMatrix = Multiply(matrix, vector.AsVerticalMatrix());
            return Vector3.FromVector(resultMatrix.GetRowVector(0));
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
            var resultMatrix = Multiply(matrix, vector.AsVerticalMatrix());
            return Vector3.FromVector(resultMatrix.GetRowVector(0));
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
            return FromMatrix(Multiply(firstMatrix, secondMatrix));
        }
    }
}