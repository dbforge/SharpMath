using BenchmarkDotNet.Attributes;
using SharpMath.Geometry;

namespace SharpMath.Benchmarks;

[MemoryDiagnoser]
public class VectorBenchmarks
{
    private readonly Vector3 _a = new(1, 2, 3);
    private readonly Vector3 _b = new(-2, 0.5, 4);

    [Benchmark]
    public Vector3 AddAndScale() => _a + _b * 2;

    [Benchmark]
    public Vector3 Normalize() => _a.Normalize();

    [Benchmark]
    public double Length() => _a.Length;

    [Benchmark]
    public Vector3 Cross() => _a.Cross(_b);

    [Benchmark]
    public bool Equality() => _a == _b;

    [Benchmark]
    public bool IsZero() => _a.IsZero;

    [Benchmark]
    public int HashCode() => _a.GetHashCode();

    [Benchmark]
    public bool IsParallelTo() => _a.IsParallelTo(_b);
}

[MemoryDiagnoser]
public class MatrixBenchmarks
{
    private readonly Matrix2x2 _m2 = new(8, 3, 4, 2);
    private readonly Matrix3x3 _m3 = new(2, 5, 2, 3, -3, 1, 1, 4, -4);
    private readonly Matrix4x4 _m4 = new(1, 0, 0, 1, 1, 2, 3, 2, 2, 3, 4, 0, 1, 2, -1, -2);
    private readonly Matrix4x4 _rotation = Matrix4x4.Rotation(0.3, 0.5, 0.7);
    private readonly Vector3 _vector = new(1, 2, 3);

    [Benchmark]
    public double Determinant2x2() => _m2.Determinant;

    [Benchmark]
    public double Determinant3x3() => _m3.Determinant;

    [Benchmark]
    public double Determinant4x4() => _m4.Determinant;

    [Benchmark]
    public Matrix2x2 Adjugate2x2() => _m2.Adjugate();

    [Benchmark]
    public Matrix3x3 Adjugate3x3() => _m3.Adjugate();

    [Benchmark]
    public Matrix4x4 Adjugate4x4() => _m4.Adjugate();

    [Benchmark]
    public Matrix2x2 Inverse2x2() => _m2.Inverse();

    [Benchmark]
    public Matrix3x3 Inverse3x3() => _m3.Inverse();

    [Benchmark]
    public Matrix4x4 Inverse4x4() => _m4.Inverse();

    [Benchmark]
    public bool IsSingular4x4() => _m4.IsSingular;

    [Benchmark]
    public Matrix3x3 Multiply3x3() => _m3 * _m3;

    [Benchmark]
    public Matrix4x4 Multiply4x4() => _m4 * _rotation;

    [Benchmark]
    public Vector3 TransformVector() => _rotation * _vector;

    [Benchmark]
    public bool Equality4x4() => _m4 == _rotation;

    [Benchmark]
    public int HashCode4x4() => _m4.GetHashCode();

    [Benchmark]
    public bool IsOrthogonal4x4() => _rotation.IsOrthogonal;
}

[MemoryDiagnoser]
public class PolygonBenchmarks
{
    private readonly Polygon _polygon = new(Enumerable.Range(0, 64)
        .Select(i => new Point2D(Math.Cos(i * Math.Tau / 64) * (i % 2 == 0 ? 10 : 7), Math.Sin(i * Math.Tau / 64) * (i % 2 == 0 ? 10 : 7))));

    [Benchmark]
    public bool Contains() => _polygon.Contains(new Point2D(1, 2));

    [Benchmark]
    public double Area() => _polygon.Area;
}
