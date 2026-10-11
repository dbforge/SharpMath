using SharpMath.Expressions;

namespace SharpMath.Tests.Expressions;

public class MathExpressionTests
{
    private static double Eval(string text) => MathExpression.Parse(text).Evaluate();

    [Theory]
    [InlineData("(-2+3)*(cos(0)*(2/3))", 2.0 / 3.0)]
    [InlineData("3*4^2", 48)]
    [InlineData("2^3^2", 512)]
    [InlineData("-2^2", -4)]
    [InlineData("(-2)^2", 4)]
    [InlineData("10 - 4 - 3", 3)]
    [InlineData("12 / 3 / 2", 2)]
    [InlineData("1 + 2 * 3", 7)]
    [InlineData("  7  ", 7)]
    public void Evaluate_RespectsPrecedenceAndAssociativity(string text, double expected) =>
        Assert.Equal(expected, Eval(text), 12);

    [Theory]
    [InlineData("5-(-3)", 8)]
    [InlineData("1-(-1)", 2)]
    [InlineData("4-(-2)*3", 10)]
    [InlineData("+3", 3)]
    [InlineData("(+3)", 3)]
    [InlineData("2*(+3)", 6)]
    [InlineData("2*-3", -6)]
    [InlineData("2^-1", 0.5)]
    [InlineData("--3", 3)]
    [InlineData("-(-(-3))", -3)]
    public void Evaluate_HandlesSignsEverywhere(string text, double expected) =>
        Assert.Equal(expected, Eval(text), 12);

    [Theory]
    [InlineData("5%3", 2)]
    [InlineData("-7%3", -1)]
    [InlineData("2*5%3", 1)]
    [InlineData("5!", 120)]
    [InlineData("0!", 1)]
    [InlineData("3!^2", 36)]
    [InlineData("2^3!", 64)]
    [InlineData(".5 + 1.", 1.5)]
    [InlineData("1e3", 1000)]
    [InlineData("2.5E-3", 0.0025)]
    [InlineData("1e+2", 100)]
    public void Evaluate_SupportsOperatorsAndNumberFormats(string text, double expected) =>
        Assert.Equal(expected, Eval(text), 12);

    [Theory]
    [InlineData("log(8, 2)", 3)]
    [InlineData("atan2(1, 1)", Math.PI / 4)]
    [InlineData("pow(2, 10)", 1024)]
    [InlineData("min(3, -1, 2)", -1)]
    [InlineData("max(3, -1, 2)", 3)]
    [InlineData("max(4)", 4)]
    [InlineData("sqrt(16) + abs(-2)", 6)]
    [InlineData("ln(e) + lg(100) + log2(8)", 6)]
    [InlineData("exp(0) + floor(2.7) + ceil(2.1) + round(2.5) + round(-2.5)", 6)]
    [InlineData("sign(-5) + sign(0) + sign(3)", 0)]
    [InlineData("sin(0) + cos(0) + tan(0)", 1)]
    [InlineData("asin(1) + acos(1) + atan(0)", Math.PI / 2)]
    [InlineData("sinh(0) + cosh(0) + tanh(0)", 1)]
    [InlineData("SIN(PI/2)", 1)]
    [InlineData("tau", Math.Tau)]
    [InlineData("sin (pi / 2)", 1)]
    public void Evaluate_KnowsBuiltIns(string text, double expected) => Assert.Equal(expected, Eval(text), 12);

    [Fact]
    public void Factorial_IsNaNOutsideItsDomain()
    {
        Assert.True(double.IsNaN(Eval("2.5!")));
        Assert.True(double.IsNaN(Eval("(-1)!")));
        Assert.True(double.IsPositiveInfinity(Eval("171!")));
        Assert.True(double.IsNaN(Eval("sign(0/0)")));
    }

    [Theory]
    [InlineData("2x", 3, 6)]
    [InlineData("2x^2", 3, 18)]
    [InlineData("3(x+1)", 3, 12)]
    [InlineData("(x+1)(x-1)", 3, 8)]
    [InlineData("2pi", 0, 2 * Math.PI)]
    [InlineData("pix", 2, 2 * Math.PI)]
    [InlineData("2sin(x)", 0, 0)]
    [InlineData("x!", 3, 6)]
    [InlineData("1/2x", 4, 2)]
    [InlineData("2e", 0, 2 * Math.E)]
    [InlineData("exp(x)", 0, 1)]
    [InlineData("-x^2", 3, -9)]
    public void Evaluate_SupportsImplicitMultiplication(string text, double x, double expected) =>
        Assert.Equal(expected, MathExpression.Parse(text).Evaluate("x", x), 12);

    [Fact]
    public void Variables_AreCollectedInOrderAndCaseInsensitive()
    {
        var expression = MathExpression.Parse("x*Y + y - X + z");
        Assert.Equal(["x", "y", "z"], expression.Variables);
        var result = expression.Evaluate(new Dictionary<string, double> { ["X"] = 2, ["y"] = 3, ["Z"] = 1, ["w"] = 9 });
        Assert.Equal(2 * 3 + 3 - 2 + 1, result);
        Assert.Equal(6, MathExpression.Parse("xy").Evaluate(new Dictionary<string, double> { ["x"] = 2, ["y"] = 3 }));
    }

