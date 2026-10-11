using SharpMath.Geometry;

namespace SharpMath.Samples.Perspective.WinForms;

/// <summary>
///     Projects a rotating wireframe cube with <see cref="Matrix4x4" />: world transformations, <see cref="Matrix4x4.LookAt" />
///     and a right-handed perspective projection, composed as <c>projection · view · model</c> (column vectors).
/// </summary>
internal sealed class PerspectiveForm : Form
{
    private static readonly Vector3[] Vertices =
    [
        new(-0.5, -0.5, 0.5), new(0.5, -0.5, 0.5), new(0.5, 0.5, 0.5), new(-0.5, 0.5, 0.5),
        new(-0.5, -0.5, -0.5), new(0.5, -0.5, -0.5), new(0.5, 0.5, -0.5), new(-0.5, 0.5, -0.5)
    ];

    private static readonly (int From, int To)[] Edges =
    [
        (0, 1), (1, 2), (2, 3), (3, 0), (4, 5), (5, 6), (6, 7), (7, 4), (0, 4), (1, 5), (2, 6), (3, 7)
    ];

    private readonly CheckBox _autoRotate = new() { Text = "Auto rotation", AutoSize = true };
    private readonly DoubleBufferedPanel _panel = new() { Dock = DockStyle.Fill, BackColor = System.Drawing.Color.Black };
    private readonly TrackBar _rotationX = CreateTrackBar(30);
    private readonly TrackBar _rotationY = CreateTrackBar(40);
    private readonly TrackBar _rotationZ = CreateTrackBar(0);
    private readonly TrackBar _scaling = CreateTrackBar(360);
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 40 };

    public PerspectiveForm()
    {
        Text = "SharpMath Perspective";
        ClientSize = new Size(900, 700);

        var controls = new TableLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, ColumnCount = 5 };
        AddRow(controls, "Rotation X:", _rotationX);
        AddRow(controls, "Rotation Y:", _rotationY);
        AddRow(controls, "Rotation Z:", _rotationZ);
        AddRow(controls, "Scaling:", _scaling);
        controls.Controls.Add(_autoRotate, 2, 0);

        Controls.Add(_panel);
        Controls.Add(controls);

        foreach (var trackBar in new[] { _rotationX, _rotationY, _rotationZ, _scaling })
            trackBar.Scroll += (_, _) => _panel.Invalidate();
        _panel.Paint += (_, e) => DrawCube(e.Graphics);
        _autoRotate.CheckedChanged += (_, _) => _timer.Enabled = _autoRotate.Checked;
        _timer.Tick += (_, _) =>
        {
            foreach (var trackBar in new[] { _rotationX, _rotationY, _rotationZ })
                trackBar.Value = (trackBar.Value + 1) % (trackBar.Maximum + 1);
            _panel.Invalidate();
        };
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _timer.Dispose();
        base.Dispose(disposing);
    }

    private static TrackBar CreateTrackBar(int value) =>
        new() { Maximum = 360, Value = value, TickFrequency = 30, Width = 260 };

    private static void AddRow(TableLayoutPanel table, string text, TrackBar trackBar)
    {
        var row = table.RowCount++;
        table.Controls.Add(new Label { Text = text, AutoSize = true, Anchor = AnchorStyles.Left }, 0, row);
        table.Controls.Add(trackBar, 1, row);
    }

    private void DrawCube(Graphics graphics)
    {
        var width = _panel.ClientSize.Width;
        var height = _panel.ClientSize.Height;
        if (width == 0 || height == 0)
            return;

        // Transformations compose from right to left: scale, then rotate, then view and project.
        var model = Matrix4x4.Rotation(double.DegreesToRadians(_rotationX.Value),
            double.DegreesToRadians(_rotationY.Value), double.DegreesToRadians(_rotationZ.Value)) *
                    Matrix4x4.Scaling(Math.Max(0.05, _scaling.Value / 360.0));
        var view = Matrix4x4.LookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.Up);
        var projection = Matrix4x4.PerspectiveFieldOfView(Math.PI / 3, (double) width / height, 0.1, 100);
        var transformation = projection * view * model;

        // Matrix4x4 * Vector3 performs the perspective divide; map normalized device coordinates to pixels.
        var screen = Vertices.Select(vertex =>
        {
            var ndc = transformation * vertex;
            return new PointF((float) ((ndc.X + 1) / 2 * width), (float) ((1 - ndc.Y) / 2 * height));
        }).ToArray();

        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using var pen = new Pen(System.Drawing.Color.White, 3);
        foreach (var (from, to) in Edges)
            graphics.DrawLine(pen, screen[from], screen[to]);
    }

    private sealed class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw,
                true);
        }
    }
}
