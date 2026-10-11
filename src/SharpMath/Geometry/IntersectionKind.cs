namespace SharpMath.Geometry;

/// <summary>
///     Describes how two lines or segments intersect.
/// </summary>
public enum IntersectionKind
{
    /// <summary>
    ///     The objects do not intersect (e.g. distinct parallel lines or disjoint segments).
    /// </summary>
    None,

    /// <summary>
    ///     The objects intersect in exactly one point.
    /// </summary>
    Point,

    /// <summary>
    ///     The lines are identical.
    /// </summary>
    Coincident,

    /// <summary>
    ///     The three-dimensional lines are neither parallel nor intersecting.
    /// </summary>
    Skew,

    /// <summary>
    ///     The collinear segments overlap in a segment of positive length.
    /// </summary>
    Overlap
}
