// Author: Dominic Beger (Trade/ProgTrade) 2016

namespace SharpMath.Expressions
{
    public class CharEx
    {
        public static bool IsMathematicOperator(char c)
        {
            return "+-*/%^!".IndexOf(c) >= 0;
        }

        public static bool IsBracket(char c)
        {
            return c == '(' || c == ')';
        }
    }
}