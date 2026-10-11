using SharpMath.Expressions;
using SharpMath.Geometry;

namespace SharpMath.Canvas;

/// <summary>
///     The base class of everything a <see cref="Canvas2D" /> draws. Items are immutable; replace an item in
///     <see cref="Canvas2D.Items" /> to change it.
/// </summary>
public abstract record CanvasItem
{
    /// <summary>
    ///     Gets the style of this item, or <see langword="null" /> to use the default style for its type from
    ///     <see cref="CanvasStyle" />.
    /// </summary>
    public ItemStyle? Style { get; init; }
}

/// <summary>
///     The graph of a function <c>y = f(x)</c>.
/// </summary>
/// <param name="Function">The function. Non-finite results and arithmetic exceptions leave gaps in the graph.</param>
public sealed record FunctionItem(Func<double, double> Function) : CanvasItem
{
    /// <summary>
    ///     Gets the parsed expression if the item was created from one; otherwise, <see langword="null" />.
    /// </summary>
    public MathExpression? Expression { get; init; }

    /// <summary>
    ///     Creates the graph of an expression in the variable <c>x</c>, e.g. <c>"2x^2 - 3x"</c>.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="context">The functions, constants and variables to use; the built-ins if <see langword="null" />.</param>
    /// <returns>The item.</returns>
    /// <exception cref="ParserException">The expression is invalid.</exception>
    /// <exception cref="ArgumentException">The expression uses variables other than <c>x</c>.</exception>
    public static FunctionItem FromExpression(string expression, ExpressionContext? context = null)
    {
        var parsed = MathExpression.Parse(expression, context);
        return new FunctionItem(parsed.ToFunc("x")) { Expression = parsed };
    }
}

/// <summary>
///     A vector drawn as an arrow from <see cref="Tail" /> to <c>Tail + Vector</c>.
/// </summary>
/// <param name="Vector">The vector.</param>
public sealed record VectorItem(Vector2 Vector) : CanvasItem
{
    /// <summary>
    ///     Gets the start of the arrow; the origin by default.
    /// </summary>
    public Point2D Tail { get; init; }
}

/// <summary>
///     A point drawn as a dot.
/// </summary>
/// <param name="Point">The point.</param>
public sealed record PointItem(Point2D Point) : CanvasItem;

/// <summary>
///     An infinite line, clipped to the visible area.
/// </summary>
/// <param name="Line">The line.</param>
public sealed record LineItem(Line2D Line) : CanvasItem;

/// <summary>
///     A line segment.
/// </summary>
/// <param name="Segment">The segment.</param>
public sealed record SegmentItem(LineSegment2D Segment) : CanvasItem;

/// <summary>
///     A polygon with an outline and an optional fill.
/// </summary>
/// <param name="Polygon">The polygon.</param>
public sealed record PolygonItem(Polygon Polygon) : CanvasItem;
