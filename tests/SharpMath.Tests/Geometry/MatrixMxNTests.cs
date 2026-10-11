using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class MatrixMxNTests
{
    private static readonly MatrixMxN Rectangular = new(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } });

    [Fact]
    public void Constructors_CopyTheirInput()
    {
        var values = new double[,] { { 1, 2 }, { 3, 4 } };
        var matrix = new MatrixMxN(values);
        values[0, 0] = 99;
        Assert.Equal(1, matrix[0, 0]);
        Assert.Equal(new MatrixMxN(2, 2, [1, 2, 3, 4]), matrix);
        Assert.Equal(2, Rectangular.RowCount);
        Assert.Equal(3, Rectangular.ColumnCount);
        Assert.False(Rectangular.IsSquare);
        Assert.Equal(6, Rectangular[1, 2]);
        Assert.Throws<ArgumentOutOfRangeException>(() => Rectangular[2, 0]);
        Assert.Throws<ArgumentOutOfRangeException>(() => Rectangular[0, 3]);
        Assert.Throws<ArgumentException>(() => new MatrixMxN(new double[0, 2]));
        Assert.Throws<ArgumentException>(() => new MatrixMxN(2, 2, [1, 2, 3]));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MatrixMxN(0, 2, []));
        Assert.Throws<ArgumentOutOfRangeException>(() => MatrixMxN.Zero(1, 0));
        Assert.Throws<ArgumentNullException>(() => new MatrixMxN(null!));
    }

    [Fact]
    public void Factories_AndConversions()
    {
        Assert.Equal(new MatrixMxN(2, 2, [1, 0, 0, 1]), MatrixMxN.Identity(2));
        Assert.Equal(new MatrixMxN(1, 2, [0, 0]), MatrixMxN.Zero(1, 2));
        Assert.Equal(MatrixMxN.Identity(4), MatrixMxN.FromMatrix(Matrix4x4.Identity));
        Assert.Equal(new double[,] { { 1, 2, 3 }, { 4, 5, 6 } }, Rectangular.ToArray());
        Assert.Equal(new MatrixMxN(new double[,] { { 1, 4 }, { 2, 5 }, { 3, 6 } }), Rectangular.Transpose());
    }

    [Fact]
    public void Arithmetic()
    {
        Assert.Equal(Rectangular * 2, Rectangular + Rectangular);
        Assert.Equal(MatrixMxN.Zero(2, 3), Rectangular - Rectangular);
        Assert.Equal(Rectangular * -1, -Rectangular);
        Assert.Equal(Rectangular * 2, 2 * Rectangular);
        Assert.Equal(Rectangular, Rectangular * 2 / 2);
        Assert.Equal(new MatrixMxN(new double[,] { { 14, 32 }, { 32, 77 } }), Rectangular * Rectangular.Transpose());
        Assert.Throws<ArgumentException>(() => Rectangular * Rectangular);
        Assert.Throws<ArgumentException>(() => Rectangular + MatrixMxN.Identity(2));
    }

    [Fact]
    public void DeterminantInverseAndSolve()
    {
        var matrix = MatrixMxN.FromMatrix(new Matrix4x4(1, 0, 0, 1, 1, 2, 3, 2, 2, 3, 4, 0, 1, 2, -1, -2));
        Assert.Equal(-24, matrix.Determinant(), 10);
        Assert.True((matrix * matrix.Inverse()).ApproximatelyEquals(MatrixMxN.Identity(4)));

        var system = new MatrixMxN(new double[,] { { 1, -1, 2 }, { 2, 3, 2 }, { 3, 2, 1 } });
        var solution = system.Solve([6, 11, 8]);
        Assert.Equal(1, solution[0], 12);
        Assert.Equal(1, solution[1], 12);
        Assert.Equal(3, solution[2], 12);

        var multiple = system.Solve(new MatrixMxN(new double[,] { { 6, 12 }, { 11, 22 }, { 8, 16 } }));
        Assert.True(multiple.ApproximatelyEquals(new MatrixMxN(new double[,] { { 1, 2 }, { 1, 2 }, { 3, 6 } })));

        Assert.Throws<ArgumentException>(() => system.Solve([1, 2]));
        Assert.Throws<ArgumentException>(() => system.Solve(MatrixMxN.Zero(2, 1)));
        Assert.Throws<InvalidOperationException>(() => Rectangular.Determinant());
        Assert.Throws<InvalidOperationException>(() => Rectangular.Inverse());
    }

    [Fact]
    public void SingularMatrices()
    {
        var singular = new MatrixMxN(new double[,] { { 1, 2 }, { 2, 4 } });
        Assert.Equal(0, singular.Determinant());
        Assert.False(singular.TryInvert(out var inverse));
        Assert.Null(inverse);
        Assert.Throws<InvalidOperationException>(() => singular.Inverse());
        Assert.Throws<InvalidOperationException>(() => singular.Solve([1, 2]));
        Assert.Throws<InvalidOperationException>(() => singular.Solve(MatrixMxN.Identity(2)));
        Assert.Equal(0, MatrixMxN.Zero(2, 2).Determinant());
    }

    [Fact]
    public void Solve_HandlesSmallButRegularScales()
    {
        // Regression: an absolute pivot threshold rejected well-conditioned systems at small scales.
        var tiny = new MatrixMxN(1, 1, [1e-10]);
        Assert.Equal(1, tiny.Solve([1e-10])[0], 12);

        // Partial pivoting keeps precision with a tiny leading entry.
        var pivoting = new MatrixMxN(2, 2, [2e-9, 1, 1, 1]);
        var solution = pivoting.Solve([1, 2]);
        Assert.Equal(1 / (1 - 2e-9), solution[0], 12);
    }

    [Fact]
    public void Rank()
    {
        Assert.Equal(2, Rectangular.Rank());
        Assert.Equal(1, new MatrixMxN(new double[,] { { 1, 2 }, { 2, 4 } }).Rank());
        Assert.Equal(0, MatrixMxN.Zero(3, 3).Rank());
        Assert.Equal(3, MatrixMxN.Identity(3).Rank());
    }

    [Fact]
    public void EqualityAndStrings()
    {
        var copy = new MatrixMxN(Rectangular.ToArray());
        Assert.True(Rectangular == copy);
        Assert.False(Rectangular != copy);
        Assert.True(Rectangular.Equals((object) copy));
        Assert.False(Rectangular.Equals(null));
        Assert.False(Rectangular == null);
        Assert.True((MatrixMxN?) null == null);
        Assert.False(Rectangular.Equals(Rectangular.Transpose()));
        Assert.Equal(Rectangular.GetHashCode(), copy.GetHashCode());
        Assert.True(Rectangular.ApproximatelyEquals(Rectangular + MatrixMxN.Zero(2, 3)));
        Assert.False(Rectangular.ApproximatelyEquals(Rectangular.Transpose()));
        Assert.Equal("MatrixMxN [[1, 2, 3], [4, 5, 6]]", Rectangular.ToString());
    }
}
