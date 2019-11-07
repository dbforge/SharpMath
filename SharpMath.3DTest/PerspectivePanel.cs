// PerspectivePanel.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System.Windows.Forms;

namespace SharpMath._3DTest
{
    public class PerspectivePanel : Panel
    {
        public PerspectivePanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
        }
    }
}