using SharpMath.Geometry;

namespace SharpMath.Canvas.WinForms.Tests;

public class Canvas2DControlTests
{
    private sealed class TestControl : Canvas2DControl
    {
        public int Invalidations { get; private set; }

        public void Press(int x, int y) => OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, x, y, 0));
        public void MovePointer(int x, int y) => OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, x, y, 0));
        public void Release(int x, int y) => OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, x, y, 0));
        public void Scroll(int x, int y, int delta) => OnMouseWheel(new MouseEventArgs(MouseButtons.None, 0, x, y, delta));
        public void LeavePointer() => OnMouseLeave(EventArgs.Empty);

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            Invalidations++;
            base.OnInvalidated(e);
        }
    }

    [Fact]
    public void Paint_RendersTheCanvas()
    {
        using var control = new Canvas2DControl { Size = new Size(200, 100) };
        control.Canvas.AddPoint(new Point2D(0, 0), new ItemStyle(new Stroke(Color.Black), Color.Black, 4));
        using var bitmap = new Bitmap(200, 100);
        control.DrawToBitmap(bitmap, new Rectangle(0, 0, 200, 100));
        Assert.Equal(System.Drawing.Color.Black.ToArgb(), bitmap.GetPixel(100, 50).ToArgb());
    }

    [Fact]
    public void MouseInput_PansZoomsAndInvalidates()
    {
        using var control = new TestControl { Size = new Size(200, 100), ShowTrackingLines = true };

        // Invalidate only raises Invalidated once the window handle exists.
        _ = control.Handle;
        control.Canvas.Viewport.Resize(200, 100);
        Assert.True(control.ShowTrackingLines);
        control.ShowValueIndicator = true;
        Assert.True(control.Canvas.ShowValueIndicator);

        control.Press(10, 10);
        control.MovePointer(30, 20);
        control.Release(30, 20);
        Assert.Equal(new CanvasPoint(120, 60), control.Canvas.Viewport.Origin);

        control.Scroll(120, 60, 120);
        Assert.Equal(25, control.Canvas.Viewport.ScaleX, 9);
        control.LeavePointer();
        Assert.Null(control.Canvas.PointerPosition);
        Assert.True(control.Invalidations > 0);
    }
}
