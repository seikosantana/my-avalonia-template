using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Material.Colors;
using Material.Styles.Themes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyAvaloniaTemplate.Abstractions;
using MyAvaloniaTemplate.Extensions;
using MyAvaloniaTemplate.Services;
using MyAvaloniaTemplate.ViewModels;
using MyAvaloniaTemplate.Views;

namespace MyAvaloniaTemplate;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public AppWindow? ActiveWindow { get; set; }

    public static async Task ConfigureServicesAsync(bool? demo = null)
    {
        await Ioc.Default.ConfigureAsync(async services =>
        {
            services.AddSingleton<MainViewModel>();
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                if (OperatingSystem.IsWindows())
                {
                    logging.AddEventLog();
                }

                logging.SetMinimumLevel(LogLevel.Information);
            });

            if ((demo.HasValue && demo.Value) || (!demo.HasValue && Design.IsDesignMode))
            {
                services.AddSingleton<ISettingsService, DummySettingsService>();
                // TODO: Initialize Ioc with demo services 
            }
            else
            {
                // TODO: Initialize Ioc with production services
                services.AddSingleton<ISettingsService, SettingsJsonService>();
            }
        });
    }

    public static event EventHandler<IApplicationLifetime?>? FrameworkInitializationCompleted;

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
            SplashWindow splashWindow = new SplashWindow();

            async void OnSplashWindowOnOpened(object? o, EventArgs eventArgs)
            {
                // Give it some time to display Window
                await Task.Delay(1000);

                // Configure and start services
                await ConfigureServicesAsync();
                Ioc.Default.CreateBackgroundServices();

                var logger = Ioc.Default.GetRequiredService<ILogger<App>>();
                logger.LogInformation("App starting");

                var settings = Ioc.Default.GetRequiredService<ISettingsService>();

                var theme = Theme.Create(settings.Settings.DarkMode ? Theme.Dark : Theme.Light,
                    SwatchHelper.Lookup[(MaterialColor)PrimaryColor.Blue],
                    SwatchHelper.Lookup[(MaterialColor)SecondaryColor.Orange]);
                var materialTheme = this.LocateMaterialTheme<MaterialTheme>();
                materialTheme.CurrentThemeChanged.Subscribe(t =>
                {
                    // TODO:React on theme change
                });
                materialTheme.PropertyChanged += (sender, args) =>
                {
                    // TODO: React on theme change
                };
                materialTheme.CurrentTheme = theme;

                var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
                MainWindow mainWindow = new MainWindow { DataContext = mainViewModel };
                await Task.Delay(1000);
                splashWindow.Close();
                desktop.MainWindow = mainWindow;
                mainWindow.Show();
                FrameworkInitializationCompleted?.Invoke(this, ApplicationLifetime);
                base.OnFrameworkInitializationCompleted();
            }

            desktop.Exit += (sender, args) =>
            {
                var logger = Ioc.Default.GetRequiredService<ILogger<App>>();
                logger.LogInformation("Cleaning up services before exiting");
                Ioc.Default.CleanupBackgroundServices();
                logger.LogInformation("Finished cleaning up services.");
            };

            splashWindow.Opened += OnSplashWindowOnOpened;
            splashWindow.Show();
        }
    }
}