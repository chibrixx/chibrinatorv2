using Chibinator.Core.Helpers;
using Chibinator.Core.Services;

namespace Chibinator.UI.ViewModels;

public class SettingsViewModel : ObservableObject
{
    private readonly AppSettingsService _appSettingsService;
    private readonly RobloxDetectionService _robloxDetectionService;
    private readonly MainViewModel _mainVm;

    private string _robloxPathOverride = string.Empty;
    private bool _autoApplyOnLaunch;
    private bool _backupBeforeApply;
    private int _launchDelayMs;
    private string _hardwareInfo = string.Empty;
    private string _suggestedPreset = string.Empty;

    public SettingsViewModel(
        AppSettingsService appSettingsService,
        RobloxDetectionService robloxDetectionService,
        MainViewModel mainVm)
    {
        _appSettingsService = appSettingsService;
        _robloxDetectionService = robloxDetectionService;
        _mainVm = mainVm;

        LoadFromSettings();

        SaveCommand = new AsyncRelayCommand(SaveAsync);

        BrowseRobloxPathCommand = new RelayCommand(_ =>
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Locate RobloxPlayerBeta.exe",
                Filter = "Roblox Executable|RobloxPlayerBeta.exe",
                FileName = "RobloxPlayerBeta.exe"
            };
            if (dialog.ShowDialog() == true)
                RobloxPathOverride = dialog.FileName;
        });

        RedetectRobloxCommand = new AsyncRelayCommand(async () =>
        {
            _appSettingsService.Update(s => s.RobloxPath = RobloxPathOverride);
            await _appSettingsService.SaveAsync();
            await _mainVm.RefreshRobloxAsync();
        });
    }

    public string RobloxPathOverride
    {
        get => _robloxPathOverride;
        set => SetProperty(ref _robloxPathOverride, value);
    }

    public bool AutoApplyOnLaunch
    {
        get => _autoApplyOnLaunch;
        set => SetProperty(ref _autoApplyOnLaunch, value);
    }

    public bool BackupBeforeApply
    {
        get => _backupBeforeApply;
        set => SetProperty(ref _backupBeforeApply, value);
    }

    public int LaunchDelayMs
    {
        get => _launchDelayMs;
        set => SetProperty(ref _launchDelayMs, value);
    }

    public string HardwareInfo
    {
        get => _hardwareInfo;
        set => SetProperty(ref _hardwareInfo, value);
    }

    public string SuggestedPreset
    {
        get => _suggestedPreset;
        set => SetProperty(ref _suggestedPreset, value);
    }

    public AsyncRelayCommand SaveCommand            { get; }
    public RelayCommand      BrowseRobloxPathCommand { get; }
    public AsyncRelayCommand RedetectRobloxCommand  { get; }

    private void LoadFromSettings()
    {
        var s = _appSettingsService.Current;
        RobloxPathOverride = s.RobloxPath;
        AutoApplyOnLaunch  = s.AutoApplyOnLaunch;
        BackupBeforeApply  = s.BackupBeforeApply;
        LaunchDelayMs      = s.LaunchDelayMs;

        if (s.DetectedHardware != null)
        {
            HardwareInfo   = $"{s.DetectedHardware.CpuName}  |  {s.DetectedHardware.GpuName}  |  {s.DetectedHardware.RamMb} MB RAM";
            SuggestedPreset = s.DetectedHardware.SuggestedPreset.ToString();
        }
        else
        {
            HardwareInfo    = "Not yet detected";
            SuggestedPreset = "—";
        }
    }

    private async Task SaveAsync()
    {
        _appSettingsService.Update(s =>
        {
            s.RobloxPath       = RobloxPathOverride;
            s.AutoApplyOnLaunch = AutoApplyOnLaunch;
            s.BackupBeforeApply = BackupBeforeApply;
            s.LaunchDelayMs    = LaunchDelayMs;
        });
        await _appSettingsService.SaveAsync();
    }
}
