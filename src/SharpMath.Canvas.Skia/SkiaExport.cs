using SkiaSharp;

namespace SharpMath.Canvas.Skia;

/// <summary>
///     Renders a <see cref="Canvas2D" /> to raster images with SkiaSharp.
/// </summary>
public static class SkiaExport
{
    /// <summary>
    ///     Renders the canvas onto a SkiaSharp canvas.
    /// </summary>
    /// <param name="canvas">The canvas to render.</param>
    /// <param name="target">The SkiaSharp canvas to draw on.</param>
    /// <param name="width">The width of the drawing area in pixels.</param>
    /// <param name="height">The height of the drawing area in pixels.</param>
    public static void Render(this Canvas2D canvas, SKCanvas target, double width, double height)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        using var context = new SkiaDrawingContext(target);
        canvas.Render(context, width, height);
    }

    /// <summary>
    ///     Renders the canvas into a new image.
    /// </summary>
    /// <param name="canvas">The canvas to render.</param>
    /// <param name="width">The width in pixels. Must be positive.</param>
    /// <param name="height">The height in pixels. Must be positive.</param>
    /// <returns>The image; the caller disposes it.</returns>
    public static SKImage RenderImage(this Canvas2D canvas, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        using var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul)) ??
                            throw new InvalidOperationException("Skia could not create a drawing surface.");
        canvas.Render(surface.Canvas, width, height);
        return surface.Snapshot();
    }

    /// <summary>
    ///     Renders the canvas as a PNG image and writes it to a stream.
    /// </summary>
    /// <param name="canvas">The canvas to render.</param>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="width">The width in pixels. Must be positive.</param>
    /// <param name="height">The height in pixels. Must be positive.</param>
    public static void RenderPng(this Canvas2D canvas, Stream stream, int width, int height)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using var image = canvas.RenderImage(width, height);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        data.SaveTo(stream);
    }
}
