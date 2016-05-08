// Author: Dominic Beger (Trade/ProgTrade) 2016

using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMath.Geometry;

namespace SharpMath.Tests
{
    [TestClass]
    public class MatrixTest
    {
        [TestMethod]
        public void CanMultiplyMatrices()
        {
            var firstMatrix = new Matrix1x3();
            var secondMatrix = new Matrix3x1();

            firstMatrix[0, 0] = 1;
            firstMatrix[0, 1] = 2;
            firstMatrix[0, 2] = 3;

            secondMatrix[0, 0] = 2;
            secondMatrix[1, 0] = 1;
            secondMatrix[2, 0] = 2;

            var matrixProduct = MatrixUtils.Multiply<Matrix1x1>(firstMatrix, secondMatrix);
            Assert.AreEqual(matrixProduct[0, 0], 10);

            // --------------------------------

            var thirdMatrix = new Matrix3x3
            {
                [0, 0] = 2,
                [0, 1] = 5,
                [0, 2] = 2,
                [1, 0] = 3,
                [1, 1] = -3,
                [1, 2] = 1,
                [2, 0] = 1,
                [2, 1] = 4,
                [2, 2] = -4
            };

            var fourthMatrix = new Matrix3x3
            {
                [0, 0] = -4,
                [0, 1] = 2.5,
                [0, 2] = 3,
                [1, 0] = 5,
                [1, 1] = 6,
                [1, 2] = 4,
                [2, 0] = 9,
                [2, 1] = 10,
                [2, 2] = -9
            };

            /* 
                35.000  55.000   8.000
                -18.000 -0.500   -12.000
                -20.000 -13.500  55.000
            */
            var resultMatrix = new Matrix3x3
            {
                [0, 0] = 35,
                [0, 1] = 55,
                [0, 2] = 8,
                [1, 0] = -18,
                [1, 1] = -0.5,
                [1, 2] = -12,
                [2, 0] = -20,
                [2, 1] = -13.5,
                [2, 2] = 55
            };

            Assert.AreEqual(resultMatrix, MatrixUtils.Multiply<Matrix3x3>(thirdMatrix, fourthMatrix));
        }

        [TestMethod]
        public void CanCalculateDeterminant()
        {
            var firstMatrix = new Matrix1x1 {[0, 0] = 2};
            Assert.AreEqual(2, firstMatrix.Determinant);

            var secondMatrix = new Matrix2x2
            {
                [0, 0] = 8,
                [0, 1] = 3,
                [1, 0] = 4,
                [1, 1] = 2
            };
            Assert.AreEqual(4, secondMatrix.Determinant);

            var thirdMatrix = new Matrix3x3
            {
                [0, 0] = 2,
                [0, 1] = 5,
                [0, 2] = 2,
                [1, 0] = 3,
                [1, 1] = -3,
                [1, 2] = 1,
                [2, 0] = 1,
                [2, 1] = 4,
                [2, 2] = -4
            };

            Assert.AreEqual(111, thirdMatrix.Determinant);

            var fourthMatrix = new Matrix3x3
            {
                [0, 0] = -4,
                [0, 1] = 2.5,
                [0, 2] = 3,
                [1, 0] = 5,
                [1, 1] = 6,
                [1, 2] = 4,
                [2, 0] = 9,
                [2, 1] = 10,
                [2, 2] = -9
            };

            Assert.AreEqual(566.5, fourthMatrix.Determinant);

            var fifthMatrix = new Matrix4x4
            {
                [0, 0] = 1,
                [0, 1] = 0,
                [0, 2] = 0,
                [0, 3] = 1,
                [1, 0] = 1,
                [1, 1] = 2,
                [1, 2] = 3,
                [1, 3] = 2,
                [2, 0] = 2,
                [2, 1] = 3,
                [2, 2] = 4,
                [2, 3] = 0,
                [3, 0] = 1,
                [3, 1] = 2,
                [3, 2] = -1,
                [3, 3] = -2
            };

            Assert.AreEqual(-24, fifthMatrix.Determinant);
        }

        [TestMethod]
        public void CanGetStringRepresentation()
        {
            var matrix = Matrix4x4.View(Vector3.Zero, Vector3.Forward, Vector3.Up);
            Debug.Print(matrix.ToString());
        }

        [TestMethod]
        public void CanDetermineIfMatrixIsDiagonal()
        {
            var matrix = new Matrix3x3
            {
                M11 = 1,
                M22 = 1,
                M33 = 1
            };
            Assert.IsTrue(matrix.IsDiagonal);

            var secondMatrix = new Matrix4x4
            {
                M11 = 1,
                M22 = 1,
                M33 = 1,
                M44 = 1
            };
            Assert.IsTrue(secondMatrix.IsDiagonal);

            var thirdMatrix = new Matrix4x4
            {
                M11 = 1,
                M22 = 1,
                M33 = 1,
                M44 = 1,
                M14 = 1
            };
            Assert.IsFalse(thirdMatrix.IsDiagonal);

            var fourthMatrix = new Matrix4x4
            {
                M11 = 1,
                M22 = 1,
                M33 = 1
            };
            Assert.IsFalse(fourthMatrix.IsDiagonal);
        }

        [TestMethod]
        public void CanDetermineIfMatrixIsTriangle()
        {
            var matrix = new Matrix3x3
            {
                M11 = 1,
                M22 = 2,
                M33 = 3,
                M12 = 3,
                M13 = 4,
                M23 = 7
            };
            Assert.IsTrue(matrix.IsTriangle);

            var secondMatrix = new Matrix4x4
            {
                M11 = 1,
                M22 = 2,
                M33 = 3,
                M44 = 8,
                M12 = 3,
                M13 = 4,
                M14 = 7,
                M23 = 2,
                M24 = 3,
                M34 = 9
            };
            Assert.IsTrue(secondMatrix.IsTriangle);

            var thirdMatrix = new Matrix4x4
            {
                M11 = 5,
                M22 = 7,
                M33 = 4,
                M44 = 3,
                M14 = 2,
                M31 = 9
            };
            Assert.IsFalse(thirdMatrix.IsTriangle);

            var fourthMatrix = new Matrix4x4
            {
                M11 = 1,
                M22 = 1,
                M33 = 1,
                M44 = 1
            };
            Assert.IsTrue(fourthMatrix.IsTriangle);
        }

        // TODO: Implement
        //[TestMethod]
        //public void CanGetCore()
        //{
        //    var matrix = new Matrix3x3
        //    {
        //        M11 = 3,
        //        M12 = -1,
        //        M13 = 4,
        //        M21 = 0,
        //        M22 = 1,
        //        M23 = 5,
        //    };

        //    Assert.AreEqual(new Vector3(-3, -5, 1), matrix.GetCore<Vector3>());
        //}
    }
}