namespace SharpMath.Canvas.Tests;

public class FunctionSamplerTests
{
    private static Viewport CreateViewport()
    {
        var viewport = new Viewport();
        viewport.Resize(400, 300);
        return viewport;
    }

    private static CanvasRect GuardBand(Viewport viewport) =>
        new(-10 * viewport.Width, -10 * viewport.Height, 21 * viewport.Width, 21 * viewport.Height);

    private static List<CanvasPoint[]> Sample(Func<double, double> function, Viewport viewport)
    {
        var sampler = new FunctionSampler();
        sampler.Sample(function, viewport, GuardBand(viewport));
        return Enumerable.Range(0, sampler.PolylineCount).Select(i => sampler.GetPolyline(i).ToArray()).ToList();
    }

    [Fact]
    public void ContinuousFunction_IsOnePolyline()
    {
        var viewport = CreateViewport();
        var polylines = Sample(x => x * x * x, viewport);
        Assert.Single(polylines);
        Assert.True(polylines[0].Length >= 401);
        Assert.All(polylines[0], point => Assert.InRange(point.Y, -3000, 3300));
    }

    [Fact]
    public void Poles_BreakTheGraph()
    {
        // Regression: the graph used to be connected across asymptotes.
        var viewport = CreateViewport();
        var polylines = Sample(Math.Tan, viewport);

        // tan has poles at ±π/2, ±3π/2, ±5π/2 within x ∈ [−10, 10].
        Assert.Equal(7, polylines.Count);
        foreach (var polyline in polylines)
            for (var i = 1; i < polyline.Length; i++)
                Assert.True(polyline[i].X >= polyline[i - 1].X);
    }

    [Fact]
    public void NonFiniteValuesAndArithmeticExceptions_LeaveGaps()
    {
        var viewport = CreateViewport();
        var sqrt = Sample(Math.Sqrt, viewport);
        Assert.Single(sqrt);
        Assert.All(sqrt[0], point => Assert.True(point.X >= 199));

        var throwing = Sample(x => x < 0 ? throw new OverflowException() : 1, viewport);
        Assert.Single(throwing);

        Assert.Empty(Sample(_ => double.NaN, viewport));
        var reciprocal = Sample(x => 1 / x, viewport);
        Assert.Equal(2, reciprocal.Count);
    }

    [Fact]
    public void Buffers_AreReusedBetweenCalls()
    {
        var viewport = CreateViewport();
        var sampler = new FunctionSampler();
        sampler.Sample(Math.Tan, viewport, GuardBand(viewport));
        Assert.Equal(7, sampler.PolylineCount);
        sampler.Sample(x => x, viewport, GuardBand(viewport));
        Assert.Equal(1, sampler.PolylineCount);
        Assert.Equal(401, sampler.GetPolyline(0).Length);

        var before = GC.GetAllocatedBytesForCurrentThread();
        sampler.Sample(Math.Tan, viewport, GuardBand(viewport));
        sampler.Sample(x => x, viewport, GuardBand(viewport));
        Assert.True(GC.GetAllocatedBytesForCurrentThread() - before < 1024);
    }

    [Fact]
    public void SteepParts_AreRefined()
    {
        var viewport = CreateViewport();
        var polylines = Sample(x => Math.Exp(3 * x), viewport);
        Assert.True(polylines.Sum(polyline => polyline.Length) > 401);
    }
}
