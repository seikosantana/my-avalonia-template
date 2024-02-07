using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;

namespace MyAvaloniaTemplate.Views;

public class AppWindow : Window
{
    public AppWindow() : base()
    {
        Activated += (_, _) =>
        {
            if (Application.Current is App app)
            {
                app.ActiveWindow = this;
            }
        };
    }

    public AppWindow(IWindowImpl impl) : base(impl)
    {
    }
}