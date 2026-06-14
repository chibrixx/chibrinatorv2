using System.Text.Json;
using Chibinator.Core.Models;
using Microsoft.Extensions.Logging;

namespace Chibinator.Core.Services;

public class ProfileService
{
    private readonly ILogger<ProfileService> _logger;
    private readonly string _profilesDir;
    private List<Profile> _profiles = [];

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public ProfileService(ILogger<ProfileService> logger)
    {
        _logger = logger;
        _profilesDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Chibinator", "Profiles");
        Directory.CreateDirectory(_profilesDir);
    }

    public async Task LoadAllAsync()
    {
        _profiles = new List<Profile>(PresetDatabase.GetAllBuiltInProfiles());

        if (!Directory.Exists(_profilesDir)) return;

        foreach (var file in Directory.GetFiles(_profilesDir, "*.json"))
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var profile = JsonSerializer.Deserialize<Profile>(json, JsonOptions);
                if (profile != null && !profile.IsBuiltIn)
                    _profiles.Add(profile);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load profile file: {File}", file);
            }
        }

        _logger.LogInformation("Loaded {Count} profiles total", _profiles.Count);
    }

    public IReadOnlyList<Profile> GetAll() => _profiles.AsReadOnly();

    public Profile? GetById(string id) => _profiles.FirstOrDefault(p => p.Id == id);

    public Profile? GetByPresetType(PresetType type) =>
        _profiles.FirstOrDefault(p => p.PresetType == type && p.IsBuiltIn);

    public async Task<bool> SaveCustomProfileAsync(Profile profile)
    {
        try
        {
            if (profile.IsBuiltIn)
            {
                _logger.LogWarning("Cannot save over a built-in profile.");
                return false;
            }

            profile.LastModified = DateTime.UtcNow;
            var safeId = SanitizeFileName(profile.Id);
            var filePath = Path.Combine(_profilesDir, $"{safeId}.json");
            var json = JsonSerializer.Serialize(profile, JsonOptions);
            await File.WriteAllTextAsync(filePath, json);

            var existing = _profiles.FindIndex(p => p.Id == profile.Id);
            if (existing >= 0)
                _profiles[existing] = profile;
            else
                _profiles.Add(profile);

            _logger.LogInformation("Saved profile: {Name}", profile.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save profile: {Name}", profile.Name);
            return false;
        }
    }

    public bool DeleteProfile(string id)
    {
        var profile = GetById(id);
        if (profile == null) return false;
        if (profile.IsBuiltIn)
        {
            _logger.LogWarning("Cannot delete a built-in profile.");
            return false;
        }

        try
        {
            var filePath = Path.Combine(_profilesDir, $"{SanitizeFileName(id)}.json");
            if (File.Exists(filePath)) File.Delete(filePath);
            _profiles.RemoveAll(p => p.Id == id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete profile {Id}", id);
            return false;
        }
    }

    public async Task<string?> ExportProfileAsync(string id, string exportPath)
    {
        var profile = GetById(id);
        if (profile == null) return null;

        try
        {
            var json = JsonSerializer.Serialize(profile, JsonOptions);
            await File.WriteAllTextAsync(exportPath, json);
            return exportPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Export failed for profile {Id}", id);
            return null;
        }
    }

    public async Task<Profile?> ImportProfileAsync(string filePath)
    {
        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var profile = JsonSerializer.Deserialize<Profile>(json, JsonOptions);
            if (profile == null) return null;

            // Give it a fresh ID to avoid collisions
            profile.Id = Guid.NewGuid().ToString();
            profile.IsBuiltIn = false;
            profile.Name = $"{profile.Name} (Imported)";
            profile.LastModified = DateTime.UtcNow;

            await SaveCustomProfileAsync(profile);
            return profile;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Import failed from {Path}", filePath);
            return null;
        }
    }

    public Profile CreateCustomFromPreset(Profile source, string newName)
    {
        var clone = new Profile
        {
            Id = Guid.NewGuid().ToString(),
            Name = newName,
            Description = $"Custom profile based on {source.Name}",
            PresetType = PresetType.Custom,
            IsBuiltIn = false,
            FpsCapMode = source.FpsCapMode,
            ClearCacheOnLaunch = source.ClearCacheOnLaunch,
            Flags = source.Flags.Select(f => new FastFlag
            {
                Key = f.Key,
                Value = f.Value,
                Description = f.Description,
                Category = f.Category,
                IsEnabled = f.IsEnabled,
                SafetyNote = f.SafetyNote
            }).ToList(),
            CreatedAt = DateTime.UtcNow,
            LastModified = DateTime.UtcNow
        };
        return clone;
    }

    private static string SanitizeFileName(string input)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(input.Select(c => invalid.Contains(c) ? '_' : c));
    }
}
