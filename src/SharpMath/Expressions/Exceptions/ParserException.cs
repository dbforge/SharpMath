// ParserException.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System;

namespace SharpMath.Expressions.Exceptions
{
    /// <summary>
    ///     The exception that is thrown when the parser encountered an invalid token.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ParserException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ParserException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ParserException(string message)
            : base(message)
        {
        }
    }
}