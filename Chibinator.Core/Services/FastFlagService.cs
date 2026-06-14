using System.Text.Json;
using Chibinator.Core.Models;
using Microsoft.Extensions.Logging;

namespace Chibinator.Core.Services;

public class FastFlagService
{
    private readonly ILogger<FastFlagService> _logger;

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true
    };

    public FastFlagService(ILogger<FastFlagService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Applies a profile's FastFlags by writing to ClientAppSettings.json.
    /// This is the ONLY way Chibinator modifies Roblox — official config file only.
    /// Only flags on Roblox's published allowlist (as of late 2025) are written;
    /// all others are silently ignored by the client.
    /// </summary>
    public async Task<bool> ApplyProfileAsync(Profile profile, RobloxInstallInfo installInfo)
    {
        try
        {
            Directory.CreateDirectory(installInfo.ClientSettingsDir);

            var flagDict = new Dictionary<string, object>();

            foreach (var flag in profile.Flags.Where(f => f.IsEnabled))
            {
                var parsed = ParseFlagValue(flag.Value);
                flagDict[flag.Key] = parsed;
            }

            var json = JsonSerializer.Serialize(flagDict, WriteOptions);
            await File.WriteAllTextAsync(installInfo.ClientAppSettingsPath, json);

            _logger.LogInformation("Applied {Count} flags to {Path}", flagDict.Count, installInfo.ClientAppSettingsPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply profile {Name}", profile.Name);
            return false;
        }
    }

    /// <summary>
    /// Backs up the existing ClientAppSettings.json before overwriting.
    /// </summary>
    public async Task<string?> BackupCurrentConfigAsync(RobloxInstallInfo installInfo)
    {
        if (!File.Exists(installInfo.ClientAppSettingsPath)) return null;

        try
        {
            var backupDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Chibinator", "Backups");
            Directory.CreateDirectory(backupDir);

            var backupFileName = $"ClientAppSettings_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var backupPath = Path.Combine(backupDir, backupFileName);

            await using (var src = File.OpenRead(installInfo.ClientAppSettingsPath)) { await using var dst = File.Create(backupPath); await src.CopyToAsync(dst); }
            _logger.LogInformation("Config backed up to: {Path}", backupPath);
            return backupPath;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Backup failed — continuing anyway.");
            return null;
        }
    }

    /// <summary>
    /// Removes ClientAppSettings.json to restore Roblox defaults.
    /// </summary>
    public bool RemoveConfig(RobloxInstallInfo installInfo)
    {
        try
        {
            if (File.Exists(installInfo.ClientAppSettingsPath))
            {
                File.Delete(installInfo.ClientAppSettingsPath);
                _logger.LogInformation("Removed ClientAppSettings.json — Roblox defaults restored.");
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove config.");
            return false;
        }
    }

    /// <summary>
    /// Reads the current ClientAppSettings.json flags as a raw dictionary.
    /// </summary>
    public async Task<Dictionary<string, JsonElement>> ReadCurrentFlagsAsync(RobloxInstallInfo installInfo)
    {
        if (!File.Exists(installInfo.ClientAppSettingsPath))
            return new Dictionary<string, JsonElement>();

        try
        {
            var json = await File.ReadAllTextAsync(installInfo.ClientAppSettingsPath);
            return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)
                   ?? new Dictionary<string, JsonElement>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not read existing ClientAppSettings.json.");
            return new Dictionary<string, JsonElement>();
        }
    }

    private static object ParseFlagValue(string value)
    {
        if (bool.TryParse(value, out var boolVal)) return boolVal ? "True" : "False";
        if (int.TryParse(value, out var intVal)) return intVal;
        return value;
    }
}

internal static class FileExtensions
{
    public static async Task CopyAsync(string source, string dest)
    {
        await using var src = File.OpenRead(source);
        await using var dst = File.Create(dest);
        await src.CopyToAsync(dst);
    }
}
