using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyAvaloniaTemplate.Abstractions;
using MyAvaloniaTemplate.Extensions;
using MyAvaloniaTemplate.Models.Settings;

namespace MyAvaloniaTemplate.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    private ISettingsService SettingsService { get; }

    [ObservableProperty] private SettingsModel _editableSettings;

    [RelayCommand]
    private async Task Save()
    {
        await SettingsService.UpdateSettings(EditableSettings);
    }

    public SettingsPageViewModel(ISettingsService settingsService)
    {
        SettingsService = settingsService;
        EditableSettings = settingsService.Settings.JsonClone()!;
        SettingsService.SettingsUpdated += (sender, model) =>
        {
            EditableSettings = settingsService.Settings.JsonClone()!;
        };
    }
}