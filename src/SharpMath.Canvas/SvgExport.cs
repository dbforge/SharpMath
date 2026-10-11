namespace SharpMath.Canvas;

/// <summary>
///     Renders a <see cref="Canvas2D" /> to SVG.
/// </summary>
public static class SvgExport
{
    /// <summary>
    ///     Renders the canvas to an SVG document.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    /// <param name="width">The width in pixels.</param>
    /// <param name="height">The height in pixels.</param>
    /// <returns>The SVG document.</returns>
    public static string RenderSvg(this Canvas2D canvas, double width, double height)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        var context = new SvgDrawingContext(width, height);
        canvas.Render(context, width, height);
        return context.ToString();
    }

    /// <summary>
    ///     Renders the canvas to an SVG document and writes it to a <see cref="TextWriter" />.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    /// <param name="writer">The writer.</param>
    /// <param name="width">The width in pixels.</param>
    /// <param name="height">The height in pixels.</param>
    public static void RenderSvg(this Canvas2D canvas, TextWriter writer, double width, double height)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.Write(canvas.RenderSvg(width, height));
    }
}
