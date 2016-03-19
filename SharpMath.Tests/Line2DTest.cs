using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMath.Geometry;

namespace SharpMath.Tests
{
    [TestClass]
    public class Line2DTest
    {
        [TestMethod]
        public void CanCreateLineFromPoints()
        {
            var line = Line2D.FromPoints(new Point2D(0, 2), new Point2D(3, 1));
            Assert.AreEqual(-1d / 3d, line.Slope);
            Assert.AreEqual(2, line.Offset);

            var secondLine = Line2D.FromPoints(new Point2D(4, 5), new Point2D(1, 3));
            Assert.AreEqual(2d / 3d, secondLine.Slope);
            Assert.AreEqual(7d / 3d, secondLine.Offset);
        }

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

        [TestMethod]
        public void CanDetermineIfPointIsOnLine()
        {
            var line = new Line2D(1, 0);
            Assert.IsTrue(line.IsPointOnLine(new Point2D(1, 1)));
            Assert.IsFalse(line.IsPointOnLine(new Point2D(2, 1)));

            var secondLine = new Line2D(2, 3);
            Assert.IsTrue(secondLine.IsPointOnLine(new Point2D(4, 11)));
            Assert.IsFalse(secondLine.IsPointOnLine(new Point2D(3, 2)));
        }

        [TestMethod]
        public void CanDetermineIfLinesAreParallel()
        {
            var line = new Line2D(2, 0);
            var secondLine = new Line2D(2, 3);
            var thirdLine = new Line2D(3, 1);
            Assert.IsTrue(line.IsParallelTo(secondLine));
            Assert.IsFalse(secondLine.IsParallelTo(thirdLine));
        }

        [TestMethod]
        public void CanDetermineIfLinesIntersect()
        {
            var line = new Line2D(2, 0);
            var secondLine = new Line2D(3, 1);
            var thirdLine = new Line2D(2, 3);
            Assert.IsTrue(line.IntersectsWith(secondLine));
            Assert.IsFalse(line.IntersectsWith(thirdLine));
        }

        [TestMethod]
        public void CanGetIntersectionPoint()
        {
            var line = new Line2D(2, 0);
            var secondLine = new Line2D(3, 1);
            Assert.AreEqual(new Point2D(-1, -2), line.GetIntersectionPoint(secondLine));
        }

        [TestMethod]
        public void CanGetPoint()
        {
            var line = new Line2D(3, 1);
            Assert.AreEqual(new Point2D(1, 4), line.GetPoint(1));
            Assert.AreEqual(new Point2D(10, 31), line.GetPoint(10));
        }
    }
}