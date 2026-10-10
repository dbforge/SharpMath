// Parser.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMath.Expressions
{
    /// <summary>
    ///     Provides functions to evaluate mathematic terms.
    /// </summary>
    public class Parser
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Parser" /> class.
        /// </summary>
        /// <param name="expression">The expression/term that should evaluated.</param>
        public Parser(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            Expression = expression;
        }

        /// <summary>
        ///     Gets or sets the expression/term that should be evaluated.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        ///     Evaluates the term.
        /// </summary>
        /// <returns>The result of the evaluation.</returns>
        public double Evaluate()
        {
            if (string.IsNullOrWhiteSpace(Expression))
                throw new Exception("The term that should be evaluated is empty.");

            var resultStack = new Stack<double>();
            var infixTokens = Token.CalculateInfixTokens(Expression);
            var postfixTokens = Algorithms.ShuntingYard(infixTokens.ToList());
            foreach (var pToken in postfixTokens)
                pToken.Evaluate(resultStack);

            return resultStack.Pop();
        }
    }
}