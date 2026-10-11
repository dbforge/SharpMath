using System.Diagnostics.CodeAnalysis;

namespace SharpMath.Expressions;

/// <summary>
///     Defines the functions, constants and multi-letter variables available when parsing a
///     <see cref="MathExpression" />.
/// </summary>
/// <remarks>
///     <para>
///         A new context contains the built-in functions <c>sin cos tan asin acos atan atan2(y, x) sinh cosh tanh sqrt
///         abs ln lg log(x, base) log2 exp floor ceil round sign pow(x, y) min(…) max(…)</c> and the constants
///         <c>pi</c>, <c>e</c> and <c>tau</c>. Names are case-insensitive and consist of a letter followed by letters or
///         digits. Adding a name that already exists replaces it. <c>new ExpressionContext()</c> is the default context;
///         use <see cref="Clone" /> to derive variants from a configured context.
///     </para>
///     <para>
///         Every instance is independent. A parsed expression captures the definitions at parse time, so later changes
///         to the context do not affect it.
///     </para>
/// </remarks>
public sealed class ExpressionContext
{
    private readonly Dictionary<string, Symbol> _symbols = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Initializes a new instance of the <see cref="ExpressionContext" /> class with the built-in functions and
    ///     constants.
    /// </summary>
    public ExpressionContext()
    {
        AddBuiltIn("sin", Math.Sin);
        AddBuiltIn("cos", Math.Cos);
        AddBuiltIn("tan", Math.Tan);
        AddBuiltIn("asin", Math.Asin);
        AddBuiltIn("acos", Math.Acos);
        AddBuiltIn("atan", Math.Atan);
        AddBuiltIn("atan2", Math.Atan2);
        AddBuiltIn("sinh", Math.Sinh);
        AddBuiltIn("cosh", Math.Cosh);
        AddBuiltIn("tanh", Math.Tanh);
        AddBuiltIn("sqrt", Math.Sqrt);
        AddBuiltIn("abs", Math.Abs);
        AddBuiltIn("ln", x => Math.Log(x));
        AddBuiltIn("lg", Math.Log10);
        AddBuiltIn("log", (x, b) => Math.Log(x, b));
        AddBuiltIn("log2", Math.Log2);
        AddBuiltIn("exp", Math.Exp);
        AddBuiltIn("floor", Math.Floor);
        AddBuiltIn("ceil", Math.Ceiling);
        AddBuiltIn("round", x => Math.Round(x, MidpointRounding.AwayFromZero));
        AddBuiltIn("sign", x => double.IsNaN(x) ? double.NaN : Math.Sign(x));
        AddBuiltIn("pow", Math.Pow);
        _symbols["min"] = new FunctionSymbol(1, int.MaxValue, Variadic: Min, IsPure: true);
        _symbols["max"] = new FunctionSymbol(1, int.MaxValue, Variadic: Max, IsPure: true);
        _symbols["pi"] = new ConstantSymbol(Math.PI);
        _symbols["e"] = new ConstantSymbol(Math.E);
        _symbols["tau"] = new ConstantSymbol(Math.Tau);
        UpdateLongestName();
    }

    internal static ExpressionContext Default { get; } = new();

    internal int LongestNameLength { get; private set; }

    /// <summary>
    ///     Adds or replaces a function with one argument.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="function">The function.</param>
    /// <returns>This context, for chaining.</returns>
    /// <exception cref="ArgumentException"><paramref name="name" /> is not a valid name.</exception>
    public ExpressionContext AddFunction(string name, Func<double, double> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        return Add(name, new FunctionSymbol(1, 1, Unary: function));
    }

    /// <summary>
    ///     Adds or replaces a function with two arguments.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="function">The function.</param>
    /// <returns>This context, for chaining.</returns>
    /// <exception cref="ArgumentException"><paramref name="name" /> is not a valid name.</exception>
    public ExpressionContext AddFunction(string name, Func<double, double, double> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        return Add(name, new FunctionSymbol(2, 2, Binary: function));
    }

