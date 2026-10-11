namespace SharpMath.Canvas;

/// <summary>
///     Describes how an item is drawn.
/// </summary>
/// <param name="Stroke">The stroke for lines and outlines.</param>
/// <param name="Fill">The fill color for polygons, points and arrowheads, or <see langword="null" /> for none.</param>
/// <param name="PointRadius">The radius of points in pixels.</param>
public sealed record ItemStyle(Stroke Stroke, Color? Fill = null, double PointRadius = 3);

/// <summary>
///     Describes the appearance of a <see cref="Canvas2D" />. The defaults reproduce the look of the original canvas.
/// </summary>
/// <remarks>
///     The style is immutable; change it with a <see langword="with" /> expression and assign it to
///     <see cref="Canvas2D.Style" />, e.g. <c>canvas.Style = canvas.Style with { Background = Color.Black };</c>.
/// </remarks>
public sealed record CanvasStyle
{
    /// <summary>
    ///     Gets the default style.
    /// </summary>
    public static CanvasStyle Default { get; } = new();

    /// <summary>
    ///     Gets the background color.
    /// </summary>
    public Color Background { get; init; } = Color.White;

    /// <summary>
    ///     Gets the stroke of the fine grid lines.
    /// </summary>
    public Stroke MinorGridStroke { get; init; } = new(Color.LightGray);

    /// <summary>
    ///     Gets the stroke of the grid lines at labelled ticks.
    /// </summary>
    public Stroke MajorGridStroke { get; init; } = new(Color.LightGray);

    /// <summary>
    ///     Gets the stroke of the coordinate axes.
    /// </summary>
    public Stroke AxisStroke { get; init; } = new(Color.FromRgb(245, 125, 0));

    /// <summary>
    ///     Gets the font of the axis labels.
    /// </summary>
    public FontSpec LabelFont { get; init; } = new("Segoe UI", 11);

    /// <summary>
    ///     Gets the color of the axis labels.
    /// </summary>
    public Color LabelColor { get; init; } = Color.Black;

    /// <summary>
    ///     Gets the minimum distance between fine grid lines in pixels.
    /// </summary>
    public double MinGridSpacing { get; init; } = 20;

    /// <summary>
    ///     Gets the minimum distance between labelled ticks in pixels.
    /// </summary>
    public double MinLabelSpacing { get; init; } = 80;

    /// <summary>
    ///     Gets the stroke of the tracking lines through the pointer.
    /// </summary>
    public Stroke TrackingStroke { get; init; } = new(Color.Gray);

    /// <summary>
    ///     Gets the background of the value indicator.
    /// </summary>
    public Color IndicatorBackground { get; init; } = Color.White;

    /// <summary>
    ///     Gets the border of the value indicator.
    /// </summary>
    public Stroke IndicatorBorder { get; init; } = new(Color.LightGray);

    /// <summary>
    ///     Gets the text color of the value indicator.
    /// </summary>
    public Color IndicatorText { get; init; } = Color.LightGray;

    /// <summary>
    ///     Gets the default style of function graphs.
    /// </summary>
    public ItemStyle FunctionStyle { get; init; } = new(new Stroke(Color.Black));

    /// <summary>
    ///     Gets the default style of vectors; the fill is used for the arrowhead.
    /// </summary>
    public ItemStyle VectorStyle { get; init; } =
        new(new Stroke(Color.FromRgb(122, 125, 201)), Color.FromRgb(122, 125, 201));

    /// <summary>
    ///     Gets the default style of points.
    /// </summary>
    public ItemStyle PointStyle { get; init; } = new(new Stroke(Color.Black), Color.Black);

    /// <summary>
    ///     Gets the default style of infinite lines.
    /// </summary>
    public ItemStyle LineStyle { get; init; } = new(new Stroke(Color.FromRgb(0, 128, 128)));

    /// <summary>
    ///     Gets the default style of line segments.
    /// </summary>
    public ItemStyle SegmentStyle { get; init; } = new(new Stroke(Color.FromRgb(0, 128, 128), 2));

    /// <summary>
    ///     Gets the default style of polygons.
    /// </summary>
    public ItemStyle PolygonStyle { get; init; } =
        new(new Stroke(Color.FromRgb(46, 139, 87)), Color.FromArgb(48, 46, 139, 87));
}
