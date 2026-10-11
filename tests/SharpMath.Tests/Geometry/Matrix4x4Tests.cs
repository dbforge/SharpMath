using SharpMath.Geometry;

namespace SharpMath.Tests.Geometry;

public class Matrix4x4Tests
{
    private static readonly Matrix4x4 A = new(1, 0, 0, 1, 1, 2, 3, 2, 2, 3, 4, 0, 1, 2, -1, -2);

    [Fact]
    public void Determinant_UsesClosedForm()
    {
        Assert.Equal(-24, A.Determinant, 10);
        Assert.Equal(1, Matrix4x4.Identity.Determinant);
        Assert.Equal(5, A.Trace);
    }

    [Fact]
    public void Inverse_AdjugateAndCofactors()
    {
        // Regression: Inverse used to return the identity matrix.
        Assert.True((A * A.Inverse()).IsIdentity);
        Assert.True((A.Adjugate() * A).ApproximatelyEquals(Matrix4x4.Identity * A.Determinant));
        Assert.Equal(A.Adjugate().Transpose(), A.Cofactors());

        var singular = new Matrix4x4(1, 2, 3, 4, 2, 4, 6, 8, 0, 1, 0, 1, 1, 0, 1, 0);
        Assert.True(singular.IsSingular);
        Assert.False(singular.TryInvert(out _));
        Assert.Throws<InvalidOperationException>(() => singular.Inverse());

        var transform = Matrix4x4.Translation(1, 2, 3) * Matrix4x4.Rotation(0.3, 0.5, 0.7) * Matrix4x4.Scaling(2);
        Assert.True((transform * transform.Inverse()).IsIdentity);
    }

    [Fact]
    public void Predicates()
    {
        Assert.True(Matrix4x4.Identity.IsIdentity);
        // Regression: M44 and off-diagonal entries used to be ignored.
        Assert.False(new Matrix4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 7).IsIdentity);
        Assert.False(Matrix4x4.Translation(1, 0, 0).IsIdentity);

