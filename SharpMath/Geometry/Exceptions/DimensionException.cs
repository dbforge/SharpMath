// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;

namespace SharpMath.Geometry.Exceptions
{
    public class DimensionException : Exception
    {
        public DimensionException(string message)
            : base(message)
        {
        }
    }
}