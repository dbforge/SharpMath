namespace SharpMath.Canvas2DTest {
    partial class Canvas2DTest {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose (bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose ();
            }
            base.Dispose (disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent () {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager (typeof (Canvas2DTest));
            this.testCanvas = new SharpMath.Presentation.Canvas2D ();
            this.SuspendLayout ();
            // 
            // testCanvas
            // 
            this.testCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testCanvas.Font = new System.Drawing.Font ("Segoe UI", 8F);
            this.testCanvas.ForeColor = System.Drawing.Color.Gray;
            this.testCanvas.GridOrigin = ((System.Drawing.PointF)(resources.GetObject ("testCanvas.GridOrigin")));
            this.testCanvas.GridScale = new System.Drawing.SizeF (20F, 10F);
            this.testCanvas.GridSize = new System.Drawing.SizeF (20F, 20F);
            this.testCanvas.Location = new System.Drawing.Point (0, 0);
            this.testCanvas.Name = "testCanvas";
            this.testCanvas.Size = new System.Drawing.Size (825, 469);
            this.testCanvas.TabIndex = 0;
            this.testCanvas.Text = "testCanvas";
            this.testCanvas.TrackingLines = false;
            this.testCanvas.ValueIndicator = true;
            // 
            // Canvas2DTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size (825, 469);
            this.Controls.Add (this.testCanvas);
            this.Name = "Canvas2DTest";
            this.Text = "Canvas2DTest";
            this.Load += new System.EventHandler (this.Form1_Load);
            this.ResumeLayout (false);

        }

        #endregion

        private SharpMath.Presentation.Canvas2D testCanvas;
    }
}

