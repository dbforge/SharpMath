namespace SharpMath.Canvas;

/// <summary>
///     A point in screen coordinates (pixels, Y pointing down).
/// </summary>
/// <param name="X">The horizontal position.</param>
/// <param name="Y">The vertical position.</param>
public readonly record struct CanvasPoint(double X, double Y);

/// <summary>
///     A rectangle in screen coordinates (pixels, Y pointing down).
/// </summary>
/// <param name="X">The left edge.</param>
/// <param name="Y">The top edge.</param>
/// <param name="Width">The width.</param>
/// <param name="Height">The height.</param>
public readonly record struct CanvasRect(double X, double Y, double Width, double Height)
{
    /// <summary>
    ///     Gets the right edge.
    /// </summary>
    public double Right => X + Width;

    /// <summary>
    ///     Gets the bottom edge.
    /// </summary>
    public double Bottom => Y + Height;

    /// <summary>
    ///     Determines whether a point lies inside the rectangle or on its border.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns><see langword="true" /> if the rectangle contains the point; otherwise, <see langword="false" />.</returns>
    public bool Contains(CanvasPoint point) =>
        point.X >= X && point.X <= Right && point.Y >= Y && point.Y <= Bottom;
}

/// <summary>
///     The dash pattern of a stroke.
/// </summary>
public enum DashStyle
{
    /// <summary>
    ///     A continuous line.
    /// </summary>
    Solid,

    /// <summary>
    ///     A dashed line.
    /// </summary>
    Dashed,

    /// <summary>
    ///     A dotted line.
    /// </summary>
    Dotted
}

/// <summary>
///     Describes how lines are drawn.
/// </summary>
/// <param name="Color">The color.</param>
/// <param name="Width">The width in pixels.</param>
/// <param name="Dash">The dash pattern.</param>
public readonly record struct Stroke(Color Color, double Width = 1, DashStyle Dash = DashStyle.Solid);

/// <summary>
///     Describes a font.
/// </summary>
/// <param name="Family">The font family; renderers fall back to a default font if it is not available.</param>
/// <param name="Size">The size in pixels.</param>
public readonly record struct FontSpec(string Family, double Size);

/// <summary>
///     The measured size of a text in pixels.
/// </summary>
/// <param name="Width">The width.</param>
/// <param name="Height">The height (line height).</param>
public readonly record struct TextSize(double Width, double Height);

/// <summary>
///     The horizontal alignment of a text relative to its position.
/// </summary>
public enum TextAlignment
{
    /// <summary>
    ///     The position is the left edge of the text.
    /// </summary>
    Left,

    /// <summary>
    ///     The position is the horizontal center of the text.
    /// </summary>
    Center,

    /// <summary>
    ///     The position is the right edge of the text.
    /// </summary>
    Right
}
