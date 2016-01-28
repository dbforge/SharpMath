using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMath.Geometry;

namespace SharpMath.Tests
{
    [TestClass]
    public class PolygonTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CanValidatePointAmount()
        {
            new Polygon(new Point2D(0, 1), new Point2D(3, 4));
        }

        [TestMethod]
        public void CanCalculateArea()
        {
            var square =
                new Polygon(new Point2D(0, 0), new Point2D(2, 0), new Point2D(2, 2), new Point2D(0, 2));
            Assert.AreEqual(4, square.Area);

            var triangle =
                new Polygon(new Point2D(0, 0), new Point2D(2, 0), new Point2D(1, 2));
            Assert.AreEqual(2, triangle.Area);
        }
    }
}