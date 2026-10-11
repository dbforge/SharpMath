namespace SharpMath.Canvas.Avalonia.Tests;

public class DisplayListTests
{
    private sealed class CountingContext : IDrawingContext
    {
        public List<string> Calls { get; } = [];
        public void Clear(Color color) => Calls.Add(nameof(Clear));
        public void DrawLine(CanvasPoint startPoint, CanvasPoint endPoint, Stroke stroke) => Calls.Add(nameof(DrawLine));
        public void DrawPolyline(ReadOnlySpan<CanvasPoint> points, Stroke stroke) => Calls.Add(nameof(DrawPolyline));
        public void DrawPolygon(ReadOnlySpan<CanvasPoint> points, Stroke? stroke, Color? fill) => Calls.Add(nameof(DrawPolygon));
        public void DrawRectangle(CanvasRect rect, Stroke? stroke, Color? fill) => Calls.Add(nameof(DrawRectangle));
        public void DrawEllipse(CanvasPoint center, double radiusX, double radiusY, Stroke? stroke, Color? fill) =>
            Calls.Add(nameof(DrawEllipse));
        public void DrawText(string text, CanvasPoint position, FontSpec font, Color color, TextAlignment alignment) =>
            Calls.Add(nameof(DrawText));
        public TextSize MeasureText(string text, FontSpec font) => new(text.Length, font.Size);
        public void PushClip(CanvasRect rect) => Calls.Add(nameof(PushClip));
        public void PopClip() => Calls.Add(nameof(PopClip));
    }

    [Fact]
    public void Replay_ReproducesAllCallsInOrder()
    {
        var list = new DisplayList(new CountingContext());
        var points = new[] { new CanvasPoint(0, 0), new CanvasPoint(1, 1), new CanvasPoint(2, 0) };
        list.Clear(Color.White);
        list.PushClip(new CanvasRect(0, 0, 1, 1));
        list.DrawLine(default, default, new Stroke(Color.Black));
        list.DrawPolyline(points, new Stroke(Color.Black));
        list.DrawPolygon(points, null, Color.Black);
        list.DrawRectangle(default, null, Color.Black);
        list.DrawEllipse(default, 1, 1, null, Color.Black);
        list.DrawText("a", default, new FontSpec("Segoe UI", 10), Color.Black, TextAlignment.Left);
        list.PopClip();
        points[0] = new CanvasPoint(99, 99);

        Assert.Equal(9, list.Count);
        Assert.Equal(new TextSize(3, 10), list.MeasureText("abc", new FontSpec("Segoe UI", 10)));
        var context = new CountingContext();
        list.Replay(context);
        Assert.Equal(["Clear", "PushClip", "DrawLine", "DrawPolyline", "DrawPolygon", "DrawRectangle", "DrawEllipse",
            "DrawText", "PopClip"], context.Calls);
    }
}
