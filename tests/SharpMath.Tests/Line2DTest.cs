using SharpMath.Geometry;

namespace SharpMath.Tests
{
    public class Line2DTest
    {
        [Fact]
        public void CanCreateLineFromPoints()
        {
            var line = Line2D.FromPoints(new Point2D(0, 2), new Point2D(3, 1));
            Assert.Equal(-1d / 3d, line.Slope);
            Assert.Equal(2, line.Offset);

            var secondLine = Line2D.FromPoints(new Point2D(4, 5), new Point2D(1, 3));
            Assert.Equal(2d / 3d, secondLine.Slope);
            Assert.Equal(7d / 3d, secondLine.Offset);
        }

        [Fact]
        public void CanDetermineIfLinesAreParallel()
        {
            var line = new Line2D(2, 0);
            var secondLine = new Line2D(2, 3);
            var thirdLine = new Line2D(3, 1);
            Assert.True(line.IsParallelTo(secondLine));
            Assert.False(secondLine.IsParallelTo(thirdLine));
        }

        [Fact]
        public void CanDetermineIfLinesIntersect()
        {
            var line = new Line2D(2, 0);
            var secondLine = new Line2D(3, 1);
            var thirdLine = new Line2D(2, 3);
            Assert.True(line.IntersectsWith(secondLine));
            Assert.False(line.IntersectsWith(thirdLine));
        }

        [Fact]
        public void CanDetermineIfPointIsOnLine()
        {
            var line = new Line2D(1, 0);
            Assert.True(line.IsPointOnLine(new Point2D(1, 1)));
            Assert.False(line.IsPointOnLine(new Point2D(2, 1)));

            var secondLine = new Line2D(2, 3);
            Assert.True(secondLine.IsPointOnLine(new Point2D(4, 11)));
            Assert.False(secondLine.IsPointOnLine(new Point2D(3, 2)));
        }

        [Fact]
        public void CanFindIntersectionPoint()
        {
            // 0.5x - 1
            var firstLine = new Line2D(0.5, -1);
            // -2x - 6
            var secondLine = new Line2D(-2, -6);

            var intersectionPoint = firstLine.GetIntersectionPoint(secondLine);
            Assert.Equal(-2, intersectionPoint.X);
            Assert.Equal(-2, intersectionPoint.Y);
        }

        [Fact]
        public void CanGetIntersectionPoint()
        {
            var line = new Line2D(2, 0);
            var secondLine = new Line2D(3, 1);
            Assert.Equal(new Point2D(-1, -2), line.GetIntersectionPoint(secondLine));
        }

        [Fact]
        public void CanGetPoint()
        {
            var line = new Line2D(3, 1);
            Assert.Equal(new Point2D(1, 4), line.GetPoint(1));
            Assert.Equal(new Point2D(10, 31), line.GetPoint(10));
        }
    }
}