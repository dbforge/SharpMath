using System;

namespace SharpMath.Expressions.Exceptions
{
    public class ParserException : Exception
    {
        public ParserException(string message)
            : base(message)
        {
        }
    }
}
