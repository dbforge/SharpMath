// Author: Dominic Beger (Trade/ProgTrade) 2016
// Improvements: Stefan Baumann 2016

namespace SharpMath
{
    /// <summary>
    ///     Provides extensions for comparing floating numbers using approximations.
    /// </summary>
    public static class FloatingNumberExtensions
    {
        /// <summary>
        ///     Determines whether two floating numbers are approximately equal to each other using the
        ///     <see cref="FloatingNumber.Epsilon" /> value.
        /// </summary>
        /// <param name="number">The current <see cref="float" />.</param>
        /// <param name="other">The other <see cref="float" />.</param>
        /// <returns>Returns <c>true</c>, if they are approximately equal, otherwise <c>false</c>.</returns>
        public static bool IsApproximatelyEqualTo(this float number, float other)
            => FloatingNumber.CheckApproximatelyEqual(number, other);

        /// <summary>
        ///     Determines whether two floating numbers are approximately equal to each other using the specified epsilon value.
        /// </summary>
        /// <param name="number">The current <see cref="float" />.</param>
        /// <param name="other">The other <see cref="float" />.</param>
        /// <param name="epsilon">The epsilon value that represents the tolerance.</param>
        /// <returns>Returns <c>true</c>, if they are approximately equal, otherwise <c>false</c>.</returns>
        public static bool IsApproximatelyEqualTo(this float number, float other, double epsilon)
            => FloatingNumber.CheckApproximatelyEqual(number, other, epsilon);

        /// <summary>
        ///     Determines whether two floating numbers are approximately equal to each other using the <see cref="Epsilon" />
        ///     value.
        /// </summary>
        /// <param name="number">The current <see cref="float" />.</param>
        /// <param name="other">The other <see cref="float" />.</param>
        /// <returns>Returns <c>true</c>, if they are approximately equal, otherwise <c>false</c>.</returns>
        public static bool IsApproximatelyEqualTo(this double number, double other)
            => FloatingNumber.CheckApproximatelyEqual(number, other);

        /// <summary>
        ///     Determines whether two floating numbers are approximately equal to each other using the specified epsilon value.
        /// </summary>
        /// <param name="number">The current <see cref="float" />.</param>
        /// <param name="other">The other <see cref="float" />.</param>
        /// <param name="epsilon">The epsilon value that represents the tolerance.</param>
        /// <returns>Returns <c>true</c>, if they are approximately equal, otherwise <c>false</c>.</returns>
        public static bool IsApproximatelyEqualTo(this double number, double other, double epsilon)
            => FloatingNumber.CheckApproximatelyEqual(number, other, epsilon);
    }
}