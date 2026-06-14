using Chibinator.Core.Models;

namespace Chibinator.Core.Services;

/// <summary>
/// All presets use ONLY Roblox-allowlisted FastFlags written to ClientAppSettings.json.
/// This is the same approach used by Bloxstrap and other legitimate launchers.
///
/// As of September 29, 2025, Roblox enforces a strict allowlist. Only the 18 flags
/// listed below are read by the client — all others are silently ignored.
///
/// NOTE: DFIntTaskSchedulerTargetFps is NOT on the allowlist and has been removed.
///       FPS is now controlled inside Roblox's own Settings menu (30/60/144/240).
/// </summary>
public static class PresetDatabase
{
    public static Profile GetUltraPotatoProfile() => new()
    {
        Id = "preset-ultra-potato",
        Name = "Ultra Potato Mode 🥔",
        Description = "Maximum performance. Every allowlisted flag pushed to its limit. Use Roblox Settings to set your FPS cap.",
        PresetType = PresetType.UltraPotato,
        IsBuiltIn = true,
        ClearCacheOnLaunch = true,
        Flags =
        [
            // ── SHADOWS ───────────────────────────────────────────
            new FastFlag
            {
                Key = "FIntRenderShadowIntensity",
                Value = "0",
                Description = "Disables shadow rendering intensity",
                Category = "Shadows",
                SafetyNote = "Safe. Removes shadows globally on client.",
                IsEnabled = true
            },

            // ── LIGHTING ──────────────────────────────────────────
            new FastFlag
            {
                Key = "DFFlagDebugRenderForceTechnologyVoxel",
                Value = "True",
                Description = "Forces Voxel (legacy) lighting — lowest rendering cost",
                Category = "Lighting",
                SafetyNote = "Safe. Voxel is Roblox's cheapest lighting mode.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FIntRenderLocalLightUpdatesMax",
                Value = "8",
                Description = "Max local light updates per frame",
                Category = "Lighting",
                SafetyNote = "Safe. Reduces light recalculation frequency.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FIntRenderLocalLightUpdatesMin",
                Value = "6",
                Description = "Min local light updates per frame",
                Category = "Lighting",
                SafetyNote = "Safe.",
                IsEnabled = true
            },

            // ── TEXTURES ──────────────────────────────────────────
            new FastFlag
            {
                Key = "DFFlagTextureQualityOverrideEnabled",
                Value = "True",
                Description = "Enables texture quality override",
                Category = "Textures",
                SafetyNote = "Safe. Required companion for DFIntTextureQualityOverride.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "DFIntTextureQualityOverride",
                Value = "1",
                Description = "Sets texture quality to minimum (1 = lowest)",
                Category = "Textures",
                SafetyNote = "Safe. Lower texture quality reduces VRAM and render time.",
                IsEnabled = true
            },

            // ── GRASS / TERRAIN ───────────────────────────────────
            new FastFlag
            {
                Key = "FIntFRMMinGrassDistance",
                Value = "0",
                Description = "Minimum grass render distance = 0",
                Category = "Terrain",
                SafetyNote = "Safe. Allowlisted. Removes grass rendering entirely.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FIntFRMMaxGrassDistance",
                Value = "0",
                Description = "Maximum grass render distance = 0",
                Category = "Terrain",
                SafetyNote = "Safe. Allowlisted. Removes grass rendering entirely.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FIntGrassMovementReducedMotionFactor",
                Value = "0",
                Description = "Disables grass movement animation",
                Category = "Terrain",
                SafetyNote = "Safe. Allowlisted. Eliminates grass motion cost.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FIntRenderGrassDetailStrands",
                Value = "0",
                Description = "Sets grass detail strands to 0",
                Category = "Terrain",
                SafetyNote = "Safe. Removes individual grass strands.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FIntRenderGrassHeightScaler",
                Value = "0",
                Description = "Sets grass height to 0",
                Category = "Terrain",
                SafetyNote = "Safe.",
                IsEnabled = true
            },

            // ── WIND ──────────────────────────────────────────────
            new FastFlag
            {
                Key = "FFlagGlobalWindRendering",
                Value = "False",
                Description = "Disables global wind rendering",
                Category = "Effects",
                SafetyNote = "Safe. Removes wind visual simulation.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FFlagGlobalWindActivated",
                Value = "False",
                Description = "Disables global wind activation",
                Category = "Effects",
                SafetyNote = "Safe. Companion to FFlagGlobalWindRendering.",
                IsEnabled = true
            },

            // ── MESH / LOD ────────────────────────────────────────
            new FastFlag
            {
                Key = "DFIntCSGLevelOfDetailSwitchingDistance",
                Value = "0",
                Description = "CSG LOD switching — forces lowest LOD immediately",
                Category = "Rendering",
                SafetyNote = "Safe. Allowlisted. Reduces mesh complexity at close distances.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "DFIntCSGLevelOfDetailSwitchingDistanceL12",
                Value = "0",
                Description = "CSG LOD L1→L2 switching distance",
                Category = "Rendering",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "DFIntCSGLevelOfDetailSwitchingDistanceL23",
                Value = "0",
                Description = "CSG LOD L2→L3 switching distance",
                Category = "Rendering",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "DFIntCSGLevelOfDetailSwitchingDistanceL34",
                Value = "0",
                Description = "CSG LOD L3→L4 switching distance",
                Category = "Rendering",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            },

            // ── QUALITY LEVEL ─────────────────────────────────────
            new FastFlag
            {
                Key = "DFIntDebugFRMQualityLevelOverride",
                Value = "1",
                Description = "Forces FRM quality level to minimum (1 = lowest)",
                Category = "Rendering",
                SafetyNote = "Safe. Allowlisted. 1 = absolute lowest rendering quality.",
                IsEnabled = true
            },

            // ── DISPLAY / RENDERING ───────────────────────────────
            new FastFlag
            {
                Key = "DFFlagDisableDPIScale",
                Value = "True",
                Description = "Disables DPI scaling — improves rendering clarity on high-res monitors",
                Category = "Display",
                SafetyNote = "Safe. Allowlisted. Removes OS-level DPI scaling overhead.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FIntDebugForceMSAASamples",
                Value = "-1",
                Description = "Disables MSAA anti-aliasing (-1 = off)",
                Category = "Display",
                SafetyNote = "Safe. Allowlisted. Removing MSAA reduces GPU load significantly.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FFlagDebugSkyGray",
                Value = "True",
                Description = "Replaces sky with flat grey — eliminates sky rendering cost",
                Category = "Display",
                SafetyNote = "Safe. Allowlisted. Visual tradeoff for maximum performance.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FFlagHandleAltEnterFullscreenManually",
                Value = "False",
                Description = "Uses Roblox's built-in fullscreen handling",
                Category = "Display",
                SafetyNote = "Safe. Allowlisted. Prevents window manager conflicts on Alt+Enter.",
                IsEnabled = true
            }
        ]
    };

