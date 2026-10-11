using System.Globalization;

namespace SharpMath.Geometry;

/// <summary>
///     Represents an immutable four-dimensional vector, e.g. a point or direction in homogeneous coordinates.
/// </summary>
/// <remarks>
///     Equality (<see cref="Equals(Vector4)" />, <c>==</c>) is exact. Use <see cref="ApproximatelyEquals(Vector4)" />
///     for tolerance-based comparisons.
/// </remarks>
public readonly struct Vector4 : IVector, IEquatable<Vector4>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Vector4" /> struct.
    /// </summary>
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component.</param>
    /// <param name="z">The Z component.</param>
    /// <param name="w">The W component.</param>
    public Vector4(double x, double y, double z, double w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Vector4" /> struct from a three-dimensional vector and a W component.
    /// </summary>
    /// <param name="xyz">The X, Y and Z components.</param>
    /// <param name="w">The W component (1 for points, 0 for directions).</param>
    public Vector4(Vector3 xyz, double w) : this(xyz.X, xyz.Y, xyz.Z, w)
    {
    }

    /// <summary>
    ///     Gets the X component.
    /// </summary>
    public double X { get; init; }

    /// <summary>
    ///     Gets the Y component.
    /// </summary>
    public double Y { get; init; }

    /// <summary>
    ///     Gets the Z component.
    /// </summary>
    public double Z { get; init; }

    /// <summary>
    ///     Gets the W component.
    /// </summary>
    public double W { get; init; }

    /// <summary>
    ///     Gets the vector (0, 0, 0, 0).
    /// </summary>
    public static Vector4 Zero => default;

    /// <summary>
    ///     Gets the vector (1, 1, 1, 1).
    /// </summary>
    public static Vector4 One => new(1, 1, 1, 1);

    /// <summary>
    ///     Gets the unit vector (1, 0, 0, 0).
    /// </summary>
    public static Vector4 UnitX => new(1, 0, 0, 0);

    /// <summary>
    ///     Gets the unit vector (0, 1, 0, 0).
    /// </summary>
    public static Vector4 UnitY => new(0, 1, 0, 0);

    /// <summary>
    ///     Gets the unit vector (0, 0, 1, 0).
    /// </summary>
    public static Vector4 UnitZ => new(0, 0, 1, 0);

    /// <summary>
    ///     Gets the unit vector (0, 0, 0, 1).
    /// </summary>
    public static Vector4 UnitW => new(0, 0, 0, 1);

    /// <inheritdoc />
    public int Dimension => 4;

    /// <inheritdoc />
    public double this[int index] => index switch
    {
        0 => X,
        1 => Y,
        2 => Z,
        3 => W,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "The index must be between 0 and 3.")
    };

    /// <inheritdoc />
    public double Length => Math.Sqrt(LengthSquared);

    /// <inheritdoc />
    public double LengthSquared => X * X + Y * Y + Z * Z + W * W;

    /// <inheritdoc />
    public bool IsZero => Tolerance.IsZero(X) && Tolerance.IsZero(Y) && Tolerance.IsZero(Z) && Tolerance.IsZero(W);

    /// <inheritdoc />
    public bool IsNormalized => Tolerance.AreEqual(LengthSquared, 1);

    /// <summary>
    ///     Returns the vector with the same direction and a length of 1.
    /// </summary>
    /// <returns>The normalized vector.</returns>
    /// <exception cref="InvalidOperationException">The vector is the zero vector.</exception>
    public Vector4 Normalize()
    {
        // Scaling by the largest component first avoids underflow and overflow of the squared length.
        var scale = Math.Max(Math.Max(Math.Max(Math.Abs(X), Math.Abs(Y)), Math.Abs(Z)), Math.Abs(W));
        if (scale == 0)
            throw new InvalidOperationException("The zero vector cannot be normalized.");
        var scaled = new Vector4(X / scale, Y / scale, Z / scale, W / scale);
        var length = scaled.Length;
        return new Vector4(scaled.X / length, scaled.Y / length, scaled.Z / length, scaled.W / length);
    }

    /// <summary>
    ///     Calculates the dot product with another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The dot product.</returns>
    public double Dot(Vector4 other) => X * other.X + Y * other.Y + Z * other.Z + W * other.W;

    /// <summary>
    ///     Calculates the dot product of two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The dot product.</returns>
    public static double Dot(Vector4 left, Vector4 right) => left.Dot(right);

    /// <summary>
    ///     Calculates the angle between this vector and another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The angle in radians, in the range [0, π].</returns>
    /// <exception cref="InvalidOperationException">One of the vectors is the zero vector.</exception>
    public double Angle(Vector4 other)
    {
        if (IsExactlyZero || other.IsExactlyZero)
            throw VectorMath.ZeroVectorAngle();
        var (u, v) = (Normalize(), other.Normalize());
        return VectorMath.Angle(u.SpannedArea(v), u.Dot(v));
    }

    /// <summary>
    ///     Calculates the angle between two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The angle in radians, in the range [0, π].</returns>
    /// <exception cref="InvalidOperationException">One of the vectors is the zero vector.</exception>
    public static double Angle(Vector4 left, Vector4 right) => left.Angle(right);

    /// <summary>
    ///     Calculates the distance between the tips of this vector and another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The distance.</returns>
    public double Distance(Vector4 other) => (this - other).Length;

    /// <summary>
    ///     Calculates the distance between the tips of two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The distance.</returns>
    public static double Distance(Vector4 left, Vector4 right) => left.Distance(right);

    /// <summary>
    ///     Calculates the squared distance between the tips of this vector and another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The squared distance.</returns>
    public double DistanceSquared(Vector4 other) => (this - other).LengthSquared;

    /// <summary>
    ///     Calculates the squared distance between the tips of two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The squared distance.</returns>
    public static double DistanceSquared(Vector4 left, Vector4 right) => left.DistanceSquared(right);

    /// <summary>
    ///     Interpolates linearly between two vectors. The interpolant is clamped to [0, 1].
    /// </summary>
    /// <param name="source">The vector returned for an interpolant of 0.</param>
    /// <param name="target">The vector returned for an interpolant of 1.</param>
    /// <param name="amount">The interpolant.</param>
    /// <returns>The interpolated vector.</returns>
    public static Vector4 Lerp(Vector4 source, Vector4 target, double amount) =>
        LerpUnclamped(source, target, Math.Clamp(amount, 0, 1));

    /// <summary>
    ///     Interpolates or extrapolates linearly between two vectors.
    /// </summary>
    /// <param name="source">The vector returned for an interpolant of 0.</param>
    /// <param name="target">The vector returned for an interpolant of 1.</param>
    /// <param name="amount">The interpolant; values outside of [0, 1] extrapolate.</param>
    /// <returns>The interpolated vector.</returns>
    public static Vector4 LerpUnclamped(Vector4 source, Vector4 target, double amount) =>
        source + (target - source) * amount;

    /// <summary>
    ///     Moves a vector towards a target by at most the specified distance without overshooting it.
    /// </summary>
    /// <param name="source">The vector to move.</param>
    /// <param name="target">The target vector.</param>
    /// <param name="maxDistanceDelta">The maximum distance to move.</param>
    /// <returns>The moved vector, or <paramref name="target" /> if it is within reach.</returns>
    public static Vector4 MoveTowards(Vector4 source, Vector4 target, double maxDistanceDelta)
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
    public bool IsParallelTo(Vector4 other)
    {
        if (IsExactlyZero || other.IsExactlyZero)
            return true;
        var (u, v) = (Normalize(), other.Normalize());
        return VectorMath.IsNegligible(u.SpannedArea(v));
    }


    /// <summary>
    ///     Determines whether this vector is orthogonal to another vector, i.e. whether the cosine of the angle between
    ///     them is at most 1e-9, independent of their lengths. The zero vector is orthogonal to every vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the vectors are orthogonal; otherwise, <see langword="false" />.</returns>
    public bool IsOrthogonalTo(Vector4 other) =>
        IsExactlyZero || other.IsExactlyZero || VectorMath.IsNegligible(Normalize().Dot(other.Normalize()));

    /// <summary>
    ///     Determines whether this vector and another vector are both normalized and orthogonal to each other.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the vectors are orthonormal; otherwise, <see langword="false" />.</returns>
    public bool IsOrthonormalTo(Vector4 other) => IsNormalized && other.IsNormalized && IsOrthogonalTo(other);

    /// <summary>
    ///     Transforms a vector with a 4x4 matrix (<c>matrix · vector</c>).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The transformed vector.</returns>
    public static Vector4 Transform(Vector4 vector, Matrix4x4 matrix) => matrix * vector;

    /// <summary>
    ///     Determines whether this vector equals another vector within the default tolerances (component-wise).
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the vectors are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Vector4 other) =>
        ApproximatelyEquals(other, Tolerance.DefaultAbsolute, Tolerance.DefaultRelative);

    /// <summary>
    ///     Determines whether this vector equals another vector within the specified tolerances (component-wise).
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <param name="absolute">The absolute tolerance.</param>
    /// <param name="relative">The relative tolerance.</param>
    /// <returns><see langword="true" /> if the vectors are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Vector4 other, double absolute, double relative) =>
        Tolerance.AreEqual(X, other.X, absolute, relative) && Tolerance.AreEqual(Y, other.Y, absolute, relative) &&
        Tolerance.AreEqual(Z, other.Z, absolute, relative) && Tolerance.AreEqual(W, other.W, absolute, relative);

    private bool IsExactlyZero => X == 0 && Y == 0 && Z == 0 && W == 0;

    /// <summary>
    ///     The area of the parallelogram spanned with another vector, from the 2x2 minors (Lagrange identity), which
    ///     avoids the cancellation of |a|²|b|² − (a·b)².
    /// </summary>
    private double SpannedArea(Vector4 other) =>
        Math.Sqrt(Square(X * other.Y - Y * other.X) + Square(X * other.Z - Z * other.X) +
                  Square(X * other.W - W * other.X) + Square(Y * other.Z - Z * other.Y) +
                  Square(Y * other.W - W * other.Y) + Square(Z * other.W - W * other.Z));

    private static double Square(double value) => value * value;

    /// <summary>
    ///     Returns a LaTeX representation of the vector as a column vector.
    /// </summary>
    /// <returns>The LaTeX string.</returns>
    public string ToLaTeXString() => VectorMath.ToLaTeX([X, Y, Z, W]);

    /// <inheritdoc />
    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"Vector4 {{X: {X}, Y: {Y}, Z: {Z}, W: {W}}}");

    /// <inheritdoc />
    public bool Equals(Vector4 other) =>
        X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Vector4 other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y, Z, W);

    /// <summary>
    ///     Adds two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The sum.</returns>
    public static Vector4 operator +(Vector4 left, Vector4 right) =>
        new(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);

    /// <summary>
    ///     Subtracts a vector from another vector.
    /// </summary>
    /// <param name="left">The minuend.</param>
    /// <param name="right">The subtrahend.</param>
    /// <returns>The difference.</returns>
    public static Vector4 operator -(Vector4 left, Vector4 right) =>
        new(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);

    /// <summary>
    ///     Negates a vector.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The negated vector.</returns>
    public static Vector4 operator -(Vector4 vector) => new(-vector.X, -vector.Y, -vector.Z, -vector.W);

    /// <summary>
    ///     Multiplies a vector with a scalar.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="scalar">The scalar.</param>
    /// <returns>The scaled vector.</returns>
    public static Vector4 operator *(Vector4 vector, double scalar) =>
        new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar, vector.W * scalar);

    /// <summary>
    ///     Multiplies a scalar with a vector.
    /// </summary>
    /// <param name="scalar">The scalar.</param>
    /// <param name="vector">The vector.</param>
    /// <returns>The scaled vector.</returns>
    public static Vector4 operator *(double scalar, Vector4 vector) => vector * scalar;

    /// <summary>
    ///     Divides a vector by a scalar.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="scalar">The scalar.</param>
    /// <returns>The scaled vector.</returns>
    public static Vector4 operator /(Vector4 vector, double scalar) =>
        new(vector.X / scalar, vector.Y / scalar, vector.Z / scalar, vector.W / scalar);

    /// <summary>
    ///     Determines whether two vectors are exactly equal (component-wise <c>==</c>).
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns><see langword="true" /> if all components are equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(Vector4 left, Vector4 right) =>
        left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W;

    /// <summary>
    ///     Determines whether two vectors differ in at least one component.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns><see langword="true" /> if any component differs; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(Vector4 left, Vector4 right) => !(left == right);
}
