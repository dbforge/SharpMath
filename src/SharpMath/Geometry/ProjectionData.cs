namespace SharpMath.Geometry;

/// <summary>
///     Describes a perspective projection onto a viewport.
/// </summary>
public readonly struct ProjectionData : IEquatable<ProjectionData>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProjectionData" /> struct.
    /// </summary>
    /// <param name="width">The width of the viewport, e.g. in pixels. Must not be negative.</param>
    /// <param name="height">The height of the viewport, e.g. in pixels. Must not be negative.</param>
    /// <param name="aspectRatio">The aspect ratio (width / height) used for the projection. Must be positive.</param>
    /// <param name="fieldOfView">The vertical field of view in radians, in (0, π).</param>
    /// <param name="nearPlane">The distance to the near plane. Must be positive.</param>
    /// <param name="farPlane">The distance to the far plane. Must be greater than <paramref name="nearPlane" />.</param>
    /// <exception cref="ArgumentOutOfRangeException">An argument is out of range.</exception>
    public ProjectionData(double width, double height, double aspectRatio, double fieldOfView, double nearPlane,
        double farPlane)
    {
        if (!(width >= 0) || double.IsPositiveInfinity(width))
            throw new ArgumentOutOfRangeException(nameof(width), width, "The width must be a finite, non-negative number.");
        if (!(height >= 0) || double.IsPositiveInfinity(height))
            throw new ArgumentOutOfRangeException(nameof(height), height,
                "The height must be a finite, non-negative number.");
        Validate(fieldOfView, aspectRatio, nearPlane, farPlane);

        Width = width;
        Height = height;
        AspectRatio = aspectRatio;
        FieldOfView = fieldOfView;
        NearPlane = nearPlane;
        FarPlane = farPlane;
    }

    /// <summary>
    ///     Gets the default projection data: a 100x100 viewport, an aspect ratio of 16:9, a field of view of 60° and the
    ///     planes 1 and 100.
    /// </summary>
    public static ProjectionData Default => new(100, 100, 16.0 / 9.0, Math.PI / 3, 1, 100);

    /// <summary>
    ///     Gets the width of the viewport.
    /// </summary>
    public double Width { get; }

    /// <summary>
    ///     Gets the height of the viewport.
    /// </summary>
    public double Height { get; }

    /// <summary>
    ///     Gets the aspect ratio (width / height) used for the projection.
    /// </summary>
    public double AspectRatio { get; }

    /// <summary>
    ///     Gets the vertical field of view in radians.
    /// </summary>
    public double FieldOfView { get; }

    /// <summary>
    ///     Gets the distance to the near plane.
    /// </summary>
    public double NearPlane { get; }

    /// <summary>
    ///     Gets the distance to the far plane.
    /// </summary>
    public double FarPlane { get; }

    /// <summary>
    ///     Gets the distance between the near and the far plane.
    /// </summary>
    public double Depth => FarPlane - NearPlane;

    /// <inheritdoc />
    public bool Equals(ProjectionData other) =>
        Width.Equals(other.Width) && Height.Equals(other.Height) && AspectRatio.Equals(other.AspectRatio) &&
        FieldOfView.Equals(other.FieldOfView) && NearPlane.Equals(other.NearPlane) && FarPlane.Equals(other.FarPlane);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ProjectionData other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Width, Height, AspectRatio, FieldOfView, NearPlane, FarPlane);

    /// <summary>
    ///     Determines whether two instances are equal.
    /// </summary>
    /// <param name="left">The first instance.</param>
    /// <param name="right">The second instance.</param>
    /// <returns><see langword="true" /> if all values are equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(ProjectionData left, ProjectionData right) =>
        left.Width == right.Width && left.Height == right.Height && left.AspectRatio == right.AspectRatio &&
        left.FieldOfView == right.FieldOfView && left.NearPlane == right.NearPlane && left.FarPlane == right.FarPlane;

    /// <summary>
    ///     Determines whether two instances differ.
    /// </summary>
    /// <param name="left">The first instance.</param>
    /// <param name="right">The second instance.</param>
    /// <returns><see langword="true" /> if any value differs; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(ProjectionData left, ProjectionData right) => !(left == right);

    internal static void Validate(double fieldOfView, double aspectRatio, double nearPlane, double farPlane)
    {
        if (!(fieldOfView > 0 && fieldOfView < Math.PI))
            throw new ArgumentOutOfRangeException(nameof(fieldOfView), fieldOfView,
                "The field of view must be greater than 0 and less than π (180°).");
        if (!(aspectRatio > 0) || double.IsPositiveInfinity(aspectRatio))
            throw new ArgumentOutOfRangeException(nameof(aspectRatio), aspectRatio,
                "The aspect ratio must be a finite, positive number.");
        if (!(nearPlane > 0) || double.IsPositiveInfinity(nearPlane))
            throw new ArgumentOutOfRangeException(nameof(nearPlane), nearPlane,
                "The near plane must be a finite, positive distance.");
        if (!(farPlane > nearPlane) || double.IsPositiveInfinity(farPlane))
            throw new ArgumentOutOfRangeException(nameof(farPlane), farPlane,
                "The far plane must be finite and farther away than the near plane.");
    }
}
