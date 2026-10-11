using System.Diagnostics.CodeAnalysis;

namespace SharpMath.Geometry;

/// <summary>
///     Represents an immutable matrix of arbitrary size.
/// </summary>
/// <remarks>
///     Determinant, inverse and solving use an LU decomposition with partial pivoting. A matrix is treated as singular if
///     a pivot is not larger than <c>n · ε · s</c>, where <c>ε</c> is the machine epsilon of <see cref="double" /> and
///     <c>s</c> the largest absolute entry; the fixed-size matrices use the same rule for <c>IsSingular</c>. Equality is exact; use <see cref="ApproximatelyEquals(MatrixMxN)" /> for
///     tolerance-based comparisons.
/// </remarks>
public sealed class MatrixMxN : IMatrix, IEquatable<MatrixMxN>
{
    private readonly double[] _values;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MatrixMxN" /> class by copying a two-dimensional array.
    /// </summary>
    /// <param name="values">The entries, indexed as <c>[row, column]</c>.</param>
    /// <exception cref="ArgumentException"><paramref name="values" /> has no rows or no columns.</exception>
    public MatrixMxN(double[,] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        RowCount = values.GetLength(0);
        ColumnCount = values.GetLength(1);
        if (RowCount == 0 || ColumnCount == 0)
            throw new ArgumentException("A matrix needs at least one row and one column.", nameof(values));

        _values = new double[RowCount * ColumnCount];
        for (var row = 0; row < RowCount; row++)
        for (var column = 0; column < ColumnCount; column++)
            _values[row * ColumnCount + column] = values[row, column];
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MatrixMxN" /> class from entries in row-major order.
    /// </summary>
    /// <param name="rowCount">The number of rows. Must be positive.</param>
    /// <param name="columnCount">The number of columns. Must be positive.</param>
    /// <param name="rowMajorValues">The entries in row-major order; exactly <c>rowCount · columnCount</c> values.</param>
    /// <exception cref="ArgumentOutOfRangeException">A count is not positive.</exception>
    /// <exception cref="ArgumentException">The number of values does not match the size.</exception>
    public MatrixMxN(int rowCount, int columnCount, ReadOnlySpan<double> rowMajorValues)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rowCount);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(columnCount);
        if (rowMajorValues.Length != rowCount * columnCount)
            throw new ArgumentException($"Expected {rowCount * columnCount} values.", nameof(rowMajorValues));

