using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpMath.Canvas2DTest {
    public partial class Canvas2DTest : Form {
        public Canvas2DTest () {
            InitializeComponent ();
        }

        private void Form1_Load (object sender, EventArgs e) {
            this.testCanvas.Functions.Add (new SharpMath.Presentation.FunctionWrapper ((x) => {
                return 0.7 * Math.Sin (x) + Math.Cos (x);
            }));
            this.testCanvas.Functions.Add (new SharpMath.Presentation.FunctionWrapper ((x) => {
                return 1.5 * Math.Sin (x) + 1.2 * x;
            }));
            this.testCanvas.Vertices.Add (new SharpMath.Geometry.Vector2 (4, 8));
            this.testCanvas.Vertices.Add (new SharpMath.Geometry.Vector2 (13, -3));
        }
    }
}
