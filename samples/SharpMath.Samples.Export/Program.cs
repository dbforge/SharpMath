// Renders two example plots to PNG (SkiaSharp) and SVG (no dependencies) without any UI.
// Usage: dotnet run --project samples/SharpMath.Samples.Export -- [output directory]
// The images in doc/images are generated with: dotnet run --project samples/SharpMath.Samples.Export -- doc/images

using SharpMath.Canvas;
using SharpMath.Canvas.Skia;
using SharpMath.Geometry;

var output = args.Length > 0 ? args[0] : "output";
Directory.CreateDirectory(output);

var functions = new Canvas2D();
functions.Viewport.SetScale(40, 40);
functions.AddFunction("x^3 / 10 - x");
functions.AddFunction(x => 1.5 * Math.Sin(2 * x), new ItemStyle(new Stroke(Color.FromRgb(200, 0, 0), 2)));
functions.AddFunction("tan(x)", new ItemStyle(new Stroke(Color.FromRgb(0, 90, 200), 2, DashStyle.Dashed)));
Export(functions, "canvas-functions");

var geometry = new Canvas2D { ShowTrackingLines = true, ShowValueIndicator = true };
geometry.Viewport.SetScale(30, 30);
geometry.AddPolygon(new Polygon(new Point2D(1, 1), new Point2D(5, 1), new Point2D(6, 4), new Point2D(2, 5)));
geometry.AddLine(Line2D.FromSlopeIntercept(-0.5, -1));
geometry.AddSegment(new LineSegment2D(new Point2D(-7, -2), new Point2D(-2, -4)));
geometry.AddVector(new Vector2(-4, 3));
geometry.AddVector(new Vector2(3, -2), new Point2D(-4, 3));
geometry.AddPoint(new Point2D(-1, 3));
geometry.PointerMoved(new CanvasPoint(560, 140));
Export(geometry, "canvas-geometry");

void Export(Canvas2D canvas, string name)
{
    const int width = 720;
    const int height = 400;
    using (var png = File.Create(Path.Combine(output, name + ".png")))
        canvas.RenderPng(png, width, height);
    File.WriteAllText(Path.Combine(output, name + ".svg"), canvas.RenderSvg(width, height));
    Console.WriteLine($"Wrote {Path.Combine(output, name)}.png and .svg");
}
