// EquationNotSolvableException.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System;

namespace SharpMath.Equations.Exceptions
{
    /// <summary>
    ///     The exception that is thrown when an equation is not solvable.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class EquationNotSolvableException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EquationNotSolvableException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EquationNotSolvableException(string message) : base(message)
        {
        }
    }
}