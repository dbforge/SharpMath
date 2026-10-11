namespace SharpMath.Canvas.Avalonia;

/// <summary>
///     Records drawing calls so the canvas can be drawn on Avalonia's render thread without touching the model there.
///     Text is measured by <paramref name="measurer" />, which should use the same engine that later replays the
///     commands.
/// </summary>
/// <param name="measurer">The context used for <see cref="MeasureText" />.</param>
internal sealed class DisplayList(IDrawingContext measurer) : IDrawingContext
{
    private readonly List<Action<IDrawingContext>> _commands = [];

    public int Count => _commands.Count;

    public void Clear(Color color) => _commands.Add(context => context.Clear(color));

    public void DrawLine(CanvasPoint startPoint, CanvasPoint endPoint, Stroke stroke) =>
        _commands.Add(context => context.DrawLine(startPoint, endPoint, stroke));

    public void DrawPolyline(ReadOnlySpan<CanvasPoint> points, Stroke stroke)
    {
        var copy = points.ToArray();
        _commands.Add(context => context.DrawPolyline(copy, stroke));
    }

    public void DrawPolygon(ReadOnlySpan<CanvasPoint> points, Stroke? stroke, Color? fill)
    {
        var copy = points.ToArray();
        _commands.Add(context => context.DrawPolygon(copy, stroke, fill));
    }

    public void DrawRectangle(CanvasRect rect, Stroke? stroke, Color? fill) =>
        _commands.Add(context => context.DrawRectangle(rect, stroke, fill));

    public void DrawEllipse(CanvasPoint center, double radiusX, double radiusY, Stroke? stroke, Color? fill) =>
        _commands.Add(context => context.DrawEllipse(center, radiusX, radiusY, stroke, fill));

    public void DrawText(string text, CanvasPoint position, FontSpec font, Color color, TextAlignment alignment) =>
        _commands.Add(context => context.DrawText(text, position, font, color, alignment));

    public TextSize MeasureText(string text, FontSpec font) => measurer.MeasureText(text, font);

    public void PushClip(CanvasRect rect) => _commands.Add(context => context.PushClip(rect));

    public void PopClip() => _commands.Add(context => context.PopClip());

    public void Replay(IDrawingContext context)
    {
        foreach (var command in _commands)
            command(context);
    }
}
