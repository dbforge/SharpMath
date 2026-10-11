namespace SharpMath.Canvas;

/// <summary>
///     Draws the items of a <see cref="Canvas2D" />, clipping them to a guard band around the visible area.
/// </summary>
internal sealed class ItemRenderer
{
    private const double ArrowLength = 10;
    private const double ArrowHalfWidth = 4;

    private readonly List<CanvasPoint> _polygon = [];
    private readonly FunctionSampler _sampler = new();

    public void Draw(IDrawingContext context, CanvasItem item, Viewport viewport, CanvasStyle style,
        CanvasRect guardBand)
    {
        switch (item)
        {
            case FunctionItem function:
                var stroke = (function.Style ?? style.FunctionStyle).Stroke;
                _sampler.Sample(function.Function, viewport, guardBand);
                for (var i = 0; i < _sampler.PolylineCount; i++)
                    context.DrawPolyline(_sampler.GetPolyline(i), stroke);
                break;
            case VectorItem vector:
                DrawVector(context, vector, viewport, vector.Style ?? style.VectorStyle, guardBand);
                break;
            case PointItem point:
                DrawPoint(context, point, viewport, point.Style ?? style.PointStyle);
                break;
            case LineItem line:
                DrawLine(context, line, viewport, (line.Style ?? style.LineStyle).Stroke);
                break;
            case SegmentItem segment:
                DrawSegment(context, viewport.WorldToScreen(segment.Segment.Start),
                    viewport.WorldToScreen(segment.Segment.End), (segment.Style ?? style.SegmentStyle).Stroke,
                    guardBand);
                break;
            case PolygonItem polygon:
                DrawPolygon(context, polygon, viewport, polygon.Style ?? style.PolygonStyle, guardBand);
                break;
        }
    }

    private static void DrawVector(IDrawingContext context, VectorItem item, Viewport viewport, ItemStyle style,
        CanvasRect guardBand)
    {
        var tail = viewport.WorldToScreen(item.Tail);
        var tip = viewport.WorldToScreen(item.Tail + item.Vector);
        var dx = tip.X - tail.X;
        var dy = tip.Y - tail.Y;
        var length = Math.Sqrt(dx * dx + dy * dy);
        if (!double.IsFinite(length) || length == 0)
            return;

        // The shaft ends at the base of the arrowhead.
        var ux = dx / length;
        var uy = dy / length;
        var headLength = Math.Min(ArrowLength, length);
        var basePoint = new CanvasPoint(tip.X - ux * headLength, tip.Y - uy * headLength);
        DrawSegment(context, tail, basePoint, style.Stroke, guardBand);

        if (!guardBand.Contains(tip))
            return;
        var halfWidth = ArrowHalfWidth * headLength / ArrowLength;
        ReadOnlySpan<CanvasPoint> head =
        [
            tip,
            new(basePoint.X - uy * halfWidth, basePoint.Y + ux * halfWidth),
            new(basePoint.X + uy * halfWidth, basePoint.Y - ux * halfWidth)
        ];
        context.DrawPolygon(head, style.Stroke, style.Fill ?? style.Stroke.Color);
    }

    private static void DrawPoint(IDrawingContext context, PointItem item, Viewport viewport, ItemStyle style)
    {
        var center = viewport.WorldToScreen(item.Point);
        var radius = style.PointRadius;
        var visible = new CanvasRect(-radius, -radius, viewport.Width + 2 * radius, viewport.Height + 2 * radius);
        if (visible.Contains(center))
            context.DrawEllipse(center, radius, radius, style.Stroke, style.Fill);
    }

    private static void DrawLine(IDrawingContext context, LineItem item, Viewport viewport, Stroke stroke)
    {
        var line = item.Line;
        if (!double.IsFinite(line.A) || (line.A == 0 && line.B == 0))
            return;

        // Two points beyond the visible area, clipped to the drawing area.
        var area = viewport.VisibleArea;
        var center = line.ClosestPoint(area.Center);
        var reach = Math.Sqrt(area.Width * area.Width + area.Height * area.Height) + 1;
        var from = viewport.WorldToScreen(center - line.Direction * reach);
        var to = viewport.WorldToScreen(center + line.Direction * reach);
        var bounds = new CanvasRect(-1, -1, viewport.Width + 2, viewport.Height + 2);
        if (Clipping.ClipSegment(bounds, ref from, ref to))
            context.DrawLine(from, to, stroke);
    }

    private static void DrawSegment(IDrawingContext context, CanvasPoint from, CanvasPoint to, Stroke stroke,
        CanvasRect guardBand)
    {
        if (Clipping.ClipSegment(guardBand, ref from, ref to))
            context.DrawLine(from, to, stroke);
    }

    private void DrawPolygon(IDrawingContext context, PolygonItem item, Viewport viewport, ItemStyle style,
        CanvasRect guardBand)
    {
        _polygon.Clear();
        foreach (var point in item.Polygon.Points)
            _polygon.Add(viewport.WorldToScreen(point));
        var clipped = Clipping.ClipPolygon(guardBand, _polygon);
        if (clipped.Count >= 3)
            context.DrawPolygon(System.Runtime.InteropServices.CollectionsMarshal.AsSpan(clipped), style.Stroke,
                style.Fill);
    }
}
