using Avalonia;
using System;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using MyAvaloniaTemplate.Extensions;

namespace MyAvaloniaTemplate;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
#if DEBUG
        if (Design.IsDesignMode)
        {
            Ioc.Default.ConfigureApplicationServices(true);
        }
        else
        {
            Ioc.Default.ConfigureApplicationServices();
        }
#else
        Ioc.Default.ConfigureApplicationServices();
#endif
        Host host = new Host();
        host.Run(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}