namespace SharpMath;

/// <summary>
///     Provides tolerance-based comparisons of floating-point numbers.
/// </summary>
/// <remarks>
///     Two numbers <c>a</c> and <c>b</c> are considered equal if
///     <c>|a − b| ≤ max(absolute, relative · max(|a|, |b|))</c>. The absolute part handles values close to zero, the
///     relative part scales with the magnitude of the compared values. <see cref="double.NaN" /> is never equal to
///     anything.
/// </remarks>
public static class Tolerance
{
    /// <summary>
    ///     The default absolute tolerance (<c>1e-12</c>).
    /// </summary>
    public const double DefaultAbsolute = 1e-12;

    /// <summary>
    ///     The default relative tolerance (<c>1e-9</c>).
    /// </summary>
    public const double DefaultRelative = 1e-9;

    /// <summary>
    ///     Determines whether two numbers are equal within the default tolerances.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns><see langword="true" /> if the numbers are approximately equal; otherwise, <see langword="false" />.</returns>
    public static bool AreEqual(double a, double b) => AreEqual(a, b, DefaultAbsolute, DefaultRelative);

    /// <summary>
    ///     Determines whether two numbers are equal within the specified tolerances.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <param name="absolute">The absolute tolerance. Must not be negative.</param>
    /// <param name="relative">The relative tolerance. Must not be negative.</param>
    /// <returns><see langword="true" /> if the numbers are approximately equal; otherwise, <see langword="false" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A tolerance is negative or NaN.</exception>
    public static bool AreEqual(double a, double b, double absolute, double relative)
    {
        ValidateTolerances(absolute, relative);

        // Also covers infinities of the same sign.
        if (a == b)
            return true;

        var difference = Math.Abs(a - b);
        if (!double.IsFinite(difference))
            return false;

        return difference <= Math.Max(absolute, relative * Math.Max(Math.Abs(a), Math.Abs(b)));
    }

    /// <summary>
    ///     Determines whether a number is zero within the default absolute tolerance.
    /// </summary>
    /// <param name="value">The number.</param>
    /// <returns><see langword="true" /> if <c>|value| ≤ </c><see cref="DefaultAbsolute" />; otherwise, <see langword="false" />.</returns>
    public static bool IsZero(double value) => Math.Abs(value) <= DefaultAbsolute;

    /// <summary>
    ///     Determines whether a number is zero within the specified absolute tolerance.
    /// </summary>
    /// <param name="value">The number.</param>
    /// <param name="absolute">The absolute tolerance. Must not be negative.</param>
    /// <returns><see langword="true" /> if <c>|value| ≤ absolute</c>; otherwise, <see langword="false" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="absolute" /> is negative or NaN.</exception>
    public static bool IsZero(double value, double absolute)
    {
        ValidateTolerances(absolute, 0);
        return Math.Abs(value) <= absolute;
    }

    /// <summary>
    ///     Determines whether a number is negligible compared to a reference magnitude, using the default tolerances.
    /// </summary>
    /// <param name="value">The number, e.g. a residual or a cross product.</param>
    /// <param name="scale">The magnitude of the quantities <paramref name="value" /> was computed from.</param>
    /// <returns>
    ///     <see langword="true" /> if <c>|value| ≤ max(</c><see cref="DefaultAbsolute" /><c>,</c>
    ///     <see cref="DefaultRelative" /><c> · |scale|)</c>; otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsNegligible(double value, double scale) =>
        Math.Abs(value) <= Math.Max(DefaultAbsolute, DefaultRelative * Math.Abs(scale));

    private static void ValidateTolerances(double absolute, double relative)
    {
        if (!(absolute >= 0))
            throw new ArgumentOutOfRangeException(nameof(absolute), absolute, "The tolerance must not be negative.");
        if (!(relative >= 0))
            throw new ArgumentOutOfRangeException(nameof(relative), relative, "The tolerance must not be negative.");
    }
}
