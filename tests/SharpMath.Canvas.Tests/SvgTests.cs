using System.Runtime.CompilerServices;
using SharpMath.Geometry;

namespace SharpMath.Canvas.Tests;

public class SvgTests
{
    /// <summary>
    ///     Compares with the golden file copied to the output directory. Set SHARPMATH_UPDATE_GOLDEN=1 to rewrite the
    ///     golden files in the source tree (the caller's file path is only meaningful in local, non-CI builds).
    /// </summary>
    private static void AssertGolden(string actual, string name, [CallerFilePath] string sourceFile = "")
    {
        if (Environment.GetEnvironmentVariable("SHARPMATH_UPDATE_GOLDEN") == "1")
        {
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(sourceFile)!, "Golden", name), actual);
            Assert.Fail($"The golden file {name} was rewritten; review and commit it, then run the tests again.");
        }

        var path = Path.Combine(AppContext.BaseDirectory, "Golden", name);
        Assert.True(File.Exists(path), $"The golden file {name} is missing; run the tests with SHARPMATH_UPDATE_GOLDEN=1.");
        Assert.Equal(File.ReadAllText(path).ReplaceLineEndings("\n"), actual.ReplaceLineEndings("\n"));
    }

    [Fact]
    public void EmptyCanvas_MatchesGoldenFile() => AssertGolden(new Canvas2D().RenderSvg(200, 150), "empty.svg");

    [Fact]
    public void AllItems_MatchGoldenFile()
    {
        var canvas = new Canvas2D { ShowTrackingLines = true, ShowValueIndicator = true };
        canvas.AddFunction("x^3 / 10");
        canvas.AddFunction(x => 1.5 * Math.Sin(x), new ItemStyle(new Stroke(Color.FromRgb(200, 0, 0), 2, DashStyle.Dashed)));
        canvas.AddVector(new Vector2(4, 3));
        canvas.AddPoint(new Point2D(-3, 2));
        canvas.AddLine(Line2D.FromSlopeIntercept(-0.5, -2));
        canvas.AddSegment(new LineSegment2D(new Point2D(-6, -4), new Point2D(-2, -6)),
            new ItemStyle(new Stroke(Color.Black, 1, DashStyle.Dotted)));
        canvas.AddPolygon(new Polygon(new Point2D(2, -2), new Point2D(6, -2), new Point2D(4, -5)));
        canvas.PointerMoved(new CanvasPoint(300, 60));
        AssertGolden(canvas.RenderSvg(400, 300), "items.svg");
    }

    [Fact]
    public void RenderSvg_WritesToATextWriter()
    {
        using var writer = new StringWriter();
        new Canvas2D().RenderSvg(writer, 10, 10);
        Assert.StartsWith("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"10\"", writer.ToString());
        Assert.Throws<ArgumentNullException>(() => ((Canvas2D) null!).RenderSvg(1, 1));
        Assert.Throws<ArgumentNullException>(() => new Canvas2D().RenderSvg(null!, 1, 1));
    }

    [Fact]
    public void Context_EscapesTextAndHandlesEdgeCases()
    {
        var context = new SvgDrawingContext(10, 10);
        context.DrawText("<a & b>", new CanvasPoint(1, 1), new FontSpec("A\"B", 10), Color.Black, TextAlignment.Right);
        context.DrawPolyline([new CanvasPoint(0, 0)], new Stroke(Color.Black));
        context.DrawPolygon([new CanvasPoint(0, 0), new CanvasPoint(1, 1)], null, Color.Black);
        context.DrawRectangle(new CanvasRect(0, 0, 1, 1), new Stroke(Color.FromArgb(128, 0, 0, 0)), null);
        context.DrawEllipse(new CanvasPoint(5, 5), 1, 2, null, Color.FromArgb(128, 255, 0, 0));
        context.PushClip(new CanvasRect(0, 0, 5, 5));
        var svg = context.ToString();

        Assert.Contains("&lt;a &amp; b&gt;", svg, StringComparison.Ordinal);
        Assert.Contains("font-family=\"A&quot;B\"", svg, StringComparison.Ordinal);
        Assert.Contains("text-anchor=\"end\"", svg, StringComparison.Ordinal);
        Assert.DoesNotContain("<polyline", svg, StringComparison.Ordinal);
        Assert.DoesNotContain("<polygon", svg, StringComparison.Ordinal);
        Assert.Contains("stroke-opacity=\"0.502\"", svg, StringComparison.Ordinal);
        Assert.Contains("fill-opacity=\"0.502\"", svg, StringComparison.Ordinal);
        Assert.EndsWith("</g>\n</svg>\n", svg, StringComparison.Ordinal);
        context.PopClip();
        Assert.Throws<InvalidOperationException>(() => context.PopClip());
        Assert.Equal(new TextSize(18, 12), context.MeasureText("abc", new FontSpec("x", 10)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new SvgDrawingContext(-1, 1));
        Assert.Throws<ArgumentNullException>(() => context.MeasureText(null!, new FontSpec("x", 1)));
        Assert.Throws<ArgumentNullException>(() =>
            context.DrawText(null!, default, new FontSpec("x", 1), Color.Black, TextAlignment.Left));
    }
}
