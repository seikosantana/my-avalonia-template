using Avalonia.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using MyAvaloniaTemplate.Extensions;

namespace MyAvaloniaTemplate.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        if (Design.IsDesignMode)
        {
            Ioc.Default.ConfigureApplicationServices(true);
            Ioc.Default.CreateBackgroundServices();
            var vm = Ioc.Default.GetMainViewModel();
            DataContext = vm;
        }
#endif
    }
}