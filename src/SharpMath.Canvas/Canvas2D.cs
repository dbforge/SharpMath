using System.Collections.ObjectModel;
using System.Collections.Specialized;
using SharpMath.Expressions;
using SharpMath.Geometry;

namespace SharpMath.Canvas;

/// <summary>
///     A platform-neutral two-dimensional coordinate system that plots functions, vectors, points, lines, segments and
///     polygons. Hosts (e.g. a WinForms or Avalonia control) forward input to it and call
///     <see cref="Render(IDrawingContext, double, double)" /> with a renderer.
/// </summary>
public sealed class Canvas2D
{
    private const double IndicatorPadding = 3;
    private const double IndicatorOffset = 15;

    private readonly ItemRenderer _itemRenderer = new();
    private CanvasPoint? _dragPosition;
    private CanvasPoint? _pointer;
    private bool _rendering;
    private bool _showTrackingLines;
    private bool _showValueIndicator;
    private CanvasStyle _style = CanvasStyle.Default;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Canvas2D" /> class.
    /// </summary>
    public Canvas2D()
    {
        Items.CollectionChanged += OnItemsChanged;
        Viewport.Changed += (_, _) => RaiseChanged();
    }

    /// <summary>
    ///     Occurs when the canvas needs to be redrawn, e.g. because an item, the style or the viewport changed.
    /// </summary>
    public event EventHandler? Changed;

    /// <summary>
    ///     Gets the items to draw, in drawing order.
    /// </summary>
    public ObservableCollection<CanvasItem> Items { get; } = [];

    /// <summary>
    ///     Gets the mapping between world and screen coordinates.
    /// </summary>
    public Viewport Viewport { get; } = new();

