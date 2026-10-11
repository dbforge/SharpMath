namespace SharpMath.Canvas;

/// <summary>
///     Clips geometry in screen coordinates to a rectangle, so renderers never receive huge coordinates.
/// </summary>
internal static class Clipping
{
    /// <summary>
    ///     Clips a segment with the Liang–Barsky algorithm.
    /// </summary>
    /// <returns><see langword="true" /> if a part of the segment lies inside the rectangle.</returns>
    public static bool ClipSegment(CanvasRect bounds, ref CanvasPoint from, ref CanvasPoint to)
    {
        if (!IsFinite(from) || !IsFinite(to))
            return false;

        double t0 = 0, t1 = 1;
        var dx = to.X - from.X;
        var dy = to.Y - from.Y;
        if (!Clip(-dx, from.X - bounds.X, ref t0, ref t1) || !Clip(dx, bounds.Right - from.X, ref t0, ref t1) ||
            !Clip(-dy, from.Y - bounds.Y, ref t0, ref t1) || !Clip(dy, bounds.Bottom - from.Y, ref t0, ref t1))
            return false;

        var start = from;
        if (t1 < 1)
            to = new CanvasPoint(start.X + t1 * dx, start.Y + t1 * dy);
        if (t0 > 0)
            from = new CanvasPoint(start.X + t0 * dx, start.Y + t0 * dy);
        return true;
    }

    /// <summary>
    ///     Clips a polygon with the Sutherland–Hodgman algorithm.
    /// </summary>
    public static List<CanvasPoint> ClipPolygon(CanvasRect bounds, IReadOnlyList<CanvasPoint> polygon)
    {
        var output = polygon.Where(IsFinite).ToList();
        output = ClipEdge(output, p => p.X >= bounds.X, (a, b) => AtX(a, b, bounds.X));
        output = ClipEdge(output, p => p.X <= bounds.Right, (a, b) => AtX(a, b, bounds.Right));
        output = ClipEdge(output, p => p.Y >= bounds.Y, (a, b) => AtY(a, b, bounds.Y));
        output = ClipEdge(output, p => p.Y <= bounds.Bottom, (a, b) => AtY(a, b, bounds.Bottom));
        return output;
    }

    public static bool IsFinite(CanvasPoint point) => double.IsFinite(point.X) && double.IsFinite(point.Y);

    private static bool Clip(double denominator, double numerator, ref double t0, ref double t1)
    {
        if (denominator == 0)
            return numerator >= 0;

        var t = numerator / denominator;
        if (denominator < 0)
        {
            if (t > t1)
                return false;
            t0 = Math.Max(t0, t);
        }
        else
        {
            if (t < t0)
                return false;
            t1 = Math.Min(t1, t);
        }

        return true;
    }

    private static List<CanvasPoint> ClipEdge(List<CanvasPoint> input, Func<CanvasPoint, bool> inside,
        Func<CanvasPoint, CanvasPoint, CanvasPoint> intersect)
    {
        var output = new List<CanvasPoint>(input.Count + 4);
        for (var i = 0; i < input.Count; i++)
        {
            var current = input[i];
            var previous = input[(i + input.Count - 1) % input.Count];
            if (inside(current))
            {
                if (!inside(previous))
                    output.Add(intersect(previous, current));
                output.Add(current);
            }
            else if (inside(previous))
            {
                output.Add(intersect(previous, current));
            }
        }

        return output;
    }

    private static CanvasPoint AtX(CanvasPoint a, CanvasPoint b, double x) =>
        new(x, a.Y + (b.Y - a.Y) * (x - a.X) / (b.X - a.X));

    private static CanvasPoint AtY(CanvasPoint a, CanvasPoint b, double y) =>
        new(a.X + (b.X - a.X) * (y - a.Y) / (b.Y - a.Y), y);
}
