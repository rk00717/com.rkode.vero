# Changelog
All notable changes to `com.rkode.vero` will be documented here.

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).  
Versioning follows [RKode Software Versioning Policy](https://github.com/rk00717).

---

## [0.1.0-alpha] — 2026-06-04

### Added

**Core**
- `Controller` — build logic, version parsing, `BuildPlayerOptions` management
- `Controller.RequestBuild()` — full build pipeline with validation, version apply, filename resolution
- `Controller.AutoIncrementPatch()` — auto-bumps patch after every successful build
- `Controller.BumpMinor()` / `Controller.BumpMajor()` — manual version bump helpers
- `Controller.GetNextVersionPreview()` — shows next version before committing to build
- `Controller.SyncBundleIdFromProjectSettings()` — derives bundle ID from company + product name
- `Controller.CanAutoIncrement()` — validates whether auto-increment can run, returns reason if not
- Scene validation — blocks builds on missing scenes, duplicate scenes, empty scene list
- Unsaved scene detection — prompts user before building with dirty scenes
- Build awareness — warns when company name is `DefaultCompany` or bundle ID is Unity default
- Platform-conditional fields — Android version code, iOS build number appear per active platform

**Version System**
- `VersionSuffix` enum — `none`, `alpha`, `beta`, `rc`, `f`, `dev`, `rp`, `p`, `hotfix`, `snapshot`
- `Utility.BuildVersionString()` — produces `MAJOR.MINOR.PATCH` or `MAJOR.MINOR.PATCH-suffix.meta`
- `Utility.TryParseVersionComponent()` — parses version fields with trailing suffix support
- `SetBundleVersion(string)` — full version string parser with fallback to `0.1.0`

**UI**
- `EditorWindow` — main IMGUI window, tab orchestration, EditorPrefs serialization
- `BuildTab` — summary table, build awareness warnings, auto-increment toggle, Build/Cancel buttons, Open Output Folder shortcut
- `ConfigTab` — version fields, company/product name sync, suffix selector, filename override, path browser
- `TabBase` / `TabContext` — shared abstract tab architecture
- `Palette` — HSV-based dynamic color theming from single tint color
- `Style` — all GUIStyle definitions, rebuilt on window focus and tint change
- `GUILibrary` — reusable IMGUI components (table rows, dividers, version fields)
- `Preferences` — Unity Preferences panel integration for build path and tint color
- `Constant` — EditorPrefs keys and default path definitions

**Infrastructure**
- `Resolver` — isolated Editor script that auto-registers OpenUPM scoped registry on package import
- `rkode.vero.Resolver.asmdef` — zero-dependency assembly definition for Resolver isolation

**Docs**
- `README.md` — full feature documentation, installation instructions, architecture overview
- `CHANGELOG.md` — this file
- `LICENSE` — MIT

**Chore**
- `.gitignore` — Unity package standard ignore rules

### Known Issues
- Scene tab (`SceneTab`, `SceneBuildManager`) excluded from this release — missing `#if UNITY_EDITOR` guard and namespace, would break builds
- `SceneBuildManager` has missing editor guard — targeted for v0.2

### Compatibility
- Unity 2021.3 LTS and above
- Requires `com.rkode.utils` 0.4.0
- Requires OpenUPM scoped registry for dependency resolution