    public static Profile GetCompetitiveProfile() => new()
    {
        Id = "preset-competitive",
        Name = "Competitive Mode ⚡",
        Description = "High performance with slight visual retention. Use Roblox Settings to set your FPS cap.",
        PresetType = PresetType.Competitive,
        IsBuiltIn = true,
        ClearCacheOnLaunch = false,
        Flags =
        [
            // ── SHADOWS ───────────────────────────────────────────
            new FastFlag
            {
                Key = "FIntRenderShadowIntensity",
                Value = "0",
                Description = "Disables shadow rendering",
                Category = "Shadows",
                SafetyNote = "Safe.",
                IsEnabled = true
            },

            // ── TEXTURES ──────────────────────────────────────────
            new FastFlag
            {
                Key = "DFFlagTextureQualityOverrideEnabled",
                Value = "True",
                Description = "Enables texture quality override",
                Category = "Textures",
                SafetyNote = "Safe.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "DFIntTextureQualityOverride",
                Value = "2",
                Description = "Texture quality level 2 (medium-low)",
                Category = "Textures",
                SafetyNote = "Safe.",
                IsEnabled = true
            },

            // ── GRASS / TERRAIN ───────────────────────────────────
            new FastFlag
            {
                Key = "FIntFRMMinGrassDistance",
                Value = "0",
                Description = "Minimum grass render distance = 0",
                Category = "Terrain",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FIntFRMMaxGrassDistance",
                Value = "0",
                Description = "Maximum grass render distance = 0",
                Category = "Terrain",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FIntGrassMovementReducedMotionFactor",
                Value = "0",
                Description = "Disables grass movement animation",
                Category = "Terrain",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            },

            // ── WIND ──────────────────────────────────────────────
            new FastFlag
            {
                Key = "FFlagGlobalWindRendering",
                Value = "False",
                Description = "Disables wind rendering",
                Category = "Effects",
                SafetyNote = "Safe.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FFlagGlobalWindActivated",
                Value = "False",
                Description = "Disables wind activation",
                Category = "Effects",
                SafetyNote = "Safe.",
                IsEnabled = true
            },

            // ── MESH / LOD ────────────────────────────────────────
            new FastFlag
            {
                Key = "DFIntCSGLevelOfDetailSwitchingDistance",
                Value = "0",
                Description = "CSG LOD switching — forces lowest LOD immediately",
                Category = "Rendering",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "DFIntCSGLevelOfDetailSwitchingDistanceL12",
                Value = "0",
                Description = "CSG LOD L1→L2 switching distance",
                Category = "Rendering",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "DFIntCSGLevelOfDetailSwitchingDistanceL23",
                Value = "0",
                Description = "CSG LOD L2→L3 switching distance",
                Category = "Rendering",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "DFIntCSGLevelOfDetailSwitchingDistanceL34",
                Value = "0",
                Description = "CSG LOD L3→L4 switching distance",
                Category = "Rendering",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            },

            // ── QUALITY LEVEL ─────────────────────────────────────
            new FastFlag
            {
                Key = "DFIntDebugFRMQualityLevelOverride",
                Value = "3",
                Description = "FRM quality level 3 — balanced/low",
                Category = "Rendering",
                SafetyNote = "Safe. Allowlisted. Moderate quality reduction.",
                IsEnabled = true
            },

            // ── DISPLAY / RENDERING ───────────────────────────────
            new FastFlag
            {
                Key = "DFFlagDisableDPIScale",
                Value = "True",
                Description = "Disables DPI scaling",
                Category = "Display",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FIntDebugForceMSAASamples",
                Value = "-1",
                Description = "Disables MSAA anti-aliasing (-1 = off)",
                Category = "Display",
                SafetyNote = "Safe. Allowlisted. Reduces GPU load.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FFlagHandleAltEnterFullscreenManually",
                Value = "False",
                Description = "Uses Roblox's built-in fullscreen handling",
                Category = "Display",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            }
        ]
    };

