namespace SharpMath.Geometry
{
    public static class VectorExtensions
    {
        public static T Negate<T>(this Vector vector) where T : Vector, new()
        {
            var resultVector = new T();
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
            for (uint i = 0; i < vector.Dimension; ++i)
                resultVector[i] /= vector.Magnitude;
            return resultVector;
        }
    }
}