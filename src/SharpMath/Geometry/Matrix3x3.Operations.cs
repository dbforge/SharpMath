namespace SharpMath.Geometry;

public readonly partial struct Matrix3x3
{
    /// <inheritdoc />
    public double Determinant =>
        M11 * (M22 * M33 - M23 * M32) - M12 * (M21 * M33 - M23 * M31) + M13 * (M21 * M32 - M22 * M31);

    /// <summary>
    ///     Returns the adjugate, i.e. the transposed cofactor matrix.
    /// </summary>
    /// <returns>The adjugate.</returns>
    public Matrix3x3 Adjugate() => new(
        M22 * M33 - M23 * M32, M13 * M32 - M12 * M33, M12 * M23 - M13 * M22,
        M23 * M31 - M21 * M33, M11 * M33 - M13 * M31, M13 * M21 - M11 * M23,
        M21 * M32 - M22 * M31, M12 * M31 - M11 * M32, M11 * M22 - M12 * M21);

    /// <summary>
    ///     Creates a matrix that rotates two-dimensional points counterclockwise around the origin in homogeneous
    ///     coordinates.
    /// </summary>
    /// <param name="angle">The angle in radians.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix3x3 Rotation(double angle)
    {
        var (sin, cos) = Math.SinCos(angle);
        return new Matrix3x3(cos, -sin, 0, sin, cos, 0, 0, 0, 1);
    }

    /// <summary>
    ///     Creates a matrix that scales two-dimensional points in homogeneous coordinates.
    /// </summary>
    /// <param name="scaleX">The scale factor along X.</param>
    /// <param name="scaleY">The scale factor along Y.</param>
    /// <returns>The scaling matrix.</returns>
    public static Matrix3x3 Scaling(double scaleX, double scaleY) => new(scaleX, 0, 0, 0, scaleY, 0, 0, 0, 1);

    /// <summary>
    ///     Creates a matrix that scales two-dimensional points uniformly in homogeneous coordinates.
    /// </summary>
    /// <param name="scale">The scale factor.</param>
    /// <returns>The scaling matrix.</returns>
    public static Matrix3x3 Scaling(double scale) => Scaling(scale, scale);

    /// <summary>
    ///     Creates a matrix that translates two-dimensional points in homogeneous coordinates.
    /// </summary>
    /// <param name="x">The translation along X.</param>
    /// <param name="y">The translation along Y.</param>
    /// <returns>The translation matrix.</returns>
    public static Matrix3x3 Translation(double x, double y) => new(1, 0, x, 0, 1, y, 0, 0, 1);

    /// <summary>
    ///     Transforms a two-dimensional vector in homogeneous coordinates (<c>matrix · (x, y, 1)</c>). The result is divided
    ///     by the resulting W component if it is not 1.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="vector">The vector.</param>
    /// <returns>The transformed vector.</returns>
    public static Vector2 operator *(Matrix3x3 matrix, Vector2 vector)
    {
        var x = matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13;
        var y = matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23;
        var w = matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33;
        return w == 1 ? new Vector2(x, y) : new Vector2(x / w, y / w);
    }
}
