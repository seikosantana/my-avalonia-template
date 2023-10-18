using CommunityToolkit.Mvvm.ComponentModel;
using MyAvaloniaTemplate.Abstractions;
using MyAvaloniaTemplate.Models.Settings;

namespace MyAvaloniaTemplate.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public ISettingsService<SettingsModel> SettingsService { get; }

    [ObservableProperty]
    private int _activeNavIndex;

    [ObservableProperty]
    private SettingsPageViewModel _settingsPageViewModel;

    public MainViewModel(ISettingsService<SettingsModel> settingsService)
    {
        SettingsService = settingsService;
        SettingsPageViewModel = new SettingsPageViewModel(settingsService);
    }
}