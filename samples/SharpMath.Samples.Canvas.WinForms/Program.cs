namespace SharpMath.Samples.Canvas.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new CanvasForm());
    }
}
