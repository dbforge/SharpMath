namespace SharpMath.Geometry;

/// <summary>
///     The result of intersecting a <see cref="LineSegment2D" /> with another segment or a line.
/// </summary>
/// <param name="Kind">
///     How the objects intersect: <see cref="IntersectionKind.None" />, <see cref="IntersectionKind.Point" /> or
///     <see cref="IntersectionKind.Overlap" />.
/// </param>
/// <param name="Point">The intersection point if <paramref name="Kind" /> is <see cref="IntersectionKind.Point" />; otherwise, <see langword="null" />.</param>
/// <param name="Segment">The overlapping segment if <paramref name="Kind" /> is <see cref="IntersectionKind.Overlap" />; otherwise, <see langword="null" />.</param>
public readonly record struct SegmentIntersection2D(IntersectionKind Kind, Point2D? Point, LineSegment2D? Segment);
