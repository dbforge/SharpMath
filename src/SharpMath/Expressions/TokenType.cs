// TokenType.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

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