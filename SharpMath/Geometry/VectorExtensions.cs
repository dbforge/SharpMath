using SharpMath.Geometry.Exceptions;

namespace SharpMath.Geometry
{
    public static class VectorExtensions
    {
        public static T Negate<T>(this Vector vector) where T : Vector, new()
        {
            var resultVector = new T();
            if (vector.Dimension != resultVector.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");

            for (uint i = 0; i < vector.Dimension; ++i)
                resultVector[i] = -vector[i];
            return resultVector;
        }

        /// <summary>
        ///     Calculates the normalized <see cref="Vector"/> of this <see cref="Vector"/>.
        /// </summary>
        /// <returns>The normalized <see cref="Vector"/>.</returns>
        public static T Normalize<T>(this Vector vector) where T : Vector, new()
        {
            var resultVector = new T();
            if (vector.Dimension != resultVector.Dimension)
                throw new DimensionException("The dimensions of the vectors do not equal each other.");

            for (uint i = 0; i < vector.Dimension; ++i)
                resultVector[i] = vector[i] / vector.Magnitude;
            return resultVector;
        }
    }
}