    [Fact]
    public void Evaluate_ReportsMissingVariables()
    {
        var expression = MathExpression.Parse("x + y");
        var exception = Assert.Throws<ArgumentException>(() =>
            expression.Evaluate(new Dictionary<string, double> { ["x"] = 1 }));
        Assert.Contains("y", exception.Message, StringComparison.Ordinal);
        Assert.Throws<ArgumentException>(() => expression.Evaluate());
        Assert.Throws<ArgumentException>(() => expression.Evaluate("x", 1));
        Assert.Throws<ArgumentException>(() => MathExpression.Parse("y").Evaluate("x", 1));
        Assert.Equal(4, MathExpression.Parse("4").Evaluate("x", 1));
        Assert.Equal(2, MathExpression.Parse("X").Evaluate("x", 2));
        Assert.Throws<ArgumentNullException>(() => expression.Evaluate((IReadOnlyDictionary<string, double>) null!));
        Assert.Throws<ArgumentNullException>(() => expression.Evaluate(null!, 1));
    }

    [Fact]
    public void ToFunc_CreatesReusableDelegate()
    {
        var function = MathExpression.Parse("x^2 + 1").ToFunc("x");
        Assert.Equal(1, function(0));
        Assert.Equal(10, function(3));
        Assert.Equal(5, MathExpression.Parse("5").ToFunc("x")(42));
        Assert.Throws<ArgumentException>(() => MathExpression.Parse("x + y").ToFunc("x"));
        Assert.Throws<ArgumentNullException>(() => MathExpression.Parse("x").ToFunc(null!));
    }

    [Fact]
    public void TextAndToString_ReturnSource()
    {
        var expression = MathExpression.Parse("1 + x");
        Assert.Equal("1 + x", expression.Text);
        Assert.Equal("1 + x", expression.ToString());
        Assert.Throws<ArgumentNullException>(() => MathExpression.Parse(null!));
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("   ", 0)]
    [InlineData("3+", 2)]
    [InlineData("*3", 0)]
    [InlineData("()", 1)]
    [InlineData("3+4)", 3)]
    [InlineData("(3+4", 4)]
    [InlineData("((3)", 4)]
    [InlineData("2 3", 2)]
    [InlineData("x2", 1)]
    [InlineData("sin()", 0)]
    [InlineData("sin 5", 4)]
    [InlineData("log(8)", 0)]
    [InlineData("atan2(1, 2, 3)", 0)]
    [InlineData("3 # 4", 2)]
    [InlineData(".", 0)]
    [InlineData("1,2", 1)]
    [InlineData("min()", 0)]
    public void Parse_ReportsErrorsWithPosition(string text, int position)
    {
        var exception = Assert.Throws<ParserException>(() => MathExpression.Parse(text));
        Assert.Equal(position, exception.Position);
    }

    [Theory]
    [InlineData("cot(x)", 0)]
    [InlineData("1 + foo(2)", 4)]
    [InlineData("xy(2)", 0)]
    [InlineData("2 * sinx (2)", 4)]
    public void Parse_ReportsUnknownFunctions(string text, int position)
    {
        var exception = Assert.Throws<ParserException>(() => MathExpression.Parse(text));
        Assert.Equal(position, exception.Position);
        Assert.Contains("Unknown function", exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("x(x+1)", 2, 6)]
    [InlineData("pi(2)", 0, 2 * Math.PI)]
    [InlineData("xsin(x)", 0, 0)]
    [InlineData("2 (x)", 3, 6)]
    public void Parentheses_AfterVariablesAndConstants_StillMultiply(string text, double x, double expected) =>
        Assert.Equal(expected, MathExpression.Parse(text).Evaluate("x", x), 12);

    [Fact]
    public void DeeplyNestedOrHugeInput_IsRejectedInsteadOfCrashing()
    {
        Assert.Throws<ParserException>(() => MathExpression.Parse(new string('(', 300) + "1" + new string(')', 300)));
        Assert.Throws<ParserException>(() => MathExpression.Parse(new string('-', 2000) + "x"));
        Assert.Throws<ParserException>(() => MathExpression.Parse(string.Join("^", Enumerable.Repeat("x", 2000))));
        Assert.Throws<ParserException>(() => MathExpression.Parse(string.Join("+", Enumerable.Repeat("x", 5000))));

        Assert.Equal(1, MathExpression.Parse(new string('(', 100) + "1" + new string(')', 100)).Evaluate());
        Assert.Equal(3000, MathExpression.Parse(string.Join("+", Enumerable.Repeat("x", 3000))).Evaluate("x", 1));
        Assert.Equal(5000, MathExpression.Parse(string.Join("+", Enumerable.Repeat("1", 5000))).Evaluate());
    }

    [Fact]
    public void ParserException_HasStandardConstructors()
    {
        Assert.Equal(-1, new ParserException().Position);
        Assert.Equal(-1, new ParserException("x").Position);
        Assert.Equal(-1, new ParserException("x", new InvalidOperationException()).Position);
        Assert.Contains("position 3", new ParserException("x", 3).Message, StringComparison.Ordinal);
    }
}
