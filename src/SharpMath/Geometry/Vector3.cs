using System.Globalization;

namespace SharpMath.Geometry;

/// <summary>
///     Represents an immutable three-dimensional vector.
/// </summary>
/// <remarks>
///     The library uses a right-handed coordinate system with column vectors: <see cref="Forward" /> points along −Z.
///     Equality (<see cref="Equals(Vector3)" />, <c>==</c>) is exact. Use <see cref="ApproximatelyEquals(Vector3)" />
///     for tolerance-based comparisons.
/// </remarks>
public readonly struct Vector3 : IVector, IEquatable<Vector3>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Vector3" /> struct.
    /// </summary>
    /// <param name="x">The X component.</param>
    /// <param name="y">The Y component.</param>
    /// <param name="z">The Z component.</param>
    public Vector3(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Vector3" /> struct from a two-dimensional vector and a Z component.
    /// </summary>
    /// <param name="xy">The X and Y components.</param>
    /// <param name="z">The Z component.</param>
    public Vector3(Vector2 xy, double z) : this(xy.X, xy.Y, z)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Vector3" /> struct with the position vector of a point.
    /// </summary>
    /// <param name="point">The point.</param>
    public Vector3(Point3D point) : this(point.X, point.Y, point.Z)
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
    ///     Gets the Z component.
    /// </summary>
    public double Z { get; }

    /// <summary>
    ///     Gets the vector (0, 0, 0).
    /// </summary>
    public static Vector3 Zero => default;

    /// <summary>
    ///     Gets the vector (1, 1, 1).
    /// </summary>
    public static Vector3 One => new(1, 1, 1);

    /// <summary>
    ///     Gets the unit vector (1, 0, 0).
    /// </summary>
    public static Vector3 UnitX => new(1, 0, 0);

    /// <summary>
    ///     Gets the unit vector (0, 1, 0).
    /// </summary>
    public static Vector3 UnitY => new(0, 1, 0);

    /// <summary>
    ///     Gets the unit vector (0, 0, 1).
    /// </summary>
    public static Vector3 UnitZ => new(0, 0, 1);

    /// <summary>
    ///     Gets the vector (0, 1, 0).
    /// </summary>
    public static Vector3 Up => new(0, 1, 0);

    /// <summary>
    ///     Gets the vector (0, −1, 0).
    /// </summary>
    public static Vector3 Down => new(0, -1, 0);

    /// <summary>
    ///     Gets the vector (−1, 0, 0).
    /// </summary>
    public static Vector3 Left => new(-1, 0, 0);

    /// <summary>
    ///     Gets the vector (1, 0, 0).
    /// </summary>
    public static Vector3 Right => new(1, 0, 0);

    /// <summary>
    ///     Gets the vector (0, 0, −1), the viewing direction in the right-handed coordinate system.
    /// </summary>
    public static Vector3 Forward => new(0, 0, -1);

    /// <summary>
    ///     Gets the vector (0, 0, 1).
    /// </summary>
    public static Vector3 Back => new(0, 0, 1);

    /// <inheritdoc />
    public int Dimension => 3;

    /// <inheritdoc />
    public double this[int index] => index switch
    {
        0 => X,
        1 => Y,
        2 => Z,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "The index must be between 0 and 2.")
    };

    /// <inheritdoc />
    public double Length => Math.Sqrt(LengthSquared);

    /// <inheritdoc />
    public double LengthSquared => X * X + Y * Y + Z * Z;

    /// <inheritdoc />
    public bool IsZero => Tolerance.IsZero(X) && Tolerance.IsZero(Y) && Tolerance.IsZero(Z);

    /// <inheritdoc />
    public bool IsNormalized => Tolerance.AreEqual(LengthSquared, 1);

    /// <summary>
    ///     Creates a vector from homogeneous coordinates by dividing X, Y and Z by W.
    /// </summary>
    /// <param name="vector">The homogeneous vector.</param>
    /// <returns>The Cartesian vector.</returns>
    /// <exception cref="ArgumentException">The W component is zero (a point at infinity).</exception>
    public static Vector3 FromHomogeneous(Vector4 vector)
    {
        if (vector.W == 0)
            throw new ArgumentException("A homogeneous vector with W = 0 has no Cartesian representation.", nameof(vector));
        return new Vector3(vector.X / vector.W, vector.Y / vector.W, vector.Z / vector.W);
    }

    /// <summary>
    ///     Returns the vector with the same direction and a length of 1.
    /// </summary>
    /// <returns>The normalized vector.</returns>
    /// <exception cref="InvalidOperationException">The vector is the zero vector.</exception>
    public Vector3 Normalize()
    {
        // Scaling by the largest component first avoids underflow and overflow of the squared length.
        var scale = Math.Max(Math.Max(Math.Abs(X), Math.Abs(Y)), Math.Abs(Z));
        if (scale == 0)
            throw new InvalidOperationException("The zero vector cannot be normalized.");
        var scaled = new Vector3(X / scale, Y / scale, Z / scale);
        var length = scaled.Length;
        return new Vector3(scaled.X / length, scaled.Y / length, scaled.Z / length);
    }

    /// <summary>
    ///     Calculates the dot product with another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The dot product.</returns>
    public double Dot(Vector3 other) => X * other.X + Y * other.Y + Z * other.Z;

    /// <summary>
    ///     Calculates the dot product of two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The dot product.</returns>
    public static double Dot(Vector3 left, Vector3 right) => left.Dot(right);

    /// <summary>
    ///     Calculates the cross product with another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The cross product <c>this × other</c>.</returns>
    public Vector3 Cross(Vector3 other) =>
        new(Y * other.Z - Z * other.Y, Z * other.X - X * other.Z, X * other.Y - Y * other.X);

    /// <summary>
    ///     Calculates the cross product of two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The cross product <c>left × right</c>.</returns>
    public static Vector3 Cross(Vector3 left, Vector3 right) => left.Cross(right);

    /// <summary>
    ///     Calculates the scalar triple product <c>first · (second × third)</c>, the signed volume of the parallelepiped
    ///     spanned by the three vectors.
    /// </summary>
    /// <param name="first">The first vector.</param>
    /// <param name="second">The second vector.</param>
    /// <param name="third">The third vector.</param>
    /// <returns>The scalar triple product.</returns>
    public static double ScalarTripleProduct(Vector3 first, Vector3 second, Vector3 third) =>
        first.Dot(second.Cross(third));

    /// <summary>
    ///     Calculates the angle between this vector and another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The angle in radians, in the range [0, π].</returns>
    /// <exception cref="InvalidOperationException">One of the vectors is the zero vector.</exception>
    public double Angle(Vector3 other)
    {
        if (IsExactlyZero || other.IsExactlyZero)
            throw VectorMath.ZeroVectorAngle();
        var (u, v) = (Normalize(), other.Normalize());
        return VectorMath.Angle(u.Cross(v).Length, u.Dot(v));
    }

    /// <summary>
    ///     Calculates the angle between two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The angle in radians, in the range [0, π].</returns>
    /// <exception cref="InvalidOperationException">One of the vectors is the zero vector.</exception>
    public static double Angle(Vector3 left, Vector3 right) => left.Angle(right);

    /// <summary>
    ///     Calculates the distance between the tips of this vector and another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The distance.</returns>
    public double Distance(Vector3 other) => (this - other).Length;

    /// <summary>
    ///     Calculates the distance between the tips of two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The distance.</returns>
    public static double Distance(Vector3 left, Vector3 right) => left.Distance(right);

    /// <summary>
    ///     Calculates the squared distance between the tips of this vector and another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The squared distance.</returns>
    public double DistanceSquared(Vector3 other) => (this - other).LengthSquared;

    /// <summary>
    ///     Calculates the squared distance between the tips of two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The squared distance.</returns>
    public static double DistanceSquared(Vector3 left, Vector3 right) => left.DistanceSquared(right);

    /// <summary>
    ///     Calculates the area of the parallelogram spanned by this vector and another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The area <c>|this × other|</c>.</returns>
    public double ParallelogramArea(Vector3 other) => Cross(other).Length;

    /// <summary>
    ///     Interpolates linearly between two vectors. The interpolant is clamped to [0, 1].
    /// </summary>
    /// <param name="source">The vector returned for an interpolant of 0.</param>
    /// <param name="target">The vector returned for an interpolant of 1.</param>
    /// <param name="amount">The interpolant.</param>
    /// <returns>The interpolated vector.</returns>
    public static Vector3 Lerp(Vector3 source, Vector3 target, double amount) =>
        LerpUnclamped(source, target, Math.Clamp(amount, 0, 1));

    /// <summary>
    ///     Interpolates or extrapolates linearly between two vectors.
    /// </summary>
    /// <param name="source">The vector returned for an interpolant of 0.</param>
    /// <param name="target">The vector returned for an interpolant of 1.</param>
    /// <param name="amount">The interpolant; values outside of [0, 1] extrapolate.</param>
    /// <returns>The interpolated vector.</returns>
    public static Vector3 LerpUnclamped(Vector3 source, Vector3 target, double amount) =>
        source + (target - source) * amount;

    /// <summary>
    ///     Moves a vector towards a target by at most the specified distance without overshooting it.
    /// </summary>
    /// <param name="source">The vector to move.</param>
    /// <param name="target">The target vector.</param>
    /// <param name="maxDistanceDelta">The maximum distance to move.</param>
    /// <returns>The moved vector, or <paramref name="target" /> if it is within reach.</returns>
    public static Vector3 MoveTowards(Vector3 source, Vector3 target, double maxDistanceDelta)
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
    public bool IsParallelTo(Vector3 other)
    {
        if (IsExactlyZero || other.IsExactlyZero)
            return true;
        var (u, v) = (Normalize(), other.Normalize());
        return VectorMath.IsNegligible(u.Cross(v).Length);
    }

    /// <summary>
    ///     Determines whether this vector is orthogonal to another vector, i.e. whether the cosine of the angle between
    ///     them is at most 1e-9, independent of their lengths. The zero vector is orthogonal to every vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the vectors are orthogonal; otherwise, <see langword="false" />.</returns>
    public bool IsOrthogonalTo(Vector3 other) =>
        IsExactlyZero || other.IsExactlyZero || VectorMath.IsNegligible(Normalize().Dot(other.Normalize()));

    /// <summary>
    ///     Determines whether this vector and another vector are both normalized and orthogonal to each other.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the vectors are orthonormal; otherwise, <see langword="false" />.</returns>
    public bool IsOrthonormalTo(Vector3 other) => IsNormalized && other.IsNormalized && IsOrthogonalTo(other);

    /// <summary>
    ///     Transforms a vector with a 3x3 matrix (<c>matrix · vector</c>).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The transformed vector.</returns>
    public static Vector3 Transform(Vector3 vector, Matrix3x3 matrix) => matrix * vector;

    /// <summary>
    ///     Transforms a vector with a 4x4 matrix in homogeneous coordinates (<c>matrix · (x, y, z, 1)</c>), so
    ///     translations are applied. The result is divided by the resulting W component if it is not 1 (perspective divide).
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The transformed vector.</returns>
    public static Vector3 Transform(Vector3 vector, Matrix4x4 matrix) => matrix * vector;

    /// <summary>
    ///     Converts the vector into the point it points to.
    /// </summary>
    /// <returns>The point (X, Y, Z).</returns>
    public Point3D ToPoint() => new(X, Y, Z);

    /// <summary>
    ///     Determines whether this vector equals another vector within the default tolerances (component-wise).
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns><see langword="true" /> if the vectors are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Vector3 other) =>
        ApproximatelyEquals(other, Tolerance.DefaultAbsolute, Tolerance.DefaultRelative);

    /// <summary>
    ///     Determines whether this vector equals another vector within the specified tolerances (component-wise).
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <param name="absolute">The absolute tolerance.</param>
    /// <param name="relative">The relative tolerance.</param>
    /// <returns><see langword="true" /> if the vectors are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Vector3 other, double absolute, double relative) =>
        Tolerance.AreEqual(X, other.X, absolute, relative) && Tolerance.AreEqual(Y, other.Y, absolute, relative) &&
        Tolerance.AreEqual(Z, other.Z, absolute, relative);

    private bool IsExactlyZero => X == 0 && Y == 0 && Z == 0;

    /// <summary>
    ///     Returns a LaTeX representation of the vector as a column vector.
    /// </summary>
    /// <returns>The LaTeX string.</returns>
    public string ToLaTeXString() => VectorMath.ToLaTeX([X, Y, Z]);

    /// <inheritdoc />
    public override string ToString() =>
        string.Create(CultureInfo.InvariantCulture, $"Vector3 {{X: {X}, Y: {Y}, Z: {Z}}}");

    /// <inheritdoc />
    public bool Equals(Vector3 other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Vector3 other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    /// <summary>
    ///     Adds two vectors.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The sum.</returns>
    public static Vector3 operator +(Vector3 left, Vector3 right) =>
        new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    /// <summary>
    ///     Subtracts a vector from another vector.
    /// </summary>
    /// <param name="left">The minuend.</param>
    /// <param name="right">The subtrahend.</param>
    /// <returns>The difference.</returns>
    public static Vector3 operator -(Vector3 left, Vector3 right) =>
        new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    /// <summary>
    ///     Negates a vector.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <returns>The negated vector.</returns>
    public static Vector3 operator -(Vector3 vector) => new(-vector.X, -vector.Y, -vector.Z);

    /// <summary>
    ///     Multiplies a vector with a scalar.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="scalar">The scalar.</param>
    /// <returns>The scaled vector.</returns>
    public static Vector3 operator *(Vector3 vector, double scalar) =>
        new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);

    /// <summary>
    ///     Multiplies a scalar with a vector.
    /// </summary>
    /// <param name="scalar">The scalar.</param>
    /// <param name="vector">The vector.</param>
    /// <returns>The scaled vector.</returns>
    public static Vector3 operator *(double scalar, Vector3 vector) => vector * scalar;

    /// <summary>
    ///     Divides a vector by a scalar.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="scalar">The scalar.</param>
    /// <returns>The scaled vector.</returns>
    public static Vector3 operator /(Vector3 vector, double scalar) =>
        new(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);

    /// <summary>
    ///     Determines whether two vectors are exactly equal (component-wise <c>==</c>).
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns><see langword="true" /> if all components are equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(Vector3 left, Vector3 right) =>
        left.X == right.X && left.Y == right.Y && left.Z == right.Z;

    /// <summary>
    ///     Determines whether two vectors differ in at least one component.
    /// </summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns><see langword="true" /> if any component differs; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(Vector3 left, Vector3 right) => !(left == right);
}
