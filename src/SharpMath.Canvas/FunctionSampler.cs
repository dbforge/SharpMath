namespace SharpMath.Canvas;

/// <summary>
///     Samples a function across the visible area and splits the graph into polylines at gaps and poles.
/// </summary>
internal static class FunctionSampler
{
    private const int MaxRefinementDepth = 6;
    private const double RefinementPixels = 2;

    public static List<CanvasPoint[]> Sample(Func<double, double> function, Viewport viewport, CanvasRect guardBand)
    {
        var polylines = new List<CanvasPoint[]>();
        var current = new List<CanvasPoint>();
        var columns = (int) Math.Ceiling(viewport.Width);

        var previousX = viewport.ScreenToWorld(new CanvasPoint(0, 0)).X;
        var previousY = Evaluate(function, previousX);
        if (double.IsFinite(previousY))
            current.Add(ToScreen(viewport, guardBand, previousX, previousY));

        for (var column = 1; column <= columns; column++)
        {
            var x = viewport.ScreenToWorld(new CanvasPoint(column, 0)).X;
            var y = Evaluate(function, x);
            if (!double.IsFinite(y))
            {
                Flush(polylines, current);
                previousX = x;
                previousY = y;
                continue;
            }

            if (double.IsFinite(previousY) && !Refine(function, viewport, guardBand, previousX, previousY, x, y,
                    MaxRefinementDepth, current))
            {
                Flush(polylines, current);
            }

            current.Add(ToScreen(viewport, guardBand, x, y));
            previousX = x;
            previousY = y;
        }

        Flush(polylines, current);
        return polylines;
    }

    /// <summary>
    ///     Adds intermediate samples where the graph is steep. Returns <see langword="false" /> if there is a gap or a pole
    ///     between the two samples; the points before the break have been added in that case.
    /// </summary>
    private static bool Refine(Func<double, double> function, Viewport viewport, CanvasRect guardBand, double x0,
        double y0, double x1, double y1, int depth, List<CanvasPoint> output)
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
        if (pixelDistance > viewport.Height && (ym < lower || ym > upper))
            return false;

        if (depth == 0)
            return pixelDistance <= viewport.Height || (ym >= lower && ym <= upper);

        if (!Refine(function, viewport, guardBand, x0, y0, xm, ym, depth - 1, output))
            return false;
        output.Add(ToScreen(viewport, guardBand, xm, ym));
        return Refine(function, viewport, guardBand, xm, ym, x1, y1, depth - 1, output);
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

    private static void Flush(List<CanvasPoint[]> polylines, List<CanvasPoint> current)
    {
        if (current.Count >= 2)
            polylines.Add([.. current]);
        current.Clear();
    }
}
