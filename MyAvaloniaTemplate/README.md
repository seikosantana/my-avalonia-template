# MyAvaloniaTemplate

Template for Avalonia-based applications, based on Avalonia templates, with added functionalities:

- Splash screen
- Initialization during splash screen
- Services and Background Services (similar to Generic Host)
- [Material.Avalonia](https://github.com/AvaloniaCommunity/Material.Avalonia) theme
- [Material.Avalonia.Dialogs](https://www.nuget.org/packages/Material.Avalonia.Dialogs)

## Must Do After Generating

After generating a new repository from this template, you may want to:

- Change solution and project name from MyAvaloniaTemplate to new names
- Refactor `MyAvaloniaTemplate` namespaces and root namespace
- Customize splash screen visuals in `Views/SplashWindow`
- Adapt settings properties in `Models/SettingsModel.cs`
- Adapt sample settings properties for design-time settings and sample configuration JSON in `CreateSample()` method
  in `Models/SettingsModels.cs`

## Active Window Tracking

### Avoid `Window`, Use `AppWindow` Instead

The `App` class has `ActiveWindow` property which gets updated automatically when a `AppWindow` is `Activated`.

### Getting Active Window

Useful for setting dialog owner, accessing `TopLevel`, cast `Application.Current` into `App` and
access `App.ActiveWindow`. For example:

```csharp
if (Application.Current is App app)
{
    if (app.ActiveWindow is Window window)
    {
        var filePickerResult = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            FileTypeFilter = new FilePickerFileType[]
            {
                new("CSV Files")
                {
                    Patterns = new[]
                    {
                        "*.csv"
                    }
                }
            }
        });
    }
}
```

## Background Services

To create a background service:

- Create a class that implements `IBackgroundService` interface that comes with this
  template.
- Register the background service in `ConfigureServices` in `App.axaml.cs`, TODOs are reserved for service registration.

```csharp
services.AddBackgroundService<MyBackgroundService>(); // register types directly
services.AddBackgroundService<IMyBackgroundService, MyBackgroundServiceImpl>(); // register as interface
```

- Background services are created and ran when `IocExtensions.CreateBackgroundServices()` is called;

### Background Service Tips

Unlike Generic Host BackgroundService, IBackgroundService does not have `StartAsync`. I suggest using `Task.Run()`
or `TaskFactory` for long-running tasks, or `DispatcherTimer` if your background service somehow interacts with Avalonia
user interface. 