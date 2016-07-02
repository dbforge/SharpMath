// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SharpMath.Expressions.Exceptions;

// ReSharper disable InconsistentNaming

namespace SharpMath.Expressions
{
    /// <summary>
    ///     Represents a part of a mathematic term.
    /// </summary>
    public class Token
    {
        private static readonly Regex SWhitespace = new Regex(@"\s+");

        protected static Dictionary<string, Action<Stack<double>>> _functionActions = new Dictionary
            <string, Action<Stack<double>>>
        {
            {"sin", stack => stack.Push(Math.Sin(stack.Pop()))},
            {"cos", stack => stack.Push(Math.Cos(stack.Pop()))},
            {"tan", stack => stack.Push(Math.Tan(stack.Pop()))},
            {"asin", stack => stack.Push(Math.Asin(stack.Pop()))},
            {"acos", stack => stack.Push(Math.Acos(stack.Pop()))},
            {"atan", stack => stack.Push(Math.Atan(stack.Pop()))},
            {"sqrt", stack => stack.Push(Math.Sqrt(stack.Pop()))},
            {"abs", stack => stack.Push(Math.Abs(stack.Pop()))},
            {"ln", stack => stack.Push(Math.Log(stack.Pop()))},
            {"lg", stack => stack.Push(Math.Log(stack.Pop(), 10))},
            {
                "log", stack =>
                {
                    double exponent = stack.Pop();
                    stack.Push(Math.Log(exponent, stack.Pop()));
                }
            }
        };

        protected static Dictionary<string, Action<Stack<double>>> _constantActions = new Dictionary
            <string, Action<Stack<double>>>
        {
            {"pi", stack => stack.Push(Math.PI)},
            {"e", stack => stack.Push(Math.E)}
        };

        protected static Dictionary<string, Action<Stack<double>>> _operatorActions = new Dictionary
            <string, Action<Stack<double>>>
        {
            {"+", stack => stack.Push(stack.Pop() + stack.Pop())},
            {
                "-", stack =>
                {
                    double subtrahend = stack.Pop();
                    stack.Push(stack.Pop() - subtrahend);
                }
            },
            {"*", stack => stack.Push(stack.Pop()*stack.Pop())},
            {
                "/", stack =>
                {
                    double divisor = stack.Pop();
                    stack.Push(stack.Pop()/divisor);
                }
            },
            {"!", stack => stack.Push(-stack.Pop())},
            {
                "^", stack =>
                {
                    double exponent = stack.Pop();
                    stack.Push(Math.Pow(stack.Pop(), exponent));
                }
            }
        };

        /// <summary>
        ///     Initializes a new instance of the <see cref="Token" /> class.
        /// </summary>
        /// <param name="type">The type of the <see cref="Token" />.</param>
        public Token(TokenType type)
        {
            Type = type;
        }

        /// <summary>
        ///     Gets or sets the type of this <see cref="Token" />.
        /// </summary>
        public TokenType Type { get; set; }

        /// <summary>
        ///     Reads a number token in the input string starting at the specified index.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="index">The index to start at.</param>
        /// <returns>The <see cref="Token" /> that has been read.</returns>
        public static Token<double> ReadNumberToken(string input, ref int index)
        {
            int endIndex = index;
            while (endIndex < input.Length && (char.IsDigit(input, endIndex) || input[endIndex] == '.'))
                endIndex++;
            string numberToken = input.Substring(index, endIndex - index);
            double numberValue;
            if (
                !double.TryParse(numberToken, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                    out numberValue))
                throw new ParserException($"Invalid token: {numberToken} is not a valid number.");
            index = endIndex;
            return new Token<double>(numberValue, TokenType.Number);
        }

        /// <summary>
        ///     Reads a bracket, constant, operator or function token in the input string starting at the specified index.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="index">The index to start at.</param>
        /// <returns>The <see cref="Token" /> that has been read.</returns>
        public static Token<string> ReadStringToken(string input, ref int index)
        {
            TokenType newType;
            int endIndex = index;
            while (endIndex < input.Length &&
                   (char.IsLetter(input, endIndex) || CharEx.IsMathematicOperator(input[endIndex]) ||
                    CharEx.IsBracket(input[endIndex])))
            {
                endIndex++;
                string currentString = input.Substring(index, endIndex - index);
                if (_functionActions.ContainsKey(currentString) || _operatorActions.ContainsKey(currentString) || _constantActions.ContainsKey(currentString) ||
                    currentString.IsBracket()) // Last two conditions to parse e.g. "* sin(5)" correctly: As soon as the current token has finished, we should stop reading.
                    break; // This indicates that it has been found in the dictionary, so that is already it.
            }

            string stringToken = input.Substring(index, endIndex - index);

            if (_functionActions.ContainsKey(stringToken))
                newType = TokenType.Function;
            else if (_operatorActions.ContainsKey(stringToken))
                newType = TokenType.Operator;
            else if (stringToken.IsBracket())
                newType = TokenType.Bracket;
            else if (_constantActions.ContainsKey(stringToken))
                newType = TokenType.Constant;
            else
                throw new ParserException(
                    $"Invalid token: {stringToken} is not a valid function/operator, bracket or number equivalent.");

            index = endIndex;
            return new Token<string>(stringToken, newType);
        }

