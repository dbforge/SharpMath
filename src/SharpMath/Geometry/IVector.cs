namespace SharpMath.Geometry;

/// <summary>
///     Represents a read-only vector of fixed dimension.
/// </summary>
public interface IVector
{
    /// <summary>
    ///     Gets the number of components of the vector.
    /// </summary>
    int Dimension { get; }

    /// <summary>
    ///     Gets the component at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the component.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> is outside of [0, <see cref="Dimension" />).</exception>
    double this[int index] { get; }

    /// <summary>
    ///     Gets the Euclidean length of the vector.
    /// </summary>
    double Length { get; }

    /// <summary>
    ///     Gets the squared Euclidean length of the vector.
    /// </summary>
    double LengthSquared { get; }

    /// <summary>
    ///     Gets a value indicating whether all components are zero within <see cref="Tolerance.DefaultAbsolute" />.
    /// </summary>
    bool IsZero { get; }

    /// <summary>
    ///     Gets a value indicating whether the vector has a length of 1 within the default tolerances.
    /// </summary>
    bool IsNormalized { get; }
}
