using SharpMath.Geometry;

namespace SharpMath.Canvas.Tests;

public class Canvas2DTests
{
    private static RecordingDrawingContext Render(Canvas2D canvas, double width = 400, double height = 300)
    {
        var context = new RecordingDrawingContext();
        canvas.Render(context, width, height);
        return context;
    }

    [Fact]
    public void Render_DrawsBackgroundGridAxesAndLabels()
    {
        var canvas = new Canvas2D();
        var context = Render(canvas);

        Assert.Equal([Color.White], context.Clears);
        Assert.Equal(0, context.ClipDepth);
        Assert.Equal(1, context.MaxClipDepth);

        // 1-unit grid at 20 px per unit: 21 vertical and 15 horizontal lines (−10…10, −7…7), plus two axes.
        var axisColor = CanvasStyle.Default.AxisStroke;
        Assert.Equal(2, context.Lines.Count(line => line.Stroke == axisColor));
        Assert.Equal(21 + 15, context.Lines.Count(line => line.Stroke == CanvasStyle.Default.MinorGridStroke));

        // Labels every 5 units; "0" only once; labels that would be cut off at the edges are skipped.
        var labels = context.Texts.Select(text => text.Text).ToList();
        Assert.Equal(["-5", "5", "-5", "0", "5"], labels);
        Assert.All(context.Texts, text => Assert.Equal(Color.Black, text.Color));
    }

    [Fact]
    public void Labels_SitOnGridLines_AtEveryZoomLevel()
    {
        // Regression: at 16 px per unit the grid step was 2 but labels were placed every 5 units.
        var canvas = new Canvas2D();
        canvas.Viewport.Resize(400, 300);
        canvas.Viewport.SetScale(16, 16);
        var context = Render(canvas);
        var gridX = context.Lines.Where(line => line.Start.X == line.End.X).Select(line => Math.Floor(line.Start.X)).ToHashSet();
        var xLabels = context.Texts.Where(text => text.Alignment == TextAlignment.Center).ToList();
        Assert.NotEmpty(xLabels);
        Assert.All(xLabels, label => Assert.Contains(Math.Floor(label.Position.X), gridX));
        Assert.Equal(["-10", "10"], xLabels.Select(label => label.Text));
    }

    [Fact]
    public void Labels_ThatWouldBeCutOff_AreSkipped()
    {
        var canvas = new Canvas2D();
        canvas.Viewport.Resize(400, 300);
        canvas.Viewport.SetOrigin(new CanvasPoint(200, 297));
        var context = Render(canvas);
        Assert.All(context.Texts, text =>
        {
            Assert.InRange(text.Position.Y, 0, 300);
            Assert.InRange(text.Position.Y + 11 * 1.2, 0, 300);
        });
    }

    [Fact]
    public void Labels_StickToTheEdgeWhenTheAxisIsOffScreen()
    {
        var canvas = new Canvas2D();
        canvas.Viewport.Resize(400, 300);
        canvas.Viewport.Pan(0, 1000);
        var context = Render(canvas);
        var xLabels = context.Texts.Where(text => text.Alignment == TextAlignment.Center).ToList();
        Assert.NotEmpty(xLabels);
        Assert.All(xLabels, text => Assert.InRange(text.Position.Y, 0, 300));
        Assert.DoesNotContain(context.Lines, line => line.Stroke == CanvasStyle.Default.AxisStroke &&
                                                      line.Start.Y == line.End.Y);
    }

    [Fact]
    public void Render_DrawsEveryItemType()
    {
        var canvas = new Canvas2D();
        canvas.AddFunction(x => x);
        canvas.AddFunction("x^2");
        canvas.AddVector(new Vector2(4, 8));
        canvas.AddPoint(new Point2D(1, 1));
        canvas.AddLine(Line2D.FromGeneralForm(1, 0, -2));
        canvas.AddSegment(new LineSegment2D(new Point2D(0, 0), new Point2D(3, -3)));
        canvas.AddPolygon(new Polygon(new Point2D(0, 0), new Point2D(2, 0), new Point2D(1, 2)));
        var context = Render(canvas);
        var style = CanvasStyle.Default;

        Assert.Equal(2, context.Polylines.Count(polyline => polyline.Stroke == style.FunctionStyle.Stroke));
        Assert.Contains(context.Lines, line => line.Stroke == style.VectorStyle.Stroke);
        var arrowhead = Assert.Single(context.Polygons, polygon => polygon.Fill == style.VectorStyle.Fill);
        Assert.Equal(new CanvasPoint(280, -10), arrowhead.Points[0]);
        var point = Assert.Single(context.Ellipses);
        Assert.Equal(new CanvasPoint(220, 130), point.Center);
        var line = Assert.Single(context.Lines, line => line.Stroke == style.LineStyle.Stroke);
        Assert.Equal(240, line.Start.X, 9);
        Assert.Equal(240, line.End.X, 9);
        Assert.Contains(context.Lines, line => line.Stroke == style.SegmentStyle.Stroke &&
                                               line.Start == new CanvasPoint(200, 150) &&
                                               line.End == new CanvasPoint(260, 210));
        Assert.Single(context.Polygons, polygon => polygon.Fill == style.PolygonStyle.Fill);
    }

