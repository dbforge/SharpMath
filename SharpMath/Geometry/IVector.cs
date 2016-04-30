namespace SharpMath.Geometry
{
    public interface IVector
    {
        /// <summary>
        ///     Gets or sets the value of the coordinate at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The value of the coordinate at the specified index.</returns>
        double this[uint index] { get; set; }

        /// <summary>
        ///     Gets the dimension of the <see cref="IVector" />.
        /// </summary>
        uint Dimension { get; }

        /// <summary>
        ///     Gets the length of the <see cref="IVector" />.
        /// </summary>
        double Magnitude { get; }

        /// <summary>
        ///     Gets the squared length of the <see cref="IVector" />.
        /// </summary>
        double SquareMagnitude { get; }

        // <summary>
        ///     Gets a value indicating whether the <see cref="IVector"/> is normalized, or not.
        /// </summary>
        bool IsNormalized { get; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="IVector"/> has all of its components set to zero, or not.
        /// </summary>
        bool IsZero { get; }
    }
}