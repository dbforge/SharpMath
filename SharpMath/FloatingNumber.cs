// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;

namespace SharpMath
{
    /// <summary>
    ///     Provides functions for comparing floating numbers using approximations.
    /// </summary>
    public class FloatingNumber
    {
        /// <summary>
        ///     Gets the default tolerance value for comparing floating numbers.
        /// </summary>
        public static double Epsilon => 1.6234133E-09;

        /// <summary>
        ///     Determines whether two floating numbers are approximately equal to each other using the <see cref="Epsilon" />
        ///     value.
        /// </summary>
        /// <param name="firstNumber">The first <see cref="float" />.</param>
        /// <param name="secondNumber">The second <see cref="float" />.</param>
        /// <returns>Returns <c>true</c>, if they are approximately equal, otherwise <c>false</c>.</returns>
        public static bool AreApproximatelyEqual(float firstNumber, float secondNumber)
            => AreApproximatelyEqual(firstNumber, secondNumber, Epsilon);

        /// <summary>
        ///     Determines whether two floating numbers are approximately equal to each other using the specified epsilon value.
        /// </summary>
        /// <param name="firstNumber">The first <see cref="float" />.</param>
        /// <param name="secondNumber">The second <see cref="float" />.</param>
        /// <param name="epsilon">The epsilon value that represents the tolerance.</param>
        /// <returns>Returns <c>true</c>, if they are approximately equal, otherwise <c>false</c>.</returns>
        public static bool AreApproximatelyEqual(float firstNumber, float secondNumber, double epsilon)
            => Math.Abs(firstNumber - secondNumber) <= Epsilon;

        /// <summary>
        ///     Determines whether two floating numbers are approximately equal to each other using the <see cref="Epsilon" />
        ///     value.
        /// </summary>
        /// <param name="firstNumber">The first <see cref="float" />.</param>
        /// <param name="secondNumber">The second <see cref="float" />.</param>
        /// <returns>Returns <c>true</c>, if they are approximately equal, otherwise <c>false</c>.</returns>
        public static bool AreApproximatelyEqual(double firstNumber, double secondNumber)
            => AreApproximatelyEqual(firstNumber, secondNumber, Epsilon);

        /// <summary>
        ///     Determines whether two floating numbers are approximately equal to each other using the specified epsilon value.
        /// </summary>
        /// <param name="firstNumber">The first <see cref="float" />.</param>
        /// <param name="secondNumber">The second <see cref="float" />.</param>
        /// <param name="epsilon">The epsilon value that represents the tolerance.</param>
        /// <returns>Returns <c>true</c>, if they are approximately equal, otherwise <c>false</c>.</returns>
        public static bool AreApproximatelyEqual(double firstNumber, double secondNumber, double epsilon)
            => Math.Abs(firstNumber - secondNumber) <= Epsilon;
    }
}