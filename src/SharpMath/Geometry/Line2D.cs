using System.Globalization;

namespace SharpMath.Geometry;

/// <summary>
///     Represents an immutable infinite line in the plane, stored in normalized general form <c>A·x + B·y + C = 0</c>.
/// </summary>
/// <remarks>
///     The coefficients are normalized so that <c>A² + B² = 1</c> and the first non-zero of <c>A</c> and <c>B</c> is
///     positive. Every line therefore has exactly one representation, so exact equality means "same line" (up to
///     rounding); use <see cref="ApproximatelyEquals(Line2D)" /> to compare lines built in different ways. Vertical
///     lines are supported. Create instances with the static factory methods; <see langword="default" /> is not a valid
///     line.
/// </remarks>
public readonly struct Line2D : IEquatable<Line2D>
{
    private Line2D(double a, double b, double c)
    {
        A = a;
        B = b;
        C = c;
    }

    /// <summary>
    ///     Gets the normalized coefficient of <c>x</c>.
    /// </summary>
    public double A { get; }

    /// <summary>
    ///     Gets the normalized coefficient of <c>y</c>.
    /// </summary>
    public double B { get; }

    /// <summary>
    ///     Gets the normalized constant term. <c>|C|</c> is the distance of the line to the origin.
    /// </summary>
    public double C { get; }

    /// <summary>
    ///     Gets the unit normal vector (A, B).
    /// </summary>
    public Vector2 Normal => new(A, B);

    /// <summary>
    ///     Gets the unit direction vector (−B, A).
    /// </summary>
    public Vector2 Direction => new(-B, A);

    /// <summary>
    ///     Gets a value indicating whether the line is parallel to the Y axis.
    /// </summary>
    public bool IsVertical => Tolerance.IsZero(B);

    /// <summary>
    ///     Gets a value indicating whether the line is parallel to the X axis.
    /// </summary>
    public bool IsHorizontal => Tolerance.IsZero(A);

    /// <summary>
    ///     Gets the slope <c>m</c> of <c>y = m·x + b</c>, or <see cref="double.NaN" /> if the line is vertical.
    /// </summary>
    public double Slope => IsVertical ? double.NaN : -A / B;

    /// <summary>
    ///     Gets the y-intercept <c>b</c> of <c>y = m·x + b</c>, or <see cref="double.NaN" /> if the line is vertical.
    /// </summary>
    public double YIntercept => IsVertical ? double.NaN : -C / B;

    /// <summary>
    ///     Creates the line <c>a·x + b·y + c = 0</c>.
    /// </summary>
    /// <param name="a">The coefficient of <c>x</c>.</param>
    /// <param name="b">The coefficient of <c>y</c>.</param>
    /// <param name="c">The constant term.</param>
    /// <returns>The line.</returns>
    /// <exception cref="ArgumentException"><paramref name="a" /> and <paramref name="b" /> are both zero, or a value is not finite.</exception>
    public static Line2D FromGeneralForm(double a, double b, double c)
    {
        if (!double.IsFinite(a) || !double.IsFinite(b) || !double.IsFinite(c))
            throw new ArgumentException("The coefficients must be finite.", double.IsFinite(a) ? double.IsFinite(b) ? nameof(c) : nameof(b) : nameof(a));
        var norm = Math.Sqrt(a * a + b * b);
        if (norm == 0)
            throw new ArgumentException("The coefficients a and b must not both be zero.", nameof(b));

        a /= norm;
        b /= norm;
        c /= norm;
        if (a < 0 || (a == 0 && b < 0))
        {
            a = -a;
            b = -b;
            c = -c;
        }

        // Avoid negative zeros so that equal lines also compare equal bit by bit.
        return new Line2D(a + 0.0, b + 0.0, c + 0.0);
    }

    /// <summary>
    ///     Creates the line <c>y = slope·x + yIntercept</c>.
    /// </summary>
    /// <param name="slope">The slope.</param>
    /// <param name="yIntercept">The y-intercept.</param>
    /// <returns>The line.</returns>
    /// <exception cref="ArgumentException">A value is not finite.</exception>
    public static Line2D FromSlopeIntercept(double slope, double yIntercept) => FromGeneralForm(slope, -1, yIntercept);

    /// <summary>
    ///     Creates the line through two points.
    /// </summary>
    /// <param name="first">The first point.</param>
    /// <param name="second">The second point.</param>
    /// <returns>The line.</returns>
    /// <exception cref="ArgumentException">The points are (approximately) equal.</exception>
    public static Line2D FromPoints(Point2D first, Point2D second)
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
    /// <exception cref="ArgumentException"><paramref name="direction" /> is the zero vector.</exception>
    public static Line2D FromPointAndDirection(Point2D point, Vector2 direction)
    {
        if (direction.LengthSquared == 0)
            throw new ArgumentException("The direction must not be the zero vector.", nameof(direction));
        return FromGeneralForm(direction.Y, -direction.X, direction.X * point.Y - direction.Y * point.X);
    }

    /// <summary>
    ///     Calculates <c>y</c> for the specified <c>x</c>.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <returns>The Y coordinate, or <see cref="double.NaN" /> if the line is vertical.</returns>
    public double SolveForY(double x) => IsVertical ? double.NaN : (-C - A * x) / B;

    /// <summary>
    ///     Calculates <c>x</c> for the specified <c>y</c>.
    /// </summary>
    /// <param name="y">The Y coordinate.</param>
    /// <returns>The X coordinate, or <see cref="double.NaN" /> if the line is horizontal.</returns>
    public double SolveForX(double y) => IsHorizontal ? double.NaN : (-C - B * y) / A;

    /// <summary>
    ///     Gets the point at the signed distance <paramref name="distance" /> along <see cref="Direction" />, measured from
    ///     the point of the line closest to the origin.
    /// </summary>
    /// <param name="distance">The signed distance along the line.</param>
    /// <returns>The point.</returns>
    public Point2D PointAt(double distance) => ClosestPointToOrigin + Direction * distance;

    /// <summary>
    ///     Calculates the signed distance of a point to the line; positive on the side the <see cref="Normal" /> points to.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The signed distance.</returns>
    public double SignedDistanceTo(Point2D point) => A * point.X + B * point.Y + C;

    /// <summary>
    ///     Calculates the distance of a point to the line.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The distance.</returns>
    public double DistanceTo(Point2D point) => Math.Abs(SignedDistanceTo(point));

    /// <summary>
    ///     Calculates the point on the line that is closest to the specified point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The orthogonal projection of <paramref name="point" /> onto the line.</returns>
    public Point2D ClosestPoint(Point2D point) => point - Normal * SignedDistanceTo(point);

    /// <summary>
    ///     Determines whether a point lies on the line within the default tolerances.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns><see langword="true" /> if the point lies on the line; otherwise, <see langword="false" />.</returns>
    public bool Contains(Point2D point) =>
        Tolerance.IsNegligible(SignedDistanceTo(point),
            Math.Max(Math.Abs(C), Math.Max(Math.Abs(point.X), Math.Abs(point.Y))));

    /// <summary>
    ///     Determines whether this line is parallel to (or identical with) another line.
    /// </summary>
    /// <param name="other">The other line.</param>
    /// <returns><see langword="true" /> if the lines are parallel; otherwise, <see langword="false" />.</returns>
    public bool IsParallelTo(Line2D other) => Tolerance.IsNegligible(ParallelDeterminant(other), 1);

    /// <summary>
    ///     Intersects this line with another line.
    /// </summary>
    /// <param name="other">The other line.</param>
    /// <returns>The kind of intersection and, for a single intersection point, the point.</returns>
    public LineIntersection2D Intersect(Line2D other)
    {
        var determinant = ParallelDeterminant(other);
        if (Tolerance.IsNegligible(determinant, 1))
        {
            return Contains(other.ClosestPointToOrigin)
                ? new LineIntersection2D(IntersectionKind.Coincident, null)
                : new LineIntersection2D(IntersectionKind.None, null);
        }

        var x = (B * other.C - other.B * C) / determinant;
        var y = (other.A * C - A * other.C) / determinant;
        return new LineIntersection2D(IntersectionKind.Point, new Point2D(x, y));
    }

    /// <summary>
    ///     Determines whether this line equals another line within the default tolerances (coefficient-wise).
    /// </summary>
    /// <param name="other">The other line.</param>
    /// <returns><see langword="true" /> if the lines are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Line2D other) =>
        ApproximatelyEquals(other, Tolerance.DefaultAbsolute, Tolerance.DefaultRelative);

    /// <summary>
    ///     Determines whether this line equals another line within the specified tolerances (coefficient-wise).
    /// </summary>
    /// <param name="other">The other line.</param>
    /// <param name="absolute">The absolute tolerance.</param>
    /// <param name="relative">The relative tolerance.</param>
    /// <returns><see langword="true" /> if the lines are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Line2D other, double absolute, double relative) =>
        Tolerance.AreEqual(A, other.A, absolute, relative) && Tolerance.AreEqual(B, other.B, absolute, relative) &&
        Tolerance.AreEqual(C, other.C, absolute, relative);

    /// <inheritdoc />
    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"Line2D [{A}·x + {B}·y + {C} = 0]");

    /// <inheritdoc />
    public bool Equals(Line2D other) => A.Equals(other.A) && B.Equals(other.B) && C.Equals(other.C);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Line2D other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(A, B, C);

    /// <summary>
    ///     Determines whether two lines have exactly the same normalized coefficients.
    /// </summary>
    /// <param name="left">The first line.</param>
    /// <param name="right">The second line.</param>
    /// <returns><see langword="true" /> if the lines are equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(Line2D left, Line2D right) =>
        left.A == right.A && left.B == right.B && left.C == right.C;

    /// <summary>
    ///     Determines whether two lines differ.
    /// </summary>
    /// <param name="left">The first line.</param>
    /// <param name="right">The second line.</param>
    /// <returns><see langword="true" /> if the lines differ; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(Line2D left, Line2D right) => !(left == right);

    private Point2D ClosestPointToOrigin => new(-C * A, -C * B);

    // Zero exactly when the (unit) normals are parallel.
    private double ParallelDeterminant(Line2D other) => A * other.B - B * other.A;
}
