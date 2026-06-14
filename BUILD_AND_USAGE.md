# Chibinator — Complete Build & Usage Guide

---

## STEP 1 — Prerequisites

You need two things installed on your Windows machine.

### 1a. .NET 8 SDK

Download and install from:
https://dotnet.microsoft.com/en-us/download/dotnet/8.0

Pick: **.NET 8.0 SDK** → **Windows** → **x64 Installer**

After installing, open a new Command Prompt and verify:
```
dotnet --version
```
You should see `8.0.x`. If you do, you're good.

### 1b. Git (to clone the repo)

Download from: https://git-scm.com/download/win  
Install with defaults. No special options needed.

---

## STEP 2 — Get the source code

Open Command Prompt or PowerShell anywhere and run:

```
git clone https://github.com/chibrixx/chibrinatorv2.git
cd chibrinatorv2
```

---

## STEP 3 — Replace the updated files

You have 8 new files from the update. Copy each one into its correct location inside the cloned folder:

| File you downloaded | Replace this file |
|---|---|
| `Profile.cs` | `chibrinatorv2\Chibinator.Core\Models\Profile.cs` |
| `FastFlagService.cs` | `chibrinatorv2\Chibinator.Core\Services\FastFlagService.cs` |
| `PresetDatabase.cs` | `chibrinatorv2\Chibinator.Core\Services\PresetDatabase.cs` |
| `MainViewModel.cs` | `chibrinatorv2\Chibinator.UI\ViewModels\MainViewModel.cs` |
| `Converters.cs` | `chibrinatorv2\Chibinator.UI\Converters\Converters.cs` |
| `DashboardView.xaml` | `chibrinatorv2\Chibinator.UI\Views\DashboardView.xaml` |
| `MainWindow.xaml` | `chibrinatorv2\Chibinator.UI\Views\MainWindow.xaml` |
| `ProfilesView.xaml` | `chibrinatorv2\Chibinator.UI\Views\ProfilesView.xaml` |

Just overwrite — every other file in the repo stays exactly as-is.

---

## STEP 4 — Build

From inside the `chibrinatorv2` folder, double-click **build.bat**  
OR run it from Command Prompt:

```
build.bat
```

The script does three things automatically:
1. Restores NuGet packages
2. Builds the solution in Release/x64
3. Publishes a single self-contained .exe to `publish\Chibinator.exe`

If it succeeds you'll see:
```
============================================
  BUILD COMPLETE
  Output: publish\Chibinator.exe
============================================
```

If you prefer the CLI manually:
```
dotnet restore Chibinator.sln
dotnet publish Chibinator.UI\Chibinator.UI.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish\
```

---

## STEP 5 — Run it