        /// <summary>
        ///     Evaluates this <see cref="Token" />.
        /// </summary>
        /// <param name="stack">The <see cref="Stack{double}" /> to take relating from and push the result to.</param>
        public void Evaluate(Stack<double> stack)
        {
            switch (Type)
            {
                case TokenType.Number:
                    var token = this as Token<double>;
                    if (token != null)
                        stack.Push(token.Value);
                    break;
                case TokenType.Function:
                    var token1 = this as Token<string>;
                    if (token1 != null)
                        _functionActions[token1.Value](stack);
                    break;
                case TokenType.Operator:
                    var token2 = this as Token<string>;
                    if (token2 != null)
                        _operatorActions[token2.Value](stack);
                    break;
                case TokenType.Constant:
                    var token3 = this as Token<string>;
                    if (token3 != null)
                        _constantActions[token3.Value](stack);
                    break;
            }
        }

        /// <summary>
        ///     Creates an <see cref="IEnumerable{Token}" /> containing infix tokens from partitioning the specified term.
        /// </summary>
        /// <param name="term">The term to partition into infix tokens.</param>
        /// <returns><see cref="IEnumerable{Token}" /> containing the created infix tokens.</returns>
        public static IEnumerable<Token> CalculateInfixTokens(string term)
        {
            var infixTokens = new List<Token>();
            term = SWhitespace.Replace(term, string.Empty).ToLowerInvariant();
            int i = 0;
            while (i < term.Length)
            {
                char current = term[i];
                if (char.IsLetter(current) || CharEx.IsMathematicOperator(current) || CharEx.IsBracket(current))
                    // Functions/Operators/Constants
                {
                    if ((current == '+' || current == '-') &&
                        (i == 0 ||
                         (infixTokens.Last().Type == TokenType.Bracket &&
                          ((Token<string>) infixTokens.Last()).Value == "(")))
                        // Must be a sign and handled differently.
                    {
                        if (current == '-')
                        {
                            var sb = new StringBuilder(term) {[term.IndexOf(current)] = '!'};
                            term = sb.ToString();
                        }
                        else
                            term.Remove(term.IndexOf(current), 1);
                        // Remove the '+' as it is redundant and disturbs our calculations
                    }
                    infixTokens.Add(ReadStringToken(term, ref i));
                }
                else if (char.IsDigit(current)) // Numbers
                    infixTokens.Add(ReadNumberToken(term, ref i));
                else
                    throw new ParserException($"Char {current} cannot be interpreted as a valid token.");
            }

            return infixTokens;
        }
    }

    /// <summary>
    ///     Represents a part of a mathematic term with a specific data type that should represent it.
    /// </summary>
    /// <typeparam name="T">The data type of the <see cref="Token" /> that represents it.</typeparam>
    public class Token<T> : Token
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Token{T}" /> class.
        /// </summary>
        /// <param name="value">The value of the <see cref="Token{T}" />.</param>
        /// <param name="type">The type of the <see cref="Token{T}" />.</param>
        public Token(T value, TokenType type)
            : base(type)
        {
            Value = value;
            switch (type)
            {
                case TokenType.Number:
                case TokenType.Constant:
                    Priority = 100;
                    break;
                case TokenType.Operator:
                    switch (value as string)
                    {
                        case "+":
                            Priority = 1;
                            IsRightAssociative = false;
                            break;
                        case "-":
                            Priority = 1;
                            IsRightAssociative = false;
                            break;
                        case "*":
                            Priority = 2;
                            IsRightAssociative = false;
                            break;
                        case "/":
                            Priority = 2;
                            IsRightAssociative = false;
                            break;
                        case "%":
                            Priority = 3;
                            IsRightAssociative = false;
                            break;
                        case "!":
                            Priority = 4;
                            IsRightAssociative = false;
                            break;
                        case "^":
                            Priority = 5;
                            IsRightAssociative = true;
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        ///     Gets or sets the value of this <see cref="Token{T}" />.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        ///     Gets or sets the priority of this <see cref="Token{T}" />.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Token" /> is right associative, or not.
        /// </summary>
        public bool IsRightAssociative { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}