using System.Runtime.InteropServices;

namespace SharpMath.Canvas;

/// <summary>
///     Samples a function across the visible area and splits the graph into polylines at gaps and poles. The buffers are
///     reused between calls, so sampling does not allocate once they have grown to size.
/// </summary>
internal sealed class FunctionSampler
{
    private const int MaxRefinementDepth = 6;
    private const double RefinementPixels = 2;

    private readonly List<CanvasPoint> _points = [];
    private readonly List<(int Start, int Length)> _polylines = [];
    private int _polylineStart;

    /// <summary>
    ///     Gets the number of polylines produced by the last call to <see cref="Sample" />.
    /// </summary>
    public int PolylineCount => _polylines.Count;

    /// <summary>
    ///     Gets a polyline produced by the last call to <see cref="Sample" />. The span is valid until the next call.
    /// </summary>
    public ReadOnlySpan<CanvasPoint> GetPolyline(int index)
    {
        var (start, length) = _polylines[index];
        return CollectionsMarshal.AsSpan(_points).Slice(start, length);
    }

    public void Sample(Func<double, double> function, Viewport viewport, CanvasRect guardBand)
    {
        _points.Clear();
        _polylines.Clear();
        _polylineStart = 0;
        var columns = (int) Math.Ceiling(viewport.Width);

        var previousX = viewport.ScreenToWorld(new CanvasPoint(0, 0)).X;
        var previousY = Evaluate(function, previousX);
        if (double.IsFinite(previousY))
            _points.Add(ToScreen(viewport, guardBand, previousX, previousY));

        for (var column = 1; column <= columns; column++)
        {
            var x = viewport.ScreenToWorld(new CanvasPoint(column, 0)).X;
            var y = Evaluate(function, x);
            if (!double.IsFinite(y))
            {
                Flush();
                previousX = x;
                previousY = y;
                continue;
            }

            if (double.IsFinite(previousY) &&
                !Refine(function, viewport, guardBand, previousX, previousY, x, y, MaxRefinementDepth))
                Flush();

            _points.Add(ToScreen(viewport, guardBand, x, y));
            previousX = x;
            previousY = y;
        }

        Flush();
    }

    /// <summary>
    ///     Adds intermediate samples where the graph is steep. Returns <see langword="false" /> if there is a gap or a pole
    ///     between the two samples; the points before the break have been added in that case.
    /// </summary>
    private bool Refine(Func<double, double> function, Viewport viewport, CanvasRect guardBand, double x0, double y0,
        double x1, double y1, int depth)
    {
        var pixelDistance = Math.Abs(y1 - y0) * viewport.ScaleY;
        if (pixelDistance <= RefinementPixels)
            return true;

        var xm = (x0 + x1) / 2;
        var ym = Evaluate(function, xm);
        if (!double.IsFinite(ym))
            return false;

        // A pole: the jump is larger than the visible area and the midpoint does not lie between the end points.
        var lower = Math.Min(y0, y1);
        var upper = Math.Max(y0, y1);
        var midpointBetween = ym >= lower && ym <= upper;
        if (pixelDistance > viewport.Height && !midpointBetween)
            return false;

        if (depth == 0)
            return true;

        if (!Refine(function, viewport, guardBand, x0, y0, xm, ym, depth - 1))
            return false;
        _points.Add(ToScreen(viewport, guardBand, xm, ym));
        return Refine(function, viewport, guardBand, xm, ym, x1, y1, depth - 1);
    }

    private static double Evaluate(Func<double, double> function, double x)
    {
        try
        {
            return function(x);
        }
        catch (ArithmeticException)
        {
            return double.NaN;
        }
    }

    private static CanvasPoint ToScreen(Viewport viewport, CanvasRect guardBand, double x, double y)
    {
        var point = viewport.WorldToScreen(new Geometry.Point2D(x, y));
        return new CanvasPoint(point.X, Math.Clamp(point.Y, guardBand.Y, guardBand.Bottom));
    }

    /// <summary>
    ///     Ends the current polyline; single points are dropped.
    /// </summary>
    private void Flush()
    {
        var length = _points.Count - _polylineStart;
        if (length >= 2)
            _polylines.Add((_polylineStart, length));
        else
            _points.RemoveRange(_polylineStart, length);
        _polylineStart = _points.Count;
    }
}
