using System.Text.Json.Serialization;

namespace Chibinator.Core.Models;

public class Profile
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("presetType")]
    public PresetType PresetType { get; set; } = PresetType.Custom;

    [JsonPropertyName("flags")]
    public List<FastFlag> Flags { get; set; } = new();

    [JsonPropertyName("fpsCapMode")]
    public FpsCapMode FpsCapMode { get; set; } = FpsCapMode.Cap60;

    [JsonPropertyName("clearCacheOnLaunch")]
    public bool ClearCacheOnLaunch { get; set; } = false;

    [JsonPropertyName("isBuiltIn")]
    public bool IsBuiltIn { get; set; } = false;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("lastModified")]
    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}

public enum PresetType
{
    UltraPotato,
    Competitive,
    Balanced,
    Default,
    Custom
}

public enum FpsCapMode
{
    Cap60 = 60,
    Cap120 = 120,
    Cap144 = 144,
    Cap240 = 240,
    Unlimited = 0
}
