using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class PointTests
{
    [Fact]
    public void Point2D_FollowsAffineRules()
    {
        var p = new Point2D(1, 2);
        var q = new Point2D(4, 6);
        Assert.Equal(new Vector2(3, 4), q - p);
        Assert.Equal(q, p + new Vector2(3, 4));
        Assert.Equal(p, q - new Vector2(3, 4));
        Assert.Equal(5, p.Distance(q));
        Assert.Equal(5, Point2D.Distance(p, q));
        Assert.Equal(25, p.DistanceSquared(q));
        Assert.Equal(25, Point2D.DistanceSquared(p, q));
        Assert.Equal(new Vector2(1, 2), p.ToVector());
        Assert.Equal(Point2D.Origin, new Point2D(0, 0));
        Assert.Equal(1, p[0]);
        Assert.Equal(2, p[1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => p[2]);
    }

    [Fact]
    public void Point2D_EqualityAndStrings()
    {
        var p = new Point2D(1, 2);
        Assert.True(p == new Point2D(1, 2));
        Assert.True(p != new Point2D(1, 3));

        // Like System.Numerics: == follows IEEE (NaN differs from itself), Equals treats NaN as equal.
        var nan = new Point2D(double.NaN, 0);
        var otherNan = new Point2D(double.NaN, 0);
        Assert.False(nan == otherNan);
        Assert.True(nan.Equals(otherNan));
        Assert.True(p.Equals((object) new Point2D(1, 2)));
        Assert.False(p.Equals((object) new Vector2(1, 2)));
        Assert.Equal(p.GetHashCode(), new Point2D(1, 2).GetHashCode());
        Assert.True(new Point2D(0.1 + 0.2, 0).ApproximatelyEquals(new Point2D(0.3, 0)));
        Assert.True(p.ApproximatelyEquals(new Point2D(1.05, 2), 0.1, 0));
        Assert.Equal("Point2D [X: 1, Y: 2]", p.ToString());
    }

    [Fact]
    public void Point3D_FollowsAffineRules()
    {
        var p = new Point3D(1, 2, 3);
        var q = new Point3D(2, 4, 5);
        Assert.Equal(new Vector3(1, 2, 2), q - p);
        Assert.Equal(q, p + new Vector3(1, 2, 2));
        Assert.Equal(p, q - new Vector3(1, 2, 2));
        Assert.Equal(3, p.Distance(q));
        Assert.Equal(3, Point3D.Distance(p, q));
        Assert.Equal(9, p.DistanceSquared(q));
        Assert.Equal(9, Point3D.DistanceSquared(p, q));
        Assert.Equal(new Vector3(1, 2, 3), p.ToVector());
        Assert.Equal(Point3D.Origin, new Point3D(0, 0, 0));
        Assert.Equal(3, p[2]);
        Assert.Throws<ArgumentOutOfRangeException>(() => p[3]);
        Assert.True(p == new Point3D(1, 2, 3));
        Assert.True(p != new Point3D(1, 2, 4));
        Assert.True(p.Equals((object) new Point3D(1, 2, 3)));
        Assert.Equal(p.GetHashCode(), new Point3D(1, 2, 3).GetHashCode());
        Assert.True(p.ApproximatelyEquals(new Point3D(1, 2, 3 + 1e-12)));
        Assert.Equal("Point3D [X: 1, Y: 2, Z: 3]", p.ToString());
    }
}