    /// <summary>
    ///     Adds or replaces a function with a variable number of arguments.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="minArguments">The minimum number of arguments. Must not be negative.</param>
    /// <param name="maxArguments">
    ///     The maximum number of arguments; at least <paramref name="minArguments" />. Use
    ///     <see cref="int.MaxValue" /> for no limit.
    /// </param>
    /// <param name="function">The function; it receives the evaluated arguments.</param>
    /// <returns>This context, for chaining.</returns>
    /// <exception cref="ArgumentException"><paramref name="name" /> is not a valid name.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The argument counts are invalid.</exception>
    public ExpressionContext AddFunction(string name, int minArguments, int maxArguments,
        Func<ReadOnlySpan<double>, double> function)
    {
        ArgumentNullException.ThrowIfNull(function);
        ArgumentOutOfRangeException.ThrowIfNegative(minArguments);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxArguments, minArguments);
        return Add(name, new FunctionSymbol(minArguments, maxArguments, Variadic: function));
    }

    /// <summary>
    ///     Adds or replaces a constant.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="value">The value.</param>
    /// <returns>This context, for chaining.</returns>
    /// <exception cref="ArgumentException"><paramref name="name" /> is not a valid name.</exception>
    public ExpressionContext AddConstant(string name, double value) => Add(name, new ConstantSymbol(value));

    /// <summary>
    ///     Declares a variable whose name has more than one character. Single letters that are not otherwise defined are
    ///     variables automatically.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>This context, for chaining.</returns>
    /// <exception cref="ArgumentException"><paramref name="name" /> is not a valid name.</exception>
    public ExpressionContext DeclareVariable(string name) => Add(name, VariableSymbol.Declared);

    /// <summary>
    ///     Creates an independent copy of this context, e.g. to extend a shared configuration.
    /// </summary>
    /// <returns>The copy.</returns>
    public ExpressionContext Clone()
    {
        var clone = new ExpressionContext();
        clone._symbols.Clear();
        foreach (var (name, symbol) in _symbols)
            clone._symbols.Add(name, symbol);
        clone.UpdateLongestName();
        return clone;
    }

    internal bool TryGetSymbol(ReadOnlySpan<char> name, [NotNullWhen(true)] out Symbol? symbol) =>
        _symbols.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(name, out symbol);

    private ExpressionContext Add(string name, Symbol symbol)
    {
        ValidateName(name);
        _symbols[name] = symbol;
        UpdateLongestName();
        return this;
    }

    private void AddBuiltIn(string name, Func<double, double> function) =>
        _symbols[name] = new FunctionSymbol(1, 1, Unary: function, IsPure: true);

    private void AddBuiltIn(string name, Func<double, double, double> function) =>
        _symbols[name] = new FunctionSymbol(2, 2, Binary: function, IsPure: true);

    private void UpdateLongestName() => LongestNameLength = _symbols.Keys.Max(key => key.Length);

    private static void ValidateName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (name.Length == 0 || !char.IsAsciiLetter(name[0]) || !name.All(char.IsAsciiLetterOrDigit))
            throw new ArgumentException(
                "A name must start with an ASCII letter and contain only ASCII letters and digits.", nameof(name));
    }

    private static double Min(ReadOnlySpan<double> values)
    {
        var min = values[0];
        foreach (var value in values[1..])
            min = Math.Min(min, value);
        return min;
    }

    private static double Max(ReadOnlySpan<double> values)
    {
        var max = values[0];
        foreach (var value in values[1..])
            max = Math.Max(max, value);
        return max;
    }
}

internal abstract record Symbol;

internal sealed record ConstantSymbol(double Value) : Symbol;

/// <summary>
///     A variable: either declared in the context (multi-letter names) or an implicit single letter.
/// </summary>
internal sealed record VariableSymbol(bool IsDeclared) : Symbol
{
    public static VariableSymbol Declared { get; } = new(true);

    public static VariableSymbol Implicit { get; } = new(false);
}

internal sealed record FunctionSymbol(
    int MinArguments,
    int MaxArguments,
    Func<double, double>? Unary = null,
    Func<double, double, double>? Binary = null,
    Func<ReadOnlySpan<double>, double>? Variadic = null,
    bool IsPure = false) : Symbol;