    public static Profile GetBalancedProfile() => new()
    {
        Id = "preset-balanced",
        Name = "Balanced Mode ⚖️",
        Description = "Light optimizations. Removes expensive effects while keeping visuals acceptable.",
        PresetType = PresetType.Balanced,
        IsBuiltIn = true,
        ClearCacheOnLaunch = false,
        Flags =
        [
            // ── SHADOWS ───────────────────────────────────────────
            new FastFlag
            {
                Key = "FIntRenderShadowIntensity",
                Value = "0",
                Description = "Disables shadow rendering",
                Category = "Shadows",
                SafetyNote = "Safe.",
                IsEnabled = true
            },

            // ── GRASS / TERRAIN ───────────────────────────────────
            new FastFlag
            {
                Key = "FIntGrassMovementReducedMotionFactor",
                Value = "0",
                Description = "Disables grass movement animation",
                Category = "Terrain",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            },

            // ── WIND ──────────────────────────────────────────────
            new FastFlag
            {
                Key = "FFlagGlobalWindRendering",
                Value = "False",
                Description = "Disables wind rendering",
                Category = "Effects",
                SafetyNote = "Safe.",
                IsEnabled = true
            },
            new FastFlag
            {
                Key = "FFlagGlobalWindActivated",
                Value = "False",
                Description = "Disables wind activation",
                Category = "Effects",
                SafetyNote = "Safe.",
                IsEnabled = true
            },

            // ── DISPLAY ───────────────────────────────────────────
            new FastFlag
            {
                Key = "FFlagHandleAltEnterFullscreenManually",
                Value = "False",
                Description = "Uses Roblox's built-in fullscreen handling",
                Category = "Display",
                SafetyNote = "Safe. Allowlisted.",
                IsEnabled = true
            }
        ]
    };

    public static Profile GetDefaultProfile() => new()
    {
        Id = "preset-default",
        Name = "Default (No Changes)",
        Description = "Writes an empty config. Roblox runs with its own default settings.",
        PresetType = PresetType.Default,
        IsBuiltIn = true,
        ClearCacheOnLaunch = false,
        Flags = []
    };

    public static List<Profile> GetAllBuiltInProfiles() =>
    [
        GetUltraPotatoProfile(),
        GetCompetitiveProfile(),
        GetBalancedProfile(),
        GetDefaultProfile()
    ];
}