        // Regression: a zero on the diagonal used to make a matrix "not diagonal".
        Assert.True(new Matrix4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0).IsDiagonal);
        Assert.True(Matrix4x4.Identity.IsDiagonal);
        Assert.False(new Matrix4x4(1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1).IsDiagonal);

        Assert.True(new Matrix4x4(1, 3, 4, 7, 0, 2, 2, 3, 0, 0, 3, 9, 0, 0, 0, 8).IsTriangular);
        Assert.False(new Matrix4x4(5, 0, 0, 2, 0, 7, 0, 0, 9, 0, 4, 0, 0, 0, 0, 3).IsTriangular);
        Assert.True(Matrix4x4.Identity.IsTriangular);

        // Regression: rotations used to be reported as not orthogonal because of exact comparisons.
        Assert.True(Matrix4x4.Rotation(0.3, 0.5, 0.7).IsOrthogonal);
        Assert.False(A.IsOrthogonal);
        Assert.True((A + A.Transpose()).IsSymmetric);
        Assert.True((A - A.Transpose()).IsSkewSymmetric);
        Assert.False(A.IsSkewSymmetric);
    }

    [Fact]
    public void AffineTransformations_UseColumnVectors()
    {
        Assert.Equal(new Vector3(2, 3, 4), Matrix4x4.Translation(1, 2, 3) * Vector3.One);
        Assert.Equal(new Vector3(2, 3, 4), Matrix4x4.Translation(new Vector3(1, 2, 3)) * Vector3.One);
        Assert.Equal(new Vector3(2, 3, 4), Matrix4x4.Scaling(2, 3, 4) * Vector3.One);
        Assert.Equal(new Vector3(5, 5, 5), Matrix4x4.Scaling(5) * Vector3.One);
        Assert.True((Matrix4x4.RotationX(Math.PI / 2) * Vector3.UnitY).ApproximatelyEquals(Vector3.UnitZ));
        Assert.True((Matrix4x4.RotationY(Math.PI / 2) * Vector3.UnitZ).ApproximatelyEquals(Vector3.UnitX));
        Assert.True((Matrix4x4.RotationZ(Math.PI / 2) * Vector3.UnitX).ApproximatelyEquals(Vector3.UnitY));
        Assert.Equal(Matrix4x4.RotationX(0.1) * Matrix4x4.RotationY(0.2) * Matrix4x4.RotationZ(0.3),
            Matrix4x4.Rotation(0.1, 0.2, 0.3));

        var composed = Matrix4x4.Translation(0, 0, -5) * Matrix4x4.RotationY(Math.PI / 2);
        Assert.True((composed * Vector3.UnitX).ApproximatelyEquals(new Vector3(0, 0, -6)));
    }

    [Fact]
    public void Perspective_IsRightHandedWithDepthZeroToOne()
    {
        var projection = Matrix4x4.PerspectiveFieldOfView(Math.PI / 2, 1, 1, 10);

        // Regression: M44 used to stay 1 and the layout was row-vector style.
        Assert.Equal(0, projection.M44);
        Assert.Equal(-1, projection.M43);

        var near = projection * new Vector3(0, 0, -1);
        var far = projection * new Vector3(0, 0, -10);
        Assert.Equal(0, near.Z, 12);
        Assert.Equal(1, far.Z, 12);
        Assert.True((projection * new Vector3(1, 1, -1)).ApproximatelyEquals(new Vector3(1, 1, 0)));

        var fromData = Matrix4x4.PerspectiveFieldOfView(new ProjectionData(100, 100, 1, Math.PI / 2, 1, 10));
        Assert.Equal(projection, fromData);
    }

    [Theory]
    [InlineData(0, 1, 1, 10)]
    [InlineData(Math.PI, 1, 1, 10)]
    [InlineData(1, 0, 1, 10)]
    [InlineData(1, 1, 0, 10)]
    [InlineData(1, 1, 10, 10)]
    public void Perspective_ValidatesArguments(double fov, double aspect, double near, double far) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Matrix4x4.PerspectiveFieldOfView(fov, aspect, near, far));

    [Fact]
    public void LookAt_KeepsTranslationAndIsOrthonormal()
    {
        // Regression: the translation of the camera used to be lost.
        var view = Matrix4x4.LookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
        Assert.True((view * Vector3.Zero).ApproximatelyEquals(new Vector3(0, 0, -5)));

        // Regression: the basis used to be scaled by the length of the up vector.
        var scaledUp = Matrix4x4.LookAt(new Vector3(1, 2, 3), new Vector3(4, 0, -1), new Vector3(0, 2, 0));
        var rotationPart = new Matrix3x3(scaledUp.M11, scaledUp.M12, scaledUp.M13, scaledUp.M21, scaledUp.M22,
            scaledUp.M23, scaledUp.M31, scaledUp.M32, scaledUp.M33);
        Assert.True(rotationPart.IsOrthogonal);
        Assert.True((scaledUp * new Vector3(1, 2, 3)).ApproximatelyEquals(Vector3.Zero));

        Assert.Throws<ArgumentException>(() => Matrix4x4.LookAt(Vector3.One, Vector3.One, Vector3.Up));
        Assert.Throws<ArgumentException>(() => Matrix4x4.LookAt(Vector3.Zero, Vector3.Up, Vector3.Up));
        Assert.Throws<ArgumentException>(() => Matrix4x4.LookAt(Vector3.Zero, Vector3.Forward, Vector3.Zero));
    }

    [Fact]
    public void World_PlacesAndOrientsObjects()
    {
        // Regression: the position used to be lost and the basis was not normalized.
        var world = Matrix4x4.World(new Vector3(1, 2, 3), new Vector3(0, 0, -6), new Vector3(0, 12, 0));
        Assert.True((world * Vector3.Zero).ApproximatelyEquals(new Vector3(1, 2, 3)));
        Assert.True((world * Vector3.Forward).ApproximatelyEquals(new Vector3(1, 2, 2)));
        Assert.True(world.ApproximatelyEquals(Matrix4x4.Translation(1, 2, 3)));

        var rotated = Matrix4x4.World(Vector3.Zero, Vector3.Right, Vector3.Up);
        Assert.True((rotated * Vector3.Forward).ApproximatelyEquals(Vector3.Right));
        Assert.Throws<ArgumentException>(() => Matrix4x4.World(Vector3.Zero, Vector3.Zero, Vector3.Up));
        Assert.Throws<ArgumentException>(() => Matrix4x4.World(Vector3.Zero, Vector3.Up, Vector3.Up));
    }

    [Fact]
    public void ElementWiseOperatorsIndexersEqualityAndStrings()
    {
        Assert.Equal(A * 2, A + A);
        Assert.Equal(Matrix4x4.Zero, A - A);
        Assert.Equal(A * -1, -A);
        Assert.Equal(2 * A, A * 2);
        Assert.Equal(A, A * 4 / 4);
        Assert.Equal(new Vector4(2, 8, 9, 0), A * Vector4.One);
        Assert.Equal(A, Matrix4x4.FromMatrix(MatrixMxN.FromMatrix(A)));
        Assert.Throws<ArgumentException>(() => Matrix4x4.FromMatrix(Matrix3x3.Identity));
        Assert.Equal(-1, A[3, 2]);
        Assert.Equal(-2, A[15]);
        Assert.Throws<ArgumentOutOfRangeException>(() => A[4, 0]);
        Assert.Throws<ArgumentOutOfRangeException>(() => A[16]);
        Assert.Equal(4, A.RowCount);
        Assert.Equal(4, A.ColumnCount);
        Assert.Equal(4, A.Dimension);
        Assert.True(A == new Matrix4x4(1, 0, 0, 1, 1, 2, 3, 2, 2, 3, 4, 0, 1, 2, -1, -2));
        Assert.True(A != Matrix4x4.Identity);
        Assert.True(A.Equals((object) new Matrix4x4(1, 0, 0, 1, 1, 2, 3, 2, 2, 3, 4, 0, 1, 2, -1, -2)));
        Assert.Equal(A.GetHashCode(), new Matrix4x4(1, 0, 0, 1, 1, 2, 3, 2, 2, 3, 4, 0, 1, 2, -1, -2).GetHashCode());
        Assert.True(A.ApproximatelyEquals(A + Matrix4x4.Identity * 1e-12));
        Assert.Equal(new Matrix4x4(new Vector4(1, 0, 0, 1), new Vector4(1, 2, 3, 2), new Vector4(2, 3, 4, 0),
            new Vector4(1, 2, -1, -2)), A);
        Assert.StartsWith("Matrix4x4 [[1, 0, 0, 1], [1, 2, 3, 2]", A.ToString());
    }
}
