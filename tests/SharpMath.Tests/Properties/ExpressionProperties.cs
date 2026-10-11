using System.Globalization;
using CsCheck;
using SharpMath.Expressions;

namespace SharpMath.Tests.Properties;

public class ExpressionProperties
{
    private sealed record Expr(string Text, Func<double, double> Evaluate);

    private static readonly Gen<Expr> Leaf = Gen.OneOf(
        Gen.Int[0, 50].Select(n => new Expr(n.ToString(CultureInfo.InvariantCulture), _ => n)),
        Gen.Const(new Expr("x", x => x)),
        Gen.Const(new Expr("pi", _ => Math.PI)));

    private static Gen<Expr> Expressions(int depth) =>
        depth == 0
            ? Leaf
            : Gen.Frequency(
                (2, Leaf),
                (6, Gen.Select(Expressions(depth - 1), Expressions(depth - 1), Gen.Int[0, 3], Combine)),
                (1, Expressions(depth - 1).Select(e => new Expr($"-({e.Text})", x => -e.Evaluate(x)))),
                (1, Expressions(depth - 1).Select(e => new Expr($"sin({e.Text})", x => Math.Sin(e.Evaluate(x))))),
                (1, Expressions(depth - 1).Select(e => new Expr($"({e.Text})^2", x => Math.Pow(e.Evaluate(x), 2)))));

    private static Expr Combine(Expr left, Expr right, int op) => op switch
    {
        0 => new Expr($"({left.Text}) + ({right.Text})", x => left.Evaluate(x) + right.Evaluate(x)),
        1 => new Expr($"({left.Text}) - ({right.Text})", x => left.Evaluate(x) - right.Evaluate(x)),
        2 => new Expr($"({left.Text})({right.Text})", x => left.Evaluate(x) * right.Evaluate(x)),
        _ => new Expr($"({left.Text}) / ({right.Text})", x => left.Evaluate(x) / right.Evaluate(x))
    };

    [Fact]
    public void ParsedExpressions_MatchTheEquivalentCSharp()
    {
        Gen.Select(Expressions(4), Gen.Double[-10, 10]).Sample((expression, x) =>
        {
            var expected = expression.Evaluate(x);
            var actual = MathExpression.Parse(expression.Text).ToFunc("x")(x);
            if (double.IsNaN(expected))
                Assert.True(double.IsNaN(actual));
            else
                Assert.True(Tolerance.AreEqual(expected, actual, 1e-9, 1e-9), $"{expression.Text} at x = {x}: {expected} vs {actual}");
        });
    }
}
