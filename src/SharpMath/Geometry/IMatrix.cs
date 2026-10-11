namespace SharpMath.Geometry;

/// <summary>
///     Represents a read-only matrix.
/// </summary>
public interface IMatrix
{
    /// <summary>
    ///     Gets the number of rows.
    /// </summary>
    int RowCount { get; }

    /// <summary>
    ///     Gets the number of columns.
    /// </summary>
    int ColumnCount { get; }

    /// <summary>
    ///     Gets the entry at the specified row and column.
    /// </summary>
    /// <param name="row">The zero-based row index.</param>
    /// <param name="column">The zero-based column index.</param>
    /// <exception cref="ArgumentOutOfRangeException">An index is out of range.</exception>
    double this[int row, int column] { get; }
}
