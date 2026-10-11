using SkiaSharp;

namespace SharpMath.Canvas.Skia;

/// <summary>
///     An <see cref="IDrawingContext" /> that draws on a SkiaSharp <see cref="SKCanvas" />.
/// </summary>
/// <remarks>
///     Paints, fonts and typefaces are cached for the lifetime of the context; dispose it after drawing. The canvas
///     itself is not owned by the context.
/// </remarks>
public sealed class SkiaDrawingContext : IDrawingContext, IDisposable
{
    private readonly SKCanvas _canvas;
    private readonly Dictionary<Color, SKPaint> _fills = [];
    private readonly SkiaFonts _fonts = new();
    private readonly Dictionary<Stroke, SKPaint> _strokes = [];
    private readonly SKPath _path = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="SkiaDrawingContext" /> class.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    public SkiaDrawingContext(SKCanvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        _canvas = canvas;
    }

    /// <inheritdoc />
    public void Clear(Color color) => _canvas.Clear(ToSkia(color));

    /// <inheritdoc />
    public void DrawLine(CanvasPoint startPoint, CanvasPoint endPoint, Stroke stroke) =>
        _canvas.DrawLine((float) startPoint.X, (float) startPoint.Y, (float) endPoint.X, (float) endPoint.Y,
            StrokePaint(stroke));

    /// <inheritdoc />
    public void DrawPolyline(ReadOnlySpan<CanvasPoint> points, Stroke stroke)
    {
        if (points.Length < 2)
            return;
        BuildPath(points, false);
        _canvas.DrawPath(_path, StrokePaint(stroke));
    }

    /// <inheritdoc />
    public void DrawPolygon(ReadOnlySpan<CanvasPoint> points, Stroke? stroke, Color? fill)
    {
        if (points.Length < 3)
            return;
        BuildPath(points, true);
        if (fill is { } fillColor)
            _canvas.DrawPath(_path, FillPaint(fillColor));
        if (stroke is { } outline)
            _canvas.DrawPath(_path, StrokePaint(outline));
    }

    /// <inheritdoc />
    public void DrawRectangle(CanvasRect rect, Stroke? stroke, Color? fill)
    {
        var skRect = SKRect.Create((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height);
        if (fill is { } fillColor)
            _canvas.DrawRect(skRect, FillPaint(fillColor));
        if (stroke is { } outline)
            _canvas.DrawRect(skRect, StrokePaint(outline));
    }

    /// <inheritdoc />
    public void DrawEllipse(CanvasPoint center, double radiusX, double radiusY, Stroke? stroke, Color? fill)
    {
        if (fill is { } fillColor)
            _canvas.DrawOval((float) center.X, (float) center.Y, (float) radiusX, (float) radiusY, FillPaint(fillColor));
        if (stroke is { } outline)
            _canvas.DrawOval((float) center.X, (float) center.Y, (float) radiusX, (float) radiusY, StrokePaint(outline));
    }

    /// <inheritdoc />
    public void DrawText(string text, CanvasPoint position, FontSpec font, Color color, TextAlignment alignment)
    {
        ArgumentNullException.ThrowIfNull(text);
        var skFont = _fonts.Get(font);
        var align = alignment switch
        {
            TextAlignment.Center => SKTextAlign.Center,
            TextAlignment.Right => SKTextAlign.Right,
            _ => SKTextAlign.Left
        };

        // Skia positions text at its baseline; the ascent is negative.
        var baseline = (float) position.Y - skFont.Metrics.Ascent;
        _canvas.DrawText(text, (float) position.X, baseline, align, skFont, FillPaint(color));
    }

    /// <inheritdoc />
    public TextSize MeasureText(string text, FontSpec font) => _fonts.Measure(text, font);

    /// <inheritdoc />
    public void PushClip(CanvasRect rect)
    {
        _canvas.Save();
        _canvas.ClipRect(SKRect.Create((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height));
    }

    /// <inheritdoc />
    public void PopClip() => _canvas.Restore();

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var paint in _strokes.Values.Concat(_fills.Values))
        {
            paint.PathEffect?.Dispose();
            paint.Dispose();
        }
        _fonts.Dispose();
        _path.Dispose();
        _strokes.Clear();
        _fills.Clear();
    }

    private static SKColor ToSkia(Color color) => new(color.R, color.G, color.B, color.A);

    private void BuildPath(ReadOnlySpan<CanvasPoint> points, bool close)
    {
        _path.Reset();
        _path.MoveTo((float) points[0].X, (float) points[0].Y);
        foreach (var point in points[1..])
            _path.LineTo((float) point.X, (float) point.Y);
        if (close)
            _path.Close();
    }

    private SKPaint StrokePaint(Stroke stroke)
    {
        if (_strokes.TryGetValue(stroke, out var paint))
            return paint;

        var width = (float) stroke.Width;
        paint = new SKPaint
        {
            Color = ToSkia(stroke.Color),
            StrokeWidth = width,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            PathEffect = stroke.Dash switch
            {
                DashStyle.Dashed => SKPathEffect.CreateDash([4 * width, 2 * width], 0),
                DashStyle.Dotted => SKPathEffect.CreateDash([width, 2 * width], 0),
                _ => null
            }
        };
        _strokes.Add(stroke, paint);
        return paint;
    }

    private SKPaint FillPaint(Color color)
    {
        if (_fills.TryGetValue(color, out var paint))
            return paint;

        paint = new SKPaint { Color = ToSkia(color), Style = SKPaintStyle.Fill, IsAntialias = true };
        _fills.Add(color, paint);
        return paint;
    }
}
