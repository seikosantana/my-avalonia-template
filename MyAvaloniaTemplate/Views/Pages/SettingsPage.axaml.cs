using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using MyAvaloniaTemplate.Extensions;

namespace MyAvaloniaTemplate.Views.Pages;
 
public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
#if DEBUG
        if (Design.IsDesignMode)
        {
            App.ConfigureServicesAsync().GetAwaiter().GetResult();
            Ioc.Default.CreateBackgroundServices();
            var vm = Ioc.Default.GetMainViewModel();
            DataContext = vm.SettingsPageViewModel;
        }
#endif
    }

    private void ToggleButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is App app)
        {
            var themeBootstrap = app.LocateMaterialTheme<MaterialTheme>();
            themeBootstrap.BaseTheme = ThemeSwitch.IsChecked ?? false ? BaseThemeMode.Dark : BaseThemeMode.Light;
        }
    }
}