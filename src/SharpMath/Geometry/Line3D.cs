using System.Globalization;

namespace SharpMath.Geometry;

/// <summary>
///     Represents an immutable infinite line in three-dimensional space.
/// </summary>
/// <remarks>
///     The line is stored canonically: <see cref="Point" /> is the point of the line closest to the origin and
///     <see cref="Direction" /> is a unit vector whose first non-zero component is positive. Every line therefore has
///     exactly one representation, so exact equality means "same line" (up to rounding); use
///     <see cref="ApproximatelyEquals(Line3D)" /> to compare lines built in different ways. Create instances with the
///     static factory methods; <see langword="default" /> is not a valid line.
/// </remarks>
public readonly struct Line3D : IEquatable<Line3D>
{
    private Line3D(Point3D point, Vector3 direction)
    {
        Point = point;
        Direction = direction;
    }

    /// <summary>
    ///     Gets the point of the line that is closest to the origin.
    /// </summary>
    public Point3D Point { get; }

    /// <summary>
    ///     Gets the unit direction of the line.
    /// </summary>
    public Vector3 Direction { get; }

    /// <summary>
    ///     Creates the line through two points.
    /// </summary>
    /// <param name="first">The first point.</param>
    /// <param name="second">The second point.</param>
    /// <returns>The line.</returns>
    /// <exception cref="ArgumentException">The points are (approximately) equal.</exception>
    public static Line3D FromPoints(Point3D first, Point3D second)
    {
        if (first.ApproximatelyEquals(second))
            throw new ArgumentException("The points must be distinct to define a line.", nameof(second));
        return FromPointAndDirection(first, second - first);
    }

    /// <summary>
    ///     Creates the line through a point with the specified direction.
    /// </summary>
    /// <param name="point">A point on the line.</param>
    /// <param name="direction">The direction; must not be the zero vector.</param>
    /// <returns>The line.</returns>
    /// <exception cref="ArgumentException"><paramref name="direction" /> is the zero vector or not finite.</exception>
    public static Line3D FromPointAndDirection(Point3D point, Vector3 direction)
    {
        if (direction.LengthSquared == 0 || !double.IsFinite(direction.LengthSquared))
            throw new ArgumentException("The direction must be a finite, non-zero vector.", nameof(direction));

        var unit = direction.Normalize();
        if (unit.X < 0 || (unit.X == 0 && (unit.Y < 0 || (unit.Y == 0 && unit.Z < 0))))
            unit = -unit;
        unit = new Vector3(unit.X + 0.0, unit.Y + 0.0, unit.Z + 0.0);

        var position = point.ToVector();
        var closest = position - unit * position.Dot(unit);
        return new Line3D(new Point3D(closest.X + 0.0, closest.Y + 0.0, closest.Z + 0.0), unit);
    }

    /// <summary>
    ///     Gets the point at the signed distance <paramref name="distance" /> along <see cref="Direction" />, measured from
    ///     <see cref="Point" />.
    /// </summary>
    /// <param name="distance">The signed distance along the line.</param>
    /// <returns>The point.</returns>
    public Point3D PointAt(double distance) => Point + Direction * distance;

    /// <summary>
    ///     Calculates the point on the line that is closest to the specified point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The orthogonal projection of <paramref name="point" /> onto the line.</returns>
    public Point3D ClosestPoint(Point3D point) => Point + Direction * (point - Point).Dot(Direction);

    /// <summary>
    ///     Calculates the distance of a point to the line.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The distance.</returns>
    public double DistanceTo(Point3D point) => (point - Point).Cross(Direction).Length;

    /// <summary>
    ///     Determines whether a point lies on the line within the default tolerances.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns><see langword="true" /> if the point lies on the line; otherwise, <see langword="false" />.</returns>
    public bool Contains(Point3D point) =>
        Tolerance.IsNegligible(DistanceTo(point), Math.Max(point.ToVector().Length, Point.ToVector().Length));

    /// <summary>
    ///     Determines whether this line is parallel to (or identical with) another line.
    /// </summary>
    /// <param name="other">The other line.</param>
    /// <returns><see langword="true" /> if the lines are parallel; otherwise, <see langword="false" />.</returns>
    public bool IsParallelTo(Line3D other) => Tolerance.IsNegligible(Direction.Cross(other.Direction).Length, 1);

    /// <summary>
    ///     Calculates the shortest distance between this line and another line.
    /// </summary>
    /// <param name="other">The other line.</param>
    /// <returns>The distance; 0 if the lines intersect.</returns>
    public double DistanceTo(Line3D other)
    {
        var (onThis, onOther) = ClosestPoints(other);
        return onThis.Distance(onOther);
    }

    /// <summary>
    ///     Calculates the pair of points, one on each line, that are closest to each other.
    /// </summary>
    /// <param name="other">The other line.</param>
    /// <returns>
    ///     The closest points. For parallel lines, <see cref="Point" /> of this line and its projection onto the other line
    ///     are returned.
    /// </returns>
    public (Point3D OnThis, Point3D OnOther) ClosestPoints(Line3D other)
    {
        if (IsParallelTo(other))
            return (Point, other.ClosestPoint(Point));

        // The connection of the closest points is perpendicular to both directions, i.e. parallel to n = d × e.
        // |n|² is computed directly instead of as 1 − (d·e)², which cancels for nearly parallel lines.
        var d = Direction;
        var e = other.Direction;
        var n = d.Cross(e);
        var denominator = n.LengthSquared;
        var w = other.Point - Point;
        var s = w.Cross(e).Dot(n) / denominator;
        var t = w.Cross(d).Dot(n) / denominator;
        return (Point + d * s, other.Point + e * t);
    }

    /// <summary>
    ///     Intersects this line with another line.
    /// </summary>
    /// <param name="other">The other line.</param>
    /// <returns>The kind of intersection and, for a single intersection point, the point.</returns>
    public LineIntersection3D Intersect(Line3D other)
    {
        if (IsParallelTo(other))
            return Contains(other.Point)
                ? new LineIntersection3D(IntersectionKind.Coincident, null)
                : new LineIntersection3D(IntersectionKind.None, null);

        var (onThis, onOther) = ClosestPoints(other);
        var scale = Math.Max(onThis.ToVector().Length, onOther.ToVector().Length);
        if (!Tolerance.IsNegligible(onThis.Distance(onOther), scale))
            return new LineIntersection3D(IntersectionKind.Skew, null);

        var midpoint = new Point3D((onThis.X + onOther.X) / 2, (onThis.Y + onOther.Y) / 2, (onThis.Z + onOther.Z) / 2);
        return new LineIntersection3D(IntersectionKind.Point, midpoint);
    }

    /// <summary>
    ///     Determines whether this line equals another line within the default tolerances.
    /// </summary>
    /// <param name="other">The other line.</param>
    /// <returns><see langword="true" /> if the lines are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Line3D other) =>
        ApproximatelyEquals(other, Tolerance.DefaultAbsolute, Tolerance.DefaultRelative);

    /// <summary>
    ///     Determines whether this line equals another line within the specified tolerances.
    /// </summary>
    /// <param name="other">The other line.</param>
    /// <param name="absolute">The absolute tolerance.</param>
    /// <param name="relative">The relative tolerance.</param>
    /// <returns><see langword="true" /> if the lines are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Line3D other, double absolute, double relative) =>
        Point.ApproximatelyEquals(other.Point, absolute, relative) &&
        Direction.ApproximatelyEquals(other.Direction, absolute, relative);

    /// <inheritdoc />
    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture,
            $"Line3D [({Point.X}, {Point.Y}, {Point.Z}) + t·({Direction.X}, {Direction.Y}, {Direction.Z})]");

    /// <inheritdoc />
    public bool Equals(Line3D other) => Point.Equals(other.Point) && Direction.Equals(other.Direction);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Line3D other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Point, Direction);

    /// <summary>
    ///     Determines whether two lines have exactly the same canonical representation.
    /// </summary>
    /// <param name="left">The first line.</param>
    /// <param name="right">The second line.</param>
    /// <returns><see langword="true" /> if the lines are equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(Line3D left, Line3D right) =>
        left.Point == right.Point && left.Direction == right.Direction;

    /// <summary>
    ///     Determines whether two lines differ.
    /// </summary>
    /// <param name="left">The first line.</param>
    /// <param name="right">The second line.</param>
    /// <returns><see langword="true" /> if the lines differ; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(Line3D left, Line3D right) => !(left == right);
}
