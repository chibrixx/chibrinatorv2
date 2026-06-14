# 🥔 Chibinator — Roblox Ultra Performance Launcher

A Windows desktop application for managing Roblox performance settings via **official configuration files only**.

> **How it works:** Chibinator writes to `ClientAppSettings.json` inside your Roblox version folder — the exact same mechanism used by Bloxstrap and other legitimate launchers. No binary modification. No process injection. No cheating. 100% safe.

---

## ✨ Features

| Feature | Details |
|---|---|
| 🥔 Ultra Potato Mode | Aggressively disables shadows, grass, wind, LOD — max FPS |
| ⚡ Competitive Mode | Balanced reduction, targets 144 FPS |
| ⚖️ Balanced Mode | Light optimizations, 60 FPS stable |
| 🔄 Default Mode | Writes empty config, pure Roblox defaults |
| 🧠 Smart Suggestion | Detects your CPU/GPU/RAM and recommends a preset |
| 📋 Profile System | Save, export, import custom profiles as JSON |
| 🚩 FastFlags Panel | Toggle individual flags per profile with search/filter |
| 💾 Auto Backup | Saves timestamped backup of config before every apply |
| 🧹 Cache Cleaner | Optional: clears Roblox log/temp cache on launch |
| 🎯 FPS Cap | 60 / 120 / 144 / 240 / Unlimited |

---

## 🏗️ Prerequisites

- Windows 10 or 11 (x64)
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) — for building
- Roblox installed (standard installation)
- Visual Studio 2022 or VS Code with C# extension (optional, for development)

---

## 🚀 Build Instructions

### Option A — Quick Build (Command Line)

```bat
git clone <your-repo-url>
cd Chibinator
build.bat
```

Output: `publish\Chibinator.exe` — single self-contained executable, no install needed.

### Option B — Visual Studio 2022

1. Open `Chibinator.sln`
2. Set configuration to **Release**, platform to **x64**
3. Right-click `Chibinator.UI` → **Set as Startup Project**
4. Press **F5** to run, or **Ctrl+Shift+B** to build
5. Output in `Chibinator.UI\bin\Release\net8.0-windows\`

### Option C — Manual dotnet CLI

```bat
# Restore
dotnet restore Chibinator.sln

# Run directly
dotnet run --project Chibinator.UI\Chibinator.UI.csproj -c Release

# Publish single-file EXE
dotnet publish Chibinator.UI\Chibinator.UI.csproj ^
  -c Release -r win-x64 --self-contained ^
  -p:PublishSingleFile=true ^
  -o publish\
```

---

## 📁 Project Structure

```
Chibinator/
├── Chibinator.sln                         # Solution file
├── build.bat                              # One-click build script
│
├── Chibinator.Core/                       # Business logic (no UI dependency)
│   ├── Models/
│   │   ├── FastFlag.cs                    # Flag data model
│   │   ├── Profile.cs                     # Profile + enums (PresetType, FpsCapMode)
│   │   └── AppSettings.cs                 # App + hardware settings models
│   ├── Services/
│   │   ├── AppSettingsService.cs          # Load/save settings.json
│   │   ├── FastFlagService.cs             # Read/write ClientAppSettings.json
│   │   ├── HardwareDetectionService.cs    # WMI CPU/GPU/RAM detection
│   │   ├── LaunchService.cs               # Apply config + start Roblox
│   │   ├── PresetDatabase.cs              # All built-in presets & flags
│   │   ├── ProfileService.cs              # Profile CRUD + import/export
│   │   └── RobloxDetectionService.cs      # Auto-detect Roblox install path
│   └── Helpers/
│       ├── ObservableObject.cs            # INotifyPropertyChanged base
│       └── RelayCommand.cs                # ICommand implementations
│
├── Chibinator.UI/                         # WPF application
│   ├── App.xaml / App.xaml.cs             # DI bootstrap, startup
│   ├── Assets/
│   │   └── Styles.xaml                    # Dark theme, all styles + converters
│   ├── Converters/
│   │   └── Converters.cs                  # All IValueConverter implementations
│   ├── ViewModels/
│   │   ├── MainViewModel.cs               # Central hub VM
│   │   ├── ProfilesViewModel.cs           # Profile list management
│   │   ├── FastFlagsViewModel.cs          # Per-flag toggle VM
│   │   └── SettingsViewModel.cs           # Settings panel VM
│   └── Views/
│       ├── MainWindow.xaml/.cs            # Shell: sidebar + panels + status bar
│       ├── DashboardView.xaml/.cs         # Home panel
│       ├── ProfilesView.xaml/.cs          # Profile list panel
│       ├── FastFlagsView.xaml/.cs         # Flag toggle panel
│       └── SettingsView.xaml/.cs          # Settings panel
│
└── Chibinator.Config/
    └── ExamplePresets.json                # Reference: what gets written to Roblox
