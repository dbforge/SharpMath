namespace SharpMath.Geometry;

/// <summary>
///     The result of intersecting two <see cref="Line3D" />s.
/// </summary>
/// <param name="Kind">
///     How the lines intersect: <see cref="IntersectionKind.None" /> (distinct parallel lines),
///     <see cref="IntersectionKind.Point" />, <see cref="IntersectionKind.Coincident" /> or
///     <see cref="IntersectionKind.Skew" />.
/// </param>
/// <param name="Point">The intersection point if <paramref name="Kind" /> is <see cref="IntersectionKind.Point" />; otherwise, <see langword="null" />.</param>
public readonly record struct LineIntersection3D(IntersectionKind Kind, Point3D? Point);
