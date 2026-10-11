namespace SharpMath.Expressions;

/// <summary>
///     A node of a parsed expression tree. Variables are resolved to slot indices at parse time, so evaluation does not
///     allocate.
/// </summary>
internal abstract class Node(int depth)
{
    /// <summary>
    ///     Gets the height of the tree below this node (1 for leaves); evaluation recurses this deep.
    /// </summary>
    public int Depth { get; } = depth;

    public abstract double Evaluate(ReadOnlySpan<double> variables);

    protected static int DepthOf(Node[] children)
    {
        var depth = 0;
        foreach (var child in children)
            depth = Math.Max(depth, child.Depth);
        return depth + 1;
    }
}

internal sealed class ConstantNode(double value) : Node(1)
{
    public double Value { get; } = value;

    public override double Evaluate(ReadOnlySpan<double> variables) => Value;
}

internal sealed class VariableNode(int slot) : Node(1)
{
    public override double Evaluate(ReadOnlySpan<double> variables) => variables[slot];
}

internal sealed class NegateNode(Node operand) : Node(operand.Depth + 1)
{
    public override double Evaluate(ReadOnlySpan<double> variables) => -operand.Evaluate(variables);
}

internal sealed class FactorialNode(Node operand) : Node(operand.Depth + 1)
{
    private static readonly double[] Factorials = CreateFactorials();

    public override double Evaluate(ReadOnlySpan<double> variables) => Factorial(operand.Evaluate(variables));

    public static double Factorial(double value)
    {
        if (!(value >= 0) || value != Math.Floor(value))
            return double.NaN;
        return value < Factorials.Length ? Factorials[(int) value] : double.PositiveInfinity;
    }

    private static double[] CreateFactorials()
    {
        // 170! is the largest factorial representable as a double.
        var factorials = new double[171];
        factorials[0] = 1;
        for (var i = 1; i < factorials.Length; i++)
            factorials[i] = factorials[i - 1] * i;
        return factorials;
    }
}

internal enum BinaryOperator
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Remainder,
    Power
}

internal sealed class BinaryNode(BinaryOperator op, Node left, Node right) : Node(Math.Max(left.Depth, right.Depth) + 1)
{
    public override double Evaluate(ReadOnlySpan<double> variables) =>
        Apply(op, left.Evaluate(variables), right.Evaluate(variables));

    public static double Apply(BinaryOperator op, double left, double right) => op switch
    {
        BinaryOperator.Add => left + right,
        BinaryOperator.Subtract => left - right,
        BinaryOperator.Multiply => left * right,
        BinaryOperator.Divide => left / right,
        BinaryOperator.Remainder => left % right,
        _ => Math.Pow(left, right)
    };
}

internal sealed class UnaryFunctionNode(Func<double, double> function, Node argument) : Node(argument.Depth + 1)
{
    public override double Evaluate(ReadOnlySpan<double> variables) => function(argument.Evaluate(variables));
}

internal sealed class BinaryFunctionNode(Func<double, double, double> function, Node first, Node second)
    : Node(Math.Max(first.Depth, second.Depth) + 1)
{
    public override double Evaluate(ReadOnlySpan<double> variables) =>
        function(first.Evaluate(variables), second.Evaluate(variables));
}

internal sealed class VariadicFunctionNode(Func<ReadOnlySpan<double>, double> function, Node[] arguments)
    : Node(DepthOf(arguments))
{
    private const int StackLimit = 32;

    public override double Evaluate(ReadOnlySpan<double> variables)
    {
        Span<double> values = arguments.Length <= StackLimit
            ? stackalloc double[arguments.Length]
            : new double[arguments.Length];
        for (var i = 0; i < arguments.Length; i++)
            values[i] = arguments[i].Evaluate(variables);
        return function(values);
    }
}
