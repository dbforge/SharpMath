namespace SharpMath.Canvas.Tests;

/// <summary>
///     Records all drawing calls so tests can assert on them without a graphics library.
/// </summary>
internal sealed class RecordingDrawingContext : IDrawingContext
{
    public List<Color> Clears { get; } = [];
    public List<(CanvasPoint Start, CanvasPoint End, Stroke Stroke)> Lines { get; } = [];
    public List<(CanvasPoint[] Points, Stroke Stroke)> Polylines { get; } = [];
    public List<(CanvasPoint[] Points, Stroke? Stroke, Color? Fill)> Polygons { get; } = [];
    public List<(CanvasRect Rect, Stroke? Stroke, Color? Fill)> Rectangles { get; } = [];
    public List<(CanvasPoint Center, double RadiusX, double RadiusY, Stroke? Stroke, Color? Fill)> Ellipses { get; } = [];
    public List<(string Text, CanvasPoint Position, Color Color, TextAlignment Alignment)> Texts { get; } = [];
    public int ClipDepth { get; private set; }
    public int MaxClipDepth { get; private set; }

    public IEnumerable<CanvasPoint> AllPoints =>
        Lines.SelectMany(line => new[] { line.Start, line.End })
            .Concat(Polylines.SelectMany(polyline => polyline.Points))
            .Concat(Polygons.SelectMany(polygon => polygon.Points))
            .Concat(Ellipses.Select(ellipse => ellipse.Center))
            .Concat(Texts.Select(text => text.Position));

    public void Clear(Color color) => Clears.Add(color);

    public void DrawLine(CanvasPoint startPoint, CanvasPoint endPoint, Stroke stroke) =>
        Lines.Add((startPoint, endPoint, stroke));

    public void DrawPolyline(ReadOnlySpan<CanvasPoint> points, Stroke stroke) => Polylines.Add((points.ToArray(), stroke));

    public void DrawPolygon(ReadOnlySpan<CanvasPoint> points, Stroke? stroke, Color? fill) =>
        Polygons.Add((points.ToArray(), stroke, fill));

    public void DrawRectangle(CanvasRect rect, Stroke? stroke, Color? fill) => Rectangles.Add((rect, stroke, fill));

    public void DrawEllipse(CanvasPoint center, double radiusX, double radiusY, Stroke? stroke, Color? fill) =>
        Ellipses.Add((center, radiusX, radiusY, stroke, fill));

    public void DrawText(string text, CanvasPoint position, FontSpec font, Color color, TextAlignment alignment) =>
        Texts.Add((text, position, color, alignment));

    public TextSize MeasureText(string text, FontSpec font) => new(text.Length * font.Size * 0.6, font.Size * 1.2);

    public void PushClip(CanvasRect rect)
    {
        ClipDepth++;
        MaxClipDepth = Math.Max(MaxClipDepth, ClipDepth);
    }

    public void PopClip() => ClipDepth--;
}
