using System.Globalization;

namespace SharpMath.Geometry;

/// <summary>
///     Represents an immutable line segment between two points in the plane.
/// </summary>
/// <remarks>
///     A segment whose end points coincide is allowed and behaves like a point. Equality is exact and depends on the
///     order of the end points.
/// </remarks>
public readonly struct LineSegment2D : IEquatable<LineSegment2D>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LineSegment2D" /> struct.
    /// </summary>
    /// <param name="start">The start point.</param>
    /// <param name="end">The end point.</param>
    public LineSegment2D(Point2D start, Point2D end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    ///     Gets the start point.
    /// </summary>
    public Point2D Start { get; init; }

    /// <summary>
    ///     Gets the end point.
    /// </summary>
    public Point2D End { get; init; }

    /// <summary>
    ///     Gets the vector from <see cref="Start" /> to <see cref="End" />.
    /// </summary>
    public Vector2 Direction => End - Start;

    /// <summary>
    ///     Gets the length.
    /// </summary>
    public double Length => Direction.Length;

    /// <summary>
    ///     Gets the midpoint.
    /// </summary>
    public Point2D Midpoint => new((Start.X + End.X) / 2, (Start.Y + End.Y) / 2);

    /// <summary>
    ///     Returns the infinite line through the segment.
    /// </summary>
    /// <returns>The line.</returns>
    /// <exception cref="InvalidOperationException">The end points coincide.</exception>
    public Line2D ToLine() =>
        IsDegenerate
            ? throw new InvalidOperationException("A segment of length zero does not define a line.")
            : Line2D.FromPointAndDirection(Start, Direction);

    /// <summary>
    ///     Calculates the point on the segment that is closest to the specified point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The closest point.</returns>
    public Point2D ClosestPoint(Point2D point) => Start + Direction * ClosestParameter(point);

    /// <summary>
    ///     Calculates the distance of a point to the segment.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The distance.</returns>
    public double DistanceTo(Point2D point) => point.Distance(ClosestPoint(point));

    /// <summary>
    ///     Determines whether a point lies on the segment within the default tolerances.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns><see langword="true" /> if the point lies on the segment; otherwise, <see langword="false" />.</returns>
    public bool Contains(Point2D point) =>
        SegmentMath.IsOnSegment(point, Start, End, SegmentMath.DistanceTolerance(Scale(point)));

    /// <summary>
    ///     Intersects this segment with another segment.
    /// </summary>
    /// <param name="other">The other segment.</param>
    /// <returns>The kind of intersection and the intersection point or overlapping segment.</returns>
    public SegmentIntersection2D Intersect(LineSegment2D other)
    {
        if (IsDegenerate)
            return other.Contains(Start) ? PointResult(Start) : NoIntersection;
        if (other.IsDegenerate)
            return Contains(other.Start) ? PointResult(other.Start) : NoIntersection;

        var r = Direction;
        var s = other.Direction;
        var offset = other.Start - Start;
        var denominator = r.Cross(s);

        // Parallelism is judged by the angle, distances by the coordinate scale, so tiny segments behave like big ones.
        if (r.IsParallelTo(s))
        {
            var distanceToLine = Math.Abs(offset.Cross(r)) / r.Length;
            if (distanceToLine > SegmentMath.DistanceTolerance(Scale(other.Start)))
                return NoIntersection;

            var lengthSquared = r.LengthSquared;
            var t0 = offset.Dot(r) / lengthSquared;
            var t1 = t0 + s.Dot(r) / lengthSquared;
            var from = Math.Max(0, Math.Min(t0, t1));
            var to = Math.Min(1, Math.Max(t0, t1));
            var tolerance = Tolerance.DefaultRelative;
            if (from > to + tolerance)
                return NoIntersection;
            if (to - from <= tolerance)
                return PointResult(Start + r * ((from + to) / 2));
            return new SegmentIntersection2D(IntersectionKind.Overlap, null,
                new LineSegment2D(Start + r * from, Start + r * to));
        }

        var t = offset.Cross(s) / denominator;
        var u = offset.Cross(r) / denominator;
        var epsilon = Tolerance.DefaultRelative;
        if (t < -epsilon || t > 1 + epsilon || u < -epsilon || u > 1 + epsilon)
            return NoIntersection;
        return PointResult(Start + r * Math.Clamp(t, 0, 1));
    }

    /// <summary>
    ///     Intersects this segment with an infinite line.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <returns>
    ///     The kind of intersection: <see cref="IntersectionKind.Overlap" /> (with this segment) if the segment lies on the
    ///     line, a point, or none.
    /// </returns>
    public SegmentIntersection2D Intersect(Line2D line)
    {
        if (IsDegenerate)
            return line.Contains(Start) ? PointResult(Start) : NoIntersection;

        var intersection = ToLine().Intersect(line);
        return intersection.Kind switch
        {
            IntersectionKind.Coincident => new SegmentIntersection2D(IntersectionKind.Overlap, null, this),
            IntersectionKind.Point when Contains(intersection.Point.GetValueOrDefault()) =>
                PointResult(ClosestPoint(intersection.Point.GetValueOrDefault())),
            _ => NoIntersection
        };
    }

    /// <summary>
    ///     Determines whether this segment equals another segment within the default tolerances (same order of end points).
    /// </summary>
    /// <param name="other">The other segment.</param>
    /// <returns><see langword="true" /> if the segments are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(LineSegment2D other) =>
        ApproximatelyEquals(other, Tolerance.DefaultAbsolute, Tolerance.DefaultRelative);

    /// <summary>
    ///     Determines whether this segment equals another segment within the specified tolerances (same order of end
    ///     points).
    /// </summary>
    /// <param name="other">The other segment.</param>
    /// <param name="absolute">The absolute tolerance.</param>
    /// <param name="relative">The relative tolerance.</param>
    /// <returns><see langword="true" /> if the segments are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(LineSegment2D other, double absolute, double relative) =>
        Start.ApproximatelyEquals(other.Start, absolute, relative) && End.ApproximatelyEquals(other.End, absolute, relative);

    /// <inheritdoc />
    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"LineSegment2D [({Start.X}, {Start.Y}) → ({End.X}, {End.Y})]");

    /// <inheritdoc />
    public bool Equals(LineSegment2D other) => Start.Equals(other.Start) && End.Equals(other.End);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is LineSegment2D other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Start, End);

    /// <summary>
    ///     Determines whether two segments are equal.
    /// </summary>
    /// <param name="left">The first segment.</param>
    /// <param name="right">The second segment.</param>
    /// <returns><see langword="true" /> if the segments are equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(LineSegment2D left, LineSegment2D right) =>
        left.Start == right.Start && left.End == right.End;

    /// <summary>
    ///     Determines whether two segments differ.
    /// </summary>
    /// <param name="left">The first segment.</param>
    /// <param name="right">The second segment.</param>
    /// <returns><see langword="true" /> if the segments differ; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(LineSegment2D left, LineSegment2D right) => !(left == right);

    private static SegmentIntersection2D NoIntersection => new(IntersectionKind.None, null, null);

    private bool IsDegenerate => Start == End;

    private static SegmentIntersection2D PointResult(Point2D point) => new(IntersectionKind.Point, point, null);

    private double ClosestParameter(Point2D point)
    {
        var direction = Direction;
        var lengthSquared = direction.LengthSquared;
        return lengthSquared == 0 ? 0 : Math.Clamp((point - Start).Dot(direction) / lengthSquared, 0, 1);
    }

    private double Scale(Point2D point) =>
        Math.Max(SegmentMath.CoordinateScale(point),
            Math.Max(SegmentMath.CoordinateScale(Start), SegmentMath.CoordinateScale(End)));
}
