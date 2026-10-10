using System;
using SharpMath.Geometry;

namespace SharpMath.Tests
{
    public class PolygonTest
    {
        [Fact]
        public void CanCalculateArea()
        {
            var square =
                new Polygon(new Point2D(0, 0), new Point2D(2, 0), new Point2D(2, 2), new Point2D(0, 2));
            Assert.True(FloatingNumber.AreApproximatelyEqual(4, square.Area));

            var triangle =
                new Polygon(new Point2D(0, 0), new Point2D(2, 0), new Point2D(1, 2));
            Assert.Equal(2, triangle.Area);

            var polygon = new Polygon(new Point2D(-2, -2), new Point2D(2, -2), new Point2D(4, 1), new Point2D(0, 2),
                new Point2D(-4, 1));
            Assert.Equal(22, polygon.Area);
        }

        [Fact]
        public void CanCalculatePerimeter()
        {
            var square =
                new Polygon(new Point2D(0, 0), new Point2D(2, 0), new Point2D(2, 2), new Point2D(0, 2));
            Assert.Equal(8, square.Perimeter);

            var polygon = new Polygon(new Point2D(-2, -2), new Point2D(2, -2), new Point2D(4, 1), new Point2D(0, 2),
                new Point2D(-4, 1));
            Assert.Equal(4 + 2 * Math.Sqrt(13) + 2 * Math.Sqrt(17), polygon.Perimeter);
        }

        [Fact]
        public void CanValidatePointAmount()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new Polygon(new Point2D(0, 1), new Point2D(3, 4));
            });
        }
    }
}