using BenchmarkDotNet.Attributes;
using SharpMath.Equations;
using SharpMath.Geometry;

namespace SharpMath.Benchmarks;

[MemoryDiagnoser]
public class LinearSystemBenchmarks
{
    private MatrixMxN _matrix = null!;
    private double[] _rightHandSide = null!;
    private LinearEquationSystem _system = null!;

    [Params(3, 10, 50)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        _system = new LinearEquationSystem(Enumerable.Range(0, Size).Select(row =>
            new LinearEquation(Enumerable.Range(0, Size).Select(column =>
                column == row ? Size + random.NextDouble() : random.NextDouble()), random.NextDouble())));
        _matrix = new MatrixMxN(Size, Size, _system.Equations.SelectMany(equation => equation.Coefficients).ToArray());
        _rightHandSide = _system.Equations.Select(equation => equation.Result).ToArray();
    }

    [Benchmark]
    public double[] Solve() => _system.Solve();

    [Benchmark]
    public double[] MatrixSolve() => _matrix.Solve(_rightHandSide);
}
