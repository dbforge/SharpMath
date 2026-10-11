using SharpMath.Geometry;
using SkiaSharp;

namespace SharpMath.Canvas.Skia.Tests;

public class SkiaRenderingTests
{
    private static SKColor ToSkia(Color color) => new(color.R, color.G, color.B, color.A);

    private static SKBitmap Render(Canvas2D canvas, int width = 400, int height = 300)
    {
        using var image = canvas.RenderImage(width, height);
        return SKBitmap.FromImage(image);
    }

    [Fact]
    public void RenderImage_DrawsBackgroundAxesAndGrid()
    {
        using var bitmap = Render(new Canvas2D());
        Assert.Equal(400, bitmap.Width);
        Assert.Equal(300, bitmap.Height);

        // Probes away from text: background between grid lines, the axes and a grid line.
        Assert.Equal(SKColors.White, bitmap.GetPixel(10, 280));
        Assert.Equal(ToSkia(CanvasStyle.Default.AxisStroke.Color), bitmap.GetPixel(30, 150));
        Assert.Equal(ToSkia(CanvasStyle.Default.AxisStroke.Color), bitmap.GetPixel(200, 30));
        // Grid lines are snapped to pixel centers, so they are crisp and exactly the grid color.
        Assert.Equal(ToSkia(CanvasStyle.Default.MinorGridStroke.Color), bitmap.GetPixel(40, 280));
    }

    [Fact]
    public void Items_AreDrawnWithTheirColors()
    {
        var canvas = new Canvas2D();
        var red = Color.FromRgb(255, 0, 0);
        canvas.AddPolygon(new Polygon(new Point2D(2, 2), new Point2D(6, 2), new Point2D(6, 6), new Point2D(2, 6)),
            new ItemStyle(new Stroke(red, 2), red));
        canvas.AddPoint(new Point2D(-5, -5), new ItemStyle(new Stroke(Color.Black), Color.Black, 5));
        using var bitmap = Render(canvas);

        Assert.Equal(SKColors.Red, bitmap.GetPixel(285, 75));
        Assert.Equal(SKColors.Black, bitmap.GetPixel(100, 250));
    }

    [Fact]
    public void Text_IsMeasuredAndDrawn()
    {
        using var surface = SKSurface.Create(new SKImageInfo(100, 40));
        using var context = new SkiaDrawingContext(surface.Canvas);
        var font = new FontSpec("Segoe UI", 20);
        context.Clear(Color.White);
        var size = context.MeasureText("1234", font);
        Assert.InRange(size.Width, 10, 100);
        Assert.InRange(size.Height, 15, 40);
        context.DrawText("1234", new CanvasPoint(50, 5), font, Color.Black, TextAlignment.Center);
        context.DrawText("1", new CanvasPoint(0, 5), font, Color.Black, TextAlignment.Left);
        context.DrawText("1", new CanvasPoint(100, 5), font, Color.Black, TextAlignment.Right);
        using var image = surface.Snapshot();
        using var bitmap = SKBitmap.FromImage(image);
        var dark = 0;
        for (var x = 0; x < 100; x++)
        for (var y = 0; y < 40; y++)
            if (bitmap.GetPixel(x, y).Red < 128)
                dark++;
        Assert.True(dark > 20, $"Expected the text to cover some pixels, got {dark}.");
    }

    [Fact]
    public void Context_DrawsEveryPrimitive()
    {
        using var surface = SKSurface.Create(new SKImageInfo(50, 50));
        using var context = new SkiaDrawingContext(surface.Canvas);
        var blue = new Stroke(Color.FromRgb(0, 0, 255), 3, DashStyle.Dashed);
        context.Clear(Color.White);
        context.PushClip(new CanvasRect(0, 0, 40, 40));
        context.DrawRectangle(new CanvasRect(5, 5, 10, 10), blue, Color.FromRgb(0, 255, 0));
        context.DrawEllipse(new CanvasPoint(30, 30), 5, 5, new Stroke(Color.Black, 1, DashStyle.Dotted), Color.Black);
        context.DrawPolyline([new CanvasPoint(0, 45), new CanvasPoint(50, 45)], blue);
        context.DrawPolyline([new CanvasPoint(0, 0)], blue);
        context.DrawPolygon([new CanvasPoint(0, 0), new CanvasPoint(1, 1)], blue, null);
        context.PopClip();
        context.DrawLine(new CanvasPoint(45, 0), new CanvasPoint(45, 50), new Stroke(Color.Black, 2));
        using var image = surface.Snapshot();
        using var bitmap = SKBitmap.FromImage(image);

        Assert.Equal(new SKColor(0, 255, 0), bitmap.GetPixel(10, 10));
        Assert.Equal(SKColors.Black, bitmap.GetPixel(30, 30));
        Assert.Equal(SKColors.White, bitmap.GetPixel(25, 45));
        Assert.Equal(SKColors.Black, bitmap.GetPixel(45, 25));
    }

    [Fact]
    public void RenderPng_WritesAPngFile()
    {
        using var stream = new MemoryStream();
        new Canvas2D().RenderPng(stream, 64, 48);
        var bytes = stream.ToArray();
        Assert.Equal([0x89, (byte) 'P', (byte) 'N', (byte) 'G'], bytes[..4]);
        using var decoded = SKBitmap.Decode(bytes);
        Assert.Equal(64, decoded.Width);
    }

    [Fact]
    public void Validation()
    {
        var canvas = new Canvas2D();
        Assert.Throws<ArgumentOutOfRangeException>(() => canvas.RenderImage(0, 1));
        Assert.Throws<ArgumentNullException>(() => canvas.RenderPng(null!, 1, 1));
        Assert.Throws<ArgumentNullException>(() => ((Canvas2D) null!).RenderImage(1, 1));
        Assert.Throws<ArgumentNullException>(() => new SkiaDrawingContext(null!));
    }
}
