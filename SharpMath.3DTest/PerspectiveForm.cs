using SharpMath.Geometry;
using SharpMath.Trigonometry;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SharpMath._3DTest
{
    public partial class PerspectiveForm : Form
    {
        private Color _color = Color.White;
        private Vector4[] _vertices = new Vector4[] { new Vector4(-0.5, 0.5, 1.5, 1), new Vector4(0.5, 0.5, 1.5, 1), new Vector4(0.5, -0.5, 1.5, 1), new Vector4(-0.5, -0.5, 1.5, 1),
                new Vector4(-0.5, 0.5, 2.5, 1), new Vector4(0.5, 0.5, 2.5, 1), new Vector4(0.5, -0.5, 2.5, 1), new Vector4(-0.5, -0.5, 2.5, 1) };
        private Matrix4x4 _view;
        private Matrix4x4 _world;
        private Matrix4x4 _scalation = Matrix4x4.Scalation(0.5f);
        private Matrix4x4 _rotationX = Matrix4x4.Identity;
        private Matrix4x4 _rotationY = Matrix4x4.Identity;
        private Matrix4x4 _rotationZ = Matrix4x4.Identity;
        //private Matrix4x4 _projection;

        public PerspectiveForm()
        {
            InitializeComponent();
        }

        private void PerspectiveForm_Load(object sender, EventArgs e)
        {
            // Bottom front left, Bottom front right, Bottom back right, Bottom back left, Top front left, Top front right, Top back right, Top back left
            //_vertices = new[] { new Vector4(-1.5, 0, 0, 1), new Vector4(1.5, 0, 0, 1), new Vector4(1.5, 0, -1.5, 1), new Vector4(-1.5, 0, -1.5, 1), new Vector4(-1.5, 1.5, 0, 1), new Vector4(1.5, 1.5, 0, 1), new Vector4(1.5, 1.5, -1.5, 1), new Vector4(-1.5, 1.5, -1.5, 1) };
            UpdateMatrices();
        }

        private void UpdateMatrices()
        {
            var cameraPosition = Vector3.Zero;
            var cameraTarget = Vector3.Back;
            _view = Matrix4x4.View(cameraPosition, cameraTarget, Vector3.Up);
            _world = Matrix4x4.World(cameraPosition, Vector3.Back, Vector3.Up);
            //_projection = Matrix4x4.PerspectiveFieldOfView(projectionData);

            perspectivePanel.Invalidate();
        }

        private void perspectivePanel_Paint(object sender, PaintEventArgs e)
        {
            var transformationMatrix = Matrix4x4.Translation(0, 0, 2) * _view * _world * _rotationX * _rotationY * _rotationZ * _scalation * Matrix4x4.Translation(0, 0, -2);
            var projectionData = new ProjectionData(perspectivePanel.Size, 16f / 9f, (float)Math.PI / 3f, 0.1f, 100f);

            Func<Vector4, Vector2> projectPerspective = (vector) =>
            {
                var perspectiveVector = vector * Matrix4x4.PerspectiveFieldOfView(projectionData);
                perspectiveVector.X /= perspectiveVector.W;
                perspectiveVector.Y /= perspectiveVector.W;
                perspectiveVector.Z /= perspectiveVector.W;

                var deviceVector = perspectiveVector * Matrix4x4.Scalation(projectionData.CanvasSize.Width / 2.0, projectionData.CanvasSize.Height / 2.0, 1);
                return new Vector2(deviceVector.X + projectionData.CanvasSize.Width / 2, (projectionData.CanvasSize.Height / 2) - deviceVector.Y);
            };

            PointF[] displayCoordinates = new PointF[_vertices.Length];
            for (int i = 0; i < _vertices.Length; i++)
            {
                Vector2 displayVector = projectPerspective(_vertices[i] * transformationMatrix);
                displayCoordinates[i] = new PointF((float)displayVector.X, (float)displayVector.Y);
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            using (Pen p = new Pen(_color, 3))
            {
                for (int i = 0, j = 3; i < 4; j = i, i++)
                    g.DrawLine(p, displayCoordinates[i], displayCoordinates[j]);
                for (int i = 4, j = 7; i < 8; j = i, i++)
                    g.DrawLine(p, displayCoordinates[i], displayCoordinates[j]);
                for (int i = 0, j = 4; i < 4; i++, j++)
                    g.DrawLine(p, displayCoordinates[i], displayCoordinates[j]);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            _rotationX = Matrix4x4.RotationX(Converter.DegreesToRadians(trackBar1.Value));
            UpdateMatrices();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            _rotationY = Matrix4x4.RotationY(Converter.DegreesToRadians(trackBar2.Value));
            UpdateMatrices();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            _rotationZ = Matrix4x4.RotationZ(Converter.DegreesToRadians(trackBar3.Value));
            UpdateMatrices();
        }

        private float delta = 1f;
        private float angle = 0f;
        private void rotationTimer_Tick(object sender, EventArgs e)
        {
            if (angle <= 0)
                delta = 1f;
            else if (angle >= 360)
                delta = -1f;
            angle += delta;

            _rotationX = Matrix4x4.RotationX(Converter.DegreesToRadians(angle));
            _rotationY = Matrix4x4.RotationY(Converter.DegreesToRadians(angle));
            _rotationZ = Matrix4x4.RotationZ(Converter.DegreesToRadians(angle));

            trackBar1.Value = (int)angle;
            trackBar2.Value = (int)angle;
            trackBar3.Value = (int)angle;

            // Causes headaches
            //Random randomGen = new Random();
            //KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            //KnownColor randomColorName = names[randomGen.Next(names.Length)];
            //_color = Color.FromKnownColor(randomColorName);

            UpdateMatrices();
        }

        private void autoRotateCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            rotationTimer.Enabled = !rotationTimer.Enabled;
            trackBar1.Enabled = !trackBar1.Enabled;
            trackBar2.Enabled = !trackBar2.Enabled;
            trackBar3.Enabled = !trackBar3.Enabled;
            if (!rotationTimer.Enabled)
                _color = Color.White;
        }

        private void scalationTrackBar_Scroll(object sender, EventArgs e)
        {
            _scalation = Matrix4x4.Scalation(scalationTrackBar.Value / 720f);
            UpdateMatrices();
        }
    }
}