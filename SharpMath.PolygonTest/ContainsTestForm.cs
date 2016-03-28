using SharpMath.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SharpMath.PolygonTest
{
    public partial class ContainsTestForm : Form
    {
        private bool _containsPoint;
        private readonly Polygon _polygon;
        private readonly PointF[] _points =
        {
            new PointF(0, 0), new PointF(200, 80), new PointF(400, 200),
            new PointF(100, 60)
        };

        public ContainsTestForm()
        {
            InitializeComponent();
            var polygonPoints = new List<Point2D>();
            foreach (var point in _points)
                polygonPoints.Add(FromPointF(point));
            _polygon = new Polygon(polygonPoints.ToArray());
        }

        private Point2D FromPointF(PointF point)
        {
            return new Point2D(point.X, point.Y);
        }

        private void ContainsTestForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Black, ClientRectangle);
            e.Graphics.FillPolygon(_containsPoint ? Brushes.AliceBlue : Brushes.Red, _points);
        }

        private void ContainsTestForm_MouseMove(object sender, MouseEventArgs e)
        {
            bool containsPointOld = _containsPoint;
            _containsPoint = _polygon.ContainsPoint(new Point2D(e.X, e.Y));
            if (_containsPoint != containsPointOld)
                Invalidate();
            Text = e.Location.ToString();
        }
    }
}