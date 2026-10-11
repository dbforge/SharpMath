using System.Globalization;

namespace SharpMath.Canvas;

/// <summary>
///     Calculates "nice" tick steps (1, 2 or 5 times a power of ten) and formats tick labels.
/// </summary>
internal static class Ticks
{
    /// <summary>
    ///     The largest number of ticks returned by <see cref="Range" />; beyond that (or once the tick indices lose
    ///     precision) no ticks are drawn.
    /// </summary>
    public const long MaxTicks = 10_000;

    /// <summary>
    ///     Returns the smallest nice step in world units whose on-screen length is at least <paramref name="minPixels" />.
    /// </summary>
    public static double NiceStep(double minPixels, double scale)
    {
        var raw = minPixels / scale;
        var magnitude = Math.Pow(10, Math.Floor(Math.Log10(raw)));
        foreach (var multiplier in (ReadOnlySpan<double>) [1, 2, 5, 10])
        {
            var step = multiplier * magnitude;
            if (step >= raw * (1 - 1e-12))
                return step;
        }

        return 10 * magnitude;
    }


    /// <summary>
    ///     Returns the smallest nice step that is a whole multiple of <paramref name="baseStep" /> and whose on-screen
    ///     length is at least <paramref name="minPixels" />.
    /// </summary>
    public static double NiceMultiple(double minPixels, double scale, double baseStep)
    {
        var step = baseStep;
        while (step * scale < minPixels * (1 - 1e-12) || !IsWholeMultiple(step, baseStep))
            step = NextNiceStep(step);
        return step;
    }

    /// <summary>
    ///     Formats a value with one decimal more than <paramref name="step" /> needs.
    /// </summary>
    public static string FormatValue(double value, double step)
    {
        var decimals = Math.Clamp(Decimals(step) + 1, 0, 15);
        return Math.Round(value, decimals).ToString("F" + decimals.ToString(CultureInfo.InvariantCulture),
            CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Returns a usable minimum spacing in pixels: at least 1, and 1 for invalid values.
    /// </summary>
    public static double SafeSpacing(double spacing) => double.IsFinite(spacing) && spacing >= 1 ? spacing : 1;

    /// <summary>
    ///     Returns the index range <c>[first, last]</c> of multiples of <paramref name="step" /> inside [min, max], or an
    ///     empty range if it would be too large or imprecise.
    /// </summary>
    public static (long First, long Last) Range(double min, double max, double step)
    {
        var first = Math.Ceiling(min / step);
        var last = Math.Floor(max / step);
        if (!double.IsFinite(first) || !double.IsFinite(last) || Math.Abs(first) > 1e15 || Math.Abs(last) > 1e15 ||
            last - first > MaxTicks)
            return (1, 0);
        return ((long) first, (long) last);
    }

    /// <summary>
    ///     Formats the tick value <c>index · step</c> with as many decimals as the step needs.
    /// </summary>
    public static string Format(long index, double step)
    {
        var value = index * step;
        if (index == 0)
            return "0";

        var decimals = Decimals(step);
        var magnitude = Math.Abs(value);
        if (decimals > 6 || magnitude >= 1e7)
            return value.ToString("0.###E+0", CultureInfo.InvariantCulture);
        return value.ToString("F" + decimals.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
    }

    private static int Decimals(double step) => Math.Max(0, (int) -Math.Floor(Math.Log10(step) + 1e-9));

    private static bool IsWholeMultiple(double step, double baseStep)
    {
        var ratio = step / baseStep;
        return Math.Abs(ratio - Math.Round(ratio)) < 1e-9;
    }

    /// <summary>
    ///     Returns the next larger nice step (… 1, 2, 5, 10, 20, 50 …).
    /// </summary>
    private static double NextNiceStep(double step)
    {
        var magnitude = Math.Pow(10, Math.Floor(Math.Log10(step) + 1e-9));
        var leading = Math.Round(step / magnitude);
        return leading switch
        {
            < 2 => 2 * magnitude,
            < 5 => 5 * magnitude,
            _ => 10 * magnitude
        };
    }
}
