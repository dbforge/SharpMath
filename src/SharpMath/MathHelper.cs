// MathHelper.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System;

// ReSharper disable InconsistentNaming

namespace SharpMath
{
    /// <summary>
    ///     Provides constants and methods for logarithmic, trigonometric and other useful mathematical functions.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        ///     Represents the base of the natural logarithm.
        /// </summary>
        public const float E = (float) Math.E;

        /// <summary>
        ///     Represents the base-10 logarithm of e, the base of natural logarithms.
        /// </summary>
        public const float Log10E = 0.4342945f;

        /// <summary>
        ///     Represents the base-2 logarithm of e, the base of natural logarithms.
        /// </summary>
        public const float Log2E = 1.442695f;

        /// <summary>
        ///     Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π.
        /// </summary>
        public const float Pi = (float) Math.PI;

        /// <summary>
        ///     Represents <see cref="Pi" /> over 2 (90 degrees).
        /// </summary>
        public const float PiOver2 = (float) (Math.PI / 2.0);

        /// <summary>
        ///     Represents <see cref="Pi" /> over 4 (45 degrees).
        /// </summary>
        public const float PiOver4 = (float) (Math.PI / 4.0);

        /// <summary>
        ///     Represents 2 <see cref="Pi" /> (360 degrees).
        /// </summary>
        public const float TwoPi = (float) (Math.PI * 2.0);

        /// <summary>
        ///     Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        /// <summary>
        ///     Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The angle in degrees.</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }
    }
}