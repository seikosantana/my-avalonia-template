namespace MyAvaloniaTemplate.Models.Settings;

public class SettingsModel
{
    public bool DarkMode { get; set; }

    public static SettingsModel CreateSample()
    {
        // TODO: implement sample settings creation
        return new SettingsModel()
        {
            DarkMode = true
        };
    }
}