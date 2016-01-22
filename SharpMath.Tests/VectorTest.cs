using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMath.Geometry;
using SharpMath.Trigonometry;
using System;
using System.Diagnostics;

namespace SharpMath.Tests
{
    [TestClass]
    public class VectorTest
    {
        [TestMethod]
        public void CanUseIndexer()
        {
            var vector = new Vector3(10, 5, 3);
            double x = vector[0];
            double y = vector[1];
            double z = vector[2];

            Assert.AreEqual(x, 10);
            Assert.AreEqual(y, 5);
            Assert.AreEqual(z, 3);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void CanNotUseIndexerWithInvalidIndex()
        {
            var vector = new Vector3(10, 5, 3);
            double w = vector[3]; // Throws an exception
        }

        [TestMethod]
        public void CanCalculateLength()
        {
            var vector = new Vector3(13, 0, 0);
            Assert.AreEqual(13, vector.Magnitude);

            var secondVector = new Vector3(1, 2, 2);
            Assert.AreEqual(3, secondVector.Magnitude);
        }

        [TestMethod]
        public void CanCalculateDistance()
        {
            var vector = new Vector3(10, 5, 3);
            var secondVector = new Vector3(5, 6, 1); // 5, -1, 2 // Magnitude: sqrt(30)
            Assert.AreEqual(Math.Sqrt(30), vector.DistanceTo(secondVector));
        }

        [TestMethod]
        public void CanCalculateCrossProduct()
        {
            var vector = new Vector3(1, -5, 2);
            var secondVector = new Vector3(2, 0, 3);
            var resultVector = vector.CrossProduct(secondVector);

            Assert.AreEqual(-15, resultVector.X);
            Assert.AreEqual(-1, resultVector.Y);
            Assert.AreEqual(10, resultVector.Z);
        }

        [TestMethod]
        public void CanGetCorrectLaTeXString()
        {
            var vector = new Vector3(1, 6, 8);
            Assert.AreEqual(@"\left( \begin{array}{c} 1 \\ 6 \\ 8 \end{array} \right)", vector.LaTeXString);
        }

        [TestMethod]
        public void CanLerp()
        {
            var firstVector = new Vector3(2, 6, 8);
            var secondVector = new Vector3(6, 10, 10);
            var lerpResult = Vector.Lerp(firstVector, secondVector, 0.5);

            // ((6,10,10)-(2,6,8))*0.5+(2,6,8) = (4,4,2)*0.5+(2,6,8) = (2,2,1)+(2,6,8) = (4,8,9)
            Assert.AreEqual(new Vector3(4, 8, 9).ToString(), Vector3.FromVector(lerpResult).ToString());

            var thirdVector = new Vector3(13, 2, 9);
            var fourthVector = new Vector3(3, 10, 5);
            var secondLerpResult = Vector.Lerp(thirdVector, fourthVector, 0.25);

            // ((3,10,5)-(13,2,9))*0.25+(13,2,9) = (-10,8,-4)*0.25+(13,2,9) = (-2.5,2,-1)+(13,2,9) = (10.5,4,8)
            Assert.AreEqual(new Vector3(10.5, 4, 8).ToString(), Vector3.FromVector(secondLerpResult).ToString());
        }

        [TestMethod]
        public void CanConvertVectors()
        {
            var firstVector = new Vector3(2, 6, 8);
            var newVector = Vector2.FromVector(firstVector.Convert<Vector2>());
            Assert.AreEqual(2, (int)newVector.Dimension);
            Assert.AreEqual(2, newVector.X);
            Assert.AreEqual(6, newVector.Y);
        }

        [TestMethod]
        public void CanRotateTwoDimensionalVector()
        {
            var firstVector = new Vector2(0, 1);
            var rotatedVector = Matrix3x3.Rotation(Converter.DegreesToRadians(180)) * firstVector;
            Assert.IsTrue(FloatingNumber.AreApproximatelyEqual(firstVector.X, 0));
            Assert.AreEqual(-1, rotatedVector.Y);
        }

        [TestMethod]
        public void CanCreateScalarTripleProduct()
        {
            var firstVector = new Vector3(2, 0, 5);
            var secondVector = new Vector3(-1, 5, -2);
            var thirdVector = new Vector3(2, 1, 2);

            var scalarTripleProduct = Vector3.ScalarTripleProduct(firstVector, secondVector, thirdVector);
            Debug.Print(scalarTripleProduct.ToString());
            Assert.AreEqual(31, scalarTripleProduct);
        }
    }
}