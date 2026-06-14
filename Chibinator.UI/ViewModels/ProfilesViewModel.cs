using System.Collections.ObjectModel;
using Chibinator.Core.Helpers;
using Chibinator.Core.Models;
using Chibinator.Core.Services;

namespace Chibinator.UI.ViewModels;

public class ProfilesViewModel : ObservableObject
{
    private readonly ProfileService _profileService;
    private readonly AppSettingsService _appSettingsService;
    private readonly MainViewModel _mainVm;

    private ObservableCollection<Profile> _profiles = [];
    private Profile? _selectedProfile;
    private string _newProfileName = string.Empty;
    private bool _isCreating;

    public ProfilesViewModel(
        ProfileService profileService,
        AppSettingsService appSettingsService,
        MainViewModel mainVm)
    {
        _profileService = profileService;
        _appSettingsService = appSettingsService;
        _mainVm = mainVm;

        SelectProfileCommand = new RelayCommand(p =>
        {
            if (p is Profile profile) ActivateProfile(profile);
        });

        DeleteProfileCommand = new RelayCommand(p =>
        {
            if (p is Profile profile && !profile.IsBuiltIn) DeleteProfile(profile);
        });

        CreateFromActiveCommand = new RelayCommand(_ => BeginCreate(),
            _ => _mainVm.ActiveProfile != null);

        ConfirmCreateCommand = new RelayCommand(_ => ConfirmCreate(),
            _ => !string.IsNullOrWhiteSpace(NewProfileName));

        CancelCreateCommand = new RelayCommand(_ =>
        {
            IsCreating = false;
            NewProfileName = string.Empty;
        });

        ExportCommand = new AsyncRelayCommand(p => ExportProfileAsync(p as Profile));
        ImportCommand = new AsyncRelayCommand(_ => ImportProfileAsync());

        Refresh();
    }

    public ObservableCollection<Profile> Profiles
    {
        get => _profiles;
        set => SetProperty(ref _profiles, value);
    }

    public Profile? SelectedProfile
    {
        get => _selectedProfile;
        set => SetProperty(ref _selectedProfile, value);
    }

    public string NewProfileName
    {
        get => _newProfileName;
        set => SetProperty(ref _newProfileName, value);
    }

    public bool IsCreating
    {
        get => _isCreating;
        set => SetProperty(ref _isCreating, value);
    }

    public RelayCommand      SelectProfileCommand     { get; }
    public RelayCommand      DeleteProfileCommand     { get; }
    public RelayCommand      CreateFromActiveCommand  { get; }
    public RelayCommand      ConfirmCreateCommand     { get; }
    public RelayCommand      CancelCreateCommand      { get; }
    public AsyncRelayCommand ExportCommand            { get; }
    public AsyncRelayCommand ImportCommand            { get; }

    public void Refresh()
    {
        Profiles = new ObservableCollection<Profile>(_profileService.GetAll());
        SelectedProfile = _mainVm.ActiveProfile;
    }

    private void ActivateProfile(Profile profile)
    {
        SelectedProfile = profile;
        _mainVm.SetActiveProfile(profile);
    }

    private void DeleteProfile(Profile profile)
    {
        if (_profileService.DeleteProfile(profile.Id)) Refresh();
    }

    private void BeginCreate()
    {
        NewProfileName = $"Custom {DateTime.Now:HH:mm}";
        IsCreating = true;
    }

    private async void ConfirmCreate()
    {
        if (_mainVm.ActiveProfile == null) return;
        var clone = _profileService.CreateCustomFromPreset(_mainVm.ActiveProfile, NewProfileName);
        await _profileService.SaveCustomProfileAsync(clone);
        IsCreating = false;
        NewProfileName = string.Empty;
        Refresh();
    }

    private async Task ExportProfileAsync(Profile? profile)
    {
        if (profile == null) return;
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = $"{profile.Name.Replace(" ", "_")}.json",
            DefaultExt = ".json",
            Filter = "Chibinator Profile (*.json)|*.json"
        };
        if (dialog.ShowDialog() == true)
            await _profileService.ExportProfileAsync(profile.Id, dialog.FileName);
    }

    private async Task ImportProfileAsync()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            DefaultExt = ".json",
            Filter = "Chibinator Profile (*.json)|*.json"
        };
        if (dialog.ShowDialog() == true)
        {
            var imported = await _profileService.ImportProfileAsync(dialog.FileName);
            if (imported != null) Refresh();
        }
    }
}
