using CsCheck;
using SharpMath.Geometry;

namespace SharpMath.Tests.Properties;

public class MatrixProperties
{
    private static readonly Gen<double> Entry = Gen.Double[-10, 10];

    private static readonly Gen<Matrix2x2> Matrices2 = Entry.Array[4].Select(v => new Matrix2x2(v[0], v[1], v[2], v[3]));

    private static readonly Gen<Matrix3x3> Matrices3 =
        Entry.Array[9].Select(v => new Matrix3x3(v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8]));

    private static readonly Gen<Matrix4x4> Matrices4 = Entry.Array[16].Select(v => new Matrix4x4(v[0], v[1], v[2], v[3],
        v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], v[12], v[13], v[14], v[15]));

    // Inverses of nearly singular matrices are legitimately inaccurate, so only well-conditioned ones are checked. The
    // Hadamard ratio |det| / Π‖rowᵢ‖ lies in [0, 1] and does not depend on the scale of the rows. Rows of wildly
    // different magnitude can still be numerically singular relative to the largest entry, so TryInvert decides.
    private static bool WellConditioned(IMatrix matrix, double determinant)
    {
        var product = 1.0;
        for (var row = 0; row < matrix.RowCount; row++)
        {
            var normSquared = 0.0;
            for (var column = 0; column < matrix.ColumnCount; column++)
                normSquared += matrix[row, column] * matrix[row, column];
            product *= Math.Sqrt(normSquared);
        }

        return Math.Abs(determinant) > 1e-3 * product;
    }

    // The rounding error of M · M⁻¹ is proportional to ‖M‖ · ‖M⁻¹‖ (here with max norms), not absolute.
    private static void AssertNearIdentity(IMatrix product, IMatrix matrix, IMatrix inverse)
    {
        var bound = 1e-12 * product.RowCount * MaxAbs(matrix) * MaxAbs(inverse);
        for (var row = 0; row < product.RowCount; row++)
        for (var column = 0; column < product.ColumnCount; column++)
            Assert.True(Math.Abs(product[row, column] - (row == column ? 1 : 0)) <= bound,
                $"Entry ({row}, {column}) of {product} exceeds {bound}.");
    }

    private static double MaxAbs(IMatrix matrix)
    {
        var max = 0.0;
        for (var row = 0; row < matrix.RowCount; row++)
        for (var column = 0; column < matrix.ColumnCount; column++)
            max = Math.Max(max, Math.Abs(matrix[row, column]));
        return max;
    }

    [Fact]
    public void Inverse_TimesMatrix_IsIdentity()
    {
        Matrices2.Sample(m =>
        {
            if (WellConditioned(m, m.Determinant) && m.TryInvert(out var inverse))
                AssertNearIdentity(m * inverse, m, inverse);
        });
        Matrices3.Sample(m =>
        {
            if (WellConditioned(m, m.Determinant) && m.TryInvert(out var inverse))
                AssertNearIdentity(m * inverse, m, inverse);
        });
        Matrices4.Sample(m =>
        {
            if (WellConditioned(m, m.Determinant) && m.TryInvert(out var inverse))
                AssertNearIdentity(m * inverse, m, inverse);
        });
    }

    [Fact]
    public void AffineTransformations_AreInvertibleAtAnyScale()
    {
        Gen.Select(Gen.Double[-1e6, 1e6].Array[3], Gen.Double[-Math.PI, Math.PI].Array[3], Gen.Double[-3, 3].Array[3])
            .Sample((translation, angles, exponents) =>
            {
                var transformation = Matrix4x4.Translation(translation[0], translation[1], translation[2]) *
                                     Matrix4x4.Rotation(angles[0], angles[1], angles[2]) *
                                     Matrix4x4.Scaling(Math.Pow(10, exponents[0]), Math.Pow(10, exponents[1]),
                                         Math.Pow(10, exponents[2]));
                Assert.False(transformation.IsSingular);
                var point = new Vector3(1, 2, 3);
                var roundTrip = transformation.Inverse() * (transformation * point);
                Assert.True(roundTrip.ApproximatelyEquals(point, 1e-6, 1e-6), $"{roundTrip} for {transformation}");
            });
    }

    [Fact]
    public void Determinant_IsMultiplicative()
    {
        Gen.Select(Matrices3, Matrices3).Sample((a, b) =>
        {
            var expected = a.Determinant * b.Determinant;
            Assert.True(Tolerance.AreEqual((a * b).Determinant, expected, 1e-6, 1e-9));
        });
        Gen.Select(Matrices4, Matrices4).Sample((a, b) =>
        {
            var expected = a.Determinant * b.Determinant;
            Assert.True(Tolerance.AreEqual((a * b).Determinant, expected, 1e-4, 1e-8));
        });
    }

    [Fact]
    public void Adjugate_TimesMatrix_IsDeterminantTimesIdentity()
    {
        Matrices3.Sample(m =>
            Assert.True((m * m.Adjugate()).ApproximatelyEquals(Matrix3x3.Identity * m.Determinant, 1e-9, 1e-9)));
        Matrices4.Sample(m =>
            Assert.True((m.Adjugate() * m).ApproximatelyEquals(Matrix4x4.Identity * m.Determinant, 1e-7, 1e-9)));
    }

    [Fact]
    public void Determinants_MatchTheLuDecomposition()
    {
        Matrices4.Sample(m =>
            Assert.True(Tolerance.AreEqual(m.Determinant, MatrixMxN.FromMatrix(m).Determinant(), 1e-8, 1e-9)));
    }

    [Fact]
    public void Transpose_ReversesProducts()
    {
        Gen.Select(Matrices4, Matrices4).Sample((a, b) =>
            Assert.True((a * b).Transpose().ApproximatelyEquals(b.Transpose() * a.Transpose())));
    }

    [Fact]
    public void Rotations_PreserveLengthsAndAreOrthogonal()
    {
        Gen.Select(Gen.Double[-10, 10], Gen.Double[-10, 10], Gen.Double[-10, 10], Entry, Entry, Entry)
            .Sample((ax, ay, az, x, y, z) =>
            {
                var rotation = Matrix4x4.Rotation(ax, ay, az);
                var vector = new Vector3(x, y, z);
                Assert.True(Tolerance.AreEqual((rotation * vector).Length, vector.Length, 1e-9, 1e-9));
                Assert.True(rotation.IsOrthogonal);
                Assert.True(Tolerance.AreEqual(rotation.Determinant, 1));
                var planar = Matrix2x2.Rotation(ax) * new Vector2(x, y);
                Assert.True(Tolerance.AreEqual(planar.Length, new Vector2(x, y).Length, 1e-9, 1e-9));
            });
    }

    [Fact]
    public void LookAtAndWorld_UseOrthonormalBases()
    {
        Gen.Select(Entry.Array[3], Entry.Array[3], Entry.Array[3]).Sample((e, t, u) =>
        {
            var eye = new Vector3(e[0], e[1], e[2]);
            var target = new Vector3(t[0], t[1], t[2]);
            var up = new Vector3(u[0], u[1], u[2]);
            var direction = target - eye;
            if (direction.Length < 1e-3 || up.Length < 1e-3 || up.Cross(direction).Length < 1e-3 * up.Length * direction.Length)
                return;

            var view = Matrix4x4.LookAt(eye, target, up);
            Assert.True(RotationPart(view).IsOrthogonal);
            Assert.True((view * eye).ApproximatelyEquals(Vector3.Zero, 1e-9, 1e-9));
            var forward = view * target;
            Assert.True(forward.Z < 0);

            var world = Matrix4x4.World(eye, direction, up);
            Assert.True(RotationPart(world).IsOrthogonal);
            Assert.True((world * Vector3.Zero).ApproximatelyEquals(eye));
            Assert.True((view * world).ApproximatelyEquals(Matrix4x4.Identity, 1e-9, 1e-9));
        });
    }

    [Fact]
    public void MatrixMxN_SolveHasSmallResiduals()
    {
        Gen.Int[1, 8].SelectMany(n => Gen.Double[-1, 1].Array[n * n].Select(Gen.Double[-10, 10].Array[n], (a, b) => (n, a, b)))
            .Sample(sample =>
            {
                var (n, values, rightHandSide) = sample;

                // Diagonal dominance guarantees a regular, well-conditioned matrix.
                for (var i = 0; i < n; i++)
                    values[i * n + i] += n + 1;
                var matrix = new MatrixMxN(n, n, values);
                var solution = matrix.Solve(rightHandSide);
                var residual = matrix * new MatrixMxN(n, 1, solution) - new MatrixMxN(n, 1, rightHandSide);
                for (var i = 0; i < n; i++)
                    Assert.True(Math.Abs(residual[i, 0]) < 1e-9);
                Assert.True((matrix * matrix.Inverse()).ApproximatelyEquals(MatrixMxN.Identity(n), 1e-9, 1e-9));
                Assert.Equal(n, matrix.Rank());
            });
    }

    private static Matrix3x3 RotationPart(Matrix4x4 m) =>
        new(m.M11, m.M12, m.M13, m.M21, m.M22, m.M23, m.M31, m.M32, m.M33);
}
