// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a 4x4 matrix and provides functions to transform 3-dimensional vectors.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    [Serializable]
    public struct Matrix4x4 : IEnumerable<double>, IEquatable<Matrix4x4>, ISquareMatrix<Matrix4x4>
    {
        /// <summary>
        ///     Initializes a <see cref="Matrix3x3" /> struct.
        /// </summary>
        /// <param name="m11">The value at row 1 and column 1.</param>
        /// <param name="m12">The value at row 1 and column 2.</param>
        /// <param name="m13">The value at row 1 and column 3.</param>
        /// <param name="m14">The value at row 1 and column 4.</param>
        /// <param name="m21">The value at row 2 and column 1.</param>
        /// <param name="m22">The value at row 2 and column 2</param>
        /// <param name="m23">The value at row 2 and column 3.</param>
        /// <param name="m24">The value at row 2 and column 4.</param>
        /// <param name="m31">The value at row 3 and column 1.</param>
        /// <param name="m32">The value at row 3 and column 2.</param>
        /// <param name="m33">The value at row 3 and column 3.</param>
        /// <param name="m34">The value at row 3 and column 4.</param>
        /// <param name="m41">The value at row 4 and column 1.</param>
        /// <param name="m42">The value at row 4 and column 2.</param>
        /// <param name="m43">The value at row 4 and column 3.</param>
        /// <param name="m44">The value at row 4 and column 4.</param>
        public Matrix4x4(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24,
            double m31, double m32, double m33, double m34, double m41, double m42, double m43, double m44)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;
            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;
            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        /// <summary>
        ///     Initializes a <see cref="Matrix4x4" /> struct.
        /// </summary>
        /// <param name="row1">The first row <see cref="Vector4" />.</param>
        /// <param name="row2">The second row <see cref="Vector4" />.</param>
        /// <param name="row3">The third row <see cref="Vector4" />.</param>
        /// <param name="row4">The fourth row <see cref="Vector4" />.</param>
        public Matrix4x4(Vector4 row1, Vector4 row2, Vector4 row3, Vector4 row4)
            : this(
                row1.X, row2.X, row3.X, row4.X, row1.Y, row2.Y, row3.Y, row4.Y, row1.Z, row2.Z, row3.Z, row4.Z, row1.W,
                row2.W, row3.W, row4.W)
        {
        }

        /// <summary>
        ///     Gets the zero <see cref="Matrix4x4" />.
        /// </summary>
        public static Matrix4x4 Zero => new Matrix4x4();

        /// <summary>
        ///     Gets or sets the identity <see cref="Matrix4x4" />.
        /// </summary>
        public static Matrix4x4 Identity => MatrixUtils.GetIdentity<Matrix4x4>();

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
        ///     Gets or sets the value at row 1 and column 4.
        /// </summary>
        public double M14 { get; set; }

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
        ///     Gets or sets the value at row 2 and column 4.
        /// </summary>
        public double M24 { get; set; }

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
        ///     Gets or sets the value at row 3 and column 4.
        /// </summary>
        public double M34 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 4 and column 1.
        /// </summary>
        public double M41 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 4 and column 2.
        /// </summary>
        public double M42 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 4 and column 3.
        /// </summary>
        public double M43 { get; set; }

        /// <summary>
        ///     Gets or sets the value at row 4 and column 4.
        /// </summary>
        public double M44 { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix4x4" /> is singular, or not. If <c>true</c>, this
        ///     <see cref="Matrix4x4" /> doesn't have an inverse.
        /// </summary>
        public bool IsSingular => Determinant.IsApproximatelyEqualTo(0);

        /// <summary>
        ///     Gets the inverse of the <see cref="Matrix4x4" />.
        /// </summary>
        public Matrix4x4 Inverse => MatrixUtils.GaussJordan(this, Identity);

        /// <summary>
        ///     Gets the cofactor <see cref="Matrix4x4" /> of the <see cref="Matrix4x4" />.
        /// </summary>
        public Matrix4x4 CofactorMatrix => this.BuildCofactorMatrix();

        /// <summary>
        ///     Gets the adjugate of the <see cref="Matrix4x4" />.
        /// </summary>
        public Matrix4x4 Adjugate => CofactorMatrix.GetTranspose();

        /// <summary>
        ///     Gets the negated <see cref="Matrix4x4" /> of the <see cref="Matrix4x4" />.
        /// </summary>
        public Matrix4x4 Negate => this.GetNegate();

        /// <summary>
        ///     Gets the transpose of the <see cref="Matrix3x3" />.
        /// </summary>
        public Matrix4x4 Transpose => this.GetTranspose();

        public IEnumerator<double> GetEnumerator()
        {
            return new MatrixEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MatrixEnumerator(this);
        }

        public bool Equals(Matrix4x4 other)
        {
            return this == other;
        }

        /// <summary>
        ///     Gets or sets the value at the specified index. The values are accessed as: (row*4) + column.
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
                        return M14;
                    case 4:
                        return M21;
                    case 5:
                        return M22;
                    case 6:
                        return M23;
                    case 7:
                        return M24;
                    case 8:
                        return M31;
                    case 9:
                        return M32;
                    case 10:
                        return M33;
                    case 11:
                        return M34;
                    case 12:
                        return M41;
                    case 13:
                        return M42;
                    case 14:
                        return M43;
                    case 15:
                        return M44;
                    default:
                        throw new IndexOutOfRangeException("The index must be between 0 and 15.");
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
                        M14 = value;
                        break;
                    case 4:
                        M21 = value;
                        break;
                    case 5:
                        M22 = value;
                        break;
                    case 6:
                        M23 = value;
                        break;
                    case 7:
                        M24 = value;
                        break;
                    case 8:
                        M31 = value;
                        break;
                    case 9:
                        M32 = value;
                        break;
                    case 10:
                        M33 = value;
                        break;
                    case 11:
                        M34 = value;
                        break;
                    case 12:
                        M41 = value;
                        break;
                    case 13:
                        M42 = value;
                        break;
                    case 14:
                        M43 = value;
                        break;
                    case 15:
                        M44 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("The index must be between 0 and 15.");
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
            get { return this[row*4 + column]; }
            set { this[row*4 + column] = value; }
        }

        /// <summary>
        ///     Gets the dimension of the <see cref="Matrix4x4" />.
        /// </summary>
        public uint Dimension => 4;

        /// <summary>
        ///     Gets the row count of the <see cref="Matrix4x4" />.
        /// </summary>
        public uint RowCount => 4;

        /// <summary>
        ///     Gets the column count of the <see cref="Matrix4x4" />.
        /// </summary>
        public uint ColumnCount => 4;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix4x4" /> is orthogonal, or not.
        /// </summary>
        public bool IsOrthogonal => (this*Transpose) == Identity;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix4x4" /> is the identity <see cref="Matrix4x4" />, or not.
        /// </summary>
        public bool IsIdentity
            => M11.IsApproximatelyEqualTo(1) && M22.IsApproximatelyEqualTo(1) && M33.IsApproximatelyEqualTo(1);

        /// <summary>
        ///     Gets the determinant of the <see cref="Matrix4x4" />.
        /// </summary>
        public double Determinant => this.GetDeterminant();

        /// <summary>
        ///     Gets the trace of the <see cref="Matrix4x4" />.
        /// </summary>
        public double Trace => M11 + M22 + M33 + M44;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix4x4" /> is symmetric, or not.
        /// </summary>
        public bool IsSymmetric => this == Transpose;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix4x4" /> is skew symmetric, or not.
        /// </summary>
        public bool IsSkewSymmetric => Negate == Transpose;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix4x4" /> is antisymmetric, or not.
        /// </summary>
        public bool IsAntiSymmetric => this == Transpose.Negate;

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix4x4" /> is a diagonal matrix, or not.
        /// </summary>
        public bool IsDiagonal => this.GetIsDiagonal();

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Matrix4x4" /> is a triangle matrix, or not.
        /// </summary>
        public bool IsTriangle => this.GetIsTriangle();

        /// <summary>
        ///     Creates a <see cref="Matrix4x4" /> from an abstract <see cref="IMatrix" /> object.
        /// </summary>
        /// <param name="matrix">The <see cref="IMatrix" /> to convert.</param>
        /// <returns>The <see cref="Matrix4x4" /> that has been created.</returns>
        public static Matrix4x4 FromMatrix(IMatrix matrix)
        {
            if (matrix.RowCount != 4 || matrix.ColumnCount != 4)
                throw new ArgumentException("The square matrix cannot be converted into a Matrix4x4");

            var resultMatrix = new Matrix4x4();
            for (uint y = 0; y < resultMatrix.Dimension; ++y)
                for (uint x = 0; x < resultMatrix.Dimension; ++x)
                    resultMatrix[y, x] = matrix[y, x];
            return resultMatrix;
        }

        /// <summary>
        ///     Creates a <see cref="Matrix4x4" /> that represents a scalation.
        /// </summary>
        /// <param name="scaleX">The scalation factor of the X-component.</param>
        /// <param name="scaleY">The scalation factor of the Y-component.</param>
        /// <param name="scaleZ">The scalation factor of the Z-component.</param>
        /// <returns>A <see cref="Matrix4x4" /> that represents a scalation using the specified factors.</returns>
        public static Matrix4x4 Scalation(double scaleX, double scaleY, double scaleZ)
        {
            var matrix = Identity;
            matrix[0, 0] = scaleX;
            matrix[1, 1] = scaleY;
            matrix[2, 2] = scaleZ;

            return matrix;
        }

        /// <summary>
        ///     Creates a <see cref="Matrix4x4" /> that represents a scalation.
        /// </summary>
        /// <param name="scale">The scalation factor of all components.</param>
        /// <returns>A <see cref="Matrix4x4" /> that represents a scalation using the specified factor.</returns>
        public static Matrix4x4 Scalation(double scale)
        {
            return Scalation(scale, scale, scale);
        }

        /// <summary>
        ///     Creates a <see cref="Matrix4x4" /> by multiplying the built <see cref="Matrix4x4" /> instances that represent
        ///     rotations around the relating angles.
        /// </summary>
        /// <param name="angleX">The angle to rotate around the X-axis in radians.</param>
        /// <param name="angleY">The angle to rotate around the Y-axis in radians.</param>
        /// <param name="angleZ">The angle to rotate around the Z-axis in radians.</param>
        /// <returns></returns>
        public static Matrix4x4 Rotation(double angleX, double angleY, double angleZ)
        {
            return RotationX(angleX)*RotationY(angleY)*RotationZ(angleZ);
        }

        /// <summary>
        ///     Creates a <see cref="Matrix4x4" /> that represents a rotation around the X-axis.
        /// </summary>
        /// <param name="angle">The angle to rotate around in radians.</param>
        /// <returns>A <see cref="Matrix4x4" /> that represents a rotation around the X-axis using the specified angle.</returns>
        // https://ksgamedev.files.wordpress.com/2010/01/matrix-rotation.png
        public static Matrix4x4 RotationX(double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            var matrix = Identity;
            matrix[1, 1] = cos;
            matrix[1, 2] = -sin;
            matrix[2, 1] = sin;
            matrix[2, 2] = cos;

            return matrix;
        }

        /// <summary>
        ///     Creates a <see cref="Matrix4x4" /> that represents a rotation around the Y-axis.
        /// </summary>
        /// <param name="angle">The angle to rotate around in radians.</param>
        /// <returns>A <see cref="Matrix4x4" /> that represents a rotation around the Y-axis using the specified angle.</returns>
        public static Matrix4x4 RotationY(double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            var matrix = Identity;
            matrix[0, 0] = cos;
            matrix[0, 2] = sin;
            matrix[2, 0] = -sin;
            matrix[2, 2] = cos;

            return matrix;
        }

        /// <summary>
        ///     Creates a <see cref="Matrix4x4" /> that represents a rotation around the Z-axis.
        /// </summary>
        /// <param name="angle">The angle to rotate around in radians.</param>
        /// <returns>A <see cref="Matrix4x4" /> that represents a rotation around the Z-axis using the specified angle.</returns>
        public static Matrix4x4 RotationZ(double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);

            var matrix = Identity;
            matrix[0, 0] = cos;
            matrix[0, 1] = -sin;
            matrix[1, 0] = sin;
            matrix[1, 1] = cos;

            return matrix;
        }

        /// <summary>
        ///     Creates a <see cref="Matrix4x4" /> that represents a translation.
        /// </summary>
        /// <param name="x">The translation of the X-component.</param>
        /// <param name="y">The translation of the Y-component.</param>
        /// <param name="z">The translation of the Z-component.</param>
        /// <returns>A <see cref="Matrix4x4" /> that represents a translation using the specified addends.</returns>
        public static Matrix4x4 Translation(double x, double y, double z)
        {
            var matrix = Identity;
            matrix[0, 3] = x;
            matrix[1, 3] = y;
            matrix[2, 3] = z;

            return matrix;
        }

        /// <summary>
        ///     Creates a perspective projection <see cref="Matrix4x4" /> based on a field of view.
        /// </summary>
        /// <param name="projectionData">The <see cref="ProjectionData" /> to use for the calculations.</param>
        /// <returns>The created perspective projection <see cref="Matrix4x4" /> using the specified <see cref="ProjectionData" />.</returns>
        public static Matrix4x4 PerspectiveFieldOfView(ProjectionData projectionData)
        {
            // https://msdn.microsoft.com/en-us/library/bb205351%28v=vs.85%29.aspx
            /*
                xScale     0          0              0
                0        yScale       0              0
                0        0        zf/(zn-zf)        -1
                0        0        zn*zf/(zn-zf)      0

                yScale = cot(fovY/2)
                xScale = yScale / aspect ratio
            */

            var yScale = 1.0f/(float) Math.Tan(projectionData.FieldOfView/2f);
            var xScale = yScale/projectionData.AspectRatio;

            var invertedDepth = -projectionData.Depth;
            var matrix = Identity;
            matrix[0, 0] = xScale;
            matrix[1, 1] = yScale;
            matrix[2, 2] = projectionData.FarPlane/invertedDepth;
            matrix[2, 3] = -1.0f;
            matrix[3, 2] = projectionData.NearPlane*projectionData.FarPlane/invertedDepth;

            return matrix;
        }

        /// <summary>
        ///     Creates a view <see cref="Matrix4x4" /> using the specified parameters.
        /// </summary>
        /// <param name="cameraPosition">The camera position.</param>
        /// <param name="cameraTarget">The target position of the camera.</param>
        /// <param name="upVector">The up vector.</param>
        /// <returns>The created view <see cref="Matrix4x4" />.</returns>
        public static Matrix4x4 View(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 upVector)
        {
            Vector3 vector = Vector3.FromVector((cameraPosition - cameraTarget).Normalize());
            Vector3 vector2 = Vector3.FromVector(Vector3.VectorProduct(upVector, vector));
            Vector3 vector3 = Vector3.FromVector(Vector3.VectorProduct(vector, vector2));

            var matrix = Identity;
            matrix[0, 0] = vector2.X;
            matrix[0, 1] = vector3.X;
            matrix[0, 2] = vector.X;

            matrix[1, 0] = vector2.Y;
            matrix[1, 1] = vector3.Y;
            matrix[1, 2] = vector.Y;

            matrix[2, 0] = vector2.Z;
            matrix[2, 1] = vector3.Z;
            matrix[2, 2] = vector.Z;

            matrix[3, 0] = -vector2.DotProduct(cameraPosition);
            matrix[3, 1] = -vector3.DotProduct(cameraPosition);
            matrix[3, 2] = -vector.DotProduct(cameraPosition);
            matrix[3, 3] = 1f;

            return matrix;
        }

        /// <summary>
        ///     Creates a world <see cref="Matrix4x4" /> by using the specified components.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="forward">The target vector.</param>
        /// <param name="up">The up vector.</param>
        /// <returns>The created world <see cref="Matrix4x4" />.</returns>
        public static Matrix4x4 World(Vector3 position, Vector3 forward, Vector3 up)
        {
            var xVector = Vector3.VectorProduct(forward, up);
            var yVector = Vector3.VectorProduct(xVector, forward);
            var zVector = Vector3.FromVector(forward.Normalize());
            xVector.Normalize();
            yVector.Normalize();

            var result = new Matrix4x4
            {
                [0, 0] = xVector.X,
                [0, 1] = xVector.Y,
                [0, 2] = xVector.Z,
                [1, 0] = yVector.X,
                [1, 1] = yVector.Y,
                [1, 2] = yVector.Z,
                [2, 0] = -zVector.X,
                [2, 1] = -zVector.Y,
                [2, 2] = -zVector.Z,
                [3, 0] = position.X,
                [3, 1] = position.Y,
                [3, 2] = position.Z,
                [3, 3] = 1f
            };

            return result;
        }

        /// <summary>
        ///     Implements the operator +.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix4x4" />.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix4x4" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix4x4 operator +(Matrix4x4 firstMatrix, Matrix4x4 secondMatrix)
        {
            return MatrixUtils.Add(firstMatrix, secondMatrix);
        }

        /// <summary>
        ///     Implements the operator -.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix4x4" />.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix4x4" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix4x4 operator -(Matrix4x4 firstMatrix, Matrix4x4 secondMatrix)
        {
            return MatrixUtils.Subtract(firstMatrix, secondMatrix);
        }

        /// <summary>
        ///     Implements the operator * to multiply a <see cref="Matrix4x4" /> with the specified scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        /// <param name="matrix">The <see cref="Matrix4x4" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix4x4 operator *(double scalar, Matrix4x4 matrix)
        {
            return MatrixUtils.Multiply(matrix, scalar);
        }

        /// <summary>
        ///     Implements the operator * to multiply a <see cref="Matrix4x4" /> with the specified scalar.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix4x4" />.</param>
        /// <param name="scalar">The scalar.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix4x4 operator *(Matrix4x4 matrix, double scalar)
        {
            return MatrixUtils.Multiply(matrix, scalar);
        }

        /// <summary>
        ///     Implements the operator *.
        /// </summary>
        /// <param name="firstMatrix">The first <see cref="Matrix4x4" />.</param>
        /// <param name="secondMatrix">The second <see cref="Matrix4x4" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Matrix4x4 operator *(Matrix4x4 firstMatrix, Matrix4x4 secondMatrix)
        {
            return MatrixUtils.Multiply<Matrix4x4>(firstMatrix, secondMatrix);
        }

        /// <summary>
        ///     Implements the operator * to transform a <see cref="Vector3" /> with a <see cref="Matrix4x4" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix4x4" />.</param>
        /// <param name="vector">The <see cref="Vector3" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector3 operator *(Matrix4x4 matrix, Vector3 vector)
        {
            var resultMatrix = MatrixUtils.Multiply<Matrix4x1>(matrix,
                new Vector4(vector.X, vector.Y, vector.Z, 1).AsVerticalMatrix<Matrix4x1>());
            var resultVector = new Vector4(resultMatrix[0], resultMatrix[1], resultMatrix[2], resultMatrix[3]);
            resultVector.X /= resultVector.W;
            resultVector.Y /= resultVector.W;
            resultVector.Z /= resultVector.W;
            return resultVector.Convert<Vector3>();
        }

        /// <summary>
        ///     Implements the operator * to transform a <see cref="Vector3" /> with a <see cref="Matrix4x4" />.
        /// </summary>
        /// <param name="vector">The <see cref="Vector3" />.</param>
        /// <param name="matrix">The <see cref="Matrix4x4" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector3 operator *(Vector3 vector, Matrix4x4 matrix)
        {
            return matrix*vector;
        }

        /// <summary>
        ///     Implements the operator * to transform a <see cref="Vector4" /> with a <see cref="Matrix4x4" />.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix4x4" />.</param>
        /// <param name="vector">The <see cref="Vector4" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector4 operator *(Matrix4x4 matrix, Vector4 vector)
        {
            var resultMatrix = MatrixUtils.Multiply<Matrix4x1>(matrix, vector.AsVerticalMatrix<Matrix4x1>());
            return new Vector4(resultMatrix[0], resultMatrix[1], resultMatrix[2], resultMatrix[3]);
        }

        /// <summary>
        ///     Implements the operator * to transform a <see cref="Vector4" /> with a <see cref="Matrix4x4" />.
        /// </summary>
        /// <param name="vector">The <see cref="Vector4" />.</param>
        /// <param name="matrix">The <see cref="Matrix4x4" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static Vector4 operator *(Vector4 vector, Matrix4x4 matrix)
        {
            return matrix*vector;
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
            if (!(obj is Matrix4x4))
                return false;
            return this == (Matrix4x4) obj;
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
        /// <param name="left">The left <see cref="Matrix4x4" />.</param>
        /// <param name="right">The right <see cref="Matrix4x4" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(Matrix4x4 left, Matrix4x4 right)
        {
            return left.SequenceEqual(right);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left <see cref="Matrix4x4" />.</param>
        /// <param name="right">The right <see cref="Matrix4x4" />.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(Matrix4x4 left, Matrix4x4 right)
        {
            return !left.SequenceEqual(right);
        }
    }
}