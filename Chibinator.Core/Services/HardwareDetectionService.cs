using System.Management;
using Chibinator.Core.Models;
using Microsoft.Extensions.Logging;

namespace Chibinator.Core.Services;

public class HardwareDetectionService
{
    private readonly ILogger<HardwareDetectionService> _logger;

    public HardwareDetectionService(ILogger<HardwareDetectionService> logger)
    {
        _logger = logger;
    }

    public HardwareProfile DetectHardware()
    {
        var profile = new HardwareProfile
        {
            CpuName = GetCpuName(),
            GpuName = GetGpuName(),
            RamMb = GetTotalRamMb(),
            DetectedAt = DateTime.UtcNow
        };

        profile.SuggestedPreset = SuggestPreset(profile);
        _logger.LogInformation("Hardware detected: CPU={Cpu}, GPU={Gpu}, RAM={Ram}MB, Suggested={Preset}",
            profile.CpuName, profile.GpuName, profile.RamMb, profile.SuggestedPreset);

        return profile;
    }

    private PresetType SuggestPreset(HardwareProfile hw)
    {
        // Low-end: under 4GB RAM, or known integrated/old GPU
        if (hw.RamMb < 4096)
            return PresetType.UltraPotato;

        if (IsLowEndGpu(hw.GpuName))
            return PresetType.UltraPotato;

        // Mid-range: 4–8GB RAM, decent GPU
        if (hw.RamMb < 8192 || IsMidRangeGpu(hw.GpuName))
            return PresetType.Competitive;

        // High-end: 8GB+ RAM, modern GPU
        return PresetType.Balanced;
    }

    private static bool IsLowEndGpu(string gpuName)
    {
        if (string.IsNullOrWhiteSpace(gpuName)) return true;
        var lower = gpuName.ToLowerInvariant();
        return lower.Contains("intel hd") ||
               lower.Contains("intel uhd") ||
               lower.Contains("intel iris") ||
               lower.Contains("vega") && lower.Contains("amd") ||
               lower.Contains("710") ||
               lower.Contains("730") ||
               lower.Contains("amd radeon r3") ||
               lower.Contains("amd radeon r4") ||
               lower.Contains("amd radeon r5");
    }

    private static bool IsMidRangeGpu(string gpuName)
    {
        if (string.IsNullOrWhiteSpace(gpuName)) return false;
        var lower = gpuName.ToLowerInvariant();
        return lower.Contains("1050") ||
               lower.Contains("1060") ||
               lower.Contains("1650") ||
               lower.Contains("rx 5500") ||
               lower.Contains("rx 5600") ||
               lower.Contains("rx 570") ||
               lower.Contains("rx 580");
    }

    private string GetCpuName()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
                return obj["Name"]?.ToString()?.Trim() ?? "Unknown CPU";
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not query CPU name.");
        }
        return "Unknown CPU";
    }

    private string GetGpuName()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
            foreach (ManagementObject obj in searcher.Get())
            {
                var name = obj["Name"]?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(name)) return name;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not query GPU name.");
        }
        return "Unknown GPU";
    }

    private long GetTotalRamMb()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj["TotalVisibleMemorySize"] is ulong kb)
                    return (long)(kb / 1024);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not query RAM.");
        }
        return 0;
    }
}
