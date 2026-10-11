namespace SharpMath.Expressions;

/// <summary>
///     Represents a parsed mathematical expression that can be evaluated many times, e.g. <c>2x^2 - 3x + sin(x)</c>.
/// </summary>
/// <remarks>
///     <para>
///         Supported syntax: numbers (<c>12</c>, <c>1.5</c>, <c>.5</c>, <c>1e5</c>), the operators <c>+ - * / % ^</c>
///         (<c>^</c> is right-associative and binds tighter than unary minus, so <c>-2^2 = -4</c>), postfix factorial
///         <c>!</c>, parentheses, function calls with comma-separated arguments, constants and variables (see
///         <see cref="ExpressionContext" />).
///     </para>
///     <para>
///         Multiplication may be implicit when an operand is directly followed by a name or an opening parenthesis:
///         <c>2x</c>, <c>2pi</c>, <c>3(x+1)</c>, <c>(x+1)(x-1)</c>. It has the same precedence as <c>*</c>, so
///         <c>1/2x</c> is <c>(1/2)·x</c>. Letter sequences are split by longest match against the known names, and any
///         other single letter is a variable: <c>xy</c> is <c>x·y</c>, <c>pix</c> is <c>pi·x</c>.
///     </para>
///     <para>Instances are immutable and thread-safe. Evaluation does not allocate.</para>
/// </remarks>
public sealed class MathExpression
{
    private readonly Node _root;

    private MathExpression(string text, Node root, IReadOnlyList<string> variables)
    {
        Text = text;
        _root = root;
        Variables = variables;
    }

    /// <summary>
    ///     Gets the source text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///     Gets the names of the variables used in the expression, in lower case and in order of first occurrence.
    /// </summary>
    public IReadOnlyList<string> Variables { get; }

    /// <summary>
    ///     Parses an expression.
    /// </summary>
    /// <param name="text">The expression.</param>
    /// <param name="context">The functions, constants and variables to use; the built-ins if <see langword="null" />.</param>
    /// <returns>The parsed expression.</returns>
    /// <exception cref="ParserException">The expression is invalid; <see cref="ParserException.Position" /> tells where.</exception>
    public static MathExpression Parse(string text, ExpressionContext? context = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        var (root, variables) = ExpressionParser.Parse(text, context ?? ExpressionContext.Default);
        return new MathExpression(text, root, variables);
    }

    /// <summary>
    ///     Evaluates an expression without variables.
    /// </summary>
    /// <returns>The result.</returns>
    /// <exception cref="ArgumentException">The expression uses variables.</exception>
    public double Evaluate()
    {
        if (Variables.Count > 0)
            throw MissingVariables(Variables);
        return _root.Evaluate([]);
    }

    /// <summary>
    ///     Evaluates an expression with at most one variable.
    /// </summary>
    /// <param name="name">The name of the variable (case-insensitive).</param>
    /// <param name="value">The value of the variable.</param>
    /// <returns>The result.</returns>
    /// <exception cref="ArgumentException">The expression uses another variable.</exception>
    public double Evaluate(string name, double value)
    {
        ArgumentNullException.ThrowIfNull(name);
        switch (Variables.Count)
        {
            case 0:
                return _root.Evaluate([]);
            case 1 when string.Equals(Variables[0], name, StringComparison.OrdinalIgnoreCase):
                return _root.Evaluate([value]);
            default:
                throw MissingVariables(Variables.Where(variable =>
                    !string.Equals(variable, name, StringComparison.OrdinalIgnoreCase)));
        }
    }

    /// <summary>
    ///     Evaluates the expression with the specified variable values. Names are matched case-insensitively; unused
    ///     entries are ignored.
    /// </summary>
    /// <param name="variables">The values of the variables.</param>
    /// <returns>The result.</returns>
    /// <exception cref="ArgumentException">A variable used in the expression has no value.</exception>
    public double Evaluate(IReadOnlyDictionary<string, double> variables)
    {
        ArgumentNullException.ThrowIfNull(variables);
        Span<double> values = Variables.Count <= 32 ? stackalloc double[Variables.Count] : new double[Variables.Count];
        List<string>? missing = null;
        for (var i = 0; i < Variables.Count; i++)
        {
            if (TryFind(variables, Variables[i], out var value))
                values[i] = value;
            else
                (missing ??= []).Add(Variables[i]);
        }

        if (missing is not null)
            throw MissingVariables(missing);
        return _root.Evaluate(values);
    }

    /// <summary>
    ///     Creates a function of one variable, e.g. for plotting.
    /// </summary>
    /// <param name="variable">The name of the variable (case-insensitive); the expression may also not use it at all.</param>
    /// <returns>A delegate that evaluates the expression without allocating.</returns>
    /// <exception cref="ArgumentException">The expression uses other variables.</exception>
    public Func<double, double> ToFunc(string variable)
    {
        ArgumentNullException.ThrowIfNull(variable);
        var others = Variables.Where(name => !string.Equals(name, variable, StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (others.Count > 0)
            throw MissingVariables(others);

        var root = _root;
        if (Variables.Count == 0)
            return _ => root.Evaluate([]);
        return x => root.Evaluate([x]);
    }

    /// <inheritdoc />
    public override string ToString() => Text;

    private static bool TryFind(IReadOnlyDictionary<string, double> variables, string name, out double value)
    {
        if (variables.TryGetValue(name, out value))
            return true;
        foreach (var (key, candidate) in variables)
        {
            if (!string.Equals(key, name, StringComparison.OrdinalIgnoreCase))
                continue;
            value = candidate;
            return true;
        }

        return false;
    }

    private static ArgumentException MissingVariables(IEnumerable<string> names) =>
        new($"No value was given for the variable(s): {string.Join(", ", names)}.");
}
