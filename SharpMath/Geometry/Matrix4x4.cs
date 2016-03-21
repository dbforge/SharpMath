// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;

namespace SharpMath.Geometry
{
    /// <summary>
    ///     Represents a 3x3 matrix and provides functions to transform 3-dimensional vectors.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class Matrix4x4 : SquareMatrix
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="Matrix4x4" /> class.
        /// </summary>
        public Matrix4x4() : base(4)
        {
        }

        /// <summary>
        ///     Gets the identity <see cref="Matrix4x4" />.
        /// </summary>
        public static Matrix4x4 Identity => FromMatrix(GetIdentity(4));

        /// <summary>
        ///     Creates a <see cref="Matrix4x4" /> from an abstract <see cref="Matrix" /> object.
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix" /> to convert.</param>
        /// <returns>The <see cref="Matrix4x4" /> that has been created.</returns>
        public new static Matrix4x4 FromMatrix(Matrix matrix)
        {
            if (!matrix.IsSquare || matrix.RowCount != 4 || matrix.ColumnCount != 4)
                throw new ArgumentException("The matrix cannot be converted into a Matrix4x4.");

            var resultMatrix = new Matrix4x4();
            for (uint y = 0; y < 4; ++y)
                for (uint x = 0; x < 4; ++x)
                    resultMatrix[y, x] = matrix[y, x];
            return resultMatrix;
        }

        /// <summary>
        ///     Creates a <see cref="Matrix4x4" /> from a <see cref="SquareMatrix" /> object.
        /// </summary>
        /// <param name="matrix">The <see cref="SquareMatrix" /> to convert.</param>
        /// <returns>The <see cref="Matrix4x4" /> that has been created.</returns>
        public static Matrix4x4 FromMatrix(SquareMatrix matrix)
        {
            if (matrix.Dimension != 4)
                throw new ArgumentException("The square matrix cannot be converted into a Matrix4x4.");

            var resultMatrix = new Matrix4x4();
            for (uint y = 0; y < 4; ++y)
                for (uint x = 0; x < 4; ++x)
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
            Vector3 vector2 = Vector3.FromVector(Vector3.CrossProduct(upVector, vector));
            Vector3 vector3 = Vector3.FromVector(Vector3.CrossProduct(vector, vector2));

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

            matrix[3, 0] = -Vector.ScalarProduct(vector2, cameraPosition);
            matrix[3, 1] = -Vector.ScalarProduct(vector3, cameraPosition);
            matrix[3, 2] = -Vector.ScalarProduct(vector, cameraPosition);
            matrix[3, 3] = 1f;

            return matrix;
        }

        /// <summary>
        ///     Creates a world <see cref="Matrix4x4" /> by using the specified components.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        public static Matrix4x4 World(Vector3 position, Vector3 forward, Vector3 up)
        {
            var xVector = Vector3.CrossProduct(forward, up);
            var yVector = Vector3.CrossProduct(xVector, forward);
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

        public static Matrix4x4 operator +(Matrix4x4 firstMatrix, Matrix4x4 secondMatrix)
        {
            for (uint y = 0; y < 4; ++y)
                for (uint x = 0; x < 4; ++x)
                    firstMatrix[y, x] += secondMatrix[y, x];
            return firstMatrix;
        }

        public static Matrix4x4 operator -(Matrix4x4 firstMatrix, Matrix4x4 secondMatrix)
        {
            for (uint y = 0; y < 4; ++y)
                for (uint x = 0; x < 4; ++x)
                    firstMatrix[y, x] -= secondMatrix[y, x];
            return firstMatrix;
        }

        public static Matrix4x4 operator *(int scalar, Matrix4x4 matrix)
        {
            return FromMatrix(Multiply(matrix, scalar));
        }

        public static Matrix4x4 operator *(Matrix4x4 firstMatrix, Matrix4x4 secondMatrix)
        {
            return FromMatrix(Multiply(firstMatrix, secondMatrix));
        }

        // TODO: Check if division is correct
        public static Vector3 operator *(Matrix4x4 matrix, Vector3 vector)
        {
            var resultMatrix = Multiply(matrix, new Vector4(vector.X, vector.Y, vector.Z, 1).AsVerticalMatrix());
            var resultVector = Vector4.FromVector(resultMatrix.GetRowVector(0));
            resultVector.X /= resultVector.W;
            resultVector.Z /= resultVector.W;
            resultVector.X /= resultVector.W;
            return resultVector.Convert<Vector3>();
        }

        public static Vector3 operator *(Vector3 vector, Matrix4x4 matrix)
        {
            var resultMatrix = Multiply(matrix, new Vector4(vector.X, vector.Y, vector.Z, 1).AsVerticalMatrix());
            var resultVector = Vector4.FromVector(resultMatrix.GetRowVector(0));
            resultVector.X /= resultVector.W;
            resultVector.Z /= resultVector.W;
            resultVector.X /= resultVector.W;
            return resultVector.Convert<Vector3>();
        }

        public static Vector4 operator *(Matrix4x4 matrix, Vector4 vector)
        {
            var resultMatrix = Multiply(matrix, vector.AsVerticalMatrix());
            return Vector4.FromVector(resultMatrix.GetRowVector(0));
        }

        public static Vector4 operator *(Vector4 vector, Matrix4x4 matrix)
        {
            var resultMatrix = Multiply(matrix, vector.AsVerticalMatrix());
            return Vector4.FromVector(resultMatrix.GetRowVector(0));
        }
    }
}