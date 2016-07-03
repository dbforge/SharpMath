// Author: Dominic Beger (Trade/ProgTrade) 2016

using System.Collections.Generic;
using SharpMath.Expressions;
using SharpMath.Expressions.Exceptions;

namespace SharpMath
{
    /// <summary>
    ///     Provides algorithms for common mathematical operations.
    /// </summary>
    public static class Algorithms
    {
        /// <summary>
        ///     Converts infix tokens to postfix tokens.
        /// </summary>
        /// <param name="infixTokens">The infix tokens to convert.</param>
        /// <returns>The postfix tokens.></returns>
        public static IEnumerable<Token> ShuntingYard(List<Token> infixTokens)
        {
            var opStack = new Stack<Token<string>>();
            var output = new List<Token>(infixTokens.Count);

            Token lastToken = null;
            foreach (var token in infixTokens)
            {
                if (lastToken != null)
                {
                    // A constant value or number must be followed by an operator or closing bracket...
                    if ((lastToken.Type == TokenType.Constant || lastToken.Type == TokenType.Number) &&
                        (token.Type != TokenType.Operator && !token.IsClosingBracket()))
                    {
                        throw new ParserException(
                            "Cannot calculate the postfix tokens of the given input as the term is invalid. A constant or number must be followed by an operator or a closing bracket.");
                    }

                    // A closing bracket must be followed by an operator or another closing bracket...
                    if (lastToken.IsClosingBracket() && (token.Type != TokenType.Operator && !token.IsClosingBracket()))
                    {
                        throw new ParserException(
                            "Cannot calculate the postfix tokens of the given input as the term is invalid. A closing bracket must be followed by an operator or another closing bracket.");
                    }
                }

                lastToken = token;
                switch (token.Type)
                {
                    case TokenType.Number:
                        output.Add((Token<double>) token);
                        break;
                    case TokenType.Function:
                        opStack.Push((Token<string>) token);
                        break;
                    case TokenType.Operator:
                        Token<string> currentOperatorToken = (Token<string>) token;
                        while (opStack.Count > 0 && opStack.Peek().Type == TokenType.Operator &&
                               (!currentOperatorToken.IsRightAssociative &&
                               (currentOperatorToken.Priority == opStack.Peek().Priority) || currentOperatorToken.Priority < opStack.Peek().Priority))
                            output.Add(opStack.Pop());
                        opStack.Push(currentOperatorToken);
                        break;
                    case TokenType.Bracket:
                        Token<string> bracketToken = (Token<string>) token;
                        switch (bracketToken.Value)
                        {
                            case "(":
                                opStack.Push(bracketToken);
                                break;
                            case ")":
                                while ((opStack.Peek()).Value != "(")
                                    output.Add(opStack.Pop());
                                opStack.Pop();

                                if (opStack.Count > 0 && (opStack.Peek().Type == TokenType.Function))
                                    output.Add(opStack.Pop());
                                break;
                        }
                        break;
                    case TokenType.Constant:
                        output.Add((Token<string>)token);
                        break;
                }
            }

            while (opStack.Count > 0) // Flush the remaining stuff
                output.Add(opStack.Pop());

            return output;
        }
    }
}