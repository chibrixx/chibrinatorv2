using System.Diagnostics;
using System.Text.RegularExpressions;
using Chibinator.Core.Models;
using Microsoft.Extensions.Logging;

namespace Chibinator.Core.Services;

public class RobloxDetectionService
{
    private readonly ILogger<RobloxDetectionService> _logger;

    private static readonly string[] KnownInstallPaths =
    [
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "Versions"),
        @"C:\Program Files (x86)\Roblox\Versions",
        @"C:\Program Files\Roblox\Versions"
    ];

    public RobloxDetectionService(ILogger<RobloxDetectionService> logger)
    {
        _logger = logger;
    }

    public RobloxInstallInfo? DetectInstallation()
    {
        foreach (var basePath in KnownInstallPaths)
        {
            var result = ScanVersionsFolder(basePath);
            if (result != null)
            {
                _logger.LogInformation("Roblox found at: {Path}", result.ExePath);
                return result;
            }
        }

        _logger.LogWarning("Roblox installation not found in known paths.");
        return null;
    }

    public RobloxInstallInfo? DetectFromPath(string manualPath)
    {
        if (string.IsNullOrWhiteSpace(manualPath)) return null;

        // If the user points to the exe directly
        if (File.Exists(manualPath) && manualPath.EndsWith("RobloxPlayerBeta.exe", StringComparison.OrdinalIgnoreCase))
        {
            return BuildInstallInfo(Path.GetDirectoryName(manualPath)!, manualPath);
        }

        // If they point to a versions folder
        return ScanVersionsFolder(manualPath);
    }

    private RobloxInstallInfo? ScanVersionsFolder(string basePath)
    {
        if (!Directory.Exists(basePath)) return null;

        try
        {
            var versionDirs = Directory.GetDirectories(basePath, "version-*");
            RobloxInstallInfo? best = null;
            DateTime bestTime = DateTime.MinValue;

            foreach (var dir in versionDirs)
            {
                var exePath = Path.Combine(dir, "RobloxPlayerBeta.exe");
                if (!File.Exists(exePath)) continue;

                var writeTime = File.GetLastWriteTimeUtc(exePath);
                if (writeTime > bestTime)
                {
                    bestTime = writeTime;
                    best = BuildInstallInfo(dir, exePath);
                }
            }

            return best;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scanning versions folder: {Path}", basePath);
            return null;
        }
    }

    private static RobloxInstallInfo BuildInstallInfo(string versionDir, string exePath)
    {
        var clientSettingsDir = Path.Combine(versionDir, "ClientSettings");
        var configPath = Path.Combine(clientSettingsDir, "ClientAppSettings.json");
        var version = ExtractVersion(versionDir);
        var fileVersion = GetExeFileVersion(exePath);

        return new RobloxInstallInfo
        {
            VersionDir = versionDir,
            ExePath = exePath,
            ClientSettingsDir = clientSettingsDir,
            ClientAppSettingsPath = configPath,
            VersionFolder = version,
            ExeFileVersion = fileVersion,
            LastExeWriteTime = File.GetLastWriteTimeUtc(exePath)
        };
    }

    private static string ExtractVersion(string versionDir)
    {
        var dirName = Path.GetFileName(versionDir);
        var match = Regex.Match(dirName, @"version-([a-f0-9]+)", RegexOptions.IgnoreCase);
        return match.Success ? match.Value : dirName;
    }

    private static string GetExeFileVersion(string exePath)
    {
        try
        {
            var info = FileVersionInfo.GetVersionInfo(exePath);
            return info.FileVersion ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    public bool IsRobloxRunning()
    {
        return Process.GetProcessesByName("RobloxPlayerBeta").Length > 0;
    }
}

public class RobloxInstallInfo
{
    public string VersionDir { get; set; } = string.Empty;
    public string ExePath { get; set; } = string.Empty;
    public string ClientSettingsDir { get; set; } = string.Empty;
    public string ClientAppSettingsPath { get; set; } = string.Empty;
    public string VersionFolder { get; set; } = string.Empty;
    public string ExeFileVersion { get; set; } = string.Empty;
    public DateTime LastExeWriteTime { get; set; }
}
