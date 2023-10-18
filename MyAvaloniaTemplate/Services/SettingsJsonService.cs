using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MyAvaloniaTemplate.Abstractions;
using MyAvaloniaTemplate.Models.Settings;

namespace MyAvaloniaTemplate.Services;

public class SettingsJsonService : ISettingsService<SettingsModel>
{
    public SettingsModel Settings { get; private set; }
    private readonly string _jsonPath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "settings.jsonc");

    SettingsModel LoadSettings()
    {
        if (!File.Exists(_jsonPath))
        {
            SettingsModel sampleSettingsModel = SettingsModel.CreateSample();
            string sampleJson = JsonSerializer.Serialize(sampleSettingsModel, new JsonSerializerOptions()
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter()
                }
            });
            File.WriteAllText(_jsonPath, sampleJson);

            App.FrameworkInitializationCompleted += async (sender, lifetime) =>
            {
                var messageBox = MessageBoxManager.GetMessageBoxStandard(new MessageBoxStandardParams
                {
                    ButtonDefinitions = ButtonEnum.Ok,
                    Topmost = true,
                    ContentTitle = "Application First Run",
                    ContentMessage = "Application is running with sample configuration.",
                    ShowInCenter = true,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Icon = Icon.Info
                });

                await messageBox.ShowAsync();
            };
        }

        string jsonContent = File.ReadAllText(_jsonPath);
        var settings = JsonSerializer.Deserialize<SettingsModel>(jsonContent, new JsonSerializerOptions()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        });
        if (settings is null)
        {
            throw new InvalidOperationException("Settings cannot be null");
        }

        return settings;
    }

    public SettingsJsonService()
    {
        Settings = LoadSettings();
    }

    public async Task UpdateSettings(SettingsModel settings)
    {
        Settings = settings;
        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions()
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        });
        await File.WriteAllTextAsync(_jsonPath, json);
        SettingsUpdated?.Invoke(this, settings);
    }

    public event EventHandler<SettingsModel>? SettingsUpdated;
}