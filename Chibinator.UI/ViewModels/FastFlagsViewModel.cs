using System.Collections.ObjectModel;
using Chibinator.Core.Helpers;
using Chibinator.Core.Models;
using Chibinator.Core.Services;

namespace Chibinator.UI.ViewModels;

public class FastFlagEntryViewModel : ObservableObject
{
    private bool _isEnabled;

    public FastFlag Flag { get; }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    public string Key => Flag.Key;
    public string Value => Flag.Value;
    public string Description => Flag.Description;
    public string Category => Flag.Category;
    public string SafetyNote => Flag.SafetyNote;

    public FastFlagEntryViewModel(FastFlag flag)
    {
        Flag = flag;
        _isEnabled = flag.IsEnabled;
    }
}

public class FastFlagsViewModel : ObservableObject
{
    private readonly ProfileService _profileService;
    private readonly Profile _profile;

    private ObservableCollection<FastFlagEntryViewModel> _flags = [];
    private string _filterText = string.Empty;
    private string _selectedCategory = "All";
    private ObservableCollection<FastFlagEntryViewModel> _filteredFlags = [];

    public FastFlagsViewModel(Profile profile, ProfileService profileService)
    {
        _profile = profile;
        _profileService = profileService;

        ToggleFlagCommand = new RelayCommand(entry =>
        {
            if (entry is FastFlagEntryViewModel vm)
            {
                vm.Flag.IsEnabled = vm.IsEnabled;
                _ = SaveAsync();
            }
        });

        ResetAllCommand = new RelayCommand(_ =>
        {
            foreach (var f in _flags)
            {
                f.IsEnabled = true;
                f.Flag.IsEnabled = true;
            }
            _ = SaveAsync();
        });

        LoadFlags();
        BuildCategories();
        ApplyFilter();
    }

    public ObservableCollection<FastFlagEntryViewModel> FilteredFlags
    {
        get => _filteredFlags;
        set => SetProperty(ref _filteredFlags, value);
    }

    public string FilterText
    {
        get => _filterText;
        set
        {
            if (SetProperty(ref _filterText, value))
                ApplyFilter();
        }
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
                ApplyFilter();
        }
    }

    public ObservableCollection<string> Categories { get; private set; } = [];

    public string ProfileName => _profile.IsBuiltIn ? $"{_profile.Name} (Built-in)" : _profile.Name;
    public bool IsBuiltIn => _profile.IsBuiltIn;

    public RelayCommand ToggleFlagCommand { get; }
    public RelayCommand ResetAllCommand { get; }

    private void LoadFlags()
    {
        _flags = new ObservableCollection<FastFlagEntryViewModel>(
            _profile.Flags.Select(f => new FastFlagEntryViewModel(f)));
    }

    private void BuildCategories()
    {
        var cats = _flags.Select(f => f.Category).Distinct().OrderBy(c => c).ToList();
        Categories = new ObservableCollection<string>(new[] { "All" }.Concat(cats));
    }

    private void ApplyFilter()
    {
        var filtered = _flags.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(_filterText))
        {
            var lc = _filterText.ToLowerInvariant();
            filtered = filtered.Where(f =>
                f.Key.ToLowerInvariant().Contains(lc) ||
                f.Description.ToLowerInvariant().Contains(lc));
        }

        if (_selectedCategory != "All")
            filtered = filtered.Where(f => f.Category == _selectedCategory);

        FilteredFlags = new ObservableCollection<FastFlagEntryViewModel>(filtered);
    }

    private async Task SaveAsync()
    {
        if (!_profile.IsBuiltIn)
            await _profileService.SaveCustomProfileAsync(_profile);
    }
}
