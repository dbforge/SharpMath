using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMath.Geometry;

namespace SharpMath.Tests
{
    [TestClass]
    public class Line3DTest
    {
        [TestMethod]
        public void CanCreateLineFromPoints()
        {
            var line = Line3D.FromPoints(new Point3D(Vector3.Zero), new Point3D(Vector3.One));
            Assert.AreEqual(new Point3D(Vector3.Zero), line.Point);
            Assert.AreEqual(Vector3.One, line.Direction);

            var secondLine = Line3D.FromPoints(new Point3D(2, 3, 1), new Point3D(3, 2, 4));
            Assert.AreEqual(new Point3D(2, 3, 1), secondLine.Point);
            Assert.AreEqual(new Vector3(1, -1, 3), secondLine.Direction);
        }

        [TestMethod]
        public void CanGetPoint()
        {
            var line = new Line3D(new Point3D(2, 3, 1), new Vector3(2, 1, 2));
            Assert.AreEqual(new Point3D(6, 5, 5), line.GetPoint(2));
        }

        [TestMethod]
        public void CanDetermineIfPointIsOnLine()
        {
            var line = new Line3D(new Point3D(2, 3, 1), new Vector3(2, 1, 2));
            Assert.IsTrue(line.IsPointOnLine(new Point3D(6, 5, 5)));
            Assert.IsFalse(line.IsPointOnLine(new Point3D(2, 3.5, 2)));
        }

    }
}