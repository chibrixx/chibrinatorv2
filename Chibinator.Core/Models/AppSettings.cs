using System.Text.Json.Serialization;

namespace Chibinator.Core.Models;

public class AppSettings
{
    [JsonPropertyName("robloxPath")]
    public string RobloxPath { get; set; } = string.Empty;

    [JsonPropertyName("activeProfileId")]
    public string ActiveProfileId { get; set; } = string.Empty;

    [JsonPropertyName("autoApplyOnLaunch")]
    public bool AutoApplyOnLaunch { get; set; } = true;

    [JsonPropertyName("backupBeforeApply")]
    public bool BackupBeforeApply { get; set; } = true;

    [JsonPropertyName("showStatusBar")]
    public bool ShowStatusBar { get; set; } = true;

    [JsonPropertyName("checkForUpdates")]
    public bool CheckForUpdates { get; set; } = true;

    [JsonPropertyName("launchDelay")]
    public int LaunchDelayMs { get; set; } = 500;

    [JsonPropertyName("lastKnownRobloxVersion")]
    public string LastKnownRobloxVersion { get; set; } = string.Empty;

    [JsonPropertyName("hardwareProfile")]
    public HardwareProfile? DetectedHardware { get; set; }
}

public class HardwareProfile
{
    [JsonPropertyName("cpuName")]
    public string CpuName { get; set; } = string.Empty;

    [JsonPropertyName("gpuName")]
    public string GpuName { get; set; } = string.Empty;

    [JsonPropertyName("ramMb")]
    public long RamMb { get; set; }

    [JsonPropertyName("suggestedPreset")]
    public PresetType SuggestedPreset { get; set; } = PresetType.Balanced;

    [JsonPropertyName("detectedAt")]
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
}
