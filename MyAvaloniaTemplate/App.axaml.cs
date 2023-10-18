using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Material.Colors;
using Material.Styles.Themes;
using MyAvaloniaTemplate.Abstractions;
using MyAvaloniaTemplate.Extensions;
using MyAvaloniaTemplate.Models.Settings;
using MyAvaloniaTemplate.Views;

namespace MyAvaloniaTemplate;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static event EventHandler<IApplicationLifetime?>? FrameworkInitializationCompleted;

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = Ioc.Default.GetMainViewModel();
            var settings = Ioc.Default.GetRequiredService<ISettingsService<SettingsModel>>();

            var theme = Theme.Create(settings.Settings.DarkMode ? Theme.Dark : Theme.Light,
                SwatchHelper.Lookup[(MaterialColor)PrimaryColor.Blue],
                SwatchHelper.Lookup[(MaterialColor)SecondaryColor.Orange]);

            var materialTheme = this.LocateMaterialTheme<MaterialTheme>();

            materialTheme.CurrentThemeChanged.Subscribe(t =>
            {
                // TODO: Set resource for new theme if necessary
            });

            materialTheme.PropertyChanged += (sender, args) =>
            {
                if (args.Property == MaterialTheme.BaseThemeProperty)
                {
                    // TODO: Set resource for new theme if necessary
                }
            };

            materialTheme.CurrentTheme = theme;

            desktop.MainWindow = new MainWindow
            {
                DataContext = vm,
            };
        }

        FrameworkInitializationCompleted?.Invoke(this, ApplicationLifetime);
        base.OnFrameworkInitializationCompleted();
    }
}