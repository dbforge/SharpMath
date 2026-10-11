using System.ComponentModel;

namespace SharpMath.Canvas.WinForms;

/// <summary>
///     A WinForms control that displays a <see cref="Canvas2D" />. Drag with the left mouse button to pan and use the
///     mouse wheel to zoom.
/// </summary>
[ToolboxItem(true)]
[DesignerCategory("Code")]
public class Canvas2DControl : Control
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Canvas2DControl" /> class.
    /// </summary>
    public Canvas2DControl()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                 ControlStyles.ResizeRedraw, true);
        Canvas.Changed += OnCanvasChanged;
    }

    /// <summary>
    ///     Gets the canvas model: add items, change the style or move the viewport through it.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Canvas2D Canvas { get; } = new();

    /// <summary>
    ///     Gets or sets a value indicating whether horizontal and vertical lines are drawn through the mouse pointer.
    /// </summary>
    [Category("Behavior")]
    [DefaultValue(false)]
    [Description("Draws horizontal and vertical lines through the mouse pointer.")]
    public bool ShowTrackingLines
    {
        get => Canvas.ShowTrackingLines;
        set => Canvas.ShowTrackingLines = value;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the world coordinates of the mouse pointer are shown next to it.
    /// </summary>
    [Category("Behavior")]
    [DefaultValue(false)]
    [Description("Shows the world coordinates of the mouse pointer.")]
    public bool ShowValueIndicator
    {
        get => Canvas.ShowValueIndicator;
        set => Canvas.ShowValueIndicator = value;
    }

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        base.OnPaint(e);
        using var context = new GdiDrawingContext(e.Graphics);
        Canvas.Render(context, ClientSize.Width, ClientSize.Height);
    }

    /// <inheritdoc />
    protected override void OnMouseDown(MouseEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left)
            Canvas.PointerPressed(new CanvasPoint(e.X, e.Y));
        Focus();
    }

    /// <inheritdoc />
    protected override void OnMouseMove(MouseEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        base.OnMouseMove(e);
        Canvas.PointerMoved(new CanvasPoint(e.X, e.Y));
    }

    /// <inheritdoc />
    protected override void OnMouseUp(MouseEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        base.OnMouseUp(e);
        if (e.Button == MouseButtons.Left)
            Canvas.PointerReleased(new CanvasPoint(e.X, e.Y));
    }

    /// <inheritdoc />
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        Canvas.PointerExited();
    }

    /// <inheritdoc />
    protected override void OnMouseWheel(MouseEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        base.OnMouseWheel(e);
        Canvas.Wheel(new CanvasPoint(e.X, e.Y), (double) e.Delta / SystemInformation.MouseWheelScrollDelta);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            Canvas.Changed -= OnCanvasChanged;
        base.Dispose(disposing);
    }

    private void OnCanvasChanged(object? sender, EventArgs e) => Invalidate();
}
