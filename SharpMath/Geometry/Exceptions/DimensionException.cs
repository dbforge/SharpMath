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
