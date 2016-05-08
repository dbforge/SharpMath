using SharpMath.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpMath.Presentation {
    public class Canvas2D : Control {
        public Canvas2D () {
            this.DoubleBuffered = true;

            this.GridSize = new SizeF (20, 20);
            this.GridOrigin = PointF.Empty;

            this.Font = new Font ("Segoe UI", 8f, FontStyle.Regular);
            this.ForeColor = Color.Gray;

            this.Paint += DrawCanvas;
            this.Resize += CanvasResized;
            this.MouseWheel += HandleMouseWheel;

            this.MouseDown += HandleMouseDown;
            this.MouseMove += HandleMouseMove;
            this.MouseUp += HandleMouseUp;
        }

        private bool _isMouseDown = false;
        private Point _previousLocation;

        private void CanvasResized (object sender, EventArgs e) {
            this.Invalidate ();
        }
        private void DrawCanvas (object sender, PaintEventArgs e) {
            if (this.GridOrigin == PointF.Empty || this.DesignMode)
                this.GridOrigin = new PointF (this.Width / 2, this.Height / 2);

            this._lastOrigin = this.GridOrigin;

            var g = e.Graphics;
            g.FillRectangle (Brushes.White, this.ClientRectangle);

            DrawCanvasLines (g);
            DrawVertices (g);
            DrawFunctions (g);
            DrawFeatures (g);
        }
        private double GetNearestScale (double value) {
            var scaleVariants = new List<double> () { 10, 5, 2 };
            var dimension = (int)Math.Floor (Math.Log10 (Math.Abs (value)) + 1);

            double normalizedValue;
            if (dimension > 1) value = value / ((dimension - 1) * 10);
            if (dimension == 0) value = value * ((dimension - 1) * 10);
            if (dimension < 0) value = value * (Math.Abs (dimension) * Math.Pow (10, Math.Abs (dimension) + 1));
            value = Math.Abs (value);

            normalizedValue = scaleVariants.Aggregate ((x, y) => Math.Abs (x - value) < Math.Abs (y - value) ? x : y);
            if (dimension == 1) return normalizedValue;
            if (dimension > 1) return (normalizedValue * ((dimension - 1) * 10));
            if (dimension < 0) return (normalizedValue / (Math.Pow (10, Math.Abs (dimension) + 1)));
            if (dimension < 1) return (normalizedValue / ((dimension + 1) * 10));
            return 0;
        }

        private void DrawCanvasLines (Graphics g) {
            #region BackgroundLines
            // Draw horizontal lines
            for (var i = 0f; i <= this.Height; i += this.GridSize.Height) {
                var startPoint = this.Height / 2 + ((this.GridOrigin.Y - this.Height / 2) % this.GridSize.Height);
                g.DrawLine (Pens.LightGray, new PointF (0, startPoint + i), new PointF (this.Width, startPoint + i));
                g.DrawLine (Pens.LightGray, new PointF (0, startPoint - i), new PointF (this.Width, startPoint - i));
            }

            // Draw vertical lines
            for (var i = 0f; i <= this.Width; i += this.GridSize.Width) {
                var startPoint = this.Width / 2 + ((this.GridOrigin.X - this.Width / 2) % this.GridSize.Width);
                g.DrawLine (Pens.LightGray, new PointF (startPoint + i, 0), new PointF (startPoint + i, this.Height));
                g.DrawLine (Pens.LightGray, new PointF (startPoint - i, 0), new PointF (startPoint - i, this.Height));
            }
            #endregion
            #region AxisScale
            // Draw vertical axis scale
            var scaleVariantY = GetNearestScale (this.Height / this.GridSize.Height / 8.0);

            for (var i = 0.0; i <= this.Height; i += this.GridSize.Height * scaleVariantY) {
                var startPoint = this.Height / 2 + ((this.GridOrigin.Y - this.Height / 2) % (this.GridSize.Height * scaleVariantY));
                var value = Math.Round (((this.GridOrigin.Y - startPoint) / this.GridSize.Height), 1);
                var valueOffset = (i / this.GridSize.Height);

                g.DrawString (Math.Round (value - valueOffset, 2).ToString (), this.Font, Brushes.Black, new PointF (this.GridOrigin.X + this.GridSize.Width / 10, (float)(startPoint + i)));
                g.DrawString (Math.Round (value + valueOffset, 2).ToString (), this.Font, Brushes.Black, new PointF (this.GridOrigin.X + this.GridSize.Width / 10, (float)(startPoint - i)));
            }

            // Draw horizontal axis scale
            var scaleVariantX = GetNearestScale (this.Width / this.GridSize.Width / 8.0);

            for (var i = 0.0; i <= this.Width; i += this.GridSize.Width * scaleVariantX) {
                var startPoint = this.Width / 2 + ((this.GridOrigin.X - this.Width / 2) % (this.GridSize.Width * scaleVariantX));
                var value = (this.GridOrigin.X > startPoint) 
                    ? -Math.Round((this.GridOrigin.X - startPoint) / this.GridSize.Width, 1)
                    : Math.Round ((Math.Abs (this.GridOrigin.X - startPoint) / this.GridSize.Width), 1);
                var valueOffset = (i / this.GridSize.Width);

                if ((Math.Round (value + valueOffset, 2) != 0))
                    g.DrawString (Math.Round (value + valueOffset, 2).ToString (), this.Font, Brushes.Black, new PointF ((float)(startPoint + i), this.GridOrigin.Y + this.GridSize.Height / 10));

                if ((Math.Round (value - valueOffset, 2) != 0))
                    g.DrawString (Math.Round (value - valueOffset, 2).ToString (), this.Font, Brushes.Black, new PointF ((float)(startPoint - i), this.GridOrigin.Y + this.GridSize.Height / 10));
            }
            #endregion
            #region AxisLines
            var axisPen = new Pen (new SolidBrush (Color.FromArgb (255, 245, 125, 0)));
            g.DrawLine (axisPen, new PointF (0, this.GridOrigin.Y), new PointF (this.Width, this.GridOrigin.Y));
            g.DrawLine (axisPen, new PointF (this.GridOrigin.X, 0), new PointF (this.GridOrigin.X, this.Height));
            #endregion
        }
        private void DrawVertices (Graphics g) {
            Pen vectorPen = new Pen (new SolidBrush (Color.FromArgb (255, 122, 125, 201)));
            vectorPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

            foreach (Vector2 v in Vertices) {
                g.DrawLine (vectorPen,
                    this.GridOrigin,
                    new PointF (this.GridOrigin.X + (float)v.X * GridSize.Width, this.GridOrigin.Y - (float)v.Y * GridSize.Height));
            }
        }
        private void DrawFunctions (Graphics g) {
            foreach (FunctionWrapper func in this.Functions) {
                var lastPos = PointF.Empty;
                for (var i = 0; i < this.Width; i++) {
                    try {
                        var currentX = (-this.GridOrigin.X + i) / this.GridSize.Width;
                        var currentY = this.GridOrigin.Y - func.GetValue (currentX) * this.GridSize.Height;
                        var currentPos = new PointF (currentX * this.GridSize.Width + this.GridOrigin.X, (float)currentY);

                        if (lastPos != PointF.Empty)
                            g.DrawLine (Pens.Black, lastPos, currentPos);

                        lastPos = currentPos;
                    } catch (OverflowException ex) {

                    }
                }
            }
        }
        private void DrawFeatures (Graphics g) {
            #region TrackingLines
            if (this.TrackingLines) {
                g.DrawLine (Pens.Gray, new PointF (_previousLocation.X, 0), new PointF (_previousLocation.X, this.Height));
                g.DrawLine (Pens.Gray, new PointF (0, _previousLocation.Y), new PointF (this.Width, _previousLocation.Y));
            }
            #endregion
            #region ValueIndicator 
            if (this.ValueIndicator) {
                var currentX = (this.GridOrigin.X - _previousLocation.X) / this.GridSize.Width;
                var currentY = (this.GridOrigin.Y - _previousLocation.Y) / this.GridSize.Height;

                var resultString = string.Format ("{0} | {1}", Math.Round (currentX, 2), Math.Round (currentY, 2));
                var stringDimensions = g.MeasureString (resultString, this.Font);

                var toolTipRect = new Rectangle (_previousLocation.X + 15, _previousLocation.Y + 15, 80, 25);
                g.FillRectangle (Brushes.White, toolTipRect);
                g.DrawRectangle (Pens.LightGray, toolTipRect);

                var invertXDirection = (this.Width - _previousLocation.X < 80);
                var invertYDirection = (this.Height - _previousLocation.Y < 45);

                g.DrawString (resultString, this.Font, Brushes.LightGray,
                    new PointF (_previousLocation.X + 15 + (80 - stringDimensions.Width) / 2, _previousLocation.Y + 17 + (25 - stringDimensions.Height) / 2));
            }
            #endregion
        }

        private PointF _lastOrigin;
        private void HandleMouseWheel (object sender, MouseEventArgs e) {
            // Needs work
            if (e.Delta > 0) {
                this.GridSize = new SizeF ((float)(this.GridSize.Width * 1.25), (float)(this.GridSize.Height * 1.25));
                this.GridOrigin = new PointF (
                    this.GridOrigin.X + ((e.X > _lastOrigin.X) ? -1.25f * (e.X - _lastOrigin.X) : 1.25f * (_lastOrigin.X - e.X)),
                    this.GridOrigin.Y + ((e.Y > _lastOrigin.Y) ? -1.25f * (e.Y - _lastOrigin.Y) : 1.25f * (_lastOrigin.Y - e.Y))
                    );
                _lastOrigin = this.GridOrigin;
            } else {
                var newSize = new SizeF ((float)(this.GridSize.Width / 1.25), (float)(this.GridSize.Height / 1.25));
                if (newSize.Width > 3 && newSize.Height > 3) {
                    this.GridSize = newSize;
                    this.GridOrigin = new PointF (
                        this.GridOrigin.X + ((_lastOrigin.X + e.X > _lastOrigin.X) ? (e.X - _lastOrigin.X) / 1.25f : (_lastOrigin.X - e.X) / -1.25f),
                        this.GridOrigin.Y + ((_lastOrigin.Y + e.Y > _lastOrigin.Y) ? (e.Y - _lastOrigin.Y) / 1.25f : (_lastOrigin.Y - e.Y) / -1.25f)
                    );
                    _lastOrigin = this.GridOrigin;
                }
            }
            this.Invalidate ();
        }
        private void HandleMouseDown (object sender, MouseEventArgs e) {
            _isMouseDown = true;
            _previousLocation = e.Location;
        }
        private void HandleMouseMove (object sender, MouseEventArgs e) {
            if (this.TrackingLines || this.ValueIndicator) 
                this.Invalidate ();

            if (_isMouseDown) {
                var deltaX = (e.X - _previousLocation.X);
                var deltaY = (e.Y - _previousLocation.Y);

                var newX = this.GridOrigin.X + deltaX;
                var newY = this.GridOrigin.Y + deltaY;

                this.GridOrigin = new PointF (newX, newY);
                this.Invalidate ();
            }

            _previousLocation = e.Location;
        }
        private void HandleMouseUp (object sender, MouseEventArgs e) {
            _isMouseDown = false;
        }

        public SizeF GridSize { get; set; }
        public PointF GridOrigin { get; set; }
        public SizeF GridScale { get; set; }
        public bool TrackingLines { get; set; }
        public bool ValueIndicator { get; set; }
        public BindingList<Vector2> Vertices { get; private set; } = new BindingList<Vector2> ();
        public BindingList<FunctionWrapper> Functions = new BindingList<FunctionWrapper> ();
    }

    public class FunctionWrapper {
        public FunctionWrapper (Func<double, double> function) {
            this.Values = new Dictionary<double, double> ();
            this.BaseFunction = function;
        }

        public Func<double, double> BaseFunction { get; }
        public Dictionary<double, double> Values { get; }
        public double GetValue (double x) {
            if (this.Values.ContainsKey (x))
                return this.Values [x];
            else {
                var resultValue = BaseFunction (x);
                this.Values.Add (x, resultValue);
                return resultValue;
            }
        }
    }
}
