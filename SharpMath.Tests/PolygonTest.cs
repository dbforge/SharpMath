// PolygonTest.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMath.Geometry;

namespace SharpMath.Tests
{
    [TestClass]
    public class PolygonTest
    {
        [TestMethod]
        public void CanCalculateArea()
        {
            var square =
                new Polygon(new Point2D(0, 0), new Point2D(2, 0), new Point2D(2, 2), new Point2D(0, 2));
            Assert.IsTrue(FloatingNumber.AreApproximatelyEqual(4, square.Area));

            var triangle =
                new Polygon(new Point2D(0, 0), new Point2D(2, 0), new Point2D(1, 2));
            Assert.AreEqual(2, triangle.Area);

            var polygon = new Polygon(new Point2D(-2, -2), new Point2D(2, -2), new Point2D(4, 1), new Point2D(0, 2),
                new Point2D(-4, 1));
            Assert.AreEqual(22, polygon.Area);
        }

        [TestMethod]
        public void CanCalculatePerimeter()
        {
            var square =
                new Polygon(new Point2D(0, 0), new Point2D(2, 0), new Point2D(2, 2), new Point2D(0, 2));
            Assert.AreEqual(8, square.Perimeter);

            var polygon = new Polygon(new Point2D(-2, -2), new Point2D(2, -2), new Point2D(4, 1), new Point2D(0, 2),
                new Point2D(-4, 1));
            Assert.AreEqual(4 + 2 * Math.Sqrt(13) + 2 * Math.Sqrt(17), polygon.Perimeter);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanValidatePointAmount()
        {
            new Polygon(new Point2D(0, 1), new Point2D(3, 4));
        }
    }
}