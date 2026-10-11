namespace SharpMath.Expressions;

/// <summary>
///     A recursive-descent parser for mathematical expressions.
/// </summary>
/// <remarks>
///     Grammar, from lowest to highest precedence:
///     <code>
///     additive       = multiplicative { ("+" | "-") multiplicative }
///     multiplicative = unary { ("*" | "/" | "%") unary | implicit power }
///     unary          = ("+" | "-") unary | power
///     power          = postfix [ "^" unary ]             (right-associative)
///     postfix        = primary { "!" }
///     primary        = number | constant | variable | function "(" [ additive { "," additive } ] ")" | "(" additive ")"
///     </code>
///     Implicit multiplication applies when an operand is directly followed by a name or an opening parenthesis.
/// </remarks>
internal sealed class ExpressionParser
{
    /// <summary>
    ///     The maximum nesting of parentheses, signs and exponents. Parsing recurses several frames per level.
    /// </summary>
    public const int MaxNesting = 256;

    /// <summary>
    ///     The maximum height of the expression tree, e.g. the number of terms in a sum. Evaluation recurses one frame
    ///     per level. Both limits prevent stack overflows, which cannot be caught.
    /// </summary>
    public const int MaxTreeDepth = 4096;

    private readonly string _text;
    private readonly List<Token> _tokens;
    private int _depth;
    private readonly Dictionary<string, int> _variableSlots = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _variableNames = [];
    private int _index;

    private ExpressionParser(string text, ExpressionContext context)
    {
        _text = text;
        _tokens = Tokenizer.Tokenize(text, context);
    }

    private Token Current => _tokens[_index];

    public static (Node Root, IReadOnlyList<string> Variables) Parse(string text, ExpressionContext context)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ParserException("The expression is empty.", 0);

