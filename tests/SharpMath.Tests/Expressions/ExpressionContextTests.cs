using SharpMath.Expressions;

namespace SharpMath.Tests.Expressions;

public class ExpressionContextTests
{
    [Fact]
    public void CustomFunctionsAndConstants()
    {
        var context = new ExpressionContext()
            .AddFunction("sec", x => 1 / Math.Cos(x))
            .AddFunction("hypot", (a, b) => Math.Sqrt(a * a + b * b))
            .AddFunction("sum", 0, int.MaxValue, values =>
            {
                var sum = 0.0;
                foreach (var value in values)
                    sum += value;
                return sum;
            })
            .AddConstant("phi", (1 + Math.Sqrt(5)) / 2);

        Assert.Equal(1, MathExpression.Parse("sec(0)", context).Evaluate());
        Assert.Equal(5, MathExpression.Parse("hypot(3, 4)", context).Evaluate());
        Assert.Equal(0, MathExpression.Parse("sum()", context).Evaluate());
        Assert.Equal(6, MathExpression.Parse("sum(1, 2, 3)", context).Evaluate());
        Assert.Equal((1 + Math.Sqrt(5)) / 2, MathExpression.Parse("PHI", context).Evaluate());
    }

    [Fact]
    public void LongestMatch_PrefersLongerNames()
    {
        // Regression: the old tokenizer stopped at the shortest match, so "sinh" was read as "sin" + "h".
        Assert.Equal(Math.Sinh(1), MathExpression.Parse("sinh(1)").Evaluate(), 12);
        var context = new ExpressionContext().AddFunction("expm", x => Math.Exp(x) - 1);
        Assert.Equal(0, MathExpression.Parse("expm(0)", context).Evaluate());
    }

    [Fact]
    public void DeclaredVariables_MayHaveSeveralLetters()
    {
        var context = new ExpressionContext().DeclareVariable("speed").DeclareVariable("t1");
        var expression = MathExpression.Parse("speed * t1", context);
        Assert.Equal(["speed", "t1"], expression.Variables);
        Assert.Equal(6, expression.Evaluate(new Dictionary<string, double> { ["speed"] = 2, ["T1"] = 3 }));
    }

    [Fact]
    public void Contexts_AreIndependentAndCapturedAtParseTime()
    {
        var context = new ExpressionContext().AddConstant("k", 2);
        var expression = MathExpression.Parse("k", context);
        context.AddConstant("k", 3);
        Assert.Equal(2, expression.Evaluate());
        Assert.Equal(3, MathExpression.Parse("k", context).Evaluate());
        Assert.Equal(["k"], MathExpression.Parse("k").Variables);
    }

    [Fact]
    public void Clone_CreatesAnIndependentCopy()
    {
        var original = new ExpressionContext().AddConstant("k", 2).DeclareVariable("speed");
        var clone = original.Clone().AddConstant("k", 5);
        Assert.Equal(2, MathExpression.Parse("k", original).Evaluate());
        Assert.Equal(5, MathExpression.Parse("k", clone).Evaluate());
        Assert.Equal(["speed"], MathExpression.Parse("speed", clone).Variables);
        Assert.Equal(1, MathExpression.Parse("sin(pi/2)", clone).Evaluate(), 12);
        Assert.Equal(6, MathExpression.Parse("speed(2)", clone).Evaluate("speed", 3));
    }

    [Fact]
    public void ImpureFunctions_AreNotFolded()
    {
        var calls = 0;
        var context = new ExpressionContext().AddFunction("count", _ => ++calls);
        var expression = MathExpression.Parse("count(1)", context);
        Assert.Equal(0, calls);
        Assert.Equal(1, expression.Evaluate());
        Assert.Equal(2, expression.Evaluate());
    }

    [Theory]
    [InlineData("")]
    [InlineData("1x")]
    [InlineData("a b")]
    [InlineData("ä")]
    [InlineData("a_b")]
    public void Names_AreValidated(string name)
    {
        var context = new ExpressionContext();
        Assert.Throws<ArgumentException>(() => context.AddConstant(name, 1));
    }

    [Fact]
    public void Arguments_AreValidated()
    {
        var context = new ExpressionContext();
        Assert.Throws<ArgumentNullException>(() => context.AddConstant(null!, 1));
        Assert.Throws<ArgumentNullException>(() => context.AddFunction("f", (Func<double, double>) null!));
        Assert.Throws<ArgumentNullException>(() => context.AddFunction("f", (Func<double, double, double>) null!));
        Assert.Throws<ArgumentNullException>(() => context.AddFunction("f", 1, 2, null!));
        Assert.Throws<ArgumentOutOfRangeException>(() => context.AddFunction("f", -1, 2, _ => 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => context.AddFunction("f", 3, 2, _ => 0));
        Assert.Throws<ParserException>(() => MathExpression.Parse("f(1, 2, 3)",
            new ExpressionContext().AddFunction("f", 1, 2, _ => 0)));
    }
}