    /// <summary>
    ///     Gets or sets the appearance.
    /// </summary>
    public CanvasStyle Style
    {
        get => _style;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (ReferenceEquals(_style, value))
                return;
            _style = value;
            RaiseChanged();
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether horizontal and vertical lines are drawn through the pointer.
    /// </summary>
    public bool ShowTrackingLines
    {
        get => _showTrackingLines;
        set
        {
            if (_showTrackingLines == value)
                return;
            _showTrackingLines = value;
            RaiseChanged();
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the world coordinates of the pointer are shown next to it.
    /// </summary>
    public bool ShowValueIndicator
    {
        get => _showValueIndicator;
        set
        {
            if (_showValueIndicator == value)
                return;
            _showValueIndicator = value;
            RaiseChanged();
        }
    }

    /// <summary>
    ///     Gets the last known pointer position in screen coordinates, or <see langword="null" /> if the pointer is outside.
    /// </summary>
    public CanvasPoint? PointerPosition => _pointer;

    /// <summary>
    ///     Adds the graph of a function.
    /// </summary>
    /// <param name="function">The function.</param>
    /// <param name="style">The style, or <see langword="null" /> for the default function style.</param>
    /// <returns>The added item.</returns>
    public FunctionItem AddFunction(Func<double, double> function, ItemStyle? style = null) =>
        Add(new FunctionItem(function) { Style = style });

    /// <summary>
    ///     Adds the graph of an expression in the variable <c>x</c>, e.g. <c>"2x^2 - 3x"</c>.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="style">The style, or <see langword="null" /> for the default function style.</param>
    /// <param name="context">The functions, constants and variables to use; the built-ins if <see langword="null" />.</param>
    /// <returns>The added item.</returns>
    /// <exception cref="ParserException">The expression is invalid.</exception>
    /// <exception cref="ArgumentException">The expression uses variables other than <c>x</c>.</exception>
    public FunctionItem AddFunction(string expression, ItemStyle? style = null, ExpressionContext? context = null) =>
        Add(FunctionItem.FromExpression(expression, context) with { Style = style });

    /// <summary>
    ///     Adds a vector drawn as an arrow.
    /// </summary>
    /// <param name="vector">The vector.</param>
    /// <param name="tail">The start of the arrow; the origin if <see langword="null" />.</param>
    /// <param name="style">The style, or <see langword="null" /> for the default vector style.</param>
    /// <returns>The added item.</returns>
    public VectorItem AddVector(Vector2 vector, Point2D? tail = null, ItemStyle? style = null) =>
        Add(new VectorItem(vector) { Tail = tail ?? Point2D.Origin, Style = style });

    /// <summary>
    ///     Adds a point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="style">The style, or <see langword="null" /> for the default point style.</param>
    /// <returns>The added item.</returns>
    public PointItem AddPoint(Point2D point, ItemStyle? style = null) => Add(new PointItem(point) { Style = style });

    /// <summary>
    ///     Adds an infinite line.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="style">The style, or <see langword="null" /> for the default line style.</param>
    /// <returns>The added item.</returns>
    public LineItem AddLine(Line2D line, ItemStyle? style = null) => Add(new LineItem(line) { Style = style });

    /// <summary>
    ///     Adds a line segment.
    /// </summary>
    /// <param name="segment">The segment.</param>
    /// <param name="style">The style, or <see langword="null" /> for the default segment style.</param>
    /// <returns>The added item.</returns>
    public SegmentItem AddSegment(LineSegment2D segment, ItemStyle? style = null) =>
        Add(new SegmentItem(segment) { Style = style });

    /// <summary>
    ///     Adds a polygon.
    /// </summary>
    /// <param name="polygon">The polygon.</param>
    /// <param name="style">The style, or <see langword="null" /> for the default polygon style.</param>
    /// <returns>The added item.</returns>
    public PolygonItem AddPolygon(Polygon polygon, ItemStyle? style = null)
    {
        ArgumentNullException.ThrowIfNull(polygon);
        return Add(new PolygonItem(polygon) { Style = style });
    }

    /// <summary>
    ///     Starts panning at the specified position (e.g. on mouse button down).
    /// </summary>
    /// <param name="position">The pointer position in screen coordinates.</param>
    public void PointerPressed(CanvasPoint position)
    {
        _dragPosition = position;
        UpdatePointer(position);
    }

    /// <summary>
    ///     Updates the pointer position and pans if a drag is in progress.
    /// </summary>
    /// <param name="position">The pointer position in screen coordinates.</param>
    public void PointerMoved(CanvasPoint position)
    {
        if (_dragPosition is { } previous)
        {
            _dragPosition = position;
            Viewport.Pan(position.X - previous.X, position.Y - previous.Y);
        }

        UpdatePointer(position);
    }

    /// <summary>
    ///     Ends panning (e.g. on mouse button up).
    /// </summary>
    /// <param name="position">The pointer position in screen coordinates.</param>
    public void PointerReleased(CanvasPoint position)
    {
        PointerMoved(position);
        _dragPosition = null;
    }

    /// <summary>
    ///     Notifies the canvas that the pointer left it.
    /// </summary>
    public void PointerExited()
    {
        _dragPosition = null;
        if (_pointer is null)
            return;
        _pointer = null;
        if (ShowTrackingLines || ShowValueIndicator)
            RaiseChanged();
    }

    /// <summary>
    ///     Zooms around the pointer (e.g. on mouse wheel). Each notch zooms by a factor of 1.25.
    /// </summary>
    /// <param name="position">The pointer position in screen coordinates.</param>
    /// <param name="notches">The number of wheel notches; positive values zoom in.</param>
    public void Wheel(CanvasPoint position, double notches)
    {
        if (notches == 0 || !double.IsFinite(notches))
            return;
        Viewport.ZoomAt(position, Math.Pow(1.25, notches));
    }

    /// <summary>
    ///     Draws the canvas.
    /// </summary>
    /// <param name="context">The renderer.</param>
    /// <param name="width">The width of the drawing area in pixels.</param>
    /// <param name="height">The height of the drawing area in pixels.</param>
    public void Render(IDrawingContext context, double width, double height)
    {
        ArgumentNullException.ThrowIfNull(context);
        _rendering = true;
        try
        {
            Viewport.Resize(width, height);
        }
        finally
        {
            _rendering = false;
        }

        var bounds = new CanvasRect(0, 0, width, height);
        var guardBand = new CanvasRect(-10 * width - 10, -10 * height - 10, 21 * width + 20, 21 * height + 20);

        context.Clear(Style.Background);
        context.PushClip(bounds);
        var gridStep = GridRenderer.Draw(context, Viewport, Style);
        foreach (var item in Items)
            _itemRenderer.Draw(context, item, Viewport, Style, guardBand);
        DrawPointerFeatures(context, gridStep);
        context.PopClip();
    }

    private T Add<T>(T item) where T : CanvasItem
    {
        Items.Add(item);
        return item;
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e) => RaiseChanged();

    private void UpdatePointer(CanvasPoint position)
    {
        if (_pointer == position)
            return;
        _pointer = position;
        if (ShowTrackingLines || ShowValueIndicator)
            RaiseChanged();
    }

    private void RaiseChanged()
    {
        if (!_rendering)
            Changed?.Invoke(this, EventArgs.Empty);
    }

    private void DrawPointerFeatures(IDrawingContext context, (double X, double Y) gridStep)
    {
        if (_pointer is not { } pointer)
            return;

        if (ShowTrackingLines)
        {
            var x = GridRenderer.Snap(pointer.X, Style.TrackingStroke);
            var y = GridRenderer.Snap(pointer.Y, Style.TrackingStroke);
            context.DrawLine(new CanvasPoint(x, 0), new CanvasPoint(x, Viewport.Height), Style.TrackingStroke);
            context.DrawLine(new CanvasPoint(0, y), new CanvasPoint(Viewport.Width, y), Style.TrackingStroke);
        }

        if (!ShowValueIndicator)
            return;

        // One decimal more than the grid step needs, so the value changes visibly while moving at any zoom level.
        var world = Viewport.ScreenToWorld(pointer);
        var text = Ticks.FormatValue(world.X, gridStep.X) + " | " + Ticks.FormatValue(world.Y, gridStep.Y);
        var size = context.MeasureText(text, Style.LabelFont);
        var box = new CanvasRect(pointer.X + IndicatorOffset, pointer.Y + IndicatorOffset,
            Math.Max(80, size.Width + 2 * IndicatorPadding), Math.Max(25, size.Height + 2 * IndicatorPadding));
        context.DrawRectangle(box, Style.IndicatorBorder, Style.IndicatorBackground);
        context.DrawText(text, new CanvasPoint(box.X + box.Width / 2, box.Y + (box.Height - size.Height) / 2),
            Style.LabelFont, Style.IndicatorText, TextAlignment.Center);
    }
}