```

---

## 📄 Data Locations

| Data | Location |
|---|---|
| App settings | `%LocalAppData%\Chibinator\settings.json` |
| Custom profiles | `%LocalAppData%\Chibinator\Profiles\*.json` |
| Config backups | `%LocalAppData%\Chibinator\Backups\` |
| **Roblox config (written)** | `%LocalAppData%\Roblox\Versions\version-XXXX\ClientSettings\ClientAppSettings.json` |

---

## 🚩 FastFlags Reference

All flags used by Chibinator are from Roblox's publicly known client allowlist.

| Flag | Effect | Category |
|---|---|---|
| `FIntRenderShadowIntensity` = 0 | Disables shadow rendering | Shadows |
| `DFFlagDebugRenderForceTechnologyVoxel` = True | Forces cheapest lighting mode | Lighting |
| `DFFlagTextureQualityOverrideEnabled` = True | Enables texture override | Textures |
| `DFIntTextureQualityOverride` = 1 | Sets texture quality to minimum | Textures |
| `FIntFRMMinGrassDistance` = 0 | Removes grass (min distance) | Terrain |
| `FIntFRMMaxGrassDistance` = 0 | Removes grass (max distance) | Terrain |
| `FIntRenderGrassDetailStrands` = 0 | Removes grass strands | Terrain |
| `FIntRenderGrassHeightScaler` = 0 | Removes grass height | Terrain |
| `FFlagGlobalWindRendering` = False | Disables wind rendering | Effects |
| `FFlagGlobalWindActivated` = False | Disables wind simulation | Effects |
| `FIntRenderLocalLightUpdatesMax` = 8 | Reduces local light recalculation | Lighting |
| `FIntRenderLocalLightUpdatesMin` = 6 | Reduces local light recalculation | Lighting |
| `DFIntCSGLevelOfDetailSwitchingDistance*` = 0 | Forces lowest mesh LOD | Rendering |
| `DFIntDebugFRMQualityLevelOverride` = 1 | Forces minimum FRM quality | Rendering |
| `DFIntTaskSchedulerTargetFps` = N | Sets FPS cap (0 = unlimited) | FPS |

---

## ⚠️ What Chibinator Does NOT Do

- ❌ Modify Roblox `.exe` or any binary files
- ❌ Inject into running Roblox processes
- ❌ Implement cheats, exploits, or memory manipulation
- ❌ Modify game scripts or client code
- ❌ Use undocumented or internal-only flags

---

## 🔧 Extending Chibinator

To add new flags as Roblox exposes them:

1. Open `Chibinator.Core/Services/PresetDatabase.cs`
2. Add a new `FastFlag` entry to any preset's `Flags` list
3. Set `Key`, `Value`, `Description`, `Category`, and `SafetyNote`
4. Rebuild — the flag appears automatically in the FastFlags panel

---

## 📦 Adding Chibinator.Config as Embedded Resource (Optional)

If you want `ExamplePresets.json` bundled into the EXE, add to `Chibinator.UI.csproj`:

```xml
<ItemGroup>
  <EmbeddedResource Include="..\Chibinator.Config\ExamplePresets.json" LogicalName="ExamplePresets.json"/>
</ItemGroup>
```

---

## 🛠️ Troubleshooting

**"Roblox not detected"**
→ Go to Settings → Browse for `RobloxPlayerBeta.exe` manually.
→ Typical path: `%LocalAppData%\Roblox\Versions\version-XXXX\RobloxPlayerBeta.exe`

**"Failed to write ClientAppSettings.json"**
→ Run Chibinator as Administrator once to set permissions.
→ Or manually create: `%LocalAppData%\Roblox\Versions\version-XXXX\ClientSettings\`

**Roblox updated and flags reset**
→ This is normal — Roblox updates overwrite the Versions folder.
→ Just click "Apply & Launch" again; Chibinator will rewrite the config.

**Profile not saving**
→ Check `%LocalAppData%\Chibinator\Profiles\` exists and is writable.

---

## 📜 License

MIT License — free to use, modify, and distribute.
