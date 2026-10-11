using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace SharpMath.Geometry;

/// <summary>
///     Represents an immutable simple polygon in the plane, defined by its vertices in order.
/// </summary>
/// <remarks>
///     The polygon is closed implicitly (the last vertex connects to the first). Equality is exact and requires the same
///     vertices in the same order; <see cref="ApproximatelyEquals(Polygon)" /> compares with tolerances.
/// </remarks>
public sealed class Polygon : IEquatable<Polygon>
{
    private readonly Point2D[] _points;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Polygon" /> class.
    /// </summary>
    /// <param name="points">The vertices in order; at least three.</param>
    /// <exception cref="ArgumentException">Fewer than three vertices are specified.</exception>
    public Polygon(params ReadOnlySpan<Point2D> points)
    {
        if (points.Length < 3)
            throw new ArgumentException("A polygon needs at least three points.", nameof(points));
        _points = points.ToArray();
        Points = Array.AsReadOnly(_points);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Polygon" /> class.
    /// </summary>
    /// <param name="points">The vertices in order; at least three.</param>
    /// <exception cref="ArgumentException">Fewer than three vertices are specified.</exception>
    public Polygon(IEnumerable<Point2D> points) : this(
        (points ?? throw new ArgumentNullException(nameof(points))).ToArray().AsSpan())
    {
    }

    /// <summary>
    ///     Gets the vertices in order.
    /// </summary>
    public ReadOnlyCollection<Point2D> Points { get; }

    /// <summary>
    ///     Gets the signed area: positive if the vertices are ordered counterclockwise, negative if clockwise.
    /// </summary>
    public double SignedArea
    {
        get
        {
            var sum = 0.0;
            for (int i = 0, j = _points.Length - 1; i < _points.Length; j = i++)
                sum += _points[j].X * _points[i].Y - _points[i].X * _points[j].Y;
            return sum / 2;
        }
    }

    /// <summary>
    ///     Gets the area (shoelace formula).
    /// </summary>
    public double Area => Math.Abs(SignedArea);

    /// <summary>
    ///     Gets the perimeter.
    /// </summary>
    public double Perimeter
    {
        get
        {
            var sum = 0.0;
            for (int i = 0, j = _points.Length - 1; i < _points.Length; j = i++)
                sum += _points[j].Distance(_points[i]);
            return sum;
        }
    }

    /// <summary>
    ///     Gets the centroid (center of mass of the enclosed area). For a polygon without area (all vertices collinear),
    ///     the average of the vertices is returned.
    /// </summary>
    public Point2D Centroid
    {
        get
        {
            double x = 0, y = 0, twiceArea = 0;
            for (int i = 0, j = _points.Length - 1; i < _points.Length; j = i++)
            {
                var cross = _points[j].X * _points[i].Y - _points[i].X * _points[j].Y;
                twiceArea += cross;
                x += (_points[j].X + _points[i].X) * cross;
                y += (_points[j].Y + _points[i].Y) * cross;
            }

            if (twiceArea == 0)
            {
                foreach (var point in _points)
                {
                    x += point.X;
                    y += point.Y;
                }

                return new Point2D(x / _points.Length, y / _points.Length);
            }

            return new Point2D(x / (3 * twiceArea), y / (3 * twiceArea));
        }
    }

    /// <summary>
    ///     Gets the axis-aligned bounding box.
    /// </summary>
    public BoundingBox2D BoundingBox => BoundingBox2D.FromPoints(_points);

    /// <summary>
    ///     Determines whether a point lies inside the polygon. Points on the border count as inside.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns><see langword="true" /> if the polygon contains the point; otherwise, <see langword="false" />.</returns>
    public bool Contains(Point2D point)
    {
        var inside = false;
        for (int i = 0, j = _points.Length - 1; i < _points.Length; j = i++)
        {
            Point2D a = _points[j], b = _points[i];
            if (new LineSegment2D(a, b).Contains(point))
                return true;

            // Crossing number test with a horizontal ray to the right.
            if ((b.Y > point.Y) != (a.Y > point.Y) && point.X < (a.X - b.X) * (point.Y - b.Y) / (a.Y - b.Y) + b.X)
                inside = !inside;
        }

        return inside;
    }

    /// <summary>
    ///     Determines whether another polygon lies completely inside this polygon (touching the border is allowed).
    /// </summary>
    /// <param name="other">The other polygon.</param>
    /// <returns><see langword="true" /> if this polygon contains the other polygon; otherwise, <see langword="false" />.</returns>
    public bool Contains(Polygon other)
    {
        ArgumentNullException.ThrowIfNull(other);

        foreach (var point in other._points)
            if (!Contains(point))
                return false;

        // An edge may still leave and re-enter (e.g. through a concave notch or exactly through vertices). Split every
        // edge at its intersections with the border; each piece must then lie inside, so testing its midpoint suffices.
        var parameters = new List<double>();
        for (int i = 0, j = other._points.Length - 1; i < other._points.Length; j = i++)
        {
            var edge = new LineSegment2D(other._points[j], other._points[i]);
            CollectBorderIntersections(edge, parameters);
            for (var k = 1; k < parameters.Count; k++)
            {
                var middle = (parameters[k - 1] + parameters[k]) / 2;
                if (parameters[k] - parameters[k - 1] > 1e-12 && !Contains(edge.Start + edge.Direction * middle))
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Determines whether this polygon equals another polygon within the default tolerances (same vertices in the same
    ///     order).
    /// </summary>
    /// <param name="other">The other polygon.</param>
    /// <returns><see langword="true" /> if the polygons are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Polygon other) =>
        ApproximatelyEquals(other, Tolerance.DefaultAbsolute, Tolerance.DefaultRelative);

    /// <summary>
    ///     Determines whether this polygon equals another polygon within the specified tolerances (same vertices in the
    ///     same order).
    /// </summary>
    /// <param name="other">The other polygon.</param>
    /// <param name="absolute">The absolute tolerance.</param>
    /// <param name="relative">The relative tolerance.</param>
    /// <returns><see langword="true" /> if the polygons are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Polygon other, double absolute, double relative)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (_points.Length != other._points.Length)
            return false;
        for (var i = 0; i < _points.Length; i++)
            if (!_points[i].ApproximatelyEquals(other._points[i], absolute, relative))
                return false;
        return true;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var builder = new StringBuilder("Polygon [");
        for (var i = 0; i < _points.Length; i++)
        {
            if (i > 0)
                builder.Append(", ");
            builder.Append(CultureInfo.InvariantCulture, $"({_points[i].X}, {_points[i].Y})");
        }

        return builder.Append(']').ToString();
    }

    /// <inheritdoc />
    public bool Equals(Polygon? other) =>
        other is not null && (ReferenceEquals(this, other) || _points.AsSpan().SequenceEqual(other._points));

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as Polygon);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var point in _points)
            hash.Add(point);
        return hash.ToHashCode();
    }

    /// <summary>
    ///     Determines whether two polygons have the same vertices in the same order.
    /// </summary>
    /// <param name="left">The first polygon.</param>
    /// <param name="right">The second polygon.</param>
    /// <returns><see langword="true" /> if both are <see langword="null" /> or equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(Polygon? left, Polygon? right)
    {
        if (left is null || right is null)
            return left is null && right is null;
        if (left._points.Length != right._points.Length)
            return false;
        for (var i = 0; i < left._points.Length; i++)
            if (left._points[i] != right._points[i])
                return false;
        return true;
    }

    /// <summary>
    ///     Determines whether two polygons differ.
    /// </summary>
    /// <param name="left">The first polygon.</param>
    /// <param name="right">The second polygon.</param>
    /// <returns><see langword="true" /> if the polygons differ; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(Polygon? left, Polygon? right) => !(left == right);

    /// <summary>
    ///     Fills <paramref name="parameters" /> with 0, 1 and the sorted positions (as fractions of the edge) where the
    ///     edge meets the border of this polygon.
    /// </summary>
    private void CollectBorderIntersections(LineSegment2D edge, List<double> parameters)
    {
        parameters.Clear();
        parameters.Add(0);
        parameters.Add(1);
        var lengthSquared = edge.Direction.LengthSquared;
        if (lengthSquared == 0)
            return;

        for (int i = 0, j = _points.Length - 1; i < _points.Length; j = i++)
        {
            var intersection = edge.Intersect(new LineSegment2D(_points[j], _points[i]));
            switch (intersection.Kind)
            {
                case IntersectionKind.Point:
                    parameters.Add(Parameter(intersection.Point.GetValueOrDefault()));
                    break;
                case IntersectionKind.Overlap:
                    parameters.Add(Parameter(intersection.Segment.GetValueOrDefault().Start));
                    parameters.Add(Parameter(intersection.Segment.GetValueOrDefault().End));
                    break;
            }
        }

        parameters.Sort();
        return;

        double Parameter(Point2D point) => Math.Clamp((point - edge.Start).Dot(edge.Direction) / lengthSquared, 0, 1);
    }
}
