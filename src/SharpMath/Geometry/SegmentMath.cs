namespace SharpMath.Geometry;

/// <summary>
///     Shared helpers for segments and polygon edges.
/// </summary>
internal static class SegmentMath
{
    /// <summary>
    ///     The tolerance for distances between points with coordinates up to <paramref name="scale" />.
    /// </summary>
    public static double DistanceTolerance(double scale) =>
        Math.Max(Tolerance.DefaultAbsolute, Tolerance.DefaultRelative * scale);

    public static double CoordinateScale(Point2D point) => Math.Max(Math.Abs(point.X), Math.Abs(point.Y));

    /// <summary>
    ///     Determines whether a point lies within <paramref name="tolerance" /> of the segment from <paramref name="a" />
    ///     to <paramref name="b" />, without square roots on the common path.
    /// </summary>
    public static bool IsOnSegment(Point2D point, Point2D a, Point2D b, double tolerance)
    {
        var edge = b - a;
        var offset = point - a;
        var lengthSquared = edge.LengthSquared;
        var toleranceSquared = tolerance * tolerance;
        if (lengthSquared == 0)
            return offset.LengthSquared <= toleranceSquared;

        // The distance to the line is |cross| / |edge|; the projection must fall within the segment (± tolerance).
        var cross = edge.Cross(offset);
        if (cross * cross > toleranceSquared * lengthSquared)
            return false;
        var dot = edge.Dot(offset);
        var slack = tolerance * Math.Sqrt(lengthSquared);
        return dot >= -slack && dot <= lengthSquared + slack;
    }
}
