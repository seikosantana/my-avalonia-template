using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using MyAvaloniaTemplate.Extensions;

namespace MyAvaloniaTemplate.Views;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        if (Design.IsDesignMode)
        {
            App.ConfigureServicesAsync().GetAwaiter().GetResult();
            Ioc.Default.CreateBackgroundServices();
            var vm = Ioc.Default.GetMainViewModel();
            DataContext = vm;
        }
#endif
    }
}