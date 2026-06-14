using System.Diagnostics;
using Chibinator.Core.Models;
using Microsoft.Extensions.Logging;

namespace Chibinator.Core.Services;

public class LaunchService
{
    private readonly ILogger<LaunchService> _logger;
    private readonly FastFlagService _fastFlagService;
    private readonly AppSettingsService _appSettingsService;

    public LaunchService(
        ILogger<LaunchService> logger,
        FastFlagService fastFlagService,
        AppSettingsService appSettingsService)
    {
        _logger = logger;
        _fastFlagService = fastFlagService;
        _appSettingsService = appSettingsService;
    }

    public event EventHandler<LaunchStatusEventArgs>? StatusChanged;

    public async Task<LaunchResult> ApplyAndLaunchAsync(
        Profile profile,
        RobloxInstallInfo installInfo,
        CancellationToken ct = default)
    {
        try
        {
            // Step 1: Backup
            if (_appSettingsService.Current.BackupBeforeApply)
            {
                ReportStatus("Backing up existing config...");
                await _fastFlagService.BackupCurrentConfigAsync(installInfo);
            }

            ct.ThrowIfCancellationRequested();

            // Step 2: Clear cache if requested
            if (profile.ClearCacheOnLaunch)
            {
                ReportStatus("Clearing Roblox cache...");
                ClearRobloxCache();
            }

            ct.ThrowIfCancellationRequested();

            // Step 3: Apply flags
            ReportStatus($"Applying profile: {profile.Name}...");
            var applied = await _fastFlagService.ApplyProfileAsync(profile, installInfo);
            if (!applied)
                return new LaunchResult(false, "Failed to write ClientAppSettings.json. Check permissions.");

            ct.ThrowIfCancellationRequested();

            // Step 4: Small delay so Windows flushes file writes
            var delay = _appSettingsService.Current.LaunchDelayMs;
            if (delay > 0)
                await Task.Delay(delay, ct);

            // Step 5: Launch Roblox
            ReportStatus("Launching Roblox...");
            var launched = LaunchRoblox(installInfo);
            if (!launched)
                return new LaunchResult(false, "Failed to start Roblox process.");

            ReportStatus("Roblox launched successfully ✔");
            return new LaunchResult(true, "Launched");
        }
        catch (OperationCanceledException)
        {
            return new LaunchResult(false, "Launch cancelled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during launch.");
            return new LaunchResult(false, $"Unexpected error: {ex.Message}");
        }
    }

    public async Task<bool> ApplyOnlyAsync(Profile profile, RobloxInstallInfo installInfo)
    {
        if (_appSettingsService.Current.BackupBeforeApply)
            await _fastFlagService.BackupCurrentConfigAsync(installInfo);

        return await _fastFlagService.ApplyProfileAsync(profile, installInfo);
    }

    private bool LaunchRoblox(RobloxInstallInfo installInfo)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = installInfo.ExePath,
                UseShellExecute = true
            };
            Process.Start(psi);
            _logger.LogInformation("Roblox process started: {Exe}", installInfo.ExePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Roblox process at {Exe}", installInfo.ExePath);
            return false;
        }
    }

    private void ClearRobloxCache()
    {
        var cachePaths = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Roblox", "logs"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Temp", "Roblox"),
            Path.Combine(Path.GetTempPath(), "Roblox")
        };

        foreach (var path in cachePaths)
        {
            if (!Directory.Exists(path)) continue;
            try
            {
                foreach (var file in Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly))
                {
                    try { File.Delete(file); } catch { /* skip locked files */ }
                }
                _logger.LogInformation("Cleared cache at: {Path}", path);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not fully clear cache at {Path}", path);
            }
        }
    }

    private void ReportStatus(string message)
    {
        _logger.LogInformation(message);
        StatusChanged?.Invoke(this, new LaunchStatusEventArgs(message));
    }
}

public class LaunchResult
{
    public bool Success { get; }
    public string Message { get; }
    public LaunchResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }
}

public class LaunchStatusEventArgs : EventArgs
{
    public string Message { get; }
    public LaunchStatusEventArgs(string message) => Message = message;
}
