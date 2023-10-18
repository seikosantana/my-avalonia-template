using System;
using System.Threading.Tasks;
using MyAvaloniaTemplate.Abstractions;
using MyAvaloniaTemplate.Models.Settings;

namespace MyAvaloniaTemplate.Services;

public class DummySettingsService : ISettingsService<SettingsModel>
{
    public SettingsModel Settings { get; private set; } = SettingsModel.CreateSample();

    public Task UpdateSettings(SettingsModel settings)
    {
        Settings = settings;
        return Task.CompletedTask;
    }

    public event EventHandler<SettingsModel>? SettingsUpdated;
}