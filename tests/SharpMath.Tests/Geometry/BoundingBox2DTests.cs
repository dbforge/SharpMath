using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class BoundingBox2DTests
{
    [Fact]
    public void PropertiesAndOperations()
    {
        var box = new BoundingBox2D(1, 2, 5, 4);
        Assert.Equal(4, box.Width);
        Assert.Equal(2, box.Height);
        Assert.Equal(new Point2D(3, 3), box.Center);
        Assert.True(box.Contains(new Point2D(1, 2)));
        Assert.False(box.Contains(new Point2D(0, 3)));
        Assert.Equal(new BoundingBox2D(0, 0, 5, 4), box.Union(new BoundingBox2D(0, 0, 1, 1)));
        Assert.Equal(box, BoundingBox2D.FromPoints([new Point2D(5, 2), new Point2D(1, 4)]));
        Assert.Throws<ArgumentException>(() => BoundingBox2D.FromPoints([]));
        Assert.Throws<ArgumentException>(() => new BoundingBox2D(2, 0, 1, 0));
        Assert.Throws<ArgumentException>(() => new BoundingBox2D(0, 2, 1, 1));
    }

    [Fact]
    public void EqualityAndStrings()
    {
        var box = new BoundingBox2D(1, 2, 5, 4);
        Assert.True(box == new BoundingBox2D(1, 2, 5, 4));
        Assert.True(box != new BoundingBox2D(1, 2, 5, 5));
        Assert.True(box.Equals((object) new BoundingBox2D(1, 2, 5, 4)));
        Assert.Equal(box.GetHashCode(), new BoundingBox2D(1, 2, 5, 4).GetHashCode());
        Assert.Equal("BoundingBox2D [Min: (1, 2), Max: (5, 4)]", box.ToString());
    }
}