        RowCount = rowCount;
        ColumnCount = columnCount;
        _values = rowMajorValues.ToArray();
    }

    private MatrixMxN(int rowCount, int columnCount, double[] values)
    {
        RowCount = rowCount;
        ColumnCount = columnCount;
        _values = values;
    }

    /// <inheritdoc />
    public int RowCount { get; }

    /// <inheritdoc />
    public int ColumnCount { get; }

    /// <summary>
    ///     Gets a value indicating whether the matrix has as many rows as columns.
    /// </summary>
    public bool IsSquare => RowCount == ColumnCount;

    /// <inheritdoc />
    public double this[int row, int column]
    {
        get
        {
            if ((uint) row >= (uint) RowCount)
                throw MatrixMath.IndexOutOfRange(nameof(row), row, RowCount);
            if ((uint) column >= (uint) ColumnCount)
                throw MatrixMath.IndexOutOfRange(nameof(column), column, ColumnCount);
            return _values[row * ColumnCount + column];
        }
    }

    /// <summary>
    ///     Creates a zero matrix.
    /// </summary>
    /// <param name="rowCount">The number of rows. Must be positive.</param>
    /// <param name="columnCount">The number of columns. Must be positive.</param>
    /// <returns>The zero matrix.</returns>
    public static MatrixMxN Zero(int rowCount, int columnCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rowCount);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(columnCount);
        return new MatrixMxN(rowCount, columnCount, new double[rowCount * columnCount]);
    }

    /// <summary>
    ///     Creates an identity matrix.
    /// </summary>
    /// <param name="dimension">The number of rows and columns. Must be positive.</param>
    /// <returns>The identity matrix.</returns>
    public static MatrixMxN Identity(int dimension)
    {
        var identity = Zero(dimension, dimension);
        for (var i = 0; i < dimension; i++)
            identity._values[i * dimension + i] = 1;
        return identity;
    }

    /// <summary>
    ///     Creates a matrix by copying any <see cref="IMatrix" />, e.g. a <see cref="Matrix4x4" />.
    /// </summary>
    /// <param name="matrix">The matrix to copy.</param>
    /// <returns>The copied matrix.</returns>
    public static MatrixMxN FromMatrix(IMatrix matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        var values = new double[matrix.RowCount * matrix.ColumnCount];
        for (var row = 0; row < matrix.RowCount; row++)
        for (var column = 0; column < matrix.ColumnCount; column++)
            values[row * matrix.ColumnCount + column] = matrix[row, column];
        return new MatrixMxN(matrix.RowCount, matrix.ColumnCount, values);
    }

    /// <summary>
    ///     Copies the entries into a new two-dimensional array.
    /// </summary>
    /// <returns>The entries, indexed as <c>[row, column]</c>.</returns>
    public double[,] ToArray()
    {
        var array = new double[RowCount, ColumnCount];
        for (var row = 0; row < RowCount; row++)
        for (var column = 0; column < ColumnCount; column++)
            array[row, column] = _values[row * ColumnCount + column];
        return array;
    }

    /// <summary>
    ///     Returns the transpose.
    /// </summary>
    /// <returns>The transposed matrix.</returns>
    public MatrixMxN Transpose()
    {
        var values = new double[_values.Length];
        for (var row = 0; row < RowCount; row++)
        for (var column = 0; column < ColumnCount; column++)
            values[column * RowCount + row] = _values[row * ColumnCount + column];
        return new MatrixMxN(ColumnCount, RowCount, values);
    }

    /// <summary>
    ///     Calculates the determinant.
    /// </summary>
    /// <returns>The determinant, computed as the signed product of the LU pivots.</returns>
    /// <exception cref="InvalidOperationException">The matrix is not square.</exception>
    public double Determinant()
    {
        EnsureSquare();
        return LuDecomposition.Decompose(RowCount, _values).Determinant;
    }

    /// <summary>
    ///     Returns the inverse.
    /// </summary>
    /// <returns>The inverse matrix.</returns>
    /// <exception cref="InvalidOperationException">The matrix is not square or singular.</exception>
    public MatrixMxN Inverse() => TryInvert(out var inverse) ? inverse : throw MatrixMath.SingularMatrix();

    /// <summary>
    ///     Tries to calculate the inverse.
    /// </summary>
    /// <param name="inverse">The inverse, or <see langword="null" /> if the matrix is singular.</param>
    /// <returns><see langword="true" /> if the matrix is invertible; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">The matrix is not square.</exception>
    public bool TryInvert([NotNullWhen(true)] out MatrixMxN? inverse)
    {
        EnsureSquare();
        var lu = LuDecomposition.Decompose(RowCount, _values);
        if (lu.IsSingular)
        {
            inverse = null;
            return false;
        }

        var n = RowCount;
        var values = new double[n * n];
        var column = new double[n];
        for (var j = 0; j < n; j++)
        {
            Array.Clear(column);
            column[j] = 1;
            lu.Solve(column);
            for (var i = 0; i < n; i++)
                values[i * n + j] = column[i];
        }

        inverse = new MatrixMxN(n, n, values);
        return true;
    }

    /// <summary>
    ///     Solves the linear system <c>this · x = rightHandSide</c>.
    /// </summary>
    /// <param name="rightHandSide">The right-hand side; one value per row.</param>
    /// <returns>The solution <c>x</c>.</returns>
    /// <exception cref="InvalidOperationException">The matrix is not square or singular.</exception>
    /// <exception cref="ArgumentException">The length of <paramref name="rightHandSide" /> does not match.</exception>
    public double[] Solve(ReadOnlySpan<double> rightHandSide) =>
        TrySolve(rightHandSide, out var solution) ? solution : throw MatrixMath.SingularMatrix();

    /// <summary>
    ///     Solves the linear system <c>this · X = rightHandSide</c> for every column of the right-hand side.
    /// </summary>
    /// <param name="rightHandSide">The right-hand side with as many rows as this matrix.</param>
    /// <returns>The solution <c>X</c>.</returns>
    /// <exception cref="InvalidOperationException">The matrix is not square or singular.</exception>
    /// <exception cref="ArgumentException">The row count of <paramref name="rightHandSide" /> does not match.</exception>
    public MatrixMxN Solve(MatrixMxN rightHandSide)
    {
        ArgumentNullException.ThrowIfNull(rightHandSide);
        EnsureSquare();
        if (rightHandSide.RowCount != RowCount)
            throw new ArgumentException("The right-hand side must have as many rows as the matrix.",
                nameof(rightHandSide));

        var lu = LuDecomposition.Decompose(RowCount, _values);
        if (lu.IsSingular)
            throw MatrixMath.SingularMatrix();

        int n = RowCount, m = rightHandSide.ColumnCount;
        var values = new double[n * m];
        var column = new double[n];
        for (var j = 0; j < m; j++)
        {
            for (var i = 0; i < n; i++)
                column[i] = rightHandSide._values[i * m + j];
            lu.Solve(column);
            for (var i = 0; i < n; i++)
                values[i * m + j] = column[i];
        }

        return new MatrixMxN(n, m, values);
    }

    /// <summary>
    ///     Calculates the rank, i.e. the number of linearly independent rows.
    /// </summary>
    /// <returns>The rank.</returns>
    public int Rank()
    {
        int rows = RowCount, columns = ColumnCount;
        var a = (double[]) _values.Clone();
        var threshold = MatrixMath.SingularPivotThreshold(Math.Max(rows, columns), MatrixMath.MaxAbs(_values));
        var rank = 0;
        for (var column = 0; column < columns && rank < rows; column++)
        {
            var pivotRow = rank;
            for (var row = rank + 1; row < rows; row++)
                if (Math.Abs(a[row * columns + column]) > Math.Abs(a[pivotRow * columns + column]))
                    pivotRow = row;
            if (Math.Abs(a[pivotRow * columns + column]) <= threshold)
                continue;

            LuDecomposition.SwapRows(a, columns, pivotRow, rank);
            for (var row = rank + 1; row < rows; row++)
            {
                var factor = a[row * columns + column] / a[rank * columns + column];
                for (var k = column; k < columns; k++)
                    a[row * columns + k] -= factor * a[rank * columns + k];
            }

            rank++;
        }

        return rank;
    }

    /// <summary>
    ///     Determines whether this matrix equals another matrix within the default tolerances (entry-wise).
    /// </summary>
    /// <param name="other">The other matrix.</param>
    /// <returns>
    ///     <see langword="true" /> if both matrices have the same size and approximately equal entries; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    public bool ApproximatelyEquals(MatrixMxN other) =>
        ApproximatelyEquals(other, Tolerance.DefaultAbsolute, Tolerance.DefaultRelative);

    /// <summary>
    ///     Determines whether this matrix equals another matrix within the specified tolerances (entry-wise).
    /// </summary>
    /// <param name="other">The other matrix.</param>
    /// <param name="absolute">The absolute tolerance.</param>
    /// <param name="relative">The relative tolerance.</param>
    /// <returns>
    ///     <see langword="true" /> if both matrices have the same size and approximately equal entries; otherwise,
    ///     <see langword="false" />.
    /// </returns>
    public bool ApproximatelyEquals(MatrixMxN other, double absolute, double relative)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (RowCount != other.RowCount || ColumnCount != other.ColumnCount)
            return false;
        for (var i = 0; i < _values.Length; i++)
            if (!Tolerance.AreEqual(_values[i], other._values[i], absolute, relative))
                return false;
        return true;
    }

    /// <inheritdoc />
    public override string ToString() => MatrixMath.Format(nameof(MatrixMxN), _values, RowCount, ColumnCount);

    /// <inheritdoc />
    public bool Equals(MatrixMxN? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return RowCount == other.RowCount && ColumnCount == other.ColumnCount &&
               _values.AsSpan().SequenceEqual(other._values);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as MatrixMxN);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(RowCount);
        hash.Add(ColumnCount);
        foreach (var value in _values)
            hash.Add(value);
        return hash.ToHashCode();
    }

    /// <summary>
    ///     Adds two matrices of the same size.
    /// </summary>
    /// <param name="left">The first matrix.</param>
    /// <param name="right">The second matrix.</param>
    /// <returns>The sum.</returns>
    /// <exception cref="ArgumentException">The sizes differ.</exception>
    public static MatrixMxN operator +(MatrixMxN left, MatrixMxN right) => Combine(left, right, static (a, b) => a + b);

    /// <summary>
    ///     Subtracts a matrix from another matrix of the same size.
    /// </summary>
    /// <param name="left">The minuend.</param>
    /// <param name="right">The subtrahend.</param>
    /// <returns>The difference.</returns>
    /// <exception cref="ArgumentException">The sizes differ.</exception>
    public static MatrixMxN operator -(MatrixMxN left, MatrixMxN right) => Combine(left, right, static (a, b) => a - b);

    /// <summary>
    ///     Negates a matrix.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The negated matrix.</returns>
    public static MatrixMxN operator -(MatrixMxN matrix) => matrix * -1;

    /// <summary>
    ///     Multiplies a matrix with a scalar.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="scalar">The scalar.</param>
    /// <returns>The scaled matrix.</returns>
    public static MatrixMxN operator *(MatrixMxN matrix, double scalar)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        var values = new double[matrix._values.Length];
        for (var i = 0; i < values.Length; i++)
            values[i] = matrix._values[i] * scalar;
        return new MatrixMxN(matrix.RowCount, matrix.ColumnCount, values);
    }

    /// <summary>
    ///     Multiplies a scalar with a matrix.
    /// </summary>
    /// <param name="scalar">The scalar.</param>
    /// <param name="matrix">The matrix.</param>
    /// <returns>The scaled matrix.</returns>
    public static MatrixMxN operator *(double scalar, MatrixMxN matrix) => matrix * scalar;

    /// <summary>
    ///     Divides a matrix by a scalar.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="scalar">The scalar.</param>
    /// <returns>The scaled matrix.</returns>
    public static MatrixMxN operator /(MatrixMxN matrix, double scalar)
    {
        ArgumentNullException.ThrowIfNull(matrix);
        var values = new double[matrix._values.Length];
        for (var i = 0; i < values.Length; i++)
            values[i] = matrix._values[i] / scalar;
        return new MatrixMxN(matrix.RowCount, matrix.ColumnCount, values);
    }

    /// <summary>
    ///     Multiplies two matrices.
    /// </summary>
    /// <param name="left">The left matrix.</param>
    /// <param name="right">The right matrix; must have as many rows as <paramref name="left" /> has columns.</param>
    /// <returns>The product <c>left · right</c>.</returns>
    /// <exception cref="ArgumentException">The sizes are incompatible.</exception>
    public static MatrixMxN operator *(MatrixMxN left, MatrixMxN right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        if (left.ColumnCount != right.RowCount)
            throw new ArgumentException(
                "The column count of the left matrix must match the row count of the right matrix.", nameof(right));

        int rows = left.RowCount, inner = left.ColumnCount, columns = right.ColumnCount;
        var values = new double[rows * columns];
        for (var row = 0; row < rows; row++)
        for (var k = 0; k < inner; k++)
        {
            var factor = left._values[row * inner + k];
            if (factor == 0)
                continue;
            for (var column = 0; column < columns; column++)
                values[row * columns + column] += factor * right._values[k * columns + column];
        }

        return new MatrixMxN(rows, columns, values);
    }

    /// <summary>
    ///     Determines whether two matrices are exactly equal.
    /// </summary>
    /// <param name="left">The first matrix.</param>
    /// <param name="right">The second matrix.</param>
    /// <returns><see langword="true" /> if both are <see langword="null" /> or equal; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(MatrixMxN? left, MatrixMxN? right)
    {
        if (left is null || right is null)
            return left is null && right is null;
        if (left.RowCount != right.RowCount || left.ColumnCount != right.ColumnCount)
            return false;
        for (var i = 0; i < left._values.Length; i++)
            if (left._values[i] != right._values[i])
                return false;
        return true;
    }

    /// <summary>
    ///     Determines whether two matrices differ.
    /// </summary>
    /// <param name="left">The first matrix.</param>
    /// <param name="right">The second matrix.</param>
    /// <returns><see langword="true" /> if the matrices differ; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(MatrixMxN? left, MatrixMxN? right) => !(left == right);

    /// <summary>
    ///     Creates a matrix that takes ownership of a row-major array without copying it.
    /// </summary>
    internal static MatrixMxN WrapRowMajor(int rowCount, int columnCount, double[] values) =>
        new(rowCount, columnCount, values);

    internal bool TrySolve(ReadOnlySpan<double> rightHandSide,
        [NotNullWhen(true)] out double[]? solution)
    {
        EnsureSquare();
        if (rightHandSide.Length != RowCount)
            throw new ArgumentException("The right-hand side must have one value per row.", nameof(rightHandSide));

        var lu = LuDecomposition.Decompose(RowCount, _values);
        if (lu.IsSingular)
        {
            solution = null;
            return false;
        }

        solution = rightHandSide.ToArray();
        lu.Solve(solution);
        return true;
    }

    private static MatrixMxN Combine(MatrixMxN left, MatrixMxN right, Func<double, double, double> operation)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        if (left.RowCount != right.RowCount || left.ColumnCount != right.ColumnCount)
            throw new ArgumentException("The matrices must have the same size.", nameof(right));

        var values = new double[left._values.Length];
        for (var i = 0; i < values.Length; i++)
            values[i] = operation(left._values[i], right._values[i]);
        return new MatrixMxN(left.RowCount, left.ColumnCount, values);
    }

    private void EnsureSquare()
    {
        if (!IsSquare)
            throw new InvalidOperationException("The operation requires a square matrix.");
    }

    /// <summary>
    ///     An LU decomposition with partial pivoting (<c>P · A = L · U</c>), stored compactly.
    /// </summary>
    private sealed class LuDecomposition
    {
        private readonly double[] _lu;
        private readonly int _n;
        private readonly int[] _permutation;
        private readonly double[] _scratch;

        private LuDecomposition(int n, double[] lu, int[] permutation, double determinant, bool isSingular)
        {
            _n = n;
            _lu = lu;
            _permutation = permutation;
            _scratch = new double[n];
            Determinant = determinant;
            IsSingular = isSingular;
        }

        /// <summary>
        ///     Gets a value indicating whether a pivot was not larger than
        ///     <see cref="MatrixMath.SingularPivotThreshold" />.
        /// </summary>
        public bool IsSingular { get; }

        public double Determinant { get; }

        public static LuDecomposition Decompose(int n, double[] values)
        {
            var lu = (double[]) values.Clone();
            var permutation = new int[n];
            for (var i = 0; i < n; i++)
                permutation[i] = i;

            var threshold = MatrixMath.SingularPivotThreshold(n, MatrixMath.MaxAbs(values));
            var isSingular = !(threshold > 0);
            var determinant = 1.0;
            for (var k = 0; k < n; k++)
            {
                var pivotRow = k;
                for (var row = k + 1; row < n; row++)
                    if (Math.Abs(lu[row * n + k]) > Math.Abs(lu[pivotRow * n + k]))
                        pivotRow = row;

                if (pivotRow != k)
                {
                    SwapRows(lu, n, pivotRow, k);
                    (permutation[pivotRow], permutation[k]) = (permutation[k], permutation[pivotRow]);
                    determinant = -determinant;
                }

                // Tiny pivots make the matrix numerically singular, but the determinant is still their product.
                var pivot = lu[k * n + k];
                determinant *= pivot;
                isSingular |= !(Math.Abs(pivot) > threshold);
                if (pivot == 0)
                    return new LuDecomposition(n, lu, permutation, 0, true);

                for (var row = k + 1; row < n; row++)
                {
                    var factor = lu[row * n + k] /= pivot;
                    for (var column = k + 1; column < n; column++)
                        lu[row * n + column] -= factor * lu[k * n + column];
                }
            }

            return new LuDecomposition(n, lu, permutation, determinant, isSingular);
        }

        public static void SwapRows(double[] values, int columns, int first, int second)
        {
            if (first == second)
                return;
            for (var column = 0; column < columns; column++)
                (values[first * columns + column], values[second * columns + column]) =
                    (values[second * columns + column], values[first * columns + column]);
        }

        /// <summary>
        ///     Overwrites <paramref name="b" /> with the solution of <c>A · x = b</c>.
        /// </summary>
        public void Solve(Span<double> b)
        {
            var permuted = _scratch.AsSpan();
            for (var i = 0; i < _n; i++)
                permuted[i] = b[_permutation[i]];

            // Forward substitution with the unit lower triangle.
            for (var i = 0; i < _n; i++)
            {
                var sum = permuted[i];
                for (var j = 0; j < i; j++)
                    sum -= _lu[i * _n + j] * permuted[j];
                permuted[i] = sum;
            }

            // Back substitution with the upper triangle.
            for (var i = _n - 1; i >= 0; i--)
            {
                var sum = permuted[i];
                for (var j = i + 1; j < _n; j++)
                    sum -= _lu[i * _n + j] * permuted[j];
                permuted[i] = sum / _lu[i * _n + i];
            }

            permuted.CopyTo(b);
        }
    }
}
