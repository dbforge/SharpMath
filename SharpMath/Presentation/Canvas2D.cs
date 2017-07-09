using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using SharpMath.Geometry;

namespace SharpMath.Presentation
{
    public sealed class Canvas2D : Control
    {
        private bool _isMouseDown;
        private PointF _lastOrigin;
        private Point _previousLocation;
        public BindingList<FunctionWrapper> Functions = new BindingList<FunctionWrapper>();

        public Canvas2D()
        {
            DoubleBuffered = true;

            GridSize = new SizeF(20, 20);
            GridOrigin = PointF.Empty;

            Font = new Font("Segoe UI", 8f, FontStyle.Regular);
            ForeColor = Color.Gray;

            Paint += DrawCanvas;
            Resize += CanvasResized;
            MouseWheel += HandleMouseWheel;

            MouseDown += HandleMouseDown;
            MouseMove += HandleMouseMove;
            MouseUp += HandleMouseUp;
        }

        public SizeF GridSize { get; set; }
        public PointF GridOrigin { get; set; }
        public SizeF GridScale { get; set; }
        public bool TrackingLines { get; set; }
        public bool ValueIndicator { get; set; }
        public BindingList<Vector2> Vertices { get; } = new BindingList<Vector2>();

        private void CanvasResized(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void DrawCanvas(object sender, PaintEventArgs e)
        {
            if (GridOrigin == PointF.Empty || DesignMode)
                GridOrigin = new PointF(Width/2, Height/2);

            _lastOrigin = GridOrigin;

            var g = e.Graphics;
            g.FillRectangle(Brushes.White, ClientRectangle);

            DrawCanvasLines(g);
            DrawVertices(g);
            DrawFunctions(g);
            DrawFeatures(g);
        }

        private double GetNearestScale(double value)
        {
            var scaleVariants = new List<double> {10, 5, 2};
            var dimension = (int) Math.Floor(Math.Log10(Math.Abs(value)) + 1);

            if (dimension > 1) value = value/((dimension - 1)*10);
            if (dimension == 0) value = value*((dimension - 1)*10);
            if (dimension < 0) value = value*(Math.Abs(dimension)*Math.Pow(10, Math.Abs(dimension) + 1));
            value = Math.Abs(value);

            var normalizedValue = scaleVariants.Aggregate((x, y) => Math.Abs(x - value) < Math.Abs(y - value) ? x : y);
            if (dimension == 1) return normalizedValue;
            if (dimension > 1) return (normalizedValue*((dimension - 1)*10));
            if (dimension < 0) return (normalizedValue/(Math.Pow(10, Math.Abs(dimension) + 1)));
            if (dimension < 1) return (normalizedValue/((dimension + 1)*10));
            return 0;
        }

        private void DrawCanvasLines(Graphics g)
        {
            #region BackgroundLines

            // Draw horizontal lines
            for (var i = 0f; i <= Height; i += GridSize.Height)
            {
                var startPoint = Height/2 + ((GridOrigin.Y - Height/2)%GridSize.Height);
                g.DrawLine(Pens.LightGray, new PointF(0, startPoint + i), new PointF(Width, startPoint + i));
                g.DrawLine(Pens.LightGray, new PointF(0, startPoint - i), new PointF(Width, startPoint - i));
            }

            // Draw vertical lines
            for (var i = 0f; i <= Width; i += GridSize.Width)
            {
                var startPoint = Width/2 + ((GridOrigin.X - Width/2)%GridSize.Width);
                g.DrawLine(Pens.LightGray, new PointF(startPoint + i, 0), new PointF(startPoint + i, Height));
                g.DrawLine(Pens.LightGray, new PointF(startPoint - i, 0), new PointF(startPoint - i, Height));
            }

            #endregion

            #region AxisScale

            // Draw vertical axis scale
            var scaleVariantY = GetNearestScale(Height/GridSize.Height/8.0);

            for (var i = 0.0; i <= Height; i += GridSize.Height*scaleVariantY)
            {
                var startPoint = Height/2 + ((GridOrigin.Y - Height/2)%(GridSize.Height*scaleVariantY));
                var value = Math.Round(((GridOrigin.Y - startPoint)/GridSize.Height), 1);
                var valueOffset = (i/GridSize.Height);

                g.DrawString(Math.Round(value - valueOffset, 2).ToString(), Font, Brushes.Black,
                    new PointF(GridOrigin.X + GridSize.Width/10, (float) (startPoint + i)));
                g.DrawString(Math.Round(value + valueOffset, 2).ToString(), Font, Brushes.Black,
                    new PointF(GridOrigin.X + GridSize.Width/10, (float) (startPoint - i)));
            }

            // Draw horizontal axis scale
            var scaleVariantX = GetNearestScale(Width/GridSize.Width/8.0);

            for (var i = 0.0; i <= Width; i += GridSize.Width*scaleVariantX)
            {
                var startPoint = Width/2 + ((GridOrigin.X - Width/2)%(GridSize.Width*scaleVariantX));
                var value = (GridOrigin.X > startPoint)
                    ? -Math.Round((GridOrigin.X - startPoint)/GridSize.Width, 1)
                    : Math.Round((Math.Abs(GridOrigin.X - startPoint)/GridSize.Width), 1);
                var valueOffset = (i/GridSize.Width);

                if (!(Math.Round(value + valueOffset, 2).IsApproximatelyEqualTo(0)))
                    g.DrawString(Math.Round(value + valueOffset, 2).ToString(CultureInfo.InvariantCulture), Font, Brushes.Black,
                        new PointF((float) (startPoint + i), GridOrigin.Y + GridSize.Height/10));

                if (!(Math.Round(value - valueOffset, 2).IsApproximatelyEqualTo(0)))
                    g.DrawString(Math.Round(value - valueOffset, 2).ToString(CultureInfo.InvariantCulture), Font, Brushes.Black,
                        new PointF((float) (startPoint - i), GridOrigin.Y + GridSize.Height/10));
            }

            #endregion

            #region AxisLines

            // TODO: Fix overflows
            var axisPen = new Pen(new SolidBrush(Color.FromArgb(255, 245, 125, 0)));
            g.DrawLine(axisPen, new PointF(0, GridOrigin.Y), new PointF(Width, GridOrigin.Y));
            g.DrawLine(axisPen, new PointF(GridOrigin.X, 0), new PointF(GridOrigin.X, Height));

            #endregion
        }

        private void DrawVertices(Graphics g)
        {
            Pen vectorPen = new Pen(new SolidBrush(Color.FromArgb(255, 122, 125, 201))) {EndCap = LineCap.ArrowAnchor};
            foreach (Vector2 v in Vertices)
            {
                g.DrawLine(vectorPen,
                    GridOrigin,
                    new PointF(GridOrigin.X + (float) v.X*GridSize.Width, GridOrigin.Y - (float) v.Y*GridSize.Height));
            }
        }

        private void DrawFunctions(Graphics g)
        {
            foreach (var func in Functions)
            {
                var lastPos = PointF.Empty;
                for (var i = 0; i < Width; i++)
                {
                    try
                    {
                        var currentX = (-GridOrigin.X + i)/GridSize.Width;
                        var currentY = GridOrigin.Y - func.GetValue(currentX)*GridSize.Height;
                        var currentPos = new PointF(currentX*GridSize.Width + GridOrigin.X, (float) currentY);

                        if (lastPos != PointF.Empty)
                            g.DrawLine(Pens.Black, lastPos, currentPos);

                        lastPos = currentPos;
                    }
                    catch (OverflowException)
                    {
                        // Content will follow
                    }
                }
            }
        }

        private void DrawFeatures(Graphics g)
        {
            #region TrackingLines

            if (TrackingLines)
            {
                g.DrawLine(Pens.Gray, new PointF(_previousLocation.X, 0), new PointF(_previousLocation.X, Height));
                g.DrawLine(Pens.Gray, new PointF(0, _previousLocation.Y), new PointF(Width, _previousLocation.Y));
            }

            #endregion

            #region ValueIndicator 

            if (!ValueIndicator)
                return;

            var currentX = (_previousLocation.X - GridOrigin.X)/GridSize.Width;
            var currentY = (GridOrigin.Y - _previousLocation.Y)/GridSize.Height;

            var resultString = $"{Math.Round(currentX, 2)} | {Math.Round(currentY, 2)}";
            var stringDimensions = g.MeasureString(resultString, Font);

            var toolTipRect = new Rectangle(_previousLocation.X + 15, _previousLocation.Y + 15, 80, 25);
            g.FillRectangle(Brushes.White, toolTipRect);
            g.DrawRectangle(Pens.LightGray, toolTipRect);

            g.DrawString(resultString, Font, Brushes.LightGray,
                new PointF(_previousLocation.X + 15 + (80 - stringDimensions.Width)/2,
                    _previousLocation.Y + 17 + (25 - stringDimensions.Height)/2));

            #endregion
        }

        private void HandleMouseWheel(object sender, MouseEventArgs e)
        {
            // Needs work
            if (e.Delta > 0)
            {
                GridSize = new SizeF((float) (GridSize.Width*1.25), (float) (GridSize.Height*1.25));
                GridOrigin = new PointF(
                    GridOrigin.X + ((e.X > _lastOrigin.X) ? -1.25f*(e.X - _lastOrigin.X) : 1.25f*(_lastOrigin.X - e.X)),
                    GridOrigin.Y + ((e.Y > _lastOrigin.Y) ? -1.25f*(e.Y - _lastOrigin.Y) : 1.25f*(_lastOrigin.Y - e.Y))
                    );
                _lastOrigin = GridOrigin;
            }
            else
            {
                var newSize = new SizeF((float) (GridSize.Width/1.25), (float) (GridSize.Height/1.25));
                if (newSize.Width > 3 && newSize.Height > 3)
                {
                    GridSize = newSize;
                    GridOrigin = new PointF(
                        GridOrigin.X +
                        ((_lastOrigin.X + e.X > _lastOrigin.X)
                            ? (e.X - _lastOrigin.X)/1.25f
                            : (_lastOrigin.X - e.X)/-1.25f),
                        GridOrigin.Y +
                        ((_lastOrigin.Y + e.Y > _lastOrigin.Y)
                            ? (e.Y - _lastOrigin.Y)/1.25f
                            : (_lastOrigin.Y - e.Y)/-1.25f)
                        );
                    _lastOrigin = GridOrigin;
                }
            }
            Invalidate();
        }

        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            _isMouseDown = true;
            _previousLocation = e.Location;
        }

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            if (TrackingLines || ValueIndicator)
                Invalidate();

            if (_isMouseDown)
            {
                var deltaX = (e.X - _previousLocation.X);
                var deltaY = (e.Y - _previousLocation.Y);

                var newX = GridOrigin.X + deltaX;
                var newY = GridOrigin.Y + deltaY;

                GridOrigin = new PointF(newX, newY);
                Invalidate();
            }

            _previousLocation = e.Location;
        }

        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            _isMouseDown = false;
        }
    }
}
