namespace SharpMath.Canvas;

/// <summary>
///     The drawing operations a renderer provides to the <see cref="Canvas2D" />. All coordinates are in pixels with the
///     origin in the top-left corner and Y pointing down.
/// </summary>
/// <remarks>
///     The canvas clips coordinates to a guard band around the visible area before calling a renderer, so implementations
///     never receive huge or non-finite coordinates.
/// </remarks>
public interface IDrawingContext
{
    /// <summary>
    ///     Fills the whole drawing area.
    /// </summary>
    /// <param name="color">The color.</param>
    void Clear(Color color);

    /// <summary>
    ///     Draws a straight line.
    /// </summary>
    /// <param name="startPoint">The start point.</param>
    /// <param name="endPoint">The end point.</param>
    /// <param name="stroke">The stroke.</param>
    void DrawLine(CanvasPoint startPoint, CanvasPoint endPoint, Stroke stroke);

    /// <summary>
    ///     Draws connected line segments through the points.
    /// </summary>
    /// <param name="points">The points; at least two.</param>
    /// <param name="stroke">The stroke.</param>
    void DrawPolyline(ReadOnlySpan<CanvasPoint> points, Stroke stroke);

    /// <summary>
    ///     Draws a closed polygon.
    /// </summary>
    /// <param name="points">The vertices; at least three.</param>
    /// <param name="stroke">The outline, or <see langword="null" /> for none.</param>
    /// <param name="fill">The fill color, or <see langword="null" /> for none.</param>
    void DrawPolygon(ReadOnlySpan<CanvasPoint> points, Stroke? stroke, Color? fill);

    /// <summary>
    ///     Draws an axis-aligned rectangle.
    /// </summary>
    /// <param name="rect">The rectangle.</param>
    /// <param name="stroke">The outline, or <see langword="null" /> for none.</param>
    /// <param name="fill">The fill color, or <see langword="null" /> for none.</param>
    void DrawRectangle(CanvasRect rect, Stroke? stroke, Color? fill);

    /// <summary>
    ///     Draws an axis-aligned ellipse.
    /// </summary>
    /// <param name="center">The center.</param>
    /// <param name="radiusX">The horizontal radius.</param>
    /// <param name="radiusY">The vertical radius.</param>
    /// <param name="stroke">The outline, or <see langword="null" /> for none.</param>
    /// <param name="fill">The fill color, or <see langword="null" /> for none.</param>
    void DrawEllipse(CanvasPoint center, double radiusX, double radiusY, Stroke? stroke, Color? fill);

    /// <summary>
    ///     Draws a single line of text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="position">The anchor: the top edge of the text and, depending on the alignment, its left edge, center or right edge.</param>
    /// <param name="font">The font.</param>
    /// <param name="color">The color.</param>
    /// <param name="alignment">The horizontal alignment relative to the anchor.</param>
    void DrawText(string text, CanvasPoint position, FontSpec font, Color color, TextAlignment alignment);

    /// <summary>
    ///     Measures a single line of text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="font">The font.</param>
    /// <returns>The size of the text.</returns>
    TextSize MeasureText(string text, FontSpec font);

    /// <summary>
    ///     Restricts subsequent drawing to a rectangle until the matching <see cref="PopClip" />.
    /// </summary>
    /// <param name="rect">The clip rectangle.</param>
    void PushClip(CanvasRect rect);

    /// <summary>
    ///     Removes the clip rectangle added by the last <see cref="PushClip" />.
    /// </summary>
    void PopClip();
}
