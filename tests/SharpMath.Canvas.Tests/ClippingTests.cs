namespace SharpMath.Canvas.Tests;

public class ClippingTests
{
    private static readonly CanvasRect Bounds = new(0, 0, 100, 100);

    [Fact]
    public void ClipSegment_ShortensSegmentsToTheRectangle()
    {
        var from = new CanvasPoint(-50, 50);
        var to = new CanvasPoint(150, 50);
        Assert.True(Clipping.ClipSegment(Bounds, ref from, ref to));
        Assert.Equal(new CanvasPoint(0, 50), from);
        Assert.Equal(new CanvasPoint(100, 50), to);

        var inside = new CanvasPoint(10, 10);
        var alsoInside = new CanvasPoint(20, 20);
        Assert.True(Clipping.ClipSegment(Bounds, ref inside, ref alsoInside));
        Assert.Equal(new CanvasPoint(10, 10), inside);

        var outside = new CanvasPoint(-10, -10);
        var stillOutside = new CanvasPoint(-5, 200);
        Assert.False(Clipping.ClipSegment(Bounds, ref outside, ref stillOutside));

        var huge = new CanvasPoint(50, -1e300);
        var end = new CanvasPoint(50, 1e300);
        Assert.True(Clipping.ClipSegment(Bounds, ref huge, ref end));
        Assert.Equal(new CanvasPoint(50, 0), huge);

        var nan = new CanvasPoint(double.NaN, 0);
        var other = new CanvasPoint(1, 1);
        Assert.False(Clipping.ClipSegment(Bounds, ref nan, ref other));

        var vertical = new CanvasPoint(150, 0);
        var verticalEnd = new CanvasPoint(150, 50);
        Assert.False(Clipping.ClipSegment(Bounds, ref vertical, ref verticalEnd));
    }

    [Fact]
    public void ClipPolygon_KeepsOnlyTheInsidePart()
    {
        var clipped = Clipping.ClipPolygon(Bounds,
            [new CanvasPoint(-50, -50), new CanvasPoint(50, -50), new CanvasPoint(50, 50), new CanvasPoint(-50, 50)]);
        Assert.Equal(4, clipped.Count);
        Assert.All(clipped, point => Assert.True(Bounds.Contains(point)));
        Assert.Contains(new CanvasPoint(50, 50), clipped);

        Assert.Empty(Clipping.ClipPolygon(Bounds,
            [new CanvasPoint(200, 200), new CanvasPoint(300, 200), new CanvasPoint(300, 300)]));
    }
}
