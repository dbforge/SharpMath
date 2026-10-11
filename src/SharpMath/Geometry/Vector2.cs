using System.Globalization;

namespace SharpMath.Geometry;

/// <summary>
///     Represents an immutable two-dimensional vector.
/// </summary>
/// <remarks>
///     Equality (<see cref="Equals(Vector2)" />, <c>==</c>) is exact. Use <see cref="ApproximatelyEquals(Vector2)" />
///     for tolerance-based comparisons.
/// </remarks>
public readonly struct Vector2 : IVector, IEquatable<Vector2>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Vector2" /> struct.
    /// </summary>
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component.</param>
    public Vector2(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Vector2" /> struct with the position vector of a point.
    /// </summary>
    /// <param name="point">The point.</param>
    public Vector2(Point2D point) : this(point.X, point.Y)
    {
    }

    /// <summary>
    ///     Gets the X component.
    /// </summary>
    public double X { get; }

    /// <summary>
    ///     Gets the Y component.
    /// </summary>
    public double Y { get; }

    /// <summary>
    ///     Gets the vector (0, 0).
    /// </summary>
    public static Vector2 Zero => default;

    /// <summary>
    ///     Gets the vector (1, 1).
    /// </summary>
    public static Vector2 One => new(1, 1);

    /// <summary>
    ///     Gets the unit vector (1, 0).
    /// </summary>
    public static Vector2 UnitX => new(1, 0);

    /// <summary>
    ///     Gets the unit vector (0, 1).
    /// </summary>
    public static Vector2 UnitY => new(0, 1);

    /// <summary>
    ///     Gets the vector (0, 1).
    /// </summary>
    public static Vector2 Up => new(0, 1);

    /// <summary>
    ///     Gets the vector (0, −1).
    /// </summary>
    public static Vector2 Down => new(0, -1);

    /// <summary>
    ///     Gets the vector (−1, 0).
    /// </summary>
    public static Vector2 Left => new(-1, 0);

    /// <summary>
    ///     Gets the vector (1, 0).
    /// </summary>
    public static Vector2 Right => new(1, 0);

    /// <inheritdoc />
    public int Dimension => 2;

    /// <inheritdoc />
    public double this[int index] => index switch
    {
        0 => X,
        1 => Y,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "The index must be 0 or 1.")
    };

    /// <inheritdoc />
    public double Length => Math.Sqrt(LengthSquared);

    /// <inheritdoc />
    public double LengthSquared => X * X + Y * Y;

    /// <inheritdoc />
    public bool IsZero => Tolerance.IsZero(X) && Tolerance.IsZero(Y);

    /// <inheritdoc />
    public bool IsNormalized => Tolerance.AreEqual(LengthSquared, 1);

    /// <summary>
    ///     Gets the vector rotated by 90° counterclockwise, i.e. (−Y, X).
    /// </summary>
    public Vector2 Perpendicular => new(-Y, X);

    /// <summary>
    ///     Returns the vector with the same direction and a length of 1.
    /// </summary>
    /// <returns>The normalized vector.</returns>
    /// <exception cref="InvalidOperationException">The vector is the zero vector.</exception>
    public Vector2 Normalize()
    {
        // Scaling by the largest component first avoids underflow and overflow of the squared length.
        var scale = Math.Max(Math.Abs(X), Math.Abs(Y));
        if (scale == 0)
            throw new InvalidOperationException("The zero vector cannot be normalized.");
        var scaled = new Vector2(X / scale, Y / scale);
        var length = scaled.Length;
        return new Vector2(scaled.X / length, scaled.Y / length);
    }

    /// <summary>
    ///     Calculates the dot product with another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The dot product.</returns>
    public double Dot(Vector2 other) => X * other.X + Y * other.Y;

    /// <summary>
    ///     Calculates the dot product of two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The dot product.</returns>
    public static double Dot(Vector2 left, Vector2 right) => left.Dot(right);

    /// <summary>
    ///     Calculates the two-dimensional cross product (the Z component of the three-dimensional cross product) with another
    ///     vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns><c>X · other.Y − Y · other.X</c>.</returns>
    public double Cross(Vector2 other) => X * other.Y - Y * other.X;

    /// <summary>
    ///     Calculates the two-dimensional cross product of two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns><c>left.X · right.Y − left.Y · right.X</c>.</returns>
    public static double Cross(Vector2 left, Vector2 right) => left.Cross(right);

    /// <summary>
    ///     Calculates the angle between this vector and another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The angle in radians, in the range [0, π].</returns>
    /// <exception cref="InvalidOperationException">One of the vectors is the zero vector.</exception>
    public double Angle(Vector2 other)
    {
        if (IsExactlyZero || other.IsExactlyZero)
            throw VectorMath.ZeroVectorAngle();
        var (u, v) = (Normalize(), other.Normalize());
        return VectorMath.Angle(Math.Abs(u.Cross(v)), u.Dot(v));
    }

    /// <summary>
    ///     Calculates the angle between two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The angle in radians, in the range [0, π].</returns>
    /// <exception cref="InvalidOperationException">One of the vectors is the zero vector.</exception>
    public static double Angle(Vector2 left, Vector2 right) => left.Angle(right);

    /// <summary>
    ///     Calculates the distance between the tips of this vector and another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The distance.</returns>
    public double Distance(Vector2 other) => (this - other).Length;

    /// <summary>
    ///     Calculates the distance between the tips of two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The distance.</returns>
    public static double Distance(Vector2 left, Vector2 right) => left.Distance(right);

    /// <summary>
    ///     Calculates the squared distance between the tips of this vector and another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The squared distance.</returns>
    public double DistanceSquared(Vector2 other) => (this - other).LengthSquared;

    /// <summary>
    ///     Calculates the squared distance between the tips of two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The squared distance.</returns>
    public static double DistanceSquared(Vector2 left, Vector2 right) => left.DistanceSquared(right);

    /// <summary>
    ///     Calculates the area of the parallelogram spanned by this vector and another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The area.</returns>
    public double ParallelogramArea(Vector2 other) => Math.Abs(Cross(other));

    /// <summary>
    ///     Interpolates linearly between two vectors. The interpolant is clamped to [0, 1].
    /// </summary>
    /// <param name="source">The vector returned for an interpolant of 0.</param>
    /// <param name="target">The vector returned for an interpolant of 1.</param>
    /// <param name="amount">The interpolant.</param>
    /// <returns>The interpolated vector.</returns>
    public static Vector2 Lerp(Vector2 source, Vector2 target, double amount) =>
        LerpUnclamped(source, target, Math.Clamp(amount, 0, 1));

    /// <summary>
    ///     Interpolates or extrapolates linearly between two vectors.
    /// </summary>
    /// <param name="source">The vector returned for an interpolant of 0.</param>
    /// <param name="target">The vector returned for an interpolant of 1.</param>
    /// <param name="amount">The interpolant; values outside of [0, 1] extrapolate.</param>
    /// <returns>The interpolated vector.</returns>
    public static Vector2 LerpUnclamped(Vector2 source, Vector2 target, double amount) =>
        source + (target - source) * amount;

    /// <summary>
    ///     Moves a vector towards a target by at most the specified distance without overshooting it.
    /// </summary>
    /// <param name="source">The vector to move.</param>
    /// <param name="target">The target vector.</param>
    /// <param name="maxDistanceDelta">The maximum distance to move.</param>
    /// <returns>The moved vector, or <paramref name="target" /> if it is within reach.</returns>
    public static Vector2 MoveTowards(Vector2 source, Vector2 target, double maxDistanceDelta)
    {
        var delta = target - source;
        var distance = delta.Length;
        if (distance <= maxDistanceDelta || distance == 0)
            return target;
        return source + delta / distance * maxDistanceDelta;
    }

    /// <summary>
    ///     Determines whether this vector is parallel (or antiparallel) to another vector, i.e. whether the sine of the
    ///     angle between them is at most 1e-9, independent of their lengths. The zero vector is parallel to every vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the vectors are parallel; otherwise, <see langword="false" />.</returns>
    public bool IsParallelTo(Vector2 other)
    {
        if (IsExactlyZero || other.IsExactlyZero)
            return true;
        var (u, v) = (Normalize(), other.Normalize());
        return VectorMath.IsNegligible(Math.Abs(u.Cross(v)));
    }

    /// <summary>
    ///     Determines whether this vector is orthogonal to another vector, i.e. whether the cosine of the angle between
    ///     them is at most 1e-9, independent of their lengths. The zero vector is orthogonal to every vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the vectors are orthogonal; otherwise, <see langword="false" />.</returns>
    public bool IsOrthogonalTo(Vector2 other) =>
        IsExactlyZero || other.IsExactlyZero || VectorMath.IsNegligible(Normalize().Dot(other.Normalize()));

    /// <summary>
    ///     Determines whether this vector and another vector are both normalized and orthogonal to each other.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the vectors are orthonormal; otherwise, <see langword="false" />.</returns>
    public bool IsOrthonormalTo(Vector2 other) => IsNormalized && other.IsNormalized && IsOrthogonalTo(other);

    /// <summary>
    ///     Transforms a vector with a 2x2 matrix (<c>matrix · vector</c>).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The transformed vector.</returns>
    public static Vector2 Transform(Vector2 vector, Matrix2x2 matrix) => matrix * vector;

    /// <summary>
    ///     Transforms a vector with a 3x3 matrix in homogeneous coordinates (<c>matrix · (x, y, 1)</c>), so translations
    ///     are applied. The result is divided by the resulting W component if it is not 1.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The transformed vector.</returns>
    public static Vector2 Transform(Vector2 vector, Matrix3x3 matrix) => matrix * vector;

    /// <summary>
    ///     Converts the vector into the point it points to.
    /// </summary>
    /// <returns>The point (X, Y).</returns>
    public Point2D ToPoint() => new(X, Y);

    /// <summary>
    ///     Determines whether this vector equals another vector within the default tolerances (component-wise).
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the vectors are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Vector2 other) =>
        ApproximatelyEquals(other, Tolerance.DefaultAbsolute, Tolerance.DefaultRelative);

    /// <summary>
    ///     Determines whether this vector equals another vector within the specified tolerances (component-wise).
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <param name="absolute">The absolute tolerance.</param>
    /// <param name="relative">The relative tolerance.</param>
    /// <returns><see langword="true" /> if the vectors are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Vector2 other, double absolute, double relative) =>
        Tolerance.AreEqual(X, other.X, absolute, relative) && Tolerance.AreEqual(Y, other.Y, absolute, relative);

    private bool IsExactlyZero => X == 0 && Y == 0;

    /// <summary>
    ///     Returns a LaTeX representation of the vector as a column vector.
    /// </summary>
    /// <returns>The LaTeX string.</returns>
    public string ToLaTeXString() => VectorMath.ToLaTeX([X, Y]);

    /// <inheritdoc />
    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"Vector2 {{X: {X}, Y: {Y}}}");

    /// <inheritdoc />
    public bool Equals(Vector2 other) => X.Equals(other.X) && Y.Equals(other.Y);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Vector2 other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y);

    /// <summary>
    ///     Adds two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The sum.</returns>
    public static Vector2 operator +(Vector2 left, Vector2 right) => new(left.X + right.X, left.Y + right.Y);

    /// <summary>
    ///     Subtracts a vector from another vector.
    /// </summary>
    /// <param name="left">The minuend.</param>
    /// <param name="right">The subtrahend.</param>
    /// <returns>The difference.</returns>
    public static Vector2 operator -(Vector2 left, Vector2 right) => new(left.X - right.X, left.Y - right.Y);

    /// <summary>
    ///     Negates a vector.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The negated vector.</returns>
    public static Vector2 operator -(Vector2 vector) => new(-vector.X, -vector.Y);

    /// <summary>
    ///     Multiplies a vector with a scalar.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="scalar">The scalar.</param>
    /// <returns>The scaled vector.</returns>
    public static Vector2 operator *(Vector2 vector, double scalar) => new(vector.X * scalar, vector.Y * scalar);

    /// <summary>
    ///     Multiplies a scalar with a vector.
    /// </summary>
    /// <param name="scalar">The scalar.</param>
    /// <param name="vector">The vector.</param>
    /// <returns>The scaled vector.</returns>
    public static Vector2 operator *(double scalar, Vector2 vector) => vector * scalar;

    /// <summary>
    ///     Divides a vector by a scalar.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="scalar">The scalar.</param>
    /// <returns>The scaled vector.</returns>
    public static Vector2 operator /(Vector2 vector, double scalar) => new(vector.X / scalar, vector.Y / scalar);

    /// <summary>
    ///     Determines whether two vectors are exactly equal (component-wise <c>==</c>).
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns><see langword="true" /> if all components are equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(Vector2 left, Vector2 right) => left.X == right.X && left.Y == right.Y;

    /// <summary>
    ///     Determines whether two vectors differ in at least one component.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns><see langword="true" /> if any component differs; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(Vector2 left, Vector2 right) => !(left == right);
}
