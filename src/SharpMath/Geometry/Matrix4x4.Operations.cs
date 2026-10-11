namespace SharpMath.Geometry;

public readonly partial struct Matrix4x4
{
    /// <inheritdoc />
    public double Determinant
    {
        get
        {
            var s0 = M11 * M22 - M21 * M12;
            var s1 = M11 * M23 - M21 * M13;
            var s2 = M11 * M24 - M21 * M14;
            var s3 = M12 * M23 - M22 * M13;
            var s4 = M12 * M24 - M22 * M14;
            var s5 = M13 * M24 - M23 * M14;
            var c5 = M33 * M44 - M43 * M34;
            var c4 = M32 * M44 - M42 * M34;
            var c3 = M32 * M43 - M42 * M33;
            var c2 = M31 * M44 - M41 * M34;
            var c1 = M31 * M43 - M41 * M33;
            var c0 = M31 * M42 - M41 * M32;
            return s0 * c5 - s1 * c4 + s2 * c3 + s3 * c2 - s4 * c1 + s5 * c0;
        }
    }

    /// <summary>
    ///     Returns the adjugate, i.e. the transposed cofactor matrix.
    /// </summary>
    /// <returns>The adjugate.</returns>
    public Matrix4x4 Adjugate()
    {
        // Laplace expansion by 2x2 minors of the upper (s) and lower (c) two rows.
        var s0 = M11 * M22 - M21 * M12;
        var s1 = M11 * M23 - M21 * M13;
        var s2 = M11 * M24 - M21 * M14;
        var s3 = M12 * M23 - M22 * M13;
        var s4 = M12 * M24 - M22 * M14;
        var s5 = M13 * M24 - M23 * M14;
        var c5 = M33 * M44 - M43 * M34;
        var c4 = M32 * M44 - M42 * M34;
        var c3 = M32 * M43 - M42 * M33;
        var c2 = M31 * M44 - M41 * M34;
        var c1 = M31 * M43 - M41 * M33;
        var c0 = M31 * M42 - M41 * M32;

        return new Matrix4x4(
            M22 * c5 - M23 * c4 + M24 * c3, -M12 * c5 + M13 * c4 - M14 * c3,
            M42 * s5 - M43 * s4 + M44 * s3, -M32 * s5 + M33 * s4 - M34 * s3,
            -M21 * c5 + M23 * c2 - M24 * c1, M11 * c5 - M13 * c2 + M14 * c1,
            -M41 * s5 + M43 * s2 - M44 * s1, M31 * s5 - M33 * s2 + M34 * s1,
            M21 * c4 - M22 * c2 + M24 * c0, -M11 * c4 + M12 * c2 - M14 * c0,
            M41 * s4 - M42 * s2 + M44 * s0, -M31 * s4 + M32 * s2 - M34 * s0,
            -M21 * c3 + M22 * c1 - M23 * c0, M11 * c3 - M12 * c1 + M13 * c0,
            -M41 * s3 + M42 * s1 - M43 * s0, M31 * s3 - M32 * s1 + M33 * s0);
    }

    /// <summary>
    ///     Creates a matrix that translates points.
    /// </summary>
    /// <param name="x">The translation along X.</param>
    /// <param name="y">The translation along Y.</param>
    /// <param name="z">The translation along Z.</param>
    /// <returns>The translation matrix.</returns>
    public static Matrix4x4 Translation(double x, double y, double z) =>
        new(1, 0, 0, x, 0, 1, 0, y, 0, 0, 1, z, 0, 0, 0, 1);

    /// <summary>
    ///     Creates a matrix that translates points.
    /// </summary>
    /// <param name="translation">The translation.</param>
    /// <returns>The translation matrix.</returns>
    public static Matrix4x4 Translation(Vector3 translation) =>
        Translation(translation.X, translation.Y, translation.Z);

    /// <summary>
    ///     Creates a matrix that scales points.
    /// </summary>
    /// <param name="scaleX">The scale factor along X.</param>
    /// <param name="scaleY">The scale factor along Y.</param>
    /// <param name="scaleZ">The scale factor along Z.</param>
    /// <returns>The scaling matrix.</returns>
    public static Matrix4x4 Scaling(double scaleX, double scaleY, double scaleZ) =>
        new(scaleX, 0, 0, 0, 0, scaleY, 0, 0, 0, 0, scaleZ, 0, 0, 0, 0, 1);

    /// <summary>
    ///     Creates a matrix that scales points uniformly.
    /// </summary>
    /// <param name="scale">The scale factor.</param>
    /// <returns>The scaling matrix.</returns>
    public static Matrix4x4 Scaling(double scale) => Scaling(scale, scale, scale);

    /// <summary>
    ///     Creates a matrix that rotates points around the X axis (counterclockwise when looking from +X towards the
    ///     origin).
    /// </summary>
    /// <param name="angle">The angle in radians.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix4x4 RotationX(double angle)
    {
        var (sin, cos) = Math.SinCos(angle);
        return new Matrix4x4(1, 0, 0, 0, 0, cos, -sin, 0, 0, sin, cos, 0, 0, 0, 0, 1);
    }

    /// <summary>
    ///     Creates a matrix that rotates points around the Y axis (counterclockwise when looking from +Y towards the
    ///     origin).
    /// </summary>
    /// <param name="angle">The angle in radians.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix4x4 RotationY(double angle)
    {
        var (sin, cos) = Math.SinCos(angle);
        return new Matrix4x4(cos, 0, sin, 0, 0, 1, 0, 0, -sin, 0, cos, 0, 0, 0, 0, 1);
    }

    /// <summary>
    ///     Creates a matrix that rotates points around the Z axis (counterclockwise when looking from +Z towards the
    ///     origin).
    /// </summary>
    /// <param name="angle">The angle in radians.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix4x4 RotationZ(double angle)
    {
        var (sin, cos) = Math.SinCos(angle);
        return new Matrix4x4(cos, -sin, 0, 0, sin, cos, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
    }

    /// <summary>
    ///     Creates a matrix that rotates around Z, then Y, then X (<c>RotationX · RotationY · RotationZ</c>).
    /// </summary>
    /// <param name="angleX">The angle around the X axis in radians.</param>
    /// <param name="angleY">The angle around the Y axis in radians.</param>
    /// <param name="angleZ">The angle around the Z axis in radians.</param>
    /// <returns>The rotation matrix.</returns>
    public static Matrix4x4 Rotation(double angleX, double angleY, double angleZ) =>
        RotationX(angleX) * RotationY(angleY) * RotationZ(angleZ);

    /// <summary>
    ///     Creates a right-handed perspective projection matrix that maps the view frustum to clip space with a depth range
    ///     of [0, 1].
    /// </summary>
    /// <param name="fieldOfView">The vertical field of view in radians, in (0, π).</param>
    /// <param name="aspectRatio">The aspect ratio (width / height). Must be positive.</param>
    /// <param name="nearPlane">The distance to the near plane. Must be positive.</param>
    /// <param name="farPlane">The distance to the far plane. Must be greater than <paramref name="nearPlane" />.</param>
    /// <returns>The projection matrix.</returns>
    /// <exception cref="ArgumentOutOfRangeException">An argument is out of range.</exception>
    public static Matrix4x4 PerspectiveFieldOfView(double fieldOfView, double aspectRatio, double nearPlane,
        double farPlane)
    {
        ProjectionData.Validate(fieldOfView, aspectRatio, nearPlane, farPlane);

        var yScale = 1 / Math.Tan(fieldOfView / 2);
        var xScale = yScale / aspectRatio;
        var range = farPlane / (nearPlane - farPlane);
        return new Matrix4x4(
            xScale, 0, 0, 0,
            0, yScale, 0, 0,
            0, 0, range, range * nearPlane,
            0, 0, -1, 0);
    }

    /// <summary>
    ///     Creates a right-handed perspective projection matrix from projection data (see
    ///     <see cref="PerspectiveFieldOfView(double, double, double, double)" />).
    /// </summary>
    /// <param name="projectionData">The projection data.</param>
    /// <returns>The projection matrix.</returns>
    public static Matrix4x4 PerspectiveFieldOfView(ProjectionData projectionData) =>
        PerspectiveFieldOfView(projectionData.FieldOfView, projectionData.AspectRatio, projectionData.NearPlane,
            projectionData.FarPlane);

    /// <summary>
    ///     Creates a right-handed view matrix for a camera at <paramref name="eye" /> looking at
    ///     <paramref name="target" />. The camera looks along its local −Z axis.
    /// </summary>
    /// <param name="eye">The camera position.</param>
    /// <param name="target">The point the camera looks at.</param>
    /// <param name="up">The approximate up direction; must not be parallel to the viewing direction.</param>
    /// <returns>The view matrix.</returns>
    /// <exception cref="ArgumentException">
    ///     <paramref name="eye" /> equals <paramref name="target" />, or <paramref name="up" /> is zero or parallel to the
    ///     viewing direction.
    /// </exception>
    public static Matrix4x4 LookAt(Vector3 eye, Vector3 target, Vector3 up)
    {
        var (xAxis, yAxis, zAxis) = OrthonormalBasis(eye - target, up, nameof(target));
        return new Matrix4x4(
            xAxis.X, xAxis.Y, xAxis.Z, -xAxis.Dot(eye),
            yAxis.X, yAxis.Y, yAxis.Z, -yAxis.Dot(eye),
            zAxis.X, zAxis.Y, zAxis.Z, -zAxis.Dot(eye),
            0, 0, 0, 1);
    }

    /// <summary>
    ///     Creates a world matrix that places an object at <paramref name="position" />, oriented so that its local −Z axis
    ///     points along <paramref name="forward" /> and its local Y axis approximately along <paramref name="up" />.
    /// </summary>
    /// <param name="position">The position of the object.</param>
    /// <param name="forward">The forward direction of the object.</param>
    /// <param name="up">The approximate up direction; must not be parallel to <paramref name="forward" />.</param>
    /// <returns>The world matrix.</returns>
    /// <exception cref="ArgumentException">
    ///     <paramref name="forward" /> is zero, or <paramref name="up" /> is zero or parallel to it.
    /// </exception>
    public static Matrix4x4 World(Vector3 position, Vector3 forward, Vector3 up)
    {
        var (xAxis, yAxis, zAxis) = OrthonormalBasis(-forward, up, nameof(forward));
        return new Matrix4x4(
            xAxis.X, yAxis.X, zAxis.X, position.X,
            xAxis.Y, yAxis.Y, zAxis.Y, position.Y,
            xAxis.Z, yAxis.Z, zAxis.Z, position.Z,
            0, 0, 0, 1);
    }

    /// <summary>
    ///     Transforms a three-dimensional vector in homogeneous coordinates (<c>matrix · (x, y, z, 1)</c>). The result is
    ///     divided by the resulting W component if it is not 1 (perspective divide).
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="vector">The vector.</param>
    /// <returns>The transformed vector.</returns>
    public static Vector3 operator *(Matrix4x4 matrix, Vector3 vector)
    {
        var x = matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z + matrix.M14;
        var y = matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z + matrix.M24;
        var z = matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z + matrix.M34;
        var w = matrix.M41 * vector.X + matrix.M42 * vector.Y + matrix.M43 * vector.Z + matrix.M44;
        return w == 1 ? new Vector3(x, y, z) : new Vector3(x / w, y / w, z / w);
    }

    private static (Vector3 XAxis, Vector3 YAxis, Vector3 ZAxis) OrthonormalBasis(Vector3 backward, Vector3 up,
        string directionParameter)
    {
        if (backward.LengthSquared == 0)
            throw new ArgumentException("The viewing direction must not be the zero vector.", directionParameter);
        var zAxis = backward.Normalize();
        var x = up.Cross(zAxis);
        if (up.LengthSquared == 0 || x.IsZero || up.IsParallelTo(zAxis))
            throw new ArgumentException("The up vector must not be zero or parallel to the viewing direction.",
                nameof(up));
        var xAxis = x.Normalize();
        return (xAxis, zAxis.Cross(xAxis), zAxis);
    }
}
