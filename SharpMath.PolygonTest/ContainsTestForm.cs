// ContainsTestForm.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SharpMath.Geometry;

namespace SharpMath.PolygonTest
{
    public partial class ContainsTestForm : Form
    {
        private readonly PointF[] _points =
        {
            new PointF(0, 0), new PointF(200, 80), new PointF(400, 200),
            new PointF(100, 60)
        };

        private readonly Polygon _polygon;
        private bool _containsPoint;

        public ContainsTestForm()
        {
            InitializeComponent();
            var polygonPoints = new List<Point2D>();
            foreach (var point in _points)
                polygonPoints.Add(FromPointF(point));
            _polygon = new Polygon(polygonPoints.ToArray());
        }

        private void ContainsTestForm_MouseMove(object sender, MouseEventArgs e)
        {
            var containsPointOld = _containsPoint;
            _containsPoint = _polygon.ContainsPoint(new Point2D(e.X, e.Y));
            if (_containsPoint != containsPointOld)
                Invalidate();
            Text = e.Location.ToString();
        }

        private void ContainsTestForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Black, ClientRectangle);
            e.Graphics.FillPolygon(_containsPoint ? Brushes.AliceBlue : Brushes.Red, _points);
        }

        private Point2D FromPointF(PointF point)
        {
            return new Point2D(point.X, point.Y);
        }
    }
}