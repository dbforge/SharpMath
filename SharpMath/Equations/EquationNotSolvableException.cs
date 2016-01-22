using System;

namespace SharpMath.Equations
{
    public class EquationNotSolvableException : Exception
    {
        public EquationNotSolvableException(string message) : base(message)
        { }
    }
}