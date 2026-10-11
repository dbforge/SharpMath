namespace SharpMath.Geometry;

/// <summary>
///     The result of intersecting two <see cref="Line2D" />s.
/// </summary>
/// <param name="Kind">
///     How the lines intersect: <see cref="IntersectionKind.None" />, <see cref="IntersectionKind.Point" /> or
///     <see cref="IntersectionKind.Coincident" />.
/// </param>
/// <param name="Point">The intersection point if <paramref name="Kind" /> is <see cref="IntersectionKind.Point" />; otherwise, <see langword="null" />.</param>
public readonly record struct LineIntersection2D(IntersectionKind Kind, Point2D? Point);