        var parser = new ExpressionParser(text, context);
        var root = parser.ParseAdditive();
        if (parser.Current.Kind != TokenKind.End)
            throw parser.Unexpected(parser.Current);
        return (root, parser._variableNames.AsReadOnly());
    }

    private Node ParseAdditive()
    {
        Enter();
        var left = ParseMultiplicative();
        while (Current.Kind is TokenKind.Plus or TokenKind.Minus)
        {
            var op = Next().Kind == TokenKind.Plus ? BinaryOperator.Add : BinaryOperator.Subtract;
            left = Binary(op, left, ParseMultiplicative());
        }

        _depth--;
        return left;
    }

    private Node ParseMultiplicative()
    {
        var left = ParseUnary();
        while (true)
        {
            switch (Current.Kind)
            {
                case TokenKind.Star:
                    Next();
                    left = Binary(BinaryOperator.Multiply, left, ParseUnary());
                    break;
                case TokenKind.Slash:
                    Next();
                    left = Binary(BinaryOperator.Divide, left, ParseUnary());
                    break;
                case TokenKind.Percent:
                    Next();
                    left = Binary(BinaryOperator.Remainder, left, ParseUnary());
                    break;
                case TokenKind.Name or TokenKind.OpenParenthesis:
                    left = Binary(BinaryOperator.Multiply, left, ParsePower());
                    break;
                case TokenKind.Number:
                    throw new ParserException("A number cannot directly follow another operand; add an operator.",
                        Current.Position);
                default:
                    return left;
            }
        }
    }

    private Node ParseUnary()
    {
        Enter();
        Node result;
        switch (Current.Kind)
        {
            case TokenKind.Plus:
                Next();
                result = ParseUnary();
                break;
            case TokenKind.Minus:
                var position = Next().Position;
                var operand = ParseUnary();
                result = operand is ConstantNode constant
                    ? new ConstantNode(-constant.Value)
                    : Limit(new NegateNode(operand), position);
                break;
            default:
                result = ParsePower();
                break;
        }

        _depth--;
        return result;
    }

    private Node ParsePower()
    {
        var basis = ParsePostfix();
        if (Current.Kind != TokenKind.Caret)
            return basis;

        Next();
        return Binary(BinaryOperator.Power, basis, ParseUnary());
    }

    private Node ParsePostfix()
    {
        var operand = ParsePrimary();
        while (Current.Kind == TokenKind.Exclamation)
        {
            var position = Next().Position;
            operand = operand is ConstantNode constant
                ? new ConstantNode(FactorialNode.Factorial(constant.Value))
                : Limit(new FactorialNode(operand), position);
        }

        return operand;
    }

    private Node ParsePrimary()
    {
        var token = Next();
        switch (token.Kind)
        {
            case TokenKind.Number:
                return new ConstantNode(token.Number);
            case TokenKind.OpenParenthesis:
                var inner = ParseAdditive();
                Expect(TokenKind.CloseParenthesis, token);
                return inner;
            case TokenKind.Name:
                return token.Symbol switch
                {
                    ConstantSymbol constant => new ConstantNode(constant.Value),
                    FunctionSymbol function => ParseFunction(token, function),
                    _ => Variable(token)
                };
            default:
                throw Unexpected(token);
        }
    }

    private Node ParseFunction(Token nameToken, FunctionSymbol function)
    {
        var name = _text.Substring(nameToken.Position, nameToken.Length);
        if (Current.Kind != TokenKind.OpenParenthesis)
            throw new ParserException($"The function '{name}' must be followed by '('.", Current.Position);
        var openParenthesis = Next();

        var arguments = new List<Node>();
        if (Current.Kind != TokenKind.CloseParenthesis)
        {
            arguments.Add(ParseAdditive());
            while (Current.Kind == TokenKind.Comma)
            {
                Next();
                arguments.Add(ParseAdditive());
            }
        }

        Expect(TokenKind.CloseParenthesis, openParenthesis);

        if (arguments.Count < function.MinArguments || arguments.Count > function.MaxArguments)
            throw new ParserException(
                $"The function '{name}' expects {DescribeArity(function)}, but got {arguments.Count}.",
                nameToken.Position);

        Node node = function switch
        {
            { Unary: { } unary } => new UnaryFunctionNode(unary, arguments[0]),
            { Binary: { } binary } => new BinaryFunctionNode(binary, arguments[0], arguments[1]),
            _ => new VariadicFunctionNode(function.Variadic!, [.. arguments])
        };

        return function.IsPure && arguments.TrueForAll(argument => argument is ConstantNode)
            ? new ConstantNode(node.Evaluate([]))
            : Limit(node, nameToken.Position);
    }

    private VariableNode Variable(Token token)
    {
        var name = _text.Substring(token.Position, token.Length).ToLowerInvariant();
        if (!_variableSlots.TryGetValue(name, out var slot))
        {
            slot = _variableNames.Count;
            _variableSlots.Add(name, slot);
            _variableNames.Add(name);
        }

        return new VariableNode(slot);
    }

    private Node Binary(BinaryOperator op, Node left, Node right) =>
        left is ConstantNode l && right is ConstantNode r
            ? new ConstantNode(BinaryNode.Apply(op, l.Value, r.Value))
            : Limit(new BinaryNode(op, left, right), Current.Position);

    private void Enter()
    {
        if (++_depth > MaxNesting)
            throw new ParserException($"The expression is nested more than {MaxNesting} levels deep.", Current.Position);
    }

    private static T Limit<T>(T node, int position) where T : Node =>
        node.Depth <= MaxTreeDepth
            ? node
            : throw new ParserException($"The expression is too complex (more than {MaxTreeDepth} nested operations).",
                position);

    private Token Next()
    {
        var token = _tokens[_index];
        if (token.Kind != TokenKind.End)
            _index++;
        return token;
    }

    private void Expect(TokenKind kind, Token openingToken)
    {
        if (Current.Kind == kind)
        {
            Next();
            return;
        }

        if (Current.Kind == TokenKind.End)
            throw new ParserException($"The parenthesis opened at position {openingToken.Position} is not closed.",
                Current.Position);
        throw Unexpected(Current);
    }

    private ParserException Unexpected(Token token) => token.Kind switch
    {
        TokenKind.End => new ParserException("Unexpected end of expression.", token.Position),
        TokenKind.CloseParenthesis => new ParserException("Unmatched ')'.", token.Position),
        _ => new ParserException($"Unexpected '{_text.Substring(token.Position, token.Length)}'.", token.Position)
    };

    private static string DescribeArity(FunctionSymbol function) =>
        function.MinArguments == function.MaxArguments
            ? $"{function.MinArguments} argument(s)"
            : function.MaxArguments == int.MaxValue
                ? $"at least {function.MinArguments} argument(s)"
                : $"{function.MinArguments} to {function.MaxArguments} arguments";
}
