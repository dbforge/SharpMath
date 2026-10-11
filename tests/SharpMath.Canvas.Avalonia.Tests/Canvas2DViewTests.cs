using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using System.Runtime.InteropServices;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SharpMath.Geometry;

namespace SharpMath.Canvas.Avalonia.Tests;

public class Canvas2DViewTests
{
    private static (Window Window, Canvas2DView View) Show(int width = 200, int height = 100)
    {
        var view = new Canvas2DView();
        var window = new Window { Width = width, Height = height, Content = view };
        window.Show();
        return (window, view);
    }

    private static Func<int, int, (byte R, byte G, byte B)> Pixels(Bitmap frame)
    {
        var size = frame.PixelSize;
        var bytes = new byte[size.Width * size.Height * 4];
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            frame.CopyPixels(new PixelRect(size), handle.AddrOfPinnedObject(), bytes.Length, size.Width * 4);
        }
        finally
        {
            handle.Free();
        }

        var bgra = frame.Format == PixelFormat.Bgra8888;
        return (x, y) =>
        {
            var i = (y * size.Width + x) * 4;
            return bgra ? (bytes[i + 2], bytes[i + 1], bytes[i]) : (bytes[i], bytes[i + 1], bytes[i + 2]);
        };
    }

    [AvaloniaFact]
    public void Render_DrawsTheCanvasWithSkia()
    {
        var (window, view) = Show();
        view.Canvas.AddPoint(Point2D.Origin, new ItemStyle(new Stroke(Color.Black), Color.Black, 4));
        using var frame = window.CaptureRenderedFrame();
        Assert.NotNull(frame);
        var pixel = Pixels(frame);

        Assert.Equal(((byte) 0, (byte) 0, (byte) 0), pixel(100, 50));
        Assert.Equal(((byte) 255, (byte) 255, (byte) 255), pixel(10, 80));
        window.Close();
    }

    [AvaloniaFact]
    public void MouseInput_PansAndZooms()
    {
        var (window, view) = Show();
        window.CaptureRenderedFrame();
        Assert.Equal(new CanvasPoint(100, 50), view.Canvas.Viewport.Origin);

        window.MouseDown(new Point(10, 10), MouseButton.Left);
        window.MouseMove(new Point(30, 20));
        window.MouseUp(new Point(30, 20), MouseButton.Left);
        Assert.Equal(new CanvasPoint(120, 60), view.Canvas.Viewport.Origin);

        window.MouseWheel(new Point(120, 60), new Vector(0, 1));
        Assert.Equal(25, view.Canvas.Viewport.ScaleX, 9);
        Assert.Equal(new CanvasPoint(120, 60), view.Canvas.Viewport.Origin);

        window.MouseDown(new Point(50, 50), MouseButton.Right);
        window.MouseMove(new Point(60, 60));
        window.MouseUp(new Point(60, 60), MouseButton.Right);
        Assert.Equal(new CanvasPoint(120, 60), view.Canvas.Viewport.Origin);
        window.Close();
    }

    [AvaloniaFact]
    public void Properties_ForwardToTheCanvas()
    {
        var view = new Canvas2DView { ShowTrackingLines = true, ShowValueIndicator = true };
        Assert.True(view.Canvas.ShowTrackingLines);
        Assert.True(view.Canvas.ShowValueIndicator);
        view.ShowTrackingLines = false;
        Assert.False(view.Canvas.ShowTrackingLines);
        view.ShowValueIndicator = false;
        Assert.False(view.Canvas.ShowValueIndicator);
    }

    [AvaloniaFact]
    public void CanvasChanges_TriggerARedraw()
    {
        var (window, view) = Show();
        window.CaptureRenderedFrame();
        view.Canvas.AddPolygon(new Polygon(new Point2D(-10, -10), new Point2D(10, -10), new Point2D(10, 10)),
            new ItemStyle(new Stroke(Color.Black), Color.FromRgb(255, 0, 0)));
        using var frame = window.CaptureRenderedFrame();
        Assert.NotNull(frame);
        Assert.Equal(((byte) 255, (byte) 0, (byte) 0), Pixels(frame)(140, 40));
        window.Close();
    }
}
