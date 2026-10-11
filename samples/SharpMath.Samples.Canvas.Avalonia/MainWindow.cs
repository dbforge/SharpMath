using Avalonia.Controls;
using Avalonia.Layout;
using SharpMath.Canvas;
using SharpMath.Canvas.Avalonia;
using SharpMath.Expressions;
using SharpMath.Geometry;

namespace SharpMath.Samples.Canvas.Avalonia;

/// <summary>
///     Shows the canvas on Windows, Linux and macOS. Drag to pan, use the wheel to zoom, and enter expressions such as
///     <c>2x^2 - 3x</c> to plot them.
/// </summary>
internal sealed class MainWindow : Window
{
    private readonly TextBox _expression = new() { Text = "x^2 / 4 - 3", Width = 220 };
    private readonly TextBlock _status = new() { VerticalAlignment = VerticalAlignment.Center };
    private readonly Canvas2DView _view = new();

    public MainWindow()
    {
        Title = "SharpMath Canvas (Avalonia, Skia)";
        Width = 900;
        Height = 560;

        var plot = new Button { Content = "Plot", IsDefault = true };
        plot.Click += (_, _) => PlotExpression();
        var clear = new Button { Content = "Clear" };
        clear.Click += (_, _) => _view.Canvas.Items.Clear();
        var tracking = new CheckBox { Content = "Tracking lines" };
        tracking.IsCheckedChanged += (_, _) => _view.ShowTrackingLines = tracking.IsChecked == true;
        var indicator = new CheckBox { Content = "Value indicator" };
        indicator.IsCheckedChanged += (_, _) => _view.ShowValueIndicator = indicator.IsChecked == true;

        var toolbar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Margin = new global::Avalonia.Thickness(8),
            Children =
            {
                new TextBlock { Text = "f(x) =", VerticalAlignment = VerticalAlignment.Center },
                _expression, plot, clear, tracking, indicator, _status
            }
        };

        var root = new DockPanel();
        DockPanel.SetDock(toolbar, Dock.Top);
        root.Children.Add(toolbar);
        root.Children.Add(_view);
        Content = root;

        _view.Canvas.AddFunction(x => Math.Pow(x, 3));
        _view.Canvas.AddFunction(x => 1.5 * Math.Sin(x), new ItemStyle(new Stroke(Color.FromRgb(200, 0, 0), 2)));
        _view.Canvas.AddVector(new Vector2(4, 8));
        _view.Canvas.AddPolygon(new Polygon(new Point2D(2, -2), new Point2D(6, -2), new Point2D(4, -5)));
    }

    private void PlotExpression()
    {
        try
        {
            _view.Canvas.AddFunction(_expression.Text ?? string.Empty,
                new ItemStyle(new Stroke(Color.FromRgb(0, 90, 200), 2)));
            _status.Text = string.Empty;
        }
        catch (Exception exception) when (exception is ParserException or ArgumentException)
        {
            _status.Text = exception.Message;
        }
    }
}
