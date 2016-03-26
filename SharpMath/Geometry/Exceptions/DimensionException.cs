// Author: Dominic Beger (Trade/ProgTrade) 2016
// Improvements: Stefan Baumann 2016

using System;

namespace SharpMath.Geometry.Exceptions
{
    /// <summary>
    /// The exception that is thrown when the dimension count of two vectors is different.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class DimensionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DimensionException(string message)
            : base(message)
        {
        }
    }
}