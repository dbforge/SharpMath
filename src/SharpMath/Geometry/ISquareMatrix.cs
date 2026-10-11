namespace SharpMath.Geometry;

/// <summary>
///     Represents a read-only square matrix.
/// </summary>
/// <remarks>
///     <see cref="IsIdentity" /> compares with the tolerances described in <see cref="Tolerance" />. The structural
///     predicates (diagonal, triangular, symmetric) treat an entry as zero if it is negligible relative to the largest
///     entry (<see cref="Tolerance.DefaultRelative" />), so they do not depend on the scale of the matrix.
/// </remarks>
public interface ISquareMatrix : IMatrix
{
    /// <summary>
    ///     Gets the number of rows and columns.
    /// </summary>
    int Dimension { get; }

    /// <summary>
    ///     Gets the determinant.
    /// </summary>
    double Determinant { get; }

    /// <summary>
    ///     Gets the trace, i.e. the sum of the diagonal entries.
    /// </summary>
    double Trace { get; }

    /// <summary>
    ///     Gets a value indicating whether the matrix is the identity matrix.
    /// </summary>
    bool IsIdentity { get; }

    /// <summary>
    ///     Gets a value indicating whether all entries outside of the diagonal are zero.
    /// </summary>
    bool IsDiagonal { get; }

    /// <summary>
    ///     Gets a value indicating whether the matrix is upper or lower triangular.
    /// </summary>
    bool IsTriangular { get; }

    /// <summary>
    ///     Gets a value indicating whether the matrix equals its transpose.
    /// </summary>
    bool IsSymmetric { get; }

    /// <summary>
    ///     Gets a value indicating whether the matrix equals its negated transpose.
    /// </summary>
    bool IsSkewSymmetric { get; }

    /// <summary>
    ///     Gets a value indicating whether the matrix multiplied with its transpose is the identity matrix.
    /// </summary>
    bool IsOrthogonal { get; }

    /// <summary>
    ///     Gets a value indicating whether the matrix has no inverse.
    /// </summary>
    /// <remarks>
    ///     The check is scale-aware and numerically robust: an LU decomposition with partial pivoting finds a pivot not
    ///     larger than <c>n · ε · s</c>, where <c>ε</c> is the machine epsilon of <see cref="double" />, <c>s</c> the
    ///     largest absolute entry and <c>n</c> the dimension. Uniformly scaled matrices such as <c>0.001 · I</c> and
    ///     transformations with large translations are therefore regular. <see cref="MatrixMxN" /> uses the same rule.
    /// </remarks>
    bool IsSingular { get; }
}
