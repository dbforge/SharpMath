using SharpMath.Geometry;

namespace SharpMath.Canvas;

/// <summary>
///     Draws the grid, the axis labels and the axes of a <see cref="Canvas2D" />.
/// </summary>
internal static class GridRenderer
{
    private const double LabelPadding = 3;

    /// <summary>
    ///     Draws grid lines, labels and axes and returns the grid step in world units (X, Y).
    /// </summary>
    public static (double StepX, double StepY) Draw(IDrawingContext context, Viewport viewport, CanvasStyle style)
    {
        var area = viewport.VisibleArea;
        var gridSpacing = Ticks.SafeSpacing(style.MinGridSpacing);
        var labelSpacing = Math.Max(gridSpacing, Ticks.SafeSpacing(style.MinLabelSpacing));
        var minorX = Ticks.NiceStep(gridSpacing, viewport.ScaleX);
        var minorY = Ticks.NiceStep(gridSpacing, viewport.ScaleY);

        // Labelled ticks are multiples of the grid step, so every label sits on a (major) grid line.
        var majorX = Ticks.NiceMultiple(labelSpacing, viewport.ScaleX, minorX);
        var majorY = Ticks.NiceMultiple(labelSpacing, viewport.ScaleY, minorY);

        var (firstX, lastX) = Ticks.Range(area.MinX, area.MaxX, minorX);
        for (var i = firstX; i <= lastX; i++)
        {
            var stroke = IsMultiple(i * minorX, majorX) ? style.MajorGridStroke : style.MinorGridStroke;
            var x = viewport.WorldToScreen(new Point2D(i * minorX, 0)).X;
            context.DrawLine(new CanvasPoint(x, 0), new CanvasPoint(x, viewport.Height), stroke);
        }

        var (firstY, lastY) = Ticks.Range(area.MinY, area.MaxY, minorY);
        for (var i = firstY; i <= lastY; i++)
        {
            var stroke = IsMultiple(i * minorY, majorY) ? style.MajorGridStroke : style.MinorGridStroke;
            var y = viewport.WorldToScreen(new Point2D(0, i * minorY)).Y;
            context.DrawLine(new CanvasPoint(0, y), new CanvasPoint(viewport.Width, y), stroke);
        }

        DrawLabels(context, viewport, style, area, majorX, majorY);
        DrawAxes(context, viewport, style.AxisStroke);
        return (minorX, minorY);
    }

    private static void DrawLabels(IDrawingContext context, Viewport viewport, CanvasStyle style, BoundingBox2D area,
        double stepX, double stepY)
    {
        var font = style.LabelFont;
        var lineHeight = context.MeasureText("0", font).Height;

        // Labels stay next to the axes; if an axis is off-screen they stick to the nearest edge. Labels that would be
        // cut off at the edges are skipped.
        var labelTop = Math.Clamp(viewport.Origin.Y + LabelPadding, LabelPadding,
            Math.Max(LabelPadding, viewport.Height - lineHeight - LabelPadding));
        var (firstX, lastX) = Ticks.Range(area.MinX, area.MaxX, stepX);
        for (var i = firstX; i <= lastX; i++)
        {
            if (i == 0)
                continue;
            var x = viewport.WorldToScreen(new Point2D(i * stepX, 0)).X;
            var text = Ticks.Format(i, stepX);
            var halfWidth = context.MeasureText(text, font).Width / 2;
            if (x - halfWidth < 0 || x + halfWidth > viewport.Width)
                continue;
            context.DrawText(text, new CanvasPoint(x, labelTop), font, style.LabelColor, TextAlignment.Center);
        }

        var (firstY, lastY) = Ticks.Range(area.MinY, area.MaxY, stepY);
        for (var i = firstY; i <= lastY; i++)
        {
            var top = i == 0 ? labelTop : viewport.WorldToScreen(new Point2D(0, i * stepY)).Y - lineHeight / 2;
            if (top < 0 || top + lineHeight > viewport.Height)
                continue;
            var text = Ticks.Format(i, stepY);
            var width = context.MeasureText(text, font).Width;
            var labelLeft = Math.Clamp(viewport.Origin.X + LabelPadding, LabelPadding,
                Math.Max(LabelPadding, viewport.Width - width - LabelPadding));
            context.DrawText(text, new CanvasPoint(labelLeft, top), font, style.LabelColor, TextAlignment.Left);
        }
    }

    private static void DrawAxes(IDrawingContext context, Viewport viewport, Stroke stroke)
    {
        var origin = viewport.Origin;
        if (origin.Y >= 0 && origin.Y <= viewport.Height)
            context.DrawLine(new CanvasPoint(0, origin.Y), new CanvasPoint(viewport.Width, origin.Y), stroke);
        if (origin.X >= 0 && origin.X <= viewport.Width)
            context.DrawLine(new CanvasPoint(origin.X, 0), new CanvasPoint(origin.X, viewport.Height), stroke);
    }

    private static bool IsMultiple(double value, double step)
    {
        var ratio = value / step;
        return Math.Abs(ratio - Math.Round(ratio)) < 1e-6;
    }
}