    [Fact]
    public void ItemStyles_OverrideTheDefaults()
    {
        var canvas = new Canvas2D();
        var red = new ItemStyle(new Stroke(Color.FromRgb(255, 0, 0), 3, DashStyle.Dashed));
        canvas.AddFunction(x => x, red);
        canvas.AddPoint(new Point2D(0, 0), red with { Fill = Color.Black, PointRadius = 5 });
        var context = Render(canvas);
        Assert.Equal(red.Stroke, Assert.Single(context.Polylines).Stroke);
        Assert.Equal(5, Assert.Single(context.Ellipses).RadiusX);
    }

    [Fact]
    public void FarAwayItems_AreClippedOrSkipped()
    {
        // Regression for issue #3: huge coordinates must never reach the renderer.
        var canvas = new Canvas2D();
        canvas.AddVector(new Vector2(1e12, 1e12));
        canvas.AddSegment(new LineSegment2D(new Point2D(-1e15, 0), new Point2D(1e15, 1)));
        canvas.AddPolygon(new Polygon(new Point2D(-1e9, -1e9), new Point2D(1e9, -1e9), new Point2D(0, 1e9)));
        canvas.AddPoint(new Point2D(1e9, 1e9));
        canvas.AddFunction(x => Math.Exp(x * 100));
        canvas.AddLine(Line2D.FromGeneralForm(1, 0, -1e9));
        canvas.Viewport.Resize(400, 300);
        for (var i = 0; i < 50; i++)
            canvas.Wheel(new CanvasPoint(390, 5), 1);

        var context = Render(canvas);
        Assert.All(context.AllPoints, point =>
        {
            Assert.InRange(point.X, -5000, 9000);
            Assert.InRange(point.Y, -4000, 7000);
        });
        Assert.Empty(context.Ellipses);
        Assert.DoesNotContain(context.Lines, line => line.Stroke == CanvasStyle.Default.LineStyle.Stroke);
    }

    [Fact]
    public void DegenerateItems_AreIgnored()
    {
        var canvas = new Canvas2D();
        canvas.AddVector(Vector2.Zero);
        canvas.Items.Add(new LineItem(default));
        var context = Render(canvas);
        Assert.Empty(context.Polygons);
        Assert.DoesNotContain(context.Lines, line => line.Stroke == CanvasStyle.Default.VectorStyle.Stroke);
    }

    [Fact]
    public void InvalidSpacing_DoesNotHang()
    {
        // Regression: a grid size of 0 used to make the drawing loops spin forever.
        var canvas = new Canvas2D { Style = CanvasStyle.Default with { MinGridSpacing = 0, MinLabelSpacing = double.NaN } };
        var context = Render(canvas, 100, 100);
        Assert.InRange(context.Lines.Count, 2, 300);
    }

    [Fact]
    public void Input_PansZoomsAndTracksThePointer()
    {
        var canvas = new Canvas2D();
        canvas.Viewport.Resize(400, 300);
        canvas.PointerPressed(new CanvasPoint(100, 100));
        canvas.PointerMoved(new CanvasPoint(110, 90));
        canvas.PointerReleased(new CanvasPoint(120, 80));
        Assert.Equal(new CanvasPoint(220, 130), canvas.Viewport.Origin);
        canvas.PointerMoved(new CanvasPoint(0, 0));
        Assert.Equal(new CanvasPoint(220, 130), canvas.Viewport.Origin);
        Assert.Equal(new CanvasPoint(0, 0), canvas.PointerPosition);

        canvas.Wheel(new CanvasPoint(220, 130), 2);
        Assert.Equal(20 * 1.25 * 1.25, canvas.Viewport.ScaleX, 9);
        Assert.Equal(new CanvasPoint(220, 130), canvas.Viewport.Origin);
        canvas.Wheel(new CanvasPoint(0, 0), 0);
        canvas.Wheel(new CanvasPoint(0, 0), double.NaN);
        Assert.Equal(20 * 1.25 * 1.25, canvas.Viewport.ScaleX, 9);

        canvas.PointerExited();
        Assert.Null(canvas.PointerPosition);
    }

