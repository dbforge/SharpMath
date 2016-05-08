// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.Windows.Forms;
using SharpMath.Geometry;
using SharpMath.Presentation;

namespace SharpMath.Canvas2DTest
{
    public partial class Canvas2DTest : Form
    {
        public Canvas2DTest()
        {
            InitializeComponent();
        }

        private void Canvas2DTest_Load(object sender, EventArgs e)
        {
            testCanvas.Functions.Add(new FunctionWrapper(x => Math.Pow(x, 3)));
            testCanvas.Functions.Add(new FunctionWrapper(x => 1.5 * Math.Sin(x)));
            testCanvas.Vertices.Add(new Vector2(4, 8));
        }
    }
}