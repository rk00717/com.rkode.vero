<div align="center">

# VERO
### Versioning Extension & Release Optimizer
#### `com.rkode.vero` · v0.1.0-alpha · Unity 2021.3+

**A Unity Editor extension that enforces release discipline.**  
Stop forgetting to version your builds.

[![Unity](https://img.shields.io/badge/Unity-2021.3+-0a0c08?style=flat-square&logo=unity&logoColor=c9933a)](https://unity.com)
[![Status](https://img.shields.io/badge/Status-Alpha-0a0c08?style=flat-square&logoColor=c9933a)]()
[![Version](https://img.shields.io/badge/Version-0.1.0--alpha-0a0c08?style=flat-square&logoColor=c9933a)]()

</div>

---

## The Problem

Every solo developer does this:

1. Build the game
2. Forget to bump the version
3. Output file is named `Build` or `MyGame` with no version
4. Three builds later — which file is which?

VERO solves it by making versioning and building the same action.

---

## What It Does

- **Auto-generates filenames** from your Bundle ID + version — `com.rkode.games.project.v0.1.0`
- **Remembers your build path** across sessions via EditorPrefs — no re-navigating every build
- **One-click build trigger** directly from the VERO window — no touching Player Settings or Build Settings
- **Auto-increments patch** after every successful build — version stays current automatically
- **Platform-aware fields** — Android version code and iOS build number appear only when relevant
- **Build awareness warnings** — flags default company name and default bundle ID before you ship
- **Scene validation** — blocks builds on missing scenes, duplicate scenes, empty scene list, warns on unsaved changes
- **Post-build shortcut** — "Open Output Folder" button appears immediately after a successful build
- **Custom IMGUI theme** with dynamic HSV tint — set your color in Preferences

---

## Tabs

### Build
Summary view before you commit. Shows active platform, version, resolved filename, output path, scene count, and next version preview. Auto-increment toggle. Build and Cancel buttons. Warns on default Company Name or Bundle ID.

### Config
Version fields: Major / Minor / Patch. Suffix selector: `alpha`, `beta`, `rc`, `f`, `dev`, `rp`, `p`, `hotfix`, `snapshot`. Company name and product name sync directly to Player Settings. Filename preview with manual override. Output path with Browse dialog and persistent memory.

> ⚠️ **Scene tab is under active development** and is not included in this release.

---

## Installation

### Step 1 — Add the RKode scoped registry

In your project's `Packages/manifest.json`, add the scoped registry block:

```json
{
  "scopedRegistries": [
    {
      "name": "OpenUPM",
      "url": "https://package.openupm.com",
      "scopes": [ "com.rkode" ]
    }
  ],
  "dependencies": {
    "..."
  }
}
```

This only needs to be done once per project. All `com.rkode` packages will resolve automatically after this.

### Step 2 — Install RKode Utility (dependency)

`Window → Package Manager → + → Add package by name`
```
com.rkode.utils
```

### Step 3 — Install V.E.R.O.

`Window → Package Manager → + → Add package by name`
```
com.rkode.vero
```

Or via git URL:
```
https://github.com/rk00717/com.rkode.vero.git
```

### Open the Window

`Tools → RKode → VERO`

### Set Your Tint Color (Optional)

`Edit → Preferences → RKode → VERO` — pick a tint color to theme the window to your project.

---

## Version String Format

```
MAJOR.MINOR.PATCH
MAJOR.MINOR.PATCH-suffixType
MAJOR.MINOR.PATCH-suffixType.suffixMeta
```

Examples: `0.1.0`, `1.0.0-beta`, `1.2.0-rc.2`

---

## Architecture

```
EditorWindow.cs          ← Main window, tab orchestration, serialization
├── TabBase.cs           ← Abstract tab base
├── TabContext.cs        ← Shared state (Controller, Style, GUILibrary, Rect)
├── BuildTab.cs          ← Summary + build awareness + auto-increment + build trigger
└── ConfigTab.cs         ← Version fields, project settings sync, filename, path

Controller.cs            ← Build logic, version parsing, scene validation, auto-increment
Utility.cs               ← Version string building, file extension resolution, build wrapper
Palette.cs               ← HSV-based dynamic color theming
Style.cs                 ← All GUIStyle definitions
GUILibrary.cs            ← Reusable IMGUI components
Preferences.cs           ← Unity Preferences panel integration
Constant.cs              ← EditorPrefs keys, default paths
VersionSuffix.cs         ← Suffix enum

Resolver/
  RKodeResolver.cs       ← Auto-registers OpenUPM scoped registry on package import
  rkode.vero.Resolver.asmdef
```

**Controller events:**
```csharp
event Action onBuildSucceeded;
event Action onBuildFailed;
event Action onBuildCancelled;
event Action<string> onValidationFailed;
```

---

## Known Issues (v0.1)

These are tracked and targeted for v0.2:

- `SceneBuildManager` missing `#if UNITY_EDITOR` guard and namespace — Scene tab excluded from this release
- Version string in ConfigTab may not reflect suffix correctly in all edge cases

> v0.1 is early access. v0.2 will be the first fully stable public release.

---

## Roadmap

| Version | Focus |
|---|---|
| **v0.2** | Scene tab, build profile presets, stability pass |
| v0.3 | Build history log, changelog per build, build report summary |
| v0.4 | Git tag on build, dirty workspace warning, version sync from git |
| v0.5 | Build profiles, per-machine vs per-repo settings, scripting defines toggle |
| v1.0 | Pre/post build hooks, Android keystore manager, first-launch wizard |

---

## Dependencies

| Package | Version |
|---|---|
| `com.rkode.utils` | 0.4.0 |

---

## Part of the RKode Ecosystem

| Package | Description | Status |
|---|---|---|
| [`com.rkode.utils`](https://github.com/rk00717/com.rkode.utils) | Foundation utilities | v0.4.0 |
| **`com.rkode.vero`** | This package — build versioning | v0.1.0-alpha |
| [`com.rkode.glad`](https://github.com/rk00717/com.rkode.glad) | Grid-based level design editor | v0.1.0-alpha |
| [`com.rkode.drip`](https://github.com/rk00717/com.rkode.drip) | Scene object placement tool | v0.1.0-alpha |

---

<div align="center">
<sub>Built by <a href="https://ronik.dev">RKode Studio</a> · <a href="https://github.com/rk00717">rk00717</a></sub>
</div>