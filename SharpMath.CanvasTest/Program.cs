// Program.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System;
using System.Windows.Forms;

namespace SharpMath.Canvas2DTest
{
    internal static class Program
    {
        /// <summary>
        ///     Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Canvas2DTest());
        }
    }
}