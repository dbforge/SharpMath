// Program.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

using System;
using System.Windows.Forms;

namespace SharpMath.PolygonTest
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ContainsTestForm());
        }
    }
}