Navigate to `chibrinatorv2\publish\` and double-click **Chibinator.exe**.

No installer. No .NET runtime needed on the target machine (it's self-contained).  
You can copy `Chibinator.exe` anywhere you want — it's fully portable.

---

## STEP 6 — First-time setup in the app

**If Roblox is detected automatically:**  
The status bar at the bottom will show a green dot and "Roblox detected ✔".  
You're ready — skip to step 7.

**If Roblox is NOT detected:**  
1. Click **⚙️ Settings** in the sidebar
2. Click **Browse** next to "Executable Path"
3. Navigate to your Roblox install and select `RobloxPlayerBeta.exe`  
   Typical path: `C:\Users\YourName\AppData\Local\Roblox\Versions\version-XXXX\RobloxPlayerBeta.exe`
4. Click **Re-detect**
5. Click **Save Settings**

---

## STEP 7 — Using the app

### Picking a preset

From the **Dashboard**, the four preset cards show what each mode does.  
Click **📋 Profiles** to see all profiles and click any one to activate it.

| Preset | What it does |
|---|---|
| 🥔 Ultra Potato | Every allowlisted flag maxed for performance. Grey sky, no grass, no shadows, no AA, lowest LOD and textures. |
| ⚡ Competitive | Strong performance reduction while keeping some visual quality. No shadows, no grass, reduced textures and LOD. |
| ⚖️ Balanced | Light touch. Removes shadows, wind, and grass movement only. |
| 🔄 Default | Writes an empty config — pure Roblox defaults, nothing changed. |

### FPS cap

FPS is **no longer set by Chibinator**. Roblox removed `DFIntTaskSchedulerTargetFps` from the allowlist in September 2025 — writing it does nothing.

Set your FPS cap **inside Roblox** instead:
1. Launch any game
2. Press **Escape** → **Settings** (☰ icon)
3. Find **Graphics** → **Frame Rate Cap**
4. Choose: 30 / 60 / 144 / 240

### Applying and launching

- **Apply & Launch Roblox** — writes the config then starts Roblox immediately
- **Apply Only** — writes the config without launching (useful if Roblox is already open)

The config is written to:
```
%LocalAppData%\Roblox\Versions\version-XXXX\ClientSettings\ClientAppSettings.json
```

### FastFlags panel

Click **🚩 FastFlags** to see every individual flag in the active profile.  
You can toggle flags on/off. Changes save automatically for custom profiles.  
Built-in profiles are read-only — clone one first via "＋ New from Active".

### Creating a custom profile

1. Activate the preset you want to start from
2. Go to **📋 Profiles** → click **＋ New from Active**
3. Type a name → **Save**
4. Go to **🚩 FastFlags** — now you can toggle individual flags for your custom profile

---

## Troubleshooting

**"Roblox not detected"**  
→ Set the path manually in Settings (see Step 6 above).

**"Failed to write ClientAppSettings.json"**  
→ Right-click Chibinator.exe → Run as Administrator (once).  
→ Or manually create the folder: `%LocalAppData%\Roblox\Versions\version-XXXX\ClientSettings\`

**Roblox updated and nothing applies**  
→ Normal. Roblox updates overwrite the Versions folder with a new version-XXXX folder.  
→ Just click Re-detect in Settings, then Apply & Launch again.

**Build fails with "SDK not found"**  
→ Make sure you installed the .NET 8 **SDK** (not just the Runtime) and opened a fresh Command Prompt after installing.

**Build fails with "platform not supported"**  
→ You must be on Windows x64. This is a WPF app — Windows only.

---

## What the allowlist means for you

As of September 29, 2025, Roblox enforces a strict allowlist of exactly 18 FastFlags.  
Any flag not on that list is **silently ignored** — it's written to the file but Roblox never reads it.

All flags Chibinator writes are on the allowlist. Nothing is wasted. The full list of what actually works:

```
DFIntCSGLevelOfDetailSwitchingDistance       ← LOD distance (geometry)
DFIntCSGLevelOfDetailSwitchingDistanceL12    ← LOD L1→L2
DFIntCSGLevelOfDetailSwitchingDistanceL23    ← LOD L2→L3
DFIntCSGLevelOfDetailSwitchingDistanceL34    ← LOD L3→L4
FFlagHandleAltEnterFullscreenManually        ← fullscreen handling
DFFlagTextureQualityOverrideEnabled          ← enables texture override
DFIntTextureQualityOverride                  ← texture quality level
FIntDebugForceMSAASamples                    ← MSAA anti-aliasing (-1 = off)
DFFlagDisableDPIScale                        ← disables DPI scaling
FFlagDebugGraphicsPreferD3D11                ← force DirectX 11
FFlagDebugSkyGray                            ← flat grey sky
DFFlagDebugPauseVoxelizer                    ← pauses voxel updates
DFIntDebugFRMQualityLevelOverride            ← FRM quality level
FIntFRMMaxGrassDistance                      ← max grass render distance
FIntFRMMinGrassDistance                      ← min grass render distance
FFlagDebugGraphicsPreferVulkan               ← force Vulkan renderer
FFlagDebugGraphicsPreferOpenGL               ← force OpenGL renderer
FIntGrassMovementReducedMotionFactor         ← grass movement animation
```

The three renderer flags (D3D11 / Vulkan / OpenGL) are mutually exclusive — only set one at a time. They appear in the FastFlags panel if you want to try them on a custom profile.
