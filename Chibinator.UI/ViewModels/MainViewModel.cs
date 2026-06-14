using Chibinator.Core.Helpers;
using Chibinator.Core.Models;
using Chibinator.Core.Services;
using Microsoft.Extensions.Logging;

namespace Chibinator.UI.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly ILogger<MainViewModel> _logger;
    private readonly AppSettingsService _appSettingsService;
    private readonly RobloxDetectionService _robloxDetectionService;
    private readonly ProfileService _profileService;
    private readonly LaunchService _launchService;
    private readonly HardwareDetectionService _hardwareService;

    private bool _robloxDetected;
    private string _robloxVersion = "Not detected";
    private string _robloxPath = string.Empty;
    private Profile? _activeProfile;
    private string _statusMessage = "Ready";
    private string _launchStatusMessage = "Apply & Launch Roblox";
    private bool _isLaunching;
    private bool _configApplied;
    private string _activeNav = "Dashboard";
    private RobloxInstallInfo? _installInfo;
    private string? _suggestedPresetName;
    private bool _showSuggestionBanner;
    private ProfilesViewModel? _profilesViewModel;
    private FastFlagsViewModel? _fastFlagsViewModel;
    private SettingsViewModel? _settingsViewModel;

    public MainViewModel(
        ILogger<MainViewModel> logger,
        AppSettingsService appSettingsService,
        RobloxDetectionService robloxDetectionService,
        ProfileService profileService,
        LaunchService launchService,
        HardwareDetectionService hardwareService)
    {
        _logger = logger;
        _appSettingsService = appSettingsService;
        _robloxDetectionService = robloxDetectionService;
        _profileService = profileService;
        _launchService = launchService;
        _hardwareService = hardwareService;

        _launchService.StatusChanged += (_, args) => LaunchStatusMessage = args.Message;

        LaunchCommand            = new AsyncRelayCommand(LaunchAsync,    () => !IsLaunching && RobloxDetected && ActiveProfile != null);
        ApplyOnlyCommand         = new AsyncRelayCommand(ApplyOnlyAsync, () => !IsLaunching && RobloxDetected && ActiveProfile != null);
        NavigateCommand          = new RelayCommand(p => Navigate(p?.ToString() ?? "Dashboard"));
        RefreshRobloxCommand     = new AsyncRelayCommand(RefreshRobloxAsync);
        DismissSuggestionCommand = new RelayCommand(_ => ShowSuggestionBanner = false);
        AcceptSuggestionCommand  = new RelayCommand(_ => AcceptSuggestedPreset());
    }

    public AsyncRelayCommand LaunchCommand            { get; }
    public AsyncRelayCommand ApplyOnlyCommand         { get; }
    public RelayCommand      NavigateCommand          { get; }
    public AsyncRelayCommand RefreshRobloxCommand     { get; }
    public RelayCommand      DismissSuggestionCommand { get; }
    public RelayCommand      AcceptSuggestionCommand  { get; }

    public bool   RobloxDetected { get => _robloxDetected; set => SetProperty(ref _robloxDetected, value); }
    public string RobloxVersion  { get => _robloxVersion;  set => SetProperty(ref _robloxVersion, value); }
    public string RobloxPath     { get => _robloxPath;     set => SetProperty(ref _robloxPath, value); }

    public Profile? ActiveProfile
    {
        get => _activeProfile;
        set
        {
            if (SetProperty(ref _activeProfile, value))
            {
                if (value != null)
                {
                    _appSettingsService.Update(s => s.ActiveProfileId = value.Id);
                    _ = _appSettingsService.SaveAsync();
                }
                OnPropertyChanged(nameof(ActiveProfileName));
                OnPropertyChanged(nameof(ActiveProfileDescription));
                OnPropertyChanged(nameof(ActivePresetBadge));
            }
        }
    }

    public string ActiveProfileName        => _activeProfile?.Name        ?? "No profile selected";
    public string ActiveProfileDescription => _activeProfile?.Description ?? "Select a profile to get started.";
    public string ActivePresetBadge        => _activeProfile?.PresetType.ToString() ?? "—";

    public string StatusMessage       { get => _statusMessage;       set => SetProperty(ref _statusMessage, value); }
    public string LaunchStatusMessage { get => _launchStatusMessage; set => SetProperty(ref _launchStatusMessage, value); }
    public bool   IsLaunching         { get => _isLaunching;         set => SetProperty(ref _isLaunching, value); }
    public bool   ConfigApplied       { get => _configApplied;       set => SetProperty(ref _configApplied, value); }
    public string ActiveNav           { get => _activeNav;           set => SetProperty(ref _activeNav, value); }
    public bool   ShowSuggestionBanner { get => _showSuggestionBanner; set => SetProperty(ref _showSuggestionBanner, value); }
    public string? SuggestedPresetName { get => _suggestedPresetName; set => SetProperty(ref _suggestedPresetName, value); }

    public IReadOnlyList<Profile> AllProfiles => _profileService.GetAll();

    public ProfilesViewModel? ProfilesViewModel  { get => _profilesViewModel;  set => SetProperty(ref _profilesViewModel, value); }
    public FastFlagsViewModel? FastFlagsViewModel { get => _fastFlagsViewModel; set => SetProperty(ref _fastFlagsViewModel, value); }
    public SettingsViewModel?  SettingsViewModel  { get => _settingsViewModel;  set => SetProperty(ref _settingsViewModel, value); }

    // ── Init ─────────────────────────────────────────────────────────────────
    public async Task InitializeAsync()
    {
        await RefreshRobloxAsync();
        RestoreActiveProfile();
        RunHardwareSuggestion();

        var mainWindow = new Views.MainWindow { DataContext = this };
        mainWindow.Show();
    }

    public async Task RefreshRobloxAsync()
    {
        StatusMessage = "Detecting Roblox...";

        RobloxInstallInfo? info = null;
        var savedPath = _appSettingsService.Current.RobloxPath;
        if (!string.IsNullOrWhiteSpace(savedPath))
            info = _robloxDetectionService.DetectFromPath(savedPath);
        if (info == null)
            info = _robloxDetectionService.DetectInstallation();

        _installInfo = info;

        if (info != null)
        {
            RobloxDetected = true;
            RobloxVersion  = info.ExeFileVersion;
            RobloxPath     = info.ExePath;
            StatusMessage  = "Roblox detected ✔";
            _appSettingsService.Update(s => s.LastKnownRobloxVersion = info.ExeFileVersion);
            await _appSettingsService.SaveAsync();
        }
        else
        {
            RobloxDetected = false;
            RobloxVersion  = "Not found";
            RobloxPath     = string.Empty;
            StatusMessage  = "⚠ Roblox not found — set path in Settings";
        }
    }

    private void RestoreActiveProfile()
    {
        var savedId = _appSettingsService.Current.ActiveProfileId;
        Profile? profile = null;
        if (!string.IsNullOrWhiteSpace(savedId))
            profile = _profileService.GetById(savedId);
        profile ??= _profileService.GetByPresetType(PresetType.Balanced);
        profile ??= _profileService.GetAll().FirstOrDefault();
        ActiveProfile = profile;
    }

    private void RunHardwareSuggestion()
    {
        try
        {
            var hw = _appSettingsService.Current.DetectedHardware
                  ?? _hardwareService.DetectHardware();
            _appSettingsService.Update(s => s.DetectedHardware = hw);
            _ = _appSettingsService.SaveAsync();

            if (ActiveProfile?.PresetType != hw.SuggestedPreset)
            {
                SuggestedPresetName = hw.SuggestedPreset.ToString();
                ShowSuggestionBanner = true;
            }
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Hardware suggestion failed."); }
    }

    private void AcceptSuggestedPreset()
    {
        if (_suggestedPresetName == null) return;
        if (Enum.TryParse<PresetType>(_suggestedPresetName, out var type))
        {
            var profile = _profileService.GetByPresetType(type);
            if (profile != null) ActiveProfile = profile;
        }
        ShowSuggestionBanner = false;
    }

    // ── Navigation ───────────────────────────────────────────────────────────
    private void Navigate(string target)
    {
        ActiveNav = target;
        switch (target)
        {
            case "Profiles":
                ProfilesViewModel = new ProfilesViewModel(_profileService, _appSettingsService, this);
                ProfilesViewModel.Refresh();
                break;
            case "FastFlags":
                if (ActiveProfile != null)
                    FastFlagsViewModel = new FastFlagsViewModel(ActiveProfile, _profileService);
                break;
            case "Settings":
                SettingsViewModel = new SettingsViewModel(_appSettingsService, _robloxDetectionService, this);
                break;
        }
    }

    // ── Launch ───────────────────────────────────────────────────────────────
    private async Task LaunchAsync()
    {
        if (_installInfo == null || ActiveProfile == null) return;
        IsLaunching = true;
        ConfigApplied = false;
        try
        {
            var result = await _launchService.ApplyAndLaunchAsync(ActiveProfile, _installInfo);
            ConfigApplied = result.Success;
            StatusMessage = result.Success ? "Config applied & Roblox launched ✔" : $"Error: {result.Message}";
            LaunchStatusMessage = result.Success ? "Launched ✔" : $"Failed: {result.Message}";
        }
        finally
        {
            IsLaunching = false;
            _ = Task.Delay(3000).ContinueWith(_ => LaunchStatusMessage = "Apply & Launch Roblox",
                System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    private async Task ApplyOnlyAsync()
    {
        if (_installInfo == null || ActiveProfile == null) return;
        IsLaunching = true;
        LaunchStatusMessage = "Applying flags...";
        try
        {
            var ok = await _launchService.ApplyOnlyAsync(ActiveProfile, _installInfo);
            ConfigApplied = ok;
            StatusMessage = ok ? "Config applied ✔ (Roblox not launched)" : "Failed to apply config.";
            LaunchStatusMessage = ok ? "Applied ✔" : "Failed";
        }
        finally
        {
            IsLaunching = false;
            _ = Task.Delay(2000).ContinueWith(_ => LaunchStatusMessage = "Apply & Launch Roblox",
                System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    public void SetActiveProfile(Profile profile)
    {
        ActiveProfile = profile;
        if (ActiveNav == "FastFlags")
            FastFlagsViewModel = new FastFlagsViewModel(profile, _profileService);
    }

    public RobloxInstallInfo? GetInstallInfo() => _installInfo;
}
