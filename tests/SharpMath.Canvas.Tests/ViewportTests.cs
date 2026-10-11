using SharpMath.Geometry;

namespace SharpMath.Canvas.Tests;

public class ViewportTests
{
    [Fact]
    public void Resize_CentersTheOriginUntilItIsSet()
    {
        var viewport = new Viewport();
        viewport.Resize(400, 200);
        Assert.Equal(new CanvasPoint(200, 100), viewport.Origin);
        viewport.Resize(600, 300);
        Assert.Equal(new CanvasPoint(300, 150), viewport.Origin);

        // Regression: an origin of (0, 0) used to be impossible because it was the "not set" sentinel.
        viewport.SetOrigin(new CanvasPoint(0, 0));
        viewport.Resize(800, 400);
        Assert.Equal(new CanvasPoint(0, 0), viewport.Origin);
    }

    [Fact]
    public void Conversions_FlipTheYAxis()
    {
        var viewport = new Viewport();
        viewport.Resize(400, 200);
        Assert.Equal(new CanvasPoint(220, 60), viewport.WorldToScreen(new Point2D(1, 2)));
        Assert.Equal(new Point2D(1, 2), viewport.ScreenToWorld(new CanvasPoint(220, 60)));
        Assert.Equal(new BoundingBox2D(-10, -5, 10, 5), viewport.VisibleArea);
    }

    [Fact]
    public void Pan_MovesTheOrigin()
    {
        var viewport = new Viewport();
        viewport.Resize(400, 200);
        viewport.Pan(10, -5);
        Assert.Equal(new CanvasPoint(210, 95), viewport.Origin);
        viewport.Pan(double.NaN, 1);
        Assert.Equal(new CanvasPoint(210, 95), viewport.Origin);
    }

    [Fact]
    public void ZoomAt_KeepsTheAnchorFixed()
    {
        var viewport = new Viewport();
        viewport.Resize(400, 200);
        var anchor = new CanvasPoint(300, 40);
        var before = viewport.ScreenToWorld(anchor);
        viewport.ZoomAt(anchor, 1.25);
        Assert.Equal(25, viewport.ScaleX, 12);
        Assert.True(viewport.ScreenToWorld(anchor).ApproximatelyEquals(before));
        viewport.ZoomAt(anchor, 0.8);
        Assert.True(viewport.ScreenToWorld(anchor).ApproximatelyEquals(before));
    }

    [Fact]
    public void ZoomAt_DoesNotDriftOrOverflow_Issue3()
    {
        // Regression for issue #3: zooming in repeatedly near a point far away used to move the origin by 1.25 times the
        // distance per step, so the world point under the cursor drifted and coordinates overflowed.
        var viewport = new Viewport();
        viewport.Resize(400, 300);
        viewport.Pan(0, 5_000);
        var anchor = new CanvasPoint(350, 10);
        var target = viewport.ScreenToWorld(anchor);
        for (var i = 0; i < 200; i++)
            viewport.ZoomAt(anchor, 1.25);

        Assert.Equal(Viewport.MaxScale, viewport.ScaleX);
        Assert.True(viewport.ScreenToWorld(anchor).ApproximatelyEquals(target, 1e-6, 1e-9));
        Assert.True(double.IsFinite(viewport.Origin.X) && double.IsFinite(viewport.Origin.Y));

        for (var i = 0; i < 400; i++)
            viewport.ZoomAt(anchor, 0.8);
        Assert.Equal(Viewport.MinScale, viewport.ScaleY);
    }

    [Fact]
    public void PanBeforeTheFirstResize_IsRelativeToTheCenteredOrigin()
    {
        // Regression: panning or zooming before the first render moved the origin to the top-left corner for good.
        var viewport = new Viewport();
        viewport.Pan(10, 5);
        viewport.ZoomAt(new CanvasPoint(1, 1), 2);
        viewport.Resize(400, 200);
        Assert.Equal(new CanvasPoint(210, 105), viewport.Origin);
        Assert.Equal(40, viewport.ScaleX);
        viewport.Resize(600, 300);
        Assert.Equal(new CanvasPoint(210, 105), viewport.Origin);

        var zoomedOnly = new Viewport();
        zoomedOnly.ZoomAt(new CanvasPoint(1, 1), 2);
        zoomedOnly.Resize(400, 200);
        zoomedOnly.Resize(600, 300);
        Assert.Equal(new CanvasPoint(300, 150), zoomedOnly.Origin);
    }

    [Fact]
    public void Validation()
    {
        var viewport = new Viewport();
        Assert.Throws<ArgumentOutOfRangeException>(() => viewport.ZoomAt(default, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => viewport.ZoomAt(default, double.PositiveInfinity));
        Assert.Throws<ArgumentOutOfRangeException>(() => viewport.SetScale(0, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => viewport.SetScale(1, double.NaN));
        Assert.Throws<ArgumentOutOfRangeException>(() => viewport.SetOrigin(new CanvasPoint(double.NaN, 0)));
        Assert.Throws<ArgumentOutOfRangeException>(() => viewport.Resize(-1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => viewport.Resize(1, double.PositiveInfinity));
        viewport.SetScale(2, 3);
        Assert.Equal(2, viewport.ScaleX);
        Assert.Equal(3, viewport.ScaleY);
    }

    [Fact]
    public void Changed_IsRaisedOnlyForRealChanges()
    {
        var viewport = new Viewport();
        var count = 0;
        viewport.Changed += (_, _) => count++;
        viewport.Resize(100, 100);
        viewport.Resize(100, 100);
        Assert.Equal(1, count);
        viewport.Pan(1, 0);
        Assert.Equal(2, count);
    }
}
