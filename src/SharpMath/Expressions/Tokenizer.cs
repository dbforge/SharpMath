using System.Globalization;

namespace SharpMath.Expressions;

internal enum TokenKind
{
    Number,
    Name,
    Plus,
    Minus,
    Star,
    Slash,
    Percent,
    Caret,
    Exclamation,
    OpenParenthesis,
    CloseParenthesis,
    Comma,
    End
}

internal readonly record struct Token(TokenKind Kind, int Position, int Length, double Number = 0, Symbol? Symbol = null);

/// <summary>
///     Splits an expression into tokens. Runs of letters are split into names by longest match against the context
///     (functions, constants, declared variables); any remaining single letter becomes a variable. Digits after a
///     letter that are not part of a known name start a number.
/// </summary>
internal static class Tokenizer
{
    public static List<Token> Tokenize(string text, ExpressionContext context)
    {
        var tokens = new List<Token>();
        var position = 0;
        while (position < text.Length)
        {
            var current = text[position];
            if (char.IsWhiteSpace(current))
            {
                position++;
                continue;
            }

            if (char.IsAsciiDigit(current) || current == '.')
            {
                tokens.Add(ReadNumber(text, ref position));
                continue;
            }

            if (char.IsAsciiLetter(current))
            {
                ReadNames(text, context, ref position, tokens);
                continue;
            }

            var kind = current switch
            {
                '+' => TokenKind.Plus,
                '-' => TokenKind.Minus,
                '*' => TokenKind.Star,
                '/' => TokenKind.Slash,
                '%' => TokenKind.Percent,
                '^' => TokenKind.Caret,
                '!' => TokenKind.Exclamation,
                '(' => TokenKind.OpenParenthesis,
                ')' => TokenKind.CloseParenthesis,
                ',' => TokenKind.Comma,
                _ => throw new ParserException($"Unexpected character '{current}'.", position)
            };
            tokens.Add(new Token(kind, position, 1));
            position++;
        }

        tokens.Add(new Token(TokenKind.End, text.Length, 0));
        return tokens;
    }

    private static Token ReadNumber(string text, ref int position)
    {
        var start = position;
        while (position < text.Length && char.IsAsciiDigit(text[position]))
            position++;
        if (position < text.Length && text[position] == '.')
        {
            position++;
            while (position < text.Length && char.IsAsciiDigit(text[position]))
                position++;
        }

        // Scientific notation only if the exponent has digits; "2e" is 2·e.
        if (position < text.Length && text[position] is 'e' or 'E')
        {
            var exponent = position + 1;
            if (exponent < text.Length && text[exponent] is '+' or '-')
                exponent++;
            if (exponent < text.Length && char.IsAsciiDigit(text[exponent]))
            {
                position = exponent;
                while (position < text.Length && char.IsAsciiDigit(text[position]))
                    position++;
            }
        }

        var literal = text.AsSpan(start, position - start);
        if (!double.TryParse(literal, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture, out var value))
            throw new ParserException($"'{literal}' is not a valid number.", start);
        return new Token(TokenKind.Number, start, position - start, value);
    }

    /// <summary>
    ///     Splits a run of letters and digits into names. A run of two or more characters directly in front of "(" must
    ///     end with a function or a known name; otherwise it is reported as an unknown function.
    /// </summary>
    private static void ReadNames(string text, ExpressionContext context, ref int position, List<Token> tokens)
    {
        var runStart = position;
        var runEnd = position;
        while (runEnd < text.Length && char.IsAsciiLetterOrDigit(text[runEnd]))
            runEnd++;

        while (position < runEnd && char.IsAsciiLetter(text[position]))
            tokens.Add(ReadName(text, context, runEnd, ref position));

        var next = runEnd;
        while (next < text.Length && char.IsWhiteSpace(text[next]))
            next++;
        var lastSymbol = tokens[^1].Symbol;
        if (position == runEnd && runEnd - runStart >= 2 && next < text.Length && text[next] == '(' &&
            lastSymbol is VariableSymbol { IsDeclared: false })
            throw new ParserException($"Unknown function '{text[runStart..runEnd]}'.", runStart);
    }

    private static Token ReadName(string text, ExpressionContext context, int runEnd, ref int position)
    {
        var start = position;
        var maxLength = Math.Min(runEnd - start, context.LongestNameLength);
        for (var length = maxLength; length > 0; length--)
        {
            if (!context.TryGetSymbol(text.AsSpan(start, length), out var symbol))
                continue;
            position += length;
            return new Token(TokenKind.Name, start, length, Symbol: symbol);
        }

        // Undefined single letter: an implicit variable.
        position++;
        return new Token(TokenKind.Name, start, 1, Symbol: VariableSymbol.Implicit);
    }
}
