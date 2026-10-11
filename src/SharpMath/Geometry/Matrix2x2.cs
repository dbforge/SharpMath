namespace SharpMath.Geometry;

/// <summary>
///     Represents an immutable 2x2 matrix.
/// </summary>
/// <remarks>
///     Vectors are column vectors: a matrix transforms a vector as <c>matrix * vector</c>. Equality is exact; use
///     <see cref="ApproximatelyEquals(Matrix2x2)" /> for tolerance-based comparisons.
/// </remarks>
public readonly struct Matrix2x2 : ISquareMatrix, IEquatable<Matrix2x2>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Matrix2x2" /> struct from its entries in row-major order.
    /// </summary>
    /// <param name="m11">The entry in row 1, column 1.</param>
    /// <param name="m12">The entry in row 1, column 2.</param>
    /// <param name="m21">The entry in row 2, column 1.</param>
    /// <param name="m22">The entry in row 2, column 2.</param>
    public Matrix2x2(double m11, double m12, double m21, double m22)
    {
        M11 = m11;
        M12 = m12;
        M21 = m21;
        M22 = m22;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Matrix2x2" /> struct from its rows.
    /// </summary>
    /// <param name="row1">The first row.</param>
    /// <param name="row2">The second row.</param>
    public Matrix2x2(Vector2 row1, Vector2 row2) : this(row1.X, row1.Y, row2.X, row2.Y)
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
    ///     Gets the entry in row 2, column 1.
    /// </summary>
    public double M21 { get; }

    /// <summary>
    ///     Gets the entry in row 2, column 2.
    /// </summary>
    public double M22 { get; }

    /// <summary>
    ///     Gets the zero matrix.
    /// </summary>
    public static Matrix2x2 Zero => default;

    /// <summary>
    ///     Gets the identity matrix.
    /// </summary>
    public static Matrix2x2 Identity => new(1, 0, 0, 1);

    /// <summary>
    ///     Gets the entry at the specified row-major index (<c>row · 2 + column</c>).
    /// </summary>
    /// <param name="index">The zero-based index between 0 and 3.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is out of range.</exception>
    public double this[int index] => index switch
    {
        0 => M11,
        1 => M12,
        2 => M21,
        3 => M22,
        _ => throw MatrixMath.IndexOutOfRange(nameof(index), index, 4)
    };

    /// <inheritdoc />
    public double this[int row, int column]
    {
        get
        {
            MatrixMath.CheckIndices(row, column, 2);
            return this[row * 2 + column];
        }
    }

    /// <inheritdoc />
    public int RowCount => 2;

    /// <inheritdoc />
    public int ColumnCount => 2;

    /// <inheritdoc />
    public int Dimension => 2;

    /// <inheritdoc />
    public double Determinant => M11 * M22 - M12 * M21;

    /// <inheritdoc />
    public double Trace => M11 + M22;

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
    ///     Creates a matrix that rotates vectors counterclockwise by the specified angle.
    /// </summary>
    /// <param name="angle">The angle in radians.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix2x2 Rotation(double angle)
    {
        var (sin, cos) = Math.SinCos(angle);
        return new Matrix2x2(cos, -sin, sin, cos);
    }

    /// <summary>
    ///     Creates a matrix that scales vectors.
    /// </summary>
    /// <param name="scaleX">The scale factor along X.</param>
    /// <param name="scaleY">The scale factor along Y.</param>
    /// <returns>The scaling matrix.</returns>
    public static Matrix2x2 Scaling(double scaleX, double scaleY) => new(scaleX, 0, 0, scaleY);

    /// <summary>
    ///     Creates a matrix that scales vectors uniformly.
    /// </summary>
    /// <param name="scale">The scale factor.</param>
    /// <returns>The scaling matrix.</returns>
    public static Matrix2x2 Scaling(double scale) => Scaling(scale, scale);

    /// <summary>
    ///     Creates a matrix from any <see cref="IMatrix" /> with two rows and two columns.
    /// </summary>
    /// <param name="matrix">The matrix to copy.</param>
    /// <returns>The copied matrix.</returns>
    /// <exception cref="ArgumentException">The matrix is not 2x2.</exception>
    public static Matrix2x2 FromMatrix(IMatrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        if (matrix.RowCount != 2 || matrix.ColumnCount != 2)
            throw new ArgumentException("The matrix must have 2 rows and 2 columns.", nameof(matrix));
        return new Matrix2x2(matrix[0, 0], matrix[0, 1], matrix[1, 0], matrix[1, 1]);
    }

    /// <summary>
    ///     Returns the transpose.
    /// </summary>
    /// <returns>The transposed matrix.</returns>
    public Matrix2x2 Transpose() => new(M11, M21, M12, M22);

    /// <summary>
    ///     Returns the matrix of cofactors.
    /// </summary>
    /// <returns>The cofactor matrix.</returns>
    public Matrix2x2 Cofactors() => new(M22, -M21, -M12, M11);

    /// <summary>
    ///     Returns the adjugate, i.e. the transposed cofactor matrix.
    /// </summary>
    /// <returns>The adjugate.</returns>
    public Matrix2x2 Adjugate() => new(M22, -M12, -M21, M11);

    /// <summary>
    ///     Returns the inverse.
    /// </summary>
    /// <returns>The inverse matrix.</returns>
    /// <exception cref="InvalidOperationException">The matrix is singular (see <see cref="IsSingular" />).</exception>
    public Matrix2x2 Inverse() => TryInvert(out var inverse) ? inverse : throw MatrixMath.SingularMatrix();

    /// <summary>
    ///     Tries to calculate the inverse.
    /// </summary>
    /// <param name="inverse">The inverse, or <see cref="Zero" /> if the matrix is singular.</param>
    /// <returns><see langword="true" /> if the matrix is invertible; otherwise, <see langword="false" />.</returns>
    public bool TryInvert(out Matrix2x2 inverse)
    {
        if (IsSingular)
        {
            inverse = default;
            return false;
        }

        // Expanding along the first row reuses the minors already computed for the adjugate.
        var adjugate = Adjugate();
        inverse = adjugate / (M11 * adjugate.M11 + M12 * adjugate.M21);
        return true;
    }

    /// <summary>
    ///     Determines whether this matrix equals another matrix within the default tolerances (entry-wise).
    /// </summary>
    /// <param name="other">The other matrix.</param>
    /// <returns><see langword="true" /> if the matrices are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Matrix2x2 other) =>
        ApproximatelyEquals(other, Tolerance.DefaultAbsolute, Tolerance.DefaultRelative);

    /// <summary>
    ///     Determines whether this matrix equals another matrix within the specified tolerances (entry-wise).
    /// </summary>
    /// <param name="other">The other matrix.</param>
    /// <param name="absolute">The absolute tolerance.</param>
    /// <param name="relative">The relative tolerance.</param>
    /// <returns><see langword="true" /> if the matrices are approximately equal; otherwise, <see langword="false" />.</returns>
    public bool ApproximatelyEquals(Matrix2x2 other, double absolute, double relative)
    {
        Span<double> entries = stackalloc double[4];
        Span<double> otherEntries = stackalloc double[4];
        CopyTo(entries);
        other.CopyTo(otherEntries);
        return MatrixMath.ApproximatelyEqual(entries, otherEntries, absolute, relative);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        Span<double> entries = stackalloc double[4];
        CopyTo(entries);
        return MatrixMath.Format(nameof(Matrix2x2), entries, 2, 2);
    }

    /// <summary>
    ///     Copies the entries in row-major order.
    /// </summary>
    internal void CopyTo(Span<double> destination)
    {
        destination[0] = M11;
        destination[1] = M12;
        destination[2] = M21;
        destination[3] = M22;
    }

    private bool Is(MatrixMath.Predicate predicate)
    {
        Span<double> entries = stackalloc double[4];
        CopyTo(entries);
        return MatrixMath.Evaluate(predicate, entries, 2);
    }

    /// <inheritdoc />
    public bool Equals(Matrix2x2 other) =>
        M11.Equals(other.M11) && M12.Equals(other.M12) && M21.Equals(other.M21) && M22.Equals(other.M22);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Matrix2x2 other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(M11, M12, M21, M22);

    /// <summary>
    ///     Adds two matrices.
    /// </summary>
    /// <param name="left">The first matrix.</param>
    /// <param name="right">The second matrix.</param>
    /// <returns>The sum.</returns>
    public static Matrix2x2 operator +(Matrix2x2 left, Matrix2x2 right) =>
        new(left.M11 + right.M11, left.M12 + right.M12, left.M21 + right.M21, left.M22 + right.M22);

    /// <summary>
    ///     Subtracts a matrix from another matrix.
    /// </summary>
    /// <param name="left">The minuend.</param>
    /// <param name="right">The subtrahend.</param>
    /// <returns>The difference.</returns>
    public static Matrix2x2 operator -(Matrix2x2 left, Matrix2x2 right) =>
        new(left.M11 - right.M11, left.M12 - right.M12, left.M21 - right.M21, left.M22 - right.M22);

    /// <summary>
    ///     Negates a matrix.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The negated matrix.</returns>
    public static Matrix2x2 operator -(Matrix2x2 matrix) => new(-matrix.M11, -matrix.M12, -matrix.M21, -matrix.M22);

    /// <summary>
    ///     Multiplies a matrix with a scalar.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="scalar">The scalar.</param>
    /// <returns>The scaled matrix.</returns>
    public static Matrix2x2 operator *(Matrix2x2 matrix, double scalar) =>
        new(matrix.M11 * scalar, matrix.M12 * scalar, matrix.M21 * scalar, matrix.M22 * scalar);

    /// <summary>
    ///     Multiplies a scalar with a matrix.
    /// </summary>
    /// <param name="scalar">The scalar.</param>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The scaled matrix.</returns>
    public static Matrix2x2 operator *(double scalar, Matrix2x2 matrix) => matrix * scalar;

    /// <summary>
    ///     Divides a matrix by a scalar.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="scalar">The scalar.</param>
    /// <returns>The scaled matrix.</returns>
    public static Matrix2x2 operator /(Matrix2x2 matrix, double scalar) =>
        new(matrix.M11 / scalar, matrix.M12 / scalar, matrix.M21 / scalar, matrix.M22 / scalar);

    /// <summary>
    ///     Multiplies two matrices.
    /// </summary>
    /// <param name="left">The left matrix.</param>
    /// <param name="right">The right matrix.</param>
    /// <returns>The product <c>left · right</c>.</returns>
    public static Matrix2x2 operator *(Matrix2x2 left, Matrix2x2 right) =>
        new(left.M11 * right.M11 + left.M12 * right.M21, left.M11 * right.M12 + left.M12 * right.M22,
            left.M21 * right.M11 + left.M22 * right.M21, left.M21 * right.M12 + left.M22 * right.M22);

    /// <summary>
    ///     Transforms a column vector.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="vector">The vector.</param>
    /// <returns>The product <c>matrix · vector</c>.</returns>
    public static Vector2 operator *(Matrix2x2 matrix, Vector2 vector) =>
        new(matrix.M11 * vector.X + matrix.M12 * vector.Y, matrix.M21 * vector.X + matrix.M22 * vector.Y);

    /// <summary>
    ///     Determines whether two matrices are exactly equal (entry-wise <c>==</c>).
    /// </summary>
    /// <param name="left">The first matrix.</param>
    /// <param name="right">The second matrix.</param>
    /// <returns><see langword="true" /> if all entries are equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(Matrix2x2 left, Matrix2x2 right) =>
        left.M11 == right.M11 && left.M12 == right.M12 && left.M21 == right.M21 && left.M22 == right.M22;

    /// <summary>
    ///     Determines whether two matrices differ in at least one entry.
    /// </summary>
    /// <param name="left">The first matrix.</param>
    /// <param name="right">The second matrix.</param>
    /// <returns><see langword="true" /> if any entry differs; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(Matrix2x2 left, Matrix2x2 right) => !(left == right);
}
