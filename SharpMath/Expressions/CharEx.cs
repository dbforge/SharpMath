// CharEx.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

namespace SharpMath.Expressions
{
    public class CharEx
    {
        public static bool IsBracket(char c)
        {
            return c == '(' || c == ')';
        }

        public static bool IsMathematicOperator(char c)
        {
            return "+-*/%^!".IndexOf(c) >= 0;
        }
    }
}