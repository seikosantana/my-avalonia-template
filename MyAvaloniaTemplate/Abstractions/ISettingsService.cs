using System;
using System.Threading.Tasks;
using MyAvaloniaTemplate.Models.Settings;

namespace MyAvaloniaTemplate.Abstractions;

public interface ISettingsService
{
    public SettingsModel Settings { get; }
    public Task UpdateSettings(SettingsModel settings);

    public event EventHandler<SettingsModel>? SettingsUpdated;
}