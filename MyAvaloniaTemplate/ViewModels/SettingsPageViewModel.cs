using CommunityToolkit.Mvvm.ComponentModel;
using MyAvaloniaTemplate.Abstractions;
using MyAvaloniaTemplate.Extensions;
using MyAvaloniaTemplate.Models.Settings;

namespace MyAvaloniaTemplate.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    public ISettingsService<SettingsModel> SettingsService { get; }

    [ObservableProperty]
    private SettingsModel _editableSettings;

    public SettingsPageViewModel(ISettingsService<SettingsModel> settingsService)
    {
        SettingsService = settingsService;
        EditableSettings = settingsService.Settings.JsonClone()!;
        SettingsService.SettingsUpdated += (sender, model) =>
        {
            EditableSettings = settingsService.Settings.JsonClone()!;
        };
    }

}