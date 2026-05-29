using Avalonia;
using Avalonia.Headless;

[assembly: AvaloniaTestApplication(typeof(Dv.App.Tests.TestAppBuilder))]

namespace Dv.App.Tests;

public class TestApp : Application
{
    public override void Initialize() { }
}

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<TestApp>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}