    [Fact]
    public void TrackingLinesAndValueIndicator()
    {
        var canvas = new Canvas2D { ShowTrackingLines = true, ShowValueIndicator = true };
        canvas.Viewport.Resize(400, 300);
        canvas.PointerMoved(new CanvasPoint(250, 100));
        var context = Render(canvas);

        Assert.Equal(2, context.Lines.Count(line => line.Stroke == CanvasStyle.Default.TrackingStroke));
        var box = Assert.Single(context.Rectangles);
        Assert.Equal(265, box.Rect.X);
        Assert.Contains(context.Texts, text => text.Text == "2.5 | 2.5" && text.Color == CanvasStyle.Default.IndicatorText);

        canvas.PointerExited();
        var hidden = Render(canvas);
        Assert.Empty(hidden.Rectangles);
    }

    [Fact]
    public void Changed_IsRaisedForRelevantChanges()
    {
        var canvas = new Canvas2D();
        var count = 0;
        canvas.Changed += (_, _) => count++;

        Render(canvas);
        Assert.Equal(0, count);

        canvas.AddPoint(Point2D.Origin);
        Assert.Equal(1, count);
        canvas.Items.Clear();
        Assert.Equal(2, count);
        canvas.Style = canvas.Style with { Background = Color.Black };
        Assert.Equal(3, count);
        canvas.Style = canvas.Style;
        Assert.Equal(3, count);
        canvas.Viewport.Pan(1, 1);
        Assert.Equal(4, count);

        canvas.PointerMoved(new CanvasPoint(1, 1));
        Assert.Equal(4, count);
        canvas.ShowTrackingLines = true;
        canvas.ShowTrackingLines = true;
        Assert.Equal(5, count);
        canvas.PointerMoved(new CanvasPoint(2, 2));
        Assert.Equal(6, count);
        canvas.PointerMoved(new CanvasPoint(2, 2));
        Assert.Equal(6, count);
        canvas.ShowValueIndicator = true;
        Assert.Equal(7, count);
        canvas.PointerExited();
        Assert.Equal(8, count);
        canvas.PointerExited();
        Assert.Equal(8, count);
        Assert.Throws<ArgumentNullException>(() => canvas.Style = null!);
        Assert.Throws<ArgumentNullException>(() => canvas.Render(null!, 1, 1));
        Assert.Throws<ArgumentNullException>(() => canvas.AddPolygon(null!));
    }

    [Fact]
    public void FunctionItems_FromExpressions()
    {
        var item = FunctionItem.FromExpression("2x + 1");
        Assert.Equal(7, item.Function(3));
        Assert.Equal("2x + 1", item.Expression!.Text);
        Assert.Throws<ArgumentException>(() => FunctionItem.FromExpression("x + y"));
        Assert.Throws<SharpMath.Expressions.ParserException>(() => new Canvas2D().AddFunction("x +"));
        var context = new SharpMath.Expressions.ExpressionContext().AddFunction("sq", x => x * x);
        var custom = new Canvas2D().AddFunction("sq(x) + 1", context: context);
        Assert.Equal(10, custom.Function(3));
    }

    [Fact]
    public void Colors()
    {
        Assert.Equal("#F57D00", Color.FromRgb(245, 125, 0).ToHex());
        Assert.Equal("#11223344", Color.FromArgb(0x44, 0x11, 0x22, 0x33).ToHex());
        Assert.Equal(0, Color.Transparent.A);
        Assert.True(new CanvasRect(0, 0, 10, 10).Contains(new CanvasPoint(10, 10)));
        Assert.Equal(10, new CanvasRect(5, 5, 5, 5).Right);
        Assert.Equal(10, new CanvasRect(5, 5, 5, 5).Bottom);
    }
}
