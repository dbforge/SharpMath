using System.Globalization;
using System.Security;
using System.Text;

namespace SharpMath.Canvas;

/// <summary>
///     An <see cref="IDrawingContext" /> that produces an SVG document. It needs no native dependencies.
/// </summary>
/// <remarks>
///     SVG has no access to font metrics, so <see cref="MeasureText" /> estimates the width as 0.6 times the font size per
///     character and the height as 1.2 times the font size.
/// </remarks>
public sealed class SvgDrawingContext : IDrawingContext
{
    private readonly StringBuilder _body = new();
    private readonly StringBuilder _definitions = new();
    private readonly double _height;
    private readonly double _width;
    private int _clipCount;
    private int _openGroups;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SvgDrawingContext" /> class.
    /// </summary>
    /// <param name="width">The width of the document in pixels.</param>
    /// <param name="height">The height of the document in pixels.</param>
    public SvgDrawingContext(double width, double height)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(width);
        ArgumentOutOfRangeException.ThrowIfNegative(height);
        _width = width;
        _height = height;
    }

    /// <inheritdoc />
    public void Clear(Color color)
    {
        _body.Clear();
        _definitions.Clear();
        _openGroups = 0;
        _body.Append(CultureInfo.InvariantCulture,
            $"<rect x=\"0\" y=\"0\" width=\"{F(_width)}\" height=\"{F(_height)}\"{Fill(color)}/>\n");
    }

    /// <inheritdoc />
    public void DrawLine(CanvasPoint startPoint, CanvasPoint endPoint, Stroke stroke) =>
        _body.Append(CultureInfo.InvariantCulture,
            $"<line x1=\"{F(startPoint.X)}\" y1=\"{F(startPoint.Y)}\" x2=\"{F(endPoint.X)}\" y2=\"{F(endPoint.Y)}\"{StrokeAttributes(stroke)}/>\n");

    /// <inheritdoc />
    public void DrawPolyline(ReadOnlySpan<CanvasPoint> points, Stroke stroke)
    {
        if (points.Length < 2)
            return;
        _body.Append("<polyline points=\"").Append(Points(points)).Append("\" fill=\"none\"")
            .Append(StrokeAttributes(stroke)).Append("/>\n");
    }

    /// <inheritdoc />
    public void DrawPolygon(ReadOnlySpan<CanvasPoint> points, Stroke? stroke, Color? fill)
    {
        if (points.Length < 3)
            return;
        _body.Append("<polygon points=\"").Append(Points(points)).Append('"').Append(Paint(stroke, fill))
            .Append("/>\n");
    }

    /// <inheritdoc />
    public void DrawRectangle(CanvasRect rect, Stroke? stroke, Color? fill) =>
        _body.Append(CultureInfo.InvariantCulture,
            $"<rect x=\"{F(rect.X)}\" y=\"{F(rect.Y)}\" width=\"{F(rect.Width)}\" height=\"{F(rect.Height)}\"{Paint(stroke, fill)}/>\n");

    /// <inheritdoc />
    public void DrawEllipse(CanvasPoint center, double radiusX, double radiusY, Stroke? stroke, Color? fill) =>
        _body.Append(CultureInfo.InvariantCulture,
            $"<ellipse cx=\"{F(center.X)}\" cy=\"{F(center.Y)}\" rx=\"{F(radiusX)}\" ry=\"{F(radiusY)}\"{Paint(stroke, fill)}/>\n");

    /// <inheritdoc />
    public void DrawText(string text, CanvasPoint position, FontSpec font, Color color, TextAlignment alignment)
    {
        ArgumentNullException.ThrowIfNull(text);
        var anchor = alignment switch
        {
            TextAlignment.Center => "middle",
            TextAlignment.Right => "end",
            _ => "start"
        };

        // SVG positions text at its baseline; 0.8 · size approximates the ascent.
        var baseline = position.Y + 0.8 * font.Size;
        _body.Append(CultureInfo.InvariantCulture,
            $"<text x=\"{F(position.X)}\" y=\"{F(baseline)}\" font-family=\"{Escape(font.Family)}\" font-size=\"{F(font.Size)}\" text-anchor=\"{anchor}\"{Fill(color)}>{Escape(text)}</text>\n");
    }

    /// <inheritdoc />
    public TextSize MeasureText(string text, FontSpec font)
    {
        ArgumentNullException.ThrowIfNull(text);
        return new TextSize(0.6 * font.Size * text.Length, 1.2 * font.Size);
    }

    /// <inheritdoc />
    public void PushClip(CanvasRect rect)
    {
        var id = "clip" + (++_clipCount).ToString(CultureInfo.InvariantCulture);
        _definitions.Append(CultureInfo.InvariantCulture,
            $"<clipPath id=\"{id}\"><rect x=\"{F(rect.X)}\" y=\"{F(rect.Y)}\" width=\"{F(rect.Width)}\" height=\"{F(rect.Height)}\"/></clipPath>\n");
        _body.Append(CultureInfo.InvariantCulture, $"<g clip-path=\"url(#{id})\">\n");
        _openGroups++;
    }

    /// <inheritdoc />
    public void PopClip()
    {
        if (_openGroups == 0)
            throw new InvalidOperationException("PopClip was called without a matching PushClip.");
        _body.Append("</g>\n");
        _openGroups--;
    }

    /// <summary>
    ///     Returns the SVG document drawn so far. Unclosed clip regions are closed.
    /// </summary>
    /// <returns>The SVG document.</returns>
    public override string ToString()
    {
        var document = new StringBuilder();
        document.Append(CultureInfo.InvariantCulture,
            $"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{F(_width)}\" height=\"{F(_height)}\" viewBox=\"0 0 {F(_width)} {F(_height)}\">\n");
        if (_definitions.Length > 0)
            document.Append("<defs>\n").Append(_definitions).Append("</defs>\n");
        document.Append(_body);
        for (var i = 0; i < _openGroups; i++)
            document.Append("</g>\n");
        return document.Append("</svg>\n").ToString();
    }

    private static string F(double value) => Math.Round(value, 3).ToString("0.###", CultureInfo.InvariantCulture);

    private static string Points(ReadOnlySpan<CanvasPoint> points)
    {
        var builder = new StringBuilder(points.Length * 12);
        foreach (var point in points)
            builder.Append(F(point.X)).Append(',').Append(F(point.Y)).Append(' ');
        return builder.ToString(0, builder.Length - 1);
    }

    private static string Fill(Color color) =>
        color.A == 255
            ? $" fill=\"{color.ToHex()[..7]}\""
            : string.Create(CultureInfo.InvariantCulture,
                $" fill=\"{color.ToHex()[..7]}\" fill-opacity=\"{F(color.A / 255.0)}\"");

    private static string StrokeAttributes(Stroke stroke)
    {
        var builder = new StringBuilder();
        builder.Append(CultureInfo.InvariantCulture,
            $" stroke=\"{stroke.Color.ToHex()[..7]}\" stroke-width=\"{F(stroke.Width)}\"");
        if (stroke.Color.A != 255)
            builder.Append(CultureInfo.InvariantCulture, $" stroke-opacity=\"{F(stroke.Color.A / 255.0)}\"");
        if (stroke.Dash == DashStyle.Dashed)
            builder.Append(CultureInfo.InvariantCulture, $" stroke-dasharray=\"{F(4 * stroke.Width)},{F(2 * stroke.Width)}\"");
        else if (stroke.Dash == DashStyle.Dotted)
            builder.Append(CultureInfo.InvariantCulture, $" stroke-dasharray=\"{F(stroke.Width)},{F(2 * stroke.Width)}\"");
        return builder.ToString();
    }

    private static string Paint(Stroke? stroke, Color? fill) =>
        (fill is { } color ? Fill(color) : " fill=\"none\"") + (stroke is { } s ? StrokeAttributes(s) : string.Empty);

    private static string Escape(string text) => SecurityElement.Escape(text);
}
