using BenchmarkDotNet.Attributes;
using SharpMath.Canvas;
using SharpMath.Geometry;

namespace SharpMath.Benchmarks;

[MemoryDiagnoser]
public class CanvasBenchmarks
{
    private readonly Canvas2D _canvas = new();
    private readonly NullDrawingContext _context = new();

    [GlobalSetup]
    public void Setup()
    {
        _canvas.AddFunction(x => x * x * x / 10);
        _canvas.AddFunction("1.5sin(x)");
        _canvas.AddFunction(Math.Tan);
        _canvas.AddVector(new Vector2(4, 3));
        _canvas.AddPolygon(new Polygon(new Point2D(2, -2), new Point2D(6, -2), new Point2D(4, -5)));
    }

    [Benchmark]
    public void Render800x600() => _canvas.Render(_context, 800, 600);

    private sealed class NullDrawingContext : IDrawingContext
    {
        public void Clear(Color color) { }
        public void DrawLine(CanvasPoint startPoint, CanvasPoint endPoint, Stroke stroke) { }
        public void DrawPolyline(ReadOnlySpan<CanvasPoint> points, Stroke stroke) { }
        public void DrawPolygon(ReadOnlySpan<CanvasPoint> points, Stroke? stroke, Color? fill) { }
        public void DrawRectangle(CanvasRect rect, Stroke? stroke, Color? fill) { }
        public void DrawEllipse(CanvasPoint center, double radiusX, double radiusY, Stroke? stroke, Color? fill) { }
        public void DrawText(string text, CanvasPoint position, FontSpec font, Color color, TextAlignment alignment) { }
        public TextSize MeasureText(string text, FontSpec font) => new(text.Length * 6, 12);
        public void PushClip(CanvasRect rect) { }
        public void PopClip() { }
    }
}
