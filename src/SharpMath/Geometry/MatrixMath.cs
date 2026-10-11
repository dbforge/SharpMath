using System.Globalization;
using System.Text;

namespace SharpMath.Geometry;

/// <summary>
///     Shared helpers for the matrix types. They work on row-major spans, so the fixed-size matrices can copy their
///     entries to the stack and the checks run without boxing or allocations.
/// </summary>
internal static class MatrixMath
{
    /// <summary>
    ///     The machine epsilon of <see cref="double" /> (2⁻⁵²).
    /// </summary>
    public const double MachineEpsilon = 2.220446049250313e-16;

    public enum Predicate
    {
        Identity,
        Diagonal,
        Triangular,
        Symmetric,
        SkewSymmetric,
        Singular
    }

    public static bool Evaluate(Predicate predicate, ReadOnlySpan<double> entries, int dimension) => predicate switch
    {
        Predicate.Identity => IsIdentity(entries, dimension),
        Predicate.Diagonal => IsDiagonal(entries, dimension),
        Predicate.Triangular => IsTriangular(entries, dimension),
        Predicate.Symmetric => IsSymmetric(entries, dimension, -1),
        Predicate.SkewSymmetric => IsSymmetric(entries, dimension, 1),
        _ => IsSingular(entries, dimension)
    };

    public static double MaxAbs(ReadOnlySpan<double> values)
    {
        var max = 0.0;
        foreach (var value in values)
            max = Math.Max(max, Math.Abs(value));
        return max;
    }

    /// <summary>
    ///     The pivot size below which a matrix counts as singular: <c>n · ε · s</c>, where <c>s</c> is the largest
    ///     absolute entry. This is the usual backward-stable rank test of an LU decomposition with partial pivoting.
    /// </summary>
    public static double SingularPivotThreshold(int dimension, double scale) => dimension * MachineEpsilon * scale;

    /// <summary>
    ///     Determines singularity with an LU decomposition with partial pivoting (on a stack copy).
    /// </summary>
    public static bool IsSingular(ReadOnlySpan<double> entries, int dimension)
    {
        var threshold = SingularPivotThreshold(dimension, MaxAbs(entries));
        if (!(threshold > 0))
            return true;

        Span<double> a = stackalloc double[entries.Length];
        entries.CopyTo(a);
        for (var k = 0; k < dimension; k++)
        {
            var pivotRow = k;
            for (var row = k + 1; row < dimension; row++)
                if (Math.Abs(a[row * dimension + k]) > Math.Abs(a[pivotRow * dimension + k]))
                    pivotRow = row;

            var pivot = a[pivotRow * dimension + k];
            if (!(Math.Abs(pivot) > threshold))
                return true;

            if (pivotRow != k)
                for (var column = k; column < dimension; column++)
                    (a[k * dimension + column], a[pivotRow * dimension + column]) =
                        (a[pivotRow * dimension + column], a[k * dimension + column]);

            for (var row = k + 1; row < dimension; row++)
            {
                var factor = a[row * dimension + k] / pivot;
                for (var column = k + 1; column < dimension; column++)
                    a[row * dimension + column] -= factor * a[k * dimension + column];
            }
        }

        return false;
    }

    public static bool IsIdentity(ReadOnlySpan<double> entries, int dimension)
    {
        for (var row = 0; row < dimension; row++)
        for (var column = 0; column < dimension; column++)
            if (!Tolerance.AreEqual(entries[row * dimension + column], row == column ? 1 : 0))
                return false;
        return true;
    }

    // Structural checks compare entries relative to the largest entry only, so they do not depend on the scale.
    private static bool IsNegligible(double value, double scale) => Math.Abs(value) <= Tolerance.DefaultRelative * scale;

    private static bool IsDiagonal(ReadOnlySpan<double> entries, int dimension)
    {
        var scale = MaxAbs(entries);
        for (var row = 0; row < dimension; row++)
        for (var column = 0; column < dimension; column++)
            if (row != column && !IsNegligible(entries[row * dimension + column], scale))
                return false;
        return true;
    }

    private static bool IsTriangular(ReadOnlySpan<double> entries, int dimension)
    {
        var scale = MaxAbs(entries);
        bool upper = true, lower = true;
        for (var row = 0; row < dimension; row++)
        for (var column = 0; column < dimension; column++)
        {
            if (row == column || IsNegligible(entries[row * dimension + column], scale))
                continue;
            if (row > column)
                upper = false;
            else
                lower = false;
        }

        return upper || lower;
    }

    /// <summary>
    ///     Checks <c>a[i,j] = −sign · a[j,i]</c>: <paramref name="sign" /> −1 tests symmetry, +1 skew symmetry.
    /// </summary>
    private static bool IsSymmetric(ReadOnlySpan<double> entries, int dimension, int sign)
    {
        var scale = MaxAbs(entries);
        for (var row = 0; row < dimension; row++)
        for (var column = row; column < dimension; column++)
            if (!IsNegligible(entries[row * dimension + column] + sign * entries[column * dimension + row], scale))
                return false;
        return true;
    }

    public static bool ApproximatelyEqual(ReadOnlySpan<double> left, ReadOnlySpan<double> right, double absolute,
        double relative)
    {
        for (var i = 0; i < left.Length; i++)
            if (!Tolerance.AreEqual(left[i], right[i], absolute, relative))
                return false;
        return true;
    }

    public static string Format(string name, ReadOnlySpan<double> entries, int rowCount, int columnCount)
    {
        var builder = new StringBuilder(name).Append(" [");
        for (var row = 0; row < rowCount; row++)
        {
            builder.Append(row == 0 ? "[" : ", [");
            for (var column = 0; column < columnCount; column++)
            {
                if (column > 0)
                    builder.Append(", ");
                builder.Append(entries[row * columnCount + column].ToString(CultureInfo.InvariantCulture));
            }

            builder.Append(']');
        }

        return builder.Append(']').ToString();
    }

    public static ArgumentOutOfRangeException IndexOutOfRange(string parameterName, int value, int count) =>
        new(parameterName, value, $"The index must be between 0 and {count - 1}.");

    public static void CheckIndices(int row, int column, int dimension)
    {
        if ((uint) row >= (uint) dimension)
            throw IndexOutOfRange(nameof(row), row, dimension);
        if ((uint) column >= (uint) dimension)
            throw IndexOutOfRange(nameof(column), column, dimension);
    }

    public static InvalidOperationException SingularMatrix() =>
        new("The matrix is singular and has no inverse.");
}
