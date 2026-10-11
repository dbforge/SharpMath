using System.Globalization;

namespace SharpMath.Geometry;

/// <summary>
///     Represents an immutable point in three-dimensional space.
/// </summary>
/// <remarks>
///     Points follow affine-space rules: <c>point − point</c> is a <see cref="Vector3" />, <c>point ± vector</c> is a
///     point. Equality is exact; use <see cref="ApproximatelyEquals(Point3D)" /> for tolerance-based comparisons.
/// </remarks>
public readonly struct Point3D : IEquatable<Point3D>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Point3D" /> struct.
    /// </summary>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="z">The Z coordinate.</param>
    public Point3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
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
    ///     Gets the Z coordinate.
    /// </summary>
    public double Z { get; init; }

    /// <summary>
    ///     Gets the origin (0, 0, 0).
    /// </summary>
    public static Point3D Origin => default;

    /// <summary>
    ///     Gets the coordinate at the specified index (0 = X, 1 = Y, 2 = Z).
    /// </summary>
    /// <param name="index">The zero-based index.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is not between 0 and 2.</exception>
    public double this[int index] => index switch
    {
        0 => X,
        1 => Y,
        2 => Z,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "The index must be between 0 and 2.")
    };

    /// <summary>
    ///     Returns the position vector of the point.
    /// </summary>
    /// <returns>The vector from the origin to the point.</returns>
    public Vector3 ToVector() => new(X, Y, Z);

    /// <summary>
    ///     Calculates the distance to another point.
    /// </summary>
    /// <param name="other">The other point.</param>
    /// <returns>The distance.</returns>
    public double Distance(Point3D other) => (other - this).Length;

    /// <summary>
    ///     Calculates the distance between two points.
    /// </summary>
    /// <param name="left">The first point.</param>
    /// <param name="right">The second point.</param>
    /// <returns>The distance.</returns>
    public static double Distance(Point3D left, Point3D right) => left.Distance(right);

    /// <summary>
    ///     Calculates the squared distance to another point.
    /// </summary>
    /// <param name="other">The other point.</param>
    /// <returns>The squared distance.</returns>
    public double DistanceSquared(Point3D other) => (other - this).LengthSquared;

    /// <summary>
    ///     Calculates the squared distance between two points.
    /// </summary>
    /// <param name="left">The first point.</param>
    /// <param name="right">The second point.</param>
    /// <returns>The squared distance.</returns>
    public static double DistanceSquared(Point3D left, Point3D right) => left.DistanceSquared(right);

    /// <summary>
    ///     Determines whether this point equals another point within the default tolerances (coordinate-wise).
    /// </summary>
    /// <param name="other">The other point.</param>
    /// <returns><see langword="true" /> if the points are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Point3D other) =>
        ApproximatelyEquals(other, Tolerance.DefaultAbsolute, Tolerance.DefaultRelative);

    /// <summary>
    ///     Determines whether this point equals another point within the specified tolerances (coordinate-wise).
    /// </summary>
    /// <param name="other">The other point.</param>
    /// <param name="absolute">The absolute tolerance.</param>
    /// <param name="relative">The relative tolerance.</param>
    /// <returns><see langword="true" /> if the points are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Point3D other, double absolute, double relative) =>
        Tolerance.AreEqual(X, other.X, absolute, relative) && Tolerance.AreEqual(Y, other.Y, absolute, relative) &&
        Tolerance.AreEqual(Z, other.Z, absolute, relative);

    /// <inheritdoc />
    public override string ToString() => string.Create(CultureInfo.InvariantCulture, $"Point3D [X: {X}, Y: {Y}, Z: {Z}]");

    /// <inheritdoc />
    public bool Equals(Point3D other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Point3D other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    /// <summary>
    ///     Translates a point by a vector.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="vector">The translation.</param>
    /// <returns>The translated point.</returns>
    public static Point3D operator +(Point3D point, Vector3 vector) => new(point.X + vector.X, point.Y + vector.Y, point.Z + vector.Z);

    /// <summary>
    ///     Translates a point by the negated vector.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="vector">The translation to subtract.</param>
    /// <returns>The translated point.</returns>
    public static Point3D operator -(Point3D point, Vector3 vector) => new(point.X - vector.X, point.Y - vector.Y, point.Z - vector.Z);

    /// <summary>
    ///     Calculates the vector from one point to another.
    /// </summary>
    /// <param name="end">The point the vector points to.</param>
    /// <param name="start">The point the vector starts at.</param>
    /// <returns>The vector <c>end − start</c>.</returns>
    public static Vector3 operator -(Point3D end, Point3D start) => new(end.X - start.X, end.Y - start.Y, end.Z - start.Z);

    /// <summary>
    ///     Determines whether two points are exactly equal (coordinate-wise <c>==</c>).
    /// </summary>
    /// <param name="left">The first point.</param>
    /// <param name="right">The second point.</param>
    /// <returns><see langword="true" /> if all coordinates are equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(Point3D left, Point3D right) => left.X == right.X && left.Y == right.Y && left.Z == right.Z;

    /// <summary>
    ///     Determines whether two points differ in at least one coordinate.
    /// </summary>
    /// <param name="left">The first point.</param>
    /// <param name="right">The second point.</param>
    /// <returns><see langword="true" /> if any coordinate differs; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(Point3D left, Point3D right) => !(left == right);
}
