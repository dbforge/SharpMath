using SharpMath.Geometry;

namespace SharpMath.Tests
{
    public class Line3DTest
    {
        [Fact]
        public void CanCreateLineFromPoints()
        {
            var line = Line3D.FromPoints(new Point3D(Vector3.Zero), new Point3D(Vector3.One));
            Assert.Equal(new Point3D(Vector3.Zero), line.Point);
            Assert.Equal(Vector3.One, line.Direction);

            var secondLine = Line3D.FromPoints(new Point3D(2, 3, 1), new Point3D(3, 2, 4));
            Assert.Equal(new Point3D(2, 3, 1), secondLine.Point);
            Assert.Equal(new Vector3(1, -1, 3), secondLine.Direction);
        }

        [Fact]
        public void CanDetermineIfPointIsOnLine()
        {
            var line = new Line3D(new Point3D(2, 3, 1), new Vector3(2, 1, 2));
            Assert.True(line.IsPointOnLine(new Point3D(6, 5, 5)));
            Assert.False(line.IsPointOnLine(new Point3D(2, 3.5, 2)));
        }

        [Fact]
        public void CanGetPoint()
        {
            var line = new Line3D(new Point3D(2, 3, 1), new Vector3(2, 1, 2));
            Assert.Equal(new Point3D(6, 5, 5), line.GetPoint(2));
        }
    }
}