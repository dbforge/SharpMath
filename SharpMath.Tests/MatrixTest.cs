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
            var firstMatrix = new Matrix(2, 3);
            var secondMatrix = new Matrix(3, 2);

            firstMatrix[0, 0] = 1;
            firstMatrix[0, 1] = 2;
            firstMatrix[0, 2] = 3;
            firstMatrix[1, 0] = 3;
            firstMatrix[1, 1] = 1;
            firstMatrix[1, 2] = 1;

            secondMatrix[0, 0] = 2;
            secondMatrix[0, 1] = 1;
            secondMatrix[1, 0] = 1;
            secondMatrix[1, 1] = 2;
            secondMatrix[2, 0] = 2;
            secondMatrix[2, 1] = 1;

            var matrixProduct = Matrix.Multiply(firstMatrix, secondMatrix);
            Assert.AreEqual(matrixProduct[0, 0], 10);
            Assert.AreEqual(matrixProduct[0, 1], 8);
            Assert.AreEqual(matrixProduct[1, 0], 9);
            Assert.AreEqual(matrixProduct[1, 1], 6);
        }

        [TestMethod]
        public void CanCalculateDeterminant()
        {
            var firstMatrix = new SquareMatrix(1) {[0, 0] = 2};
            Assert.AreEqual(2, firstMatrix.Determinant);

            var secondMatrix = new SquareMatrix(2)
            {
                [0, 0] = 8,
                [0, 1] = 3,
                [1, 0] = 4,
                [1, 1] = 2
            };
            Assert.AreEqual(4, secondMatrix.Determinant);

            var thirdMatrix = new SquareMatrix(3)
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

            var fourthMatrix = new SquareMatrix(3)
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

            var fifthMatrix = new SquareMatrix(4)
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
    }
}