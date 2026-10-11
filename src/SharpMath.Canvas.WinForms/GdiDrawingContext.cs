using System.Drawing.Drawing2D;
using System.Drawing.Text;
using DrawingColor = System.Drawing.Color;
using GdiDashStyle = System.Drawing.Drawing2D.DashStyle;

namespace SharpMath.Canvas.WinForms;

/// <summary>
///     An <see cref="IDrawingContext" /> that draws with GDI+ on a <see cref="Graphics" /> (Windows only).
/// </summary>
/// <remarks>
///     Pens, brushes and fonts are cached for the lifetime of the context; dispose it after drawing. The rendering
///     settings of the <see cref="Graphics" /> are changed for anti-aliasing and restored on disposal; the
///     <see cref="Graphics" /> itself is not owned by the context.
/// </remarks>
public sealed class GdiDrawingContext : IDrawingContext, IDisposable
{
    private readonly Dictionary<Color, SolidBrush> _brushes = [];
    private readonly Stack<GraphicsState> _clips = new();
    private readonly Dictionary<FontSpec, Font> _fonts = [];
    private readonly Graphics _graphics;
    private readonly PixelOffsetMode _originalPixelOffsetMode;
    private readonly SmoothingMode _originalSmoothingMode;
    private readonly TextRenderingHint _originalTextRenderingHint;
    private readonly Dictionary<Stroke, Pen> _pens = [];
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GdiDrawingContext" /> class.
    /// </summary>
    /// <param name="graphics">The GDI+ surface to draw on.</param>
    public GdiDrawingContext(Graphics graphics)
    {
        ArgumentNullException.ThrowIfNull(graphics);
        _graphics = graphics;
        _originalSmoothingMode = graphics.SmoothingMode;
        _originalPixelOffsetMode = graphics.PixelOffsetMode;
        _originalTextRenderingHint = graphics.TextRenderingHint;

        // Pixel centers at .5 like the other renderers, so snapped grid lines stay crisp.
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.PixelOffsetMode = PixelOffsetMode.Half;
        graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
    }

    /// <inheritdoc />
    public void Clear(Color color) => _graphics.Clear(ToGdi(color));

    /// <inheritdoc />
    public void DrawLine(CanvasPoint startPoint, CanvasPoint endPoint, Stroke stroke) =>
        _graphics.DrawLine(Pen(stroke), ToGdi(startPoint), ToGdi(endPoint));

    /// <inheritdoc />
    public void DrawPolyline(ReadOnlySpan<CanvasPoint> points, Stroke stroke)
    {
        if (points.Length >= 2)
            _graphics.DrawLines(Pen(stroke), ToGdi(points));
    }

    /// <inheritdoc />
    public void DrawPolygon(ReadOnlySpan<CanvasPoint> points, Stroke? stroke, Color? fill)
    {
        if (points.Length < 3)
            return;
        var gdiPoints = ToGdi(points);
        if (fill is { } fillColor)
            _graphics.FillPolygon(Brush(fillColor), gdiPoints);
        if (stroke is { } outline)
            _graphics.DrawPolygon(Pen(outline), gdiPoints);
    }

    /// <inheritdoc />
    public void DrawRectangle(CanvasRect rect, Stroke? stroke, Color? fill)
    {
        var bounds = new RectangleF((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height);
        if (fill is { } fillColor)
            _graphics.FillRectangle(Brush(fillColor), bounds);
        if (stroke is { } outline)
            _graphics.DrawRectangle(Pen(outline), bounds.X, bounds.Y, bounds.Width, bounds.Height);
    }

    /// <inheritdoc />
    public void DrawEllipse(CanvasPoint center, double radiusX, double radiusY, Stroke? stroke, Color? fill)
    {
        var bounds = new RectangleF((float) (center.X - radiusX), (float) (center.Y - radiusY), (float) (2 * radiusX),
            (float) (2 * radiusY));
        if (fill is { } fillColor)
            _graphics.FillEllipse(Brush(fillColor), bounds);
        if (stroke is { } outline)
            _graphics.DrawEllipse(Pen(outline), bounds);
    }

    /// <inheritdoc />
    public void DrawText(string text, CanvasPoint position, FontSpec font, Color color, TextAlignment alignment)
    {
        ArgumentNullException.ThrowIfNull(text);
        using var format = (StringFormat) StringFormat.GenericTypographic.Clone();
        format.Alignment = alignment switch
        {
            TextAlignment.Center => StringAlignment.Center,
            TextAlignment.Right => StringAlignment.Far,
            _ => StringAlignment.Near
        };
        _graphics.DrawString(text, Font(font), Brush(color), ToGdi(position), format);
    }

    /// <inheritdoc />
    public TextSize MeasureText(string text, FontSpec font)
    {
        ArgumentNullException.ThrowIfNull(text);
        var gdiFont = Font(font);
        var size = _graphics.MeasureString(text, gdiFont, PointF.Empty, StringFormat.GenericTypographic);
        return new TextSize(size.Width, gdiFont.GetHeight(_graphics));
    }

    /// <inheritdoc />
    public void PushClip(CanvasRect rect)
    {
        _clips.Push(_graphics.Save());
        _graphics.SetClip(new RectangleF((float) rect.X, (float) rect.Y, (float) rect.Width, (float) rect.Height),
            CombineMode.Intersect);
    }

    /// <inheritdoc />
    public void PopClip()
    {
        if (_clips.Count == 0)
            throw new InvalidOperationException("PopClip was called without a matching PushClip.");
        _graphics.Restore(_clips.Pop());
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;

        while (_clips.Count > 0)
            _graphics.Restore(_clips.Pop());
        _graphics.SmoothingMode = _originalSmoothingMode;
        _graphics.PixelOffsetMode = _originalPixelOffsetMode;
        _graphics.TextRenderingHint = _originalTextRenderingHint;

        foreach (var pen in _pens.Values)
            pen.Dispose();
        foreach (var brush in _brushes.Values)
            brush.Dispose();
        foreach (var font in _fonts.Values)
            font.Dispose();
    }

    private static DrawingColor ToGdi(Color color) => DrawingColor.FromArgb(color.A, color.R, color.G, color.B);

    private static PointF ToGdi(CanvasPoint point) => new((float) point.X, (float) point.Y);

    private static PointF[] ToGdi(ReadOnlySpan<CanvasPoint> points)
    {
        var result = new PointF[points.Length];
        for (var i = 0; i < points.Length; i++)
            result[i] = ToGdi(points[i]);
        return result;
    }

    private Pen Pen(Stroke stroke)
    {
        if (_pens.TryGetValue(stroke, out var pen))
            return pen;

        pen = new Pen(ToGdi(stroke.Color), (float) stroke.Width)
        {
            DashStyle = stroke.Dash switch
            {
                DashStyle.Dashed => GdiDashStyle.Dash,
                DashStyle.Dotted => GdiDashStyle.Dot,
                _ => GdiDashStyle.Solid
            }
        };
        _pens.Add(stroke, pen);
        return pen;
    }

    private SolidBrush Brush(Color color)
    {
        if (_brushes.TryGetValue(color, out var brush))
            return brush;

        brush = new SolidBrush(ToGdi(color));
        _brushes.Add(color, brush);
        return brush;
    }

    private Font Font(FontSpec font)
    {
        if (_fonts.TryGetValue(font, out var gdiFont))
            return gdiFont;

        // GDI+ falls back to a default font if the family is not installed.
        gdiFont = new Font(font.Family, (float) font.Size, GraphicsUnit.Pixel);
        _fonts.Add(font, gdiFont);
        return gdiFont;
    }
}
