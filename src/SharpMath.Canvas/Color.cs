using System.Globalization;

namespace SharpMath.Canvas;

/// <summary>
///     Represents an immutable RGBA color with 8 bits per channel.
/// </summary>
/// <param name="R">The red channel.</param>
/// <param name="G">The green channel.</param>
/// <param name="B">The blue channel.</param>
/// <param name="A">The alpha channel (255 is opaque).</param>
public readonly record struct Color(byte R, byte G, byte B, byte A = 255)
{
    /// <summary>
    ///     Gets opaque white.
    /// </summary>
    public static Color White => new(255, 255, 255);

    /// <summary>
    ///     Gets opaque black.
    /// </summary>
    public static Color Black => new(0, 0, 0);

    /// <summary>
    ///     Gets the gray used for tracking lines (#808080).
    /// </summary>
    public static Color Gray => new(128, 128, 128);

    /// <summary>
    ///     Gets the light gray used for grid lines (#D3D3D3).
    /// </summary>
    public static Color LightGray => new(211, 211, 211);

    /// <summary>
    ///     Gets a fully transparent color.
    /// </summary>
    public static Color Transparent => new(0, 0, 0, 0);

    /// <summary>
    ///     Creates an opaque color.
    /// </summary>
    /// <param name="red">The red channel.</param>
    /// <param name="green">The green channel.</param>
    /// <param name="blue">The blue channel.</param>
    /// <returns>The color.</returns>
    public static Color FromRgb(byte red, byte green, byte blue) => new(red, green, blue);

    /// <summary>
    ///     Creates a color with an alpha channel.
    /// </summary>
    /// <param name="alpha">The alpha channel (255 is opaque).</param>
    /// <param name="red">The red channel.</param>
    /// <param name="green">The green channel.</param>
    /// <param name="blue">The blue channel.</param>
    /// <returns>The color.</returns>
    public static Color FromArgb(byte alpha, byte red, byte green, byte blue) => new(red, green, blue, alpha);

    /// <summary>
    ///     Returns the color as <c>#RRGGBB</c> (opaque) or <c>#RRGGBBAA</c>.
    /// </summary>
    /// <returns>The hexadecimal representation.</returns>
    public string ToHex() =>
        A == 255
            ? string.Create(CultureInfo.InvariantCulture, $"#{R:X2}{G:X2}{B:X2}")
            : string.Create(CultureInfo.InvariantCulture, $"#{R:X2}{G:X2}{B:X2}{A:X2}");
}
