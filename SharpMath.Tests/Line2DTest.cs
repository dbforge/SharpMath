using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMath.Geometry;

namespace SharpMath.Tests
{
    [TestClass]
    public class Line2DTest
    {
        [TestMethod]
        public void CanFindIntersectionPoint()
        {
            // 0.5x - 1
            var firstLine = new Line2D(0.5, -1);
            // -2x - 6
            var secondLine = new Line2D(-2, -6);

            var intersectionPoint = firstLine.GetIntersectionPoint(secondLine);
            Assert.AreEqual(-2, intersectionPoint.X);
            Assert.AreEqual(-2, intersectionPoint.Y);
        }
    }
}