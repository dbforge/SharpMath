using SharpMath.Geometry;

namespace SharpMath.Samples.Polygon.WinForms;

/// <summary>
///     Colors a polygon depending on whether the mouse pointer is inside it (<see cref="Geometry.Polygon.Contains(Point2D)" />).
/// </summary>
internal sealed class ContainsForm : Form
{
    private static readonly PointF[] Points = [new(20, 20), new(220, 100), new(420, 220), new(120, 80), new(60, 300)];

    private readonly Geometry.Polygon _polygon = new(Points.Select(point => new Point2D(point.X, point.Y)));
    private bool _containsPointer;

    public ContainsForm()
    {
        Text = "SharpMath Polygon.Contains";
        ClientSize = new Size(560, 420);
        DoubleBuffered = true;
        BackColor = System.Drawing.Color.Black;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        base.OnMouseMove(e);
        var contains = _polygon.Contains(new Point2D(e.X, e.Y));
        Text = $"SharpMath Polygon.Contains: {e.Location} is {(contains ? "inside" : "outside")}";
        if (contains == _containsPointer)
            return;
        _containsPointer = contains;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        base.OnPaint(e);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.FillPolygon(_containsPointer ? Brushes.AliceBlue : Brushes.Red, Points);
    }
}
