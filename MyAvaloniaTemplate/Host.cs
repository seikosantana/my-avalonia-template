using System;
using System.Linq;
using System.Threading;
using Avalonia;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyAvaloniaTemplate.Extensions;

namespace MyAvaloniaTemplate;

public class Host
{
    public void Run(string[] args)
    {
        var logger = Ioc.Default.GetRequiredService<ILogger<Host>>();
        logger.LogInformation("Host starting");

        Ioc.Default.CreateBackgroundServices();

        bool headless = args.Any(arg => arg.ToLower() == "--headless");

        if (!headless)
        {
            logger.LogInformation("User interface starting");
            Program.BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        else
        {
            logger.LogInformation("Running in headless mode");
            ManualResetEventSlim mre = new ManualResetEventSlim(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                mre.Set();
                eventArgs.Cancel = true;
            };
            mre.Wait();
        }

        Ioc.Default.CleanupBackgroundServices();

        logger.LogInformation("Host shutdown");
    }
}