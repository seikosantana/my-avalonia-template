using System;
using System.Threading.Tasks;

namespace MyAvaloniaTemplate.Abstractions;

public interface ISettingsService<TSettings>
{
    public TSettings Settings { get; }
    public Task UpdateSettings(TSettings settings);

    public event EventHandler<TSettings>? SettingsUpdated;
}