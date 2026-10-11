using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using SharpMath.Canvas.Skia;
using SkiaSharp;

namespace SharpMath.Canvas.Avalonia;

/// <summary>
///     An Avalonia control that displays a <see cref="Canvas2D" />. Drag with the left mouse button to pan and use the
///     mouse wheel to zoom. Requires Avalonia's Skia renderer (the default on all desktop platforms).
/// </summary>
public class Canvas2DView : Control
{
    /// <summary>
    ///     Defines the <see cref="ShowTrackingLines" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowTrackingLinesProperty =
        AvaloniaProperty.Register<Canvas2DView, bool>(nameof(ShowTrackingLines));

    /// <summary>
    ///     Defines the <see cref="ShowValueIndicator" /> property.
    /// </summary>
    public static readonly StyledProperty<bool> ShowValueIndicatorProperty =
        AvaloniaProperty.Register<Canvas2DView, bool>(nameof(ShowValueIndicator));

    // Text is measured with Skia on the UI thread through a context that is kept while the view is attached, so
    // fonts are looked up once rather than every frame.
    private TextMeasurer? _measurer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Canvas2DView" /> class.
    /// </summary>
    public Canvas2DView()
    {
        ClipToBounds = true;
        Focusable = true;
        Canvas.Changed += OnCanvasChanged;
    }

    /// <summary>
    ///     Gets the canvas model: add items, change the style or move the viewport through it.
    /// </summary>
    public Canvas2D Canvas { get; } = new();

    /// <summary>
    ///     Gets or sets a value indicating whether horizontal and vertical lines are drawn through the pointer.
    /// </summary>
    public bool ShowTrackingLines
    {
        get => GetValue(ShowTrackingLinesProperty);
        set => SetValue(ShowTrackingLinesProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the world coordinates of the pointer are shown next to it.
    /// </summary>
    public bool ShowValueIndicator
    {
        get => GetValue(ShowValueIndicatorProperty);
        set => SetValue(ShowValueIndicatorProperty, value);
    }

    /// <inheritdoc />
    public override void Render(DrawingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var size = Bounds.Size;
        _measurer ??= new TextMeasurer();
        var displayList = new DisplayList(_measurer.Context);
        Canvas.Render(displayList, size.Width, size.Height);
        context.Custom(new CanvasDrawOperation(new Rect(size), displayList));
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _measurer?.Dispose();
        _measurer = null;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        ArgumentNullException.ThrowIfNull(change);
        base.OnPropertyChanged(change);
        if (change.Property == ShowTrackingLinesProperty)
            Canvas.ShowTrackingLines = change.GetNewValue<bool>();
        else if (change.Property == ShowValueIndicatorProperty)
            Canvas.ShowValueIndicator = change.GetNewValue<bool>();
    }

    /// <inheritdoc />
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        base.OnPointerPressed(e);
        var point = e.GetCurrentPoint(this);
        if (!point.Properties.IsLeftButtonPressed)
            return;
        Canvas.PointerPressed(ToCanvas(point.Position));
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    /// <inheritdoc />
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        base.OnPointerMoved(e);
        Canvas.PointerMoved(ToCanvas(e.GetPosition(this)));
    }

    /// <inheritdoc />
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        base.OnPointerReleased(e);
        if (e.InitialPressMouseButton != MouseButton.Left)
            return;
        Canvas.PointerReleased(ToCanvas(e.GetPosition(this)));
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    /// <inheritdoc />
    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        Canvas.PointerExited();
    }

    /// <inheritdoc />
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        base.OnPointerWheelChanged(e);
        Canvas.Wheel(ToCanvas(e.GetPosition(this)), e.Delta.Y);
        e.Handled = true;
    }

    private static CanvasPoint ToCanvas(Point point) => new(point.X, point.Y);

    private void OnCanvasChanged(object? sender, EventArgs e)
    {
        if (Dispatcher.UIThread.CheckAccess())
            InvalidateVisual();
        else
            Dispatcher.UIThread.Post(InvalidateVisual);
    }

    /// <summary>
    ///     Replays the recorded canvas on Skia's canvas on the render thread.
    /// </summary>
    private sealed class CanvasDrawOperation(Rect bounds, DisplayList displayList) : ICustomDrawOperation
    {
        public Rect Bounds => bounds;

        public bool HitTest(Point p) => bounds.Contains(p);

        public void Render(ImmediateDrawingContext context)
        {
            if (context.TryGetFeature(typeof(ISkiaSharpApiLeaseFeature)) is not ISkiaSharpApiLeaseFeature feature)
                return;

            using var lease = feature.Lease();
            using var skia = new SkiaDrawingContext(lease.SkCanvas);
            skia.PushClip(new CanvasRect(0, 0, bounds.Width, bounds.Height));
            displayList.Replay(skia);
            skia.PopClip();
        }

        public bool Equals(ICustomDrawOperation? other) => ReferenceEquals(this, other);

        public void Dispose()
        {
        }
    }

    /// <summary>
    ///     A Skia context on a 1x1 bitmap, used only to measure text.
    /// </summary>
    private sealed class TextMeasurer : IDisposable
    {
        private readonly SKBitmap _bitmap = new(1, 1);
        private readonly SKCanvas _canvas;

        public TextMeasurer()
        {
            _canvas = new SKCanvas(_bitmap);
            Context = new SkiaDrawingContext(_canvas);
        }

        public SkiaDrawingContext Context { get; }

        public void Dispose()
        {
            Context.Dispose();
            _canvas.Dispose();
            _bitmap.Dispose();
        }
    }
}
