using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class ProjectionDataTests
{
    [Fact]
    public void Default_HasDocumentedValues()
    {
        var data = ProjectionData.Default;
        Assert.Equal(100, data.Width);
        Assert.Equal(100, data.Height);
        Assert.Equal(16.0 / 9.0, data.AspectRatio);
        Assert.Equal(Math.PI / 3, data.FieldOfView);
        Assert.Equal(1, data.NearPlane);
        Assert.Equal(100, data.FarPlane);
        Assert.Equal(99, data.Depth);
    }

    [Theory]
    [InlineData(-1, 1, 1, 1, 1, 2)]
    [InlineData(1, double.NaN, 1, 1, 1, 2)]
    [InlineData(1, 1, 0, 1, 1, 2)]
    [InlineData(1, 1, double.PositiveInfinity, 1, 1, 2)]
    [InlineData(1, 1, 1, 0, 1, 2)]
    [InlineData(1, 1, 1, 4, 1, 2)]
    [InlineData(1, 1, 1, 1, -1, 2)]
    [InlineData(1, 1, 1, 1, 2, 1)]
    public void Constructor_ValidatesArguments(double width, double height, double aspect, double fov, double near,
        double far) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new ProjectionData(width, height, aspect, fov, near, far));

    [Fact]
    public void Equality()
    {
        Assert.True(ProjectionData.Default == ProjectionData.Default);
        Assert.True(ProjectionData.Default != new ProjectionData(1, 1, 1, 1, 1, 2));
        Assert.True(ProjectionData.Default.Equals((object) ProjectionData.Default));
        Assert.Equal(ProjectionData.Default.GetHashCode(), ProjectionData.Default.GetHashCode());
    }
}
