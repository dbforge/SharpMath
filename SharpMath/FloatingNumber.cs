﻿// FloatingNumber.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System;

namespace SharpMath
{
    /// <summary>
    ///     Provides functions for comparing floating numbers using approximations.
    /// </summary>
    public static class FloatingNumber
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
        {
            return AreApproximatelyEqual(firstNumber, secondNumber, Epsilon);
        }

        /// <summary>
        ///     Determines whether two floating numbers are approximately equal to each other using the specified epsilon value.
        /// </summary>
        /// <param name="firstNumber">The first <see cref="float" />.</param>
        /// <param name="secondNumber">The second <see cref="float" />.</param>
        /// <param name="epsilon">The epsilon value that represents the tolerance.</param>
        /// <returns>Returns <c>true</c>, if they are approximately equal, otherwise <c>false</c>.</returns>
        public static bool AreApproximatelyEqual(float firstNumber, float secondNumber, double epsilon)
        {
            return Math.Abs(firstNumber - secondNumber) <= epsilon;
        }

        /// <summary>
        ///     Determines whether two floating numbers are approximately equal to each other using the <see cref="Epsilon" />
        ///     value.
        /// </summary>
        /// <param name="firstNumber">The first <see cref="float" />.</param>
        /// <param name="secondNumber">The second <see cref="float" />.</param>
        /// <returns>Returns <c>true</c>, if they are approximately equal, otherwise <c>false</c>.</returns>
        public static bool AreApproximatelyEqual(double firstNumber, double secondNumber)
        {
            return AreApproximatelyEqual(firstNumber, secondNumber, Epsilon);
        }

        /// <summary>
        ///     Determines whether two floating numbers are approximately equal to each other using the specified epsilon value.
        /// </summary>
        /// <param name="firstNumber">The first <see cref="float" />.</param>
        /// <param name="secondNumber">The second <see cref="float" />.</param>
        /// <param name="epsilon">The epsilon value that represents the tolerance.</param>
        /// <returns>Returns <c>true</c>, if they are approximately equal, otherwise <c>false</c>.</returns>
        public static bool AreApproximatelyEqual(double firstNumber, double secondNumber, double epsilon)
        {
            return Math.Abs(firstNumber - secondNumber) <= epsilon;
        }
    }
}