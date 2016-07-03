namespace SharpMath.Expressions
{
    public static class TokenEx
    {
        public static bool IsOpeningBracket(this Token token)
        {
            return token.Type == TokenType.Bracket && ((Token<string>)token).Value == "(";
        }

        public static bool IsClosingBracket(this Token token)
        {
            return token.Type == TokenType.Bracket && ((Token<string>) token).Value == ")";
        }
    }
}