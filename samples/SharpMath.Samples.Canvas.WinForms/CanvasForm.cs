using SharpMath.Canvas;
using SharpMath.Canvas.WinForms;
using SharpMath.Expressions;
using SharpMath.Geometry;

namespace SharpMath.Samples.Canvas.WinForms;

/// <summary>
///     Shows the canvas with a few functions and a vector. Drag to pan, use the wheel to zoom, and enter expressions
///     such as <c>2x^2 - 3x</c> to plot them.
/// </summary>
internal sealed class CanvasForm : Form
{
    private readonly Canvas2DControl _canvas = new() { Dock = DockStyle.Fill };
    private readonly TextBox _expression = new() { Width = 220, Text = "x^2 / 4 - 3" };

    public CanvasForm()
    {
        Text = "SharpMath Canvas (WinForms, GDI+)";
        ClientSize = new Size(900, 560);

        var plot = new Button { Text = "Plot", AutoSize = true };
        plot.Click += (_, _) => PlotExpression();
        var clear = new Button { Text = "Clear", AutoSize = true };
        clear.Click += (_, _) => _canvas.Canvas.Items.Clear();
        var tracking = new CheckBox { Text = "Tracking lines", AutoSize = true };
        tracking.CheckedChanged += (_, _) => _canvas.ShowTrackingLines = tracking.Checked;
        var indicator = new CheckBox { Text = "Value indicator", AutoSize = true };
        indicator.CheckedChanged += (_, _) => _canvas.ShowValueIndicator = indicator.Checked;

        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(4) };
        toolbar.Controls.AddRange([new Label { Text = "f(x) =", AutoSize = true, Margin = new Padding(3, 7, 0, 0) },
            _expression, plot, clear, tracking, indicator]);

        Controls.Add(_canvas);
        Controls.Add(toolbar);
        AcceptButton = plot;

        _canvas.Canvas.AddFunction(x => Math.Pow(x, 3));
        _canvas.Canvas.AddFunction(x => 1.5 * Math.Sin(x), new ItemStyle(new Stroke(SharpMath.Canvas.Color.FromRgb(200, 0, 0), 2)));
        _canvas.Canvas.AddVector(new Vector2(4, 8));
    }

    private void PlotExpression()
    {
        try
        {
            _canvas.Canvas.AddFunction(_expression.Text, new ItemStyle(new Stroke(SharpMath.Canvas.Color.FromRgb(0, 90, 200), 2)));
        }
        catch (Exception exception) when (exception is ParserException or ArgumentException)
        {
            MessageBox.Show(this, exception.Message, "Invalid expression", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
