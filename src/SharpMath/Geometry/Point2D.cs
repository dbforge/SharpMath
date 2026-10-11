using System.Globalization;

namespace SharpMath.Geometry;

/// <summary>
///     Represents an immutable point in the plane.
/// </summary>
/// <remarks>
///     Points follow affine-space rules: <c>point − point</c> is a <see cref="Vector2" />, <c>point ± vector</c> is a
///     point. Equality is exact; use <see cref="ApproximatelyEquals(Point2D)" /> for tolerance-based comparisons.
/// </remarks>
public readonly struct Point2D : IEquatable<Point2D>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Point2D" /> struct.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    public Point2D(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    ///     Gets the X coordinate.
    /// </summary>
    public double X { get; init; }

    /// <summary>
    ///     Gets the Y coordinate.
    /// </summary>
    public double Y { get; init; }

    /// <summary>
    ///     Gets the origin (0, 0).
    /// </summary>
    public static Point2D Origin => default;

    /// <summary>
    ///     Gets the coordinate at the specified index (0 = X, 1 = Y).
    /// </summary>
    /// <param name="index">The zero-based index.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is not 0 or 1.</exception>
    public double this[int index] => index switch
    {
        0 => X,
        1 => Y,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "The index must be 0 or 1.")
    };

    /// <summary>
    ///     Returns the position vector of the point.
    /// </summary>
    /// <returns>The vector from the origin to the point.</returns>
    public Vector2 ToVector() => new(X, Y);

    /// <summary>
    ///     Calculates the distance to another point.
    /// </summary>
    /// <param name="other">The other point.</param>
    /// <returns>The distance.</returns>
    public double Distance(Point2D other) => (other - this).Length;

    /// <summary>
    ///     Calculates the distance between two points.
    /// </summary>
    /// <param name="left">The first point.</param>
    /// <param name="right">The second point.</param>
    /// <returns>The distance.</returns>
    public static double Distance(Point2D left, Point2D right) => left.Distance(right);

    /// <summary>
    ///     Calculates the squared distance to another point.
    /// </summary>
    /// <param name="other">The other point.</param>
    /// <returns>The squared distance.</returns>
    public double DistanceSquared(Point2D other) => (other - this).LengthSquared;

    /// <summary>
    ///     Calculates the squared distance between two points.
    /// </summary>
    /// <param name="left">The first point.</param>
    /// <param name="right">The second point.</param>
    /// <returns>The squared distance.</returns>
    public static double DistanceSquared(Point2D left, Point2D right) => left.DistanceSquared(right);

    /// <summary>
    ///     Determines whether this point equals another point within the default tolerances (coordinate-wise).
    /// </summary>
    /// <param name="other">The other point.</param>
    /// <returns><see langword="true" /> if the points are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Point2D other) =>
        ApproximatelyEquals(other, Tolerance.DefaultAbsolute, Tolerance.DefaultRelative);

    /// <summary>
    ///     Determines whether this point equals another point within the specified tolerances (coordinate-wise).
    /// </summary>
    /// <param name="other">The other point.</param>
    /// <param name="absolute">The absolute tolerance.</param>
    /// <param name="relative">The relative tolerance.</param>
    /// <returns><see langword="true" /> if the points are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Point2D other, double absolute, double relative) =>
        Tolerance.AreEqual(X, other.X, absolute, relative) && Tolerance.AreEqual(Y, other.Y, absolute, relative);

    /// <inheritdoc />
    public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"Point2D [X: {X}, Y: {Y}]");

    /// <inheritdoc />
    public bool Equals(Point2D other) => X.Equals(other.X) && Y.Equals(other.Y);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Point2D other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y);

    /// <summary>
    ///     Translates a point by a vector.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="vector">The translation.</param>
    /// <returns>The translated point.</returns>
    public static Point2D operator +(Point2D point, Vector2 vector) => new(point.X + vector.X, point.Y + vector.Y);

    /// <summary>
    ///     Translates a point by the negated vector.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="vector">The translation to subtract.</param>
    /// <returns>The translated point.</returns>
    public static Point2D operator -(Point2D point, Vector2 vector) => new(point.X - vector.X, point.Y - vector.Y);

    /// <summary>
    ///     Calculates the vector from one point to another.
    /// </summary>
    /// <param name="end">The point the vector points to.</param>
    /// <param name="start">The point the vector starts at.</param>
    /// <returns>The vector <c>end − start</c>.</returns>
    public static Vector2 operator -(Point2D end, Point2D start) => new(end.X - start.X, end.Y - start.Y);

    /// <summary>
    ///     Determines whether two points are exactly equal (coordinate-wise <c>==</c>).
    /// </summary>
    /// <param name="left">The first point.</param>
    /// <param name="right">The second point.</param>
    /// <returns><see langword="true" /> if all coordinates are equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(Point2D left, Point2D right) => left.X == right.X && left.Y == right.Y;

    /// <summary>
    ///     Determines whether two points differ in at least one coordinate.
    /// </summary>
    /// <param name="left">The first point.</param>
    /// <param name="right">The second point.</param>
    /// <returns><see langword="true" /> if any coordinate differs; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(Point2D left, Point2D right) => !(left == right);
}
