namespace SharpMath.Geometry;

/// <summary>
///     Represents an immutable 3x3 matrix.
/// </summary>
/// <remarks>
///     Vectors are column vectors: a matrix transforms a vector as <c>matrix * vector</c>, and transformations compose
///     from right to left (<c>T * R * S * v</c> scales first). Equality is exact; use
///     <see cref="ApproximatelyEquals(Matrix3x3)" /> for tolerance-based comparisons.
/// </remarks>
public readonly partial struct Matrix3x3 : ISquareMatrix, IEquatable<Matrix3x3>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Matrix3x3" /> struct from its entries in row-major order.
    /// </summary>
    /// <param name="m11">The entry in row 1, column 1.</param>
    /// <param name="m12">The entry in row 1, column 2.</param>
    /// <param name="m13">The entry in row 1, column 3.</param>
    /// <param name="m21">The entry in row 2, column 1.</param>
    /// <param name="m22">The entry in row 2, column 2.</param>
    /// <param name="m23">The entry in row 2, column 3.</param>
    /// <param name="m31">The entry in row 3, column 1.</param>
    /// <param name="m32">The entry in row 3, column 2.</param>
    /// <param name="m33">The entry in row 3, column 3.</param>
    public Matrix3x3(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M21 = m21;
        M22 = m22;
        M23 = m23;
        M31 = m31;
        M32 = m32;
        M33 = m33;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Matrix3x3" /> struct from its rows.
    /// </summary>
    /// <param name="row1">Row 1.</param>
    /// <param name="row2">Row 2.</param>
    /// <param name="row3">Row 3.</param>
    public Matrix3x3(Vector3 row1, Vector3 row2, Vector3 row3)
        : this(row1.X, row1.Y, row1.Z, row2.X, row2.Y, row2.Z, row3.X, row3.Y, row3.Z)
    {
    }

    /// <summary>
    ///     Gets the entry in row 1, column 1.
    /// </summary>
    public double M11 { get; }

    /// <summary>
    ///     Gets the entry in row 1, column 2.
    /// </summary>
    public double M12 { get; }

    /// <summary>
    ///     Gets the entry in row 1, column 3.
    /// </summary>
    public double M13 { get; }

    /// <summary>
    ///     Gets the entry in row 2, column 1.
    /// </summary>
    public double M21 { get; }

    /// <summary>
    ///     Gets the entry in row 2, column 2.
    /// </summary>
    public double M22 { get; }

    /// <summary>
    ///     Gets the entry in row 2, column 3.
    /// </summary>
    public double M23 { get; }

    /// <summary>
    ///     Gets the entry in row 3, column 1.
    /// </summary>
    public double M31 { get; }

    /// <summary>
    ///     Gets the entry in row 3, column 2.
    /// </summary>
    public double M32 { get; }

    /// <summary>
    ///     Gets the entry in row 3, column 3.
    /// </summary>
    public double M33 { get; }

    /// <summary>
    ///     Gets the zero matrix.
    /// </summary>
    public static Matrix3x3 Zero => default;

    /// <summary>
    ///     Gets the identity matrix.
    /// </summary>
    public static Matrix3x3 Identity => new(1, 0, 0, 0, 1, 0, 0, 0, 1);

    /// <summary>
    ///     Gets the entry at the specified row-major index (<c>row · 3 + column</c>).
    /// </summary>
    /// <param name="index">The zero-based index between 0 and 8.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is out of range.</exception>
    public double this[int index] => index switch
    {
        0 => M11,
        1 => M12,
        2 => M13,
        3 => M21,
        4 => M22,
        5 => M23,
        6 => M31,
        7 => M32,
        8 => M33,
        _ => throw MatrixMath.IndexOutOfRange(nameof(index), index, 9)
    };

    /// <inheritdoc />
    public double this[int row, int column]
    {
        get
        {
            MatrixMath.CheckIndices(row, column, 3);
            return this[row * 3 + column];
        }
    }

    /// <inheritdoc />
    public int RowCount => 3;

    /// <inheritdoc />
    public int ColumnCount => 3;

    /// <inheritdoc />
    public int Dimension => 3;

    /// <inheritdoc />
    public double Trace => M11 + M22 + M33;

    /// <inheritdoc />
    public bool IsIdentity => Is(MatrixMath.Predicate.Identity);

    /// <inheritdoc />
    public bool IsDiagonal => Is(MatrixMath.Predicate.Diagonal);

    /// <inheritdoc />
    public bool IsTriangular => Is(MatrixMath.Predicate.Triangular);

    /// <inheritdoc />
    public bool IsSymmetric => Is(MatrixMath.Predicate.Symmetric);

    /// <inheritdoc />
    public bool IsSkewSymmetric => Is(MatrixMath.Predicate.SkewSymmetric);

    /// <inheritdoc />
    public bool IsOrthogonal => (this * Transpose()).IsIdentity;

    /// <inheritdoc />
    public bool IsSingular => Is(MatrixMath.Predicate.Singular);

    /// <summary>
    ///     Creates a matrix from any <see cref="IMatrix" /> with 3 rows and 3 columns.
    /// </summary>
    /// <param name="matrix">The matrix to copy.</param>
    /// <returns>The copied matrix.</returns>
    /// <exception cref="ArgumentException">The matrix is not 3x3.</exception>
    public static Matrix3x3 FromMatrix(IMatrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        if (matrix.RowCount != 3 || matrix.ColumnCount != 3)
            throw new ArgumentException("The matrix must have 3 rows and 3 columns.", nameof(matrix));
        return new Matrix3x3(matrix[0, 0], matrix[0, 1], matrix[0, 2], matrix[1, 0], matrix[1, 1], matrix[1, 2], matrix[2, 0], matrix[2, 1], matrix[2, 2]);
    }

    /// <summary>
    ///     Returns the transpose.
    /// </summary>
    /// <returns>The transposed matrix.</returns>
    public Matrix3x3 Transpose() => new(M11, M21, M31, M12, M22, M32, M13, M23, M33);

    /// <summary>
    ///     Returns the matrix of cofactors.
    /// </summary>
    /// <returns>The cofactor matrix.</returns>
    public Matrix3x3 Cofactors() => Adjugate().Transpose();

    /// <summary>
    ///     Returns the inverse.
    /// </summary>
    /// <returns>The inverse matrix.</returns>
    /// <exception cref="InvalidOperationException">The matrix is singular (see <see cref="IsSingular" />).</exception>
    public Matrix3x3 Inverse() => TryInvert(out var inverse) ? inverse : throw MatrixMath.SingularMatrix();

    /// <summary>
    ///     Tries to calculate the inverse.
    /// </summary>
    /// <param name="inverse">The inverse, or <see cref="Zero" /> if the matrix is singular.</param>
    /// <returns><see langword="true" /> if the matrix is invertible; otherwise, <see langword="false" />.</returns>
    public bool TryInvert(out Matrix3x3 inverse)
    {
        if (IsSingular)
        {
            inverse = default;
            return false;
        }

        // Expanding along the first row reuses the minors already computed for the adjugate.
        var adjugate = Adjugate();
        inverse = adjugate / (M11 * adjugate.M11 + M12 * adjugate.M21 + M13 * adjugate.M31);
        return true;
    }

    /// <summary>
    ///     Determines whether this matrix equals another matrix within the default tolerances (entry-wise).
    /// </summary>
    /// <param name="other">The other matrix.</param>
    /// <returns><see langword="true" /> if the matrices are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Matrix3x3 other) =>
        ApproximatelyEquals(other, Tolerance.DefaultAbsolute, Tolerance.DefaultRelative);

    /// <summary>
    ///     Determines whether this matrix equals another matrix within the specified tolerances (entry-wise).
    /// </summary>
    /// <param name="other">The other matrix.</param>
    /// <param name="absolute">The absolute tolerance.</param>
    /// <param name="relative">The relative tolerance.</param>
    /// <returns><see langword="true" /> if the matrices are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Matrix3x3 other, double absolute, double relative)
    {
        Span<double> entries = stackalloc double[9];
        Span<double> otherEntries = stackalloc double[9];
        CopyTo(entries);
        other.CopyTo(otherEntries);
        return MatrixMath.ApproximatelyEqual(entries, otherEntries, absolute, relative);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        Span<double> entries = stackalloc double[9];
        CopyTo(entries);
        return MatrixMath.Format(nameof(Matrix3x3), entries, 3, 3);
    }

    /// <summary>
    ///     Copies the entries in row-major order.
    /// </summary>
    internal void CopyTo(Span<double> destination)
    {
        destination[0] = M11;
        destination[1] = M12;
        destination[2] = M13;
        destination[3] = M21;
        destination[4] = M22;
        destination[5] = M23;
        destination[6] = M31;
        destination[7] = M32;
        destination[8] = M33;
    }

    private bool Is(MatrixMath.Predicate predicate)
    {
        Span<double> entries = stackalloc double[9];
        CopyTo(entries);
        return MatrixMath.Evaluate(predicate, entries, 3);
    }

    /// <inheritdoc />
    public bool Equals(Matrix3x3 other) =>
        M11.Equals(other.M11) &&
        M12.Equals(other.M12) &&
        M13.Equals(other.M13) &&
        M21.Equals(other.M21) &&
        M22.Equals(other.M22) &&
        M23.Equals(other.M23) &&
        M31.Equals(other.M31) &&
        M32.Equals(other.M32) &&
        M33.Equals(other.M33);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Matrix3x3 other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(M11);
        hash.Add(M12);
        hash.Add(M13);
        hash.Add(M21);
        hash.Add(M22);
        hash.Add(M23);
        hash.Add(M31);
        hash.Add(M32);
        hash.Add(M33);
        return hash.ToHashCode();
    }

    /// <summary>
    ///     Adds two matrices.
    /// </summary>
    /// <param name="left">The first matrix.</param>
    /// <param name="right">The second matrix.</param>
    /// <returns>The sum.</returns>
    public static Matrix3x3 operator +(Matrix3x3 left, Matrix3x3 right) =>
        new(left.M11 + right.M11, left.M12 + right.M12, left.M13 + right.M13, left.M21 + right.M21, left.M22 + right.M22, left.M23 + right.M23, left.M31 + right.M31, left.M32 + right.M32, left.M33 + right.M33);

    /// <summary>
    ///     Subtracts a matrix from another matrix.
    /// </summary>
    /// <param name="left">The minuend.</param>
    /// <param name="right">The subtrahend.</param>
    /// <returns>The difference.</returns>
    public static Matrix3x3 operator -(Matrix3x3 left, Matrix3x3 right) =>
        new(left.M11 - right.M11, left.M12 - right.M12, left.M13 - right.M13, left.M21 - right.M21, left.M22 - right.M22, left.M23 - right.M23, left.M31 - right.M31, left.M32 - right.M32, left.M33 - right.M33);

    /// <summary>
    ///     Negates a matrix.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The negated matrix.</returns>
    public static Matrix3x3 operator -(Matrix3x3 matrix) =>
        new(-matrix.M11, -matrix.M12, -matrix.M13, -matrix.M21, -matrix.M22, -matrix.M23, -matrix.M31, -matrix.M32, -matrix.M33);

    /// <summary>
    ///     Multiplies a matrix with a scalar.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="scalar">The scalar.</param>
    /// <returns>The scaled matrix.</returns>
    public static Matrix3x3 operator *(Matrix3x3 matrix, double scalar) =>
        new(matrix.M11 * scalar, matrix.M12 * scalar, matrix.M13 * scalar, matrix.M21 * scalar, matrix.M22 * scalar, matrix.M23 * scalar, matrix.M31 * scalar, matrix.M32 * scalar, matrix.M33 * scalar);

    /// <summary>
    ///     Multiplies a scalar with a matrix.
    /// </summary>
    /// <param name="scalar">The scalar.</param>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The scaled matrix.</returns>
    public static Matrix3x3 operator *(double scalar, Matrix3x3 matrix) => matrix * scalar;

    /// <summary>
    ///     Divides a matrix by a scalar.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="scalar">The scalar.</param>
    /// <returns>The scaled matrix.</returns>
    public static Matrix3x3 operator /(Matrix3x3 matrix, double scalar) =>
        new(matrix.M11 / scalar, matrix.M12 / scalar, matrix.M13 / scalar, matrix.M21 / scalar, matrix.M22 / scalar, matrix.M23 / scalar, matrix.M31 / scalar, matrix.M32 / scalar, matrix.M33 / scalar);

    /// <summary>
    ///     Multiplies two matrices.
    /// </summary>
    /// <param name="left">The left matrix.</param>
    /// <param name="right">The right matrix.</param>
    /// <returns>The product <c>left · right</c>.</returns>
    public static Matrix3x3 operator *(Matrix3x3 left, Matrix3x3 right) =>
        new(left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31,
            left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32,
            left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33,
            left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31,
            left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32,
            left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33,
            left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31,
            left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32,
            left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33);

    /// <summary>
    ///     Transforms a column vector.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="vector">The vector.</param>
    /// <returns>The product <c>matrix · vector</c>.</returns>
    public static Vector3 operator *(Matrix3x3 matrix, Vector3 vector) =>
        new(matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z,
            matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z,
            matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z);

    /// <summary>
    ///     Determines whether two matrices are exactly equal (entry-wise <c>==</c>).
    /// </summary>
    /// <param name="left">The first matrix.</param>
    /// <param name="right">The second matrix.</param>
    /// <returns><see langword="true" /> if all entries are equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(Matrix3x3 left, Matrix3x3 right) =>
        left.M11 == right.M11 &&
        left.M12 == right.M12 &&
        left.M13 == right.M13 &&
        left.M21 == right.M21 &&
        left.M22 == right.M22 &&
        left.M23 == right.M23 &&
        left.M31 == right.M31 &&
        left.M32 == right.M32 &&
        left.M33 == right.M33;

    /// <summary>
    ///     Determines whether two matrices differ in at least one entry.
    /// </summary>
    /// <param name="left">The first matrix.</param>
    /// <param name="right">The second matrix.</param>
    /// <returns><see langword="true" /> if any entry differs; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(Matrix3x3 left, Matrix3x3 right) => !(left == right);
}
