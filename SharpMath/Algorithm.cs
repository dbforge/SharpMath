// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Collections.Generic;
using SharpMath.Equations.Exceptions;
using SharpMath.Expressions;
using SharpMath.Geometry;

namespace SharpMath
{
    /// <summary>
    ///     Provides algorithms for common mathematical operations.
    /// </summary>
    public static class Algorithms
    {
        /// <summary>
        ///     Implements the Gauss-Jordan-algorithm.
        /// </summary>
        /// <param name="leftSide">The left side <see cref="Matrix" />.</param>
        /// <param name="rightSide">The right side <see cref="Matrix" />.</param>
        /// <returns>The resulting <see cref="Matrix" />.</returns>
        /// <exception cref="EquationNotSolvableException">The linear equation system cannot be solved clearly.</exception>
        public static Matrix GaussJordan(Matrix leftSide, Matrix rightSide)
        {
            for (uint x = 0; x < leftSide.ColumnCount; x++)
            {
                uint nextX = x;
                while (Math.Abs(leftSide[x, x]) < FloatingNumber.Epsilon)
                {
                    nextX++;

                    if (nextX >= leftSide.ColumnCount)
                        throw new EquationNotSolvableException("The linear equation system cannot be solved clearly.");

                    if (Math.Abs(leftSide[x, nextX]) < FloatingNumber.Epsilon)
                        continue;

                    leftSide.InterchangeRows(nextX, x);
                    rightSide.InterchangeRows(nextX, x);
                }

                for (uint y = 0; y < leftSide.RowCount; y++)
                {
                    if (y != x && Math.Abs(leftSide[y, x]) >= FloatingNumber.Epsilon)
                    {
                        double factor = leftSide[y, x]/leftSide[x, x];
                        leftSide.SubtractRows(y, x, factor);
                        rightSide.SubtractRows(y, x, factor);
                    }
                }
            }

            for (uint i = 0; i < leftSide.ColumnCount; i++)
            {
                double factor = 1/leftSide[i, i];
                leftSide.MultiplyRow(i, factor);
                rightSide.MultiplyRow(i, factor);
            }

            return rightSide;
        }

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
                }
            }

            while (opStack.Count > 0) // Flush the remaining stuff
                output.Add(opStack.Pop());

            return output;
        }
    }
}