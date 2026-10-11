using Avalonia;
using Avalonia.Headless;
using Avalonia.Themes.Fluent;
using SharpMath.Canvas.Avalonia.Tests;

[assembly: AvaloniaTestApplication(typeof(TestApplication))]

namespace SharpMath.Canvas.Avalonia.Tests;

public sealed class TestApplication : Application
{
    public override void Initialize() => Styles.Add(new FluentTheme());

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<TestApplication>()
            .UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
}
