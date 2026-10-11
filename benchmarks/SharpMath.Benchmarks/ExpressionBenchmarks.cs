using BenchmarkDotNet.Attributes;
using SharpMath.Expressions;

namespace SharpMath.Benchmarks;

[MemoryDiagnoser]
public class ExpressionBenchmarks
{
    private const string Text = "3*4^2+sin(1)/2-7*(2+cos(0))";
    private readonly MathExpression _parsed = MathExpression.Parse("3*x^2+sin(x)/2-7*(2+cos(x))");
    private readonly Func<double, double> _function = MathExpression.Parse("3*x^2+sin(x)/2-7*(2+cos(x))").ToFunc("x");

    [Benchmark]
    public double ParseAndEvaluate() => MathExpression.Parse(Text).Evaluate();

    [Benchmark]
    public double EvaluateParsed() => _parsed.Evaluate("x", 1.5);

    [Benchmark]
    public double EvaluateFunc() => _function(1.5);
}
