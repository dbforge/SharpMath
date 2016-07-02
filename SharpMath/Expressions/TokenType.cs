// Author: Dominic Beger (Trade/ProgTrade) 2016

namespace SharpMath.Expressions
{
    /// <summary>
    ///     Represents the different token types of a mathematic term.
    /// </summary>
    public enum TokenType
    {
        Function,
        Operator,
        Number,
        Constant,
        Bracket
    }
}