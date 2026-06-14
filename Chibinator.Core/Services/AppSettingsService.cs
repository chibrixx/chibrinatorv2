using System.Text.Json;
using Chibinator.Core.Models;
using Microsoft.Extensions.Logging;

namespace Chibinator.Core.Services;

public class AppSettingsService
{
    private readonly ILogger<AppSettingsService> _logger;
    private readonly string _settingsPath;
    private AppSettings _settings = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public AppSettingsService(ILogger<AppSettingsService> logger)
    {
        _logger = logger;
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Chibinator");
        Directory.CreateDirectory(dir);
        _settingsPath = Path.Combine(dir, "settings.json");
    }

    public AppSettings Current => _settings;

    public async Task LoadAsync()
    {
        if (!File.Exists(_settingsPath))
        {
            _settings = new AppSettings();
            _logger.LogInformation("No settings file found. Using defaults.");
            return;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_settingsPath);
            _settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
            _logger.LogInformation("Settings loaded from {Path}", _settingsPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load settings. Using defaults.");
            _settings = new AppSettings();
        }
    }

    public async Task SaveAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_settings, JsonOptions);
            await File.WriteAllTextAsync(_settingsPath, json);
            _logger.LogInformation("Settings saved.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings.");
        }
    }

    public void Update(Action<AppSettings> updater)
    {
        updater(_settings);
    }
}
