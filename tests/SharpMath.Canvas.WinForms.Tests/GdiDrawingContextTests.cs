using SharpMath.Geometry;
using CanvasColor = SharpMath.Canvas.Color;
using DrawingColor = System.Drawing.Color;

namespace SharpMath.Canvas.WinForms.Tests;

public class GdiDrawingContextTests
{
    private static DrawingColor ToGdi(CanvasColor color) => DrawingColor.FromArgb(color.A, color.R, color.G, color.B);

    private static Bitmap Render(Canvas2D canvas, int width = 400, int height = 300)
    {
        var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        using var context = new GdiDrawingContext(graphics);
        canvas.Render(context, width, height);
        return bitmap;
    }

    [Fact]
    public void Render_DrawsBackgroundAxesAndGrid()
    {
        using var bitmap = Render(new Canvas2D());
        Assert.Equal(DrawingColor.White.ToArgb(), bitmap.GetPixel(10, 280).ToArgb());
        Assert.Equal(ToGdi(CanvasStyle.Default.AxisStroke.Color).ToArgb(), bitmap.GetPixel(30, 150).ToArgb());
        Assert.Equal(ToGdi(CanvasStyle.Default.AxisStroke.Color).ToArgb(), bitmap.GetPixel(200, 30).ToArgb());
        Assert.Equal(ToGdi(CanvasStyle.Default.MinorGridStroke.Color).ToArgb(), bitmap.GetPixel(40, 280).ToArgb());
    }

    [Fact]
    public void Render_DrawsItems()
    {
        var canvas = new Canvas2D();
        var red = CanvasColor.FromRgb(255, 0, 0);
        canvas.AddPolygon(new Polygon(new Point2D(2, 2), new Point2D(6, 2), new Point2D(6, 6), new Point2D(2, 6)),
            new ItemStyle(new Stroke(red, 2), red));
        canvas.AddPoint(new Point2D(-5, -5), new ItemStyle(new Stroke(CanvasColor.Black), CanvasColor.Black, 5));
        canvas.AddFunction("x", new ItemStyle(new Stroke(CanvasColor.Black, 1, DashStyle.Dashed)));
        canvas.AddVector(new Vector2(-4, 4));
        canvas.ShowTrackingLines = true;
        canvas.ShowValueIndicator = true;
        // Away from the probed pixels, so the tracking lines do not cover them.
        canvas.PointerMoved(new CanvasPoint(370, 20));
        using var bitmap = Render(canvas);

        Assert.Equal(DrawingColor.Red.ToArgb(), bitmap.GetPixel(285, 75).ToArgb());
        Assert.Equal(DrawingColor.Black.ToArgb(), bitmap.GetPixel(100, 250).ToArgb());
    }

    [Fact]
    public void Text_IsMeasured()
    {
        using var bitmap = new Bitmap(10, 10);
        using var graphics = Graphics.FromImage(bitmap);
        using var context = new GdiDrawingContext(graphics);
        var size = context.MeasureText("1234", new FontSpec("Segoe UI", 20));
        Assert.InRange(size.Width, 10, 100);
        Assert.InRange(size.Height, 15, 40);
        context.DrawText("x", new CanvasPoint(0, 0), new FontSpec("Font that does not exist", 10), CanvasColor.Black,
            TextAlignment.Right);
    }

    [Fact]
    public void Dispose_RestoresGraphicsSettings()
    {
        using var bitmap = new Bitmap(10, 10);
        using var graphics = Graphics.FromImage(bitmap);
        var smoothing = graphics.SmoothingMode;
        var context = new GdiDrawingContext(graphics);
        context.PushClip(new CanvasRect(0, 0, 5, 5));
        context.Dispose();
        context.Dispose();
        Assert.Equal(smoothing, graphics.SmoothingMode);
        using (var clip = graphics.Clip)
            Assert.True(clip.IsInfinite(graphics));
        using var other = new GdiDrawingContext(graphics);
        Assert.Throws<InvalidOperationException>(() => other.PopClip());
        Assert.Throws<ArgumentNullException>(() => new GdiDrawingContext(null!));
    }
}
