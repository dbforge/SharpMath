using System.Globalization;

namespace SharpMath.Geometry;

/// <summary>
///     Represents an immutable axis-aligned bounding box in the plane.
/// </summary>
public readonly struct BoundingBox2D : IEquatable<BoundingBox2D>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BoundingBox2D" /> struct.
    /// </summary>
    /// <param name="minX">The smallest X coordinate.</param>
    /// <param name="minY">The smallest Y coordinate.</param>
    /// <param name="maxX">The largest X coordinate.</param>
    /// <param name="maxY">The largest Y coordinate.</param>
    /// <exception cref="ArgumentException">A minimum is greater than the corresponding maximum, or a value is NaN.</exception>
    public BoundingBox2D(double minX, double minY, double maxX, double maxY)
    {
        if (!(minX <= maxX))
            throw new ArgumentException("minX must not be greater than maxX.", nameof(minX));
        if (!(minY <= maxY))
            throw new ArgumentException("minY must not be greater than maxY.", nameof(minY));

        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }

    /// <summary>
    ///     Gets the smallest X coordinate.
    /// </summary>
    public double MinX { get; }

    /// <summary>
    ///     Gets the smallest Y coordinate.
    /// </summary>
    public double MinY { get; }

    /// <summary>
    ///     Gets the largest X coordinate.
    /// </summary>
    public double MaxX { get; }

    /// <summary>
    ///     Gets the largest Y coordinate.
    /// </summary>
    public double MaxY { get; }

    /// <summary>
    ///     Gets the width.
    /// </summary>
    public double Width => MaxX - MinX;

    /// <summary>
    ///     Gets the height.
    /// </summary>
    public double Height => MaxY - MinY;

    /// <summary>
    ///     Gets the center.
    /// </summary>
    public Point2D Center => new((MinX + MaxX) / 2, (MinY + MaxY) / 2);

    /// <summary>
    ///     Creates the smallest bounding box that contains all specified points.
    /// </summary>
    /// <param name="points">The points; at least one.</param>
    /// <returns>The bounding box.</returns>
    /// <exception cref="ArgumentException"><paramref name="points" /> is empty.</exception>
    public static BoundingBox2D FromPoints(ReadOnlySpan<Point2D> points)
    {
        if (points.IsEmpty)
            throw new ArgumentException("At least one point is required.", nameof(points));

        double minX = points[0].X, minY = points[0].Y, maxX = minX, maxY = minY;
        foreach (var point in points[1..])
        {
            minX = Math.Min(minX, point.X);
            minY = Math.Min(minY, point.Y);
            maxX = Math.Max(maxX, point.X);
            maxY = Math.Max(maxY, point.Y);
        }

        return new BoundingBox2D(minX, minY, maxX, maxY);
    }

    /// <summary>
    ///     Determines whether a point lies inside the box or on its border.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns><see langword="true" /> if the box contains the point; otherwise, <see langword="false" />.</returns>
    public bool Contains(Point2D point) =>
        point.X >= MinX && point.X <= MaxX && point.Y >= MinY && point.Y <= MaxY;

    /// <summary>
    ///     Returns the smallest bounding box that contains this box and another box.
    /// </summary>
    /// <param name="other">The other box.</param>
    /// <returns>The union.</returns>
    public BoundingBox2D Union(BoundingBox2D other) =>
        new(Math.Min(MinX, other.MinX), Math.Min(MinY, other.MinY), Math.Max(MaxX, other.MaxX),
            Math.Max(MaxY, other.MaxY));

    /// <summary>
    ///     Determines whether this box equals another box within the default tolerances.
    /// </summary>
    /// <param name="other">The other box.</param>
    /// <returns><see langword="true" /> if the boxes are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(BoundingBox2D other) =>
        ApproximatelyEquals(other, Tolerance.DefaultAbsolute, Tolerance.DefaultRelative);

    /// <summary>
    ///     Determines whether this box equals another box within the specified tolerances.
    /// </summary>
    /// <param name="other">The other box.</param>
    /// <param name="absolute">The absolute tolerance.</param>
    /// <param name="relative">The relative tolerance.</param>
    /// <returns><see langword="true" /> if the boxes are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(BoundingBox2D other, double absolute, double relative) =>
        Tolerance.AreEqual(MinX, other.MinX, absolute, relative) && Tolerance.AreEqual(MinY, other.MinY, absolute, relative) &&
        Tolerance.AreEqual(MaxX, other.MaxX, absolute, relative) && Tolerance.AreEqual(MaxY, other.MaxY, absolute, relative);

    /// <inheritdoc />
    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"BoundingBox2D [Min: ({MinX}, {MinY}), Max: ({MaxX}, {MaxY})]");

    /// <inheritdoc />
    public bool Equals(BoundingBox2D other) =>
        MinX.Equals(other.MinX) && MinY.Equals(other.MinY) && MaxX.Equals(other.MaxX) && MaxY.Equals(other.MaxY);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is BoundingBox2D other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(MinX, MinY, MaxX, MaxY);

    /// <summary>
    ///     Determines whether two boxes are equal.
    /// </summary>
    /// <param name="left">The first box.</param>
    /// <param name="right">The second box.</param>
    /// <returns><see langword="true" /> if the boxes are equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(BoundingBox2D left, BoundingBox2D right) =>
        left.MinX == right.MinX && left.MinY == right.MinY && left.MaxX == right.MaxX && left.MaxY == right.MaxY;

    /// <summary>
    ///     Determines whether two boxes differ.
    /// </summary>
    /// <param name="left">The first box.</param>
    /// <param name="right">The second box.</param>
    /// <returns><see langword="true" /> if the boxes differ; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(BoundingBox2D left, BoundingBox2D right) => !(left == right);
}
