using System;

namespace SharpMath.Equations.Exceptions
{
    public class EquationNotSolvableException : Exception
    {
        public EquationNotSolvableException(string message) : base(message)
        { }
    }
}