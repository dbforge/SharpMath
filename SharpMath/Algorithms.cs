// Author: Dominic Beger (Trade/ProgTrade) 2016

using System.Collections.Generic;
using SharpMath.Expressions;

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

            foreach (var token in infixTokens)
            {
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
                               !opStack.Peek().IsRightAssociative &&
                               currentOperatorToken.Priority <= opStack.Peek().Priority)
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