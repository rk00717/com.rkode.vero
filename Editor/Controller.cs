// V.E.R.O. => Versioning Extension & Release Optimizer

#if UNITY_EDITOR
using System;
using System.IO;

using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using System.Linq;
using System.Collections.Generic;

namespace RKode.VERO
{
    public class Controller
    {
        private BuildPlayerOptions _options;
        public BuildPlayerOptions Options => _options;

        public string Major { private set; get; }
        public string Minor { private set; get; }
        public string Patch { private set; get; }
        public VersionSuffix SuffixType { private set; get; }
        public string SuffixMeta { private set; get; }

        public string BundleVersion => Utility.BuildVersionString(Major, Minor, Patch, SuffixType, SuffixMeta);

#if UNITY_ANDROID
    private int _androidVersionCode;
    public int AndroidVersionCode => _androidVersionCode;
#endif

#if UNITY_IOS
    private string _iOSBuildNumber;
    public string IOSBuildNumber => _iOSBuildNumber;
#endif

        private string _buildFileName;
        public string BuildFileName => _buildFileName;
        public string BuildPath { private set; get; }

        public bool UseDefaultIdentifier { private set; get; } = true;
        public bool AutoIncrementEnabled { private set; get; } = true;

        public event Action onBuildSucceeded;
        public event Action onBuildFailed;
        public event Action onBuildCancelled;
        public event Action<string> onValidationFailed;

        public Controller(BuildPlayerOptions options)
        {
            _options = options;

            SetBundleVersion(PlayerSettings.bundleVersion);
            _buildFileName = Utility.GetDefaultFileName(options.target, BundleVersion);

            LoadSettings();

#if UNITY_ANDROID
        _androidVersionCode = PlayerSettings.Android.bundleVersionCode;
#endif

#if UNITY_IOS
        _iOSBuildNumber = PlayerSettings.iOS.buildNumber;
#endif
        }

        public void LoadSettings()
        {
            BuildPath = EditorPrefs.GetString(Constant.buildPathKey, Utility.GetDefaultBuildPath());
        }

        #region Public API

        public void SetBundleVersion(string major, string minor, string patch, VersionSuffix suffixType = VersionSuffix.none, string suffixMeta = "")
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            SuffixType = suffixType;
            SuffixMeta = suffixMeta;
        }

        public void SetBundleVersion(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                Debug.Log($"[VERO] Invalid version string format: '{version}'. Defaulting to 0.1.0");
                ApplyDefaultVersion();
                return;
            }

            string cleanedVersion = version.Trim().ToLower();
            if (cleanedVersion.StartsWith("v"))
                cleanedVersion = cleanedVersion.Substring(1);

            var mainParts = cleanedVersion.Trim().Split('-', 2);
            var versionCodes = mainParts[0].Split('.');

            if (versionCodes.Length >= 3)
            {
                var suffix = VersionSuffix.none;
                var suffixMeta = "";

                if (mainParts.Length > 1)
                {
                    var suffixParts = mainParts[1].Split('.', '-');
                    if (!Enum.TryParse(suffixParts[0], true, out suffix))
                    {
                        Debug.LogWarning($"[VERO] Suffix '{suffixParts[0]}' not recognized. Setting to 'none'.");
                    }
                    else
                    {
                        if (suffixParts.Length > 1)
                        {
                            suffixMeta = string.Join(".", suffixParts, 1, suffixParts.Length - 1);
                        }
                    }
                }

                SetBundleVersion(versionCodes[0], versionCodes[1], versionCodes[2], suffix, suffixMeta);
            }
            else
            {
                Debug.Log($"[VERO] Invalid version string format: '{version}'. Defaulting to 0.1.0");
                ApplyDefaultVersion();
            }
        }

#if UNITY_ANDROID
    public void SetAndroidVersionCode(int value) =>
        _androidVersionCode = value;
#endif
#if UNITY_IOS
    public void SetIOSBuildNumber(string value) =>
        _iOSBuildNumber = value;
#endif

        public void SetBuildFileName(string value) =>
            _buildFileName = value;

        public void SetBuildPath(string value)
        {
            BuildPath = value;
            EditorPrefs.SetString(Constant.buildPathKey, value);
        }

        public void SetAutoIncrement(bool value) =>
            AutoIncrementEnabled = value;

        #endregion

        public void SyncBundleIdFromProjectSettings()
        {
            BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(_options.target);
            string company = SanitizeBundleSegment(PlayerSettings.companyName);
            string product = SanitizeBundleSegment(PlayerSettings.productName);

            if (string.IsNullOrEmpty(company)) company = "company";
            if (string.IsNullOrEmpty(product)) product = "product";

            string derived = $"com.{company}.{product}";
            PlayerSettings.SetApplicationIdentifier(group, derived);
            RefreshFileName();
        }

        private static string SanitizeBundleSegment(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            // Lowercase, replace spaces/hyphens/underscores with nothing, strip invalid chars
            var sb = new System.Text.StringBuilder();
            foreach (char c in input.ToLower())
            {
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                // everything else (spaces, hyphens, underscores) is dropped
            }
            return sb.ToString();
        }

        public void RefreshFileName() =>
            _buildFileName = Utility.GetDefaultFileName(_options.target, BundleVersion);

        private void ApplyDefaultVersion() =>
            SetBundleVersion("0", "1", "0");

        private void ApplyVersionSettings()
        {
            PlayerSettings.bundleVersion = BundleVersion;

#if UNITY_ANDROID
        PlayerSettings.Android.bundleVersionCode = _androidVersionCode;
#endif
#if UNITY_IOS
        PlayerSettings.iOS.buildNumber = _iOSBuildNumber;
#endif
        }

        public void RequestBuild()
        {
            string error = Validate();
            if (!string.IsNullOrEmpty(error))
            {
                onValidationFailed?.Invoke(error);
                return;
            }

            ApplyVersionSettings();

            string ext = Utility.GetDefaultExtension(_options.target);
            string fileName = string.IsNullOrEmpty(ext) ? _buildFileName : $"{_buildFileName}.{ext}";

            _options.locationPathName = Path.Combine(BuildPath, fileName);
            EditorUserBuildSettings.SetBuildLocation(_options.target, _options.locationPathName);

            var result = Utility.Build(_options);
            if (result == BuildResult.Succeeded)
            {
                AutoIncrementPatch();
                onBuildSucceeded?.Invoke();
            }
            else
            {
                onBuildFailed?.Invoke();
            }
        }

        public bool IsValidBuildPath()
        {
            return !string.IsNullOrEmpty(BuildPath) && Directory.Exists(BuildPath);
        }

        public void ResetBuildPath()
        {
            SetBuildPath(Utility.GetDefaultBuildPath());
        }

        #region Auto Increment API

        public bool CanAutoIncrement(out string reason)
        {
            if (!AutoIncrementEnabled)
            {
                reason = null;
                return false;
            }

            if (string.IsNullOrEmpty(Patch))
            {
                reason = "Patch is empty.";
                return false;
            }

            if (!Utility.TryParseVersionComponent(Patch, out _, out _))
            {
                reason = $"Patch \"{Patch}\" has no leading digit — cannot auto-increment.";
                return false;
            }

            reason = null;
            return true;
        }

        public void AutoIncrementPatch()
        {
            if (!AutoIncrementEnabled) return;

            Utility.TryParseVersionComponent(Patch, out int numericPart, out string trailing);
            SetBundleVersion(Major, Minor, $"{numericPart + 1}{trailing}");
            RefreshFileName();
        }

        public void BumpMinor()
        {
            if (int.TryParse(Minor, out int minor))
            {
                SetBundleVersion(Major, (minor + 1).ToString(), "0", SuffixType, SuffixMeta);
                RefreshFileName();
            }
        }

        public void BumpMajor()
        {
            if (int.TryParse(Major, out int major))
            {
                SetBundleVersion((major + 1).ToString(), "0", "0", SuffixType, SuffixMeta);
                RefreshFileName();
            }
        }

        public string GetNextVersionPreview()
        {
            if (!AutoIncrementEnabled)
                return BundleVersion;

            if (!int.TryParse(Patch, out int patch))
                return BundleVersion;

            string nextPatch = (patch + 1).ToString();
            return Utility.BuildVersionString(Major, Minor, nextPatch, SuffixType, SuffixMeta);
        }

        #endregion

        private string Validate()
        {
            if (string.IsNullOrEmpty(BundleVersion))
                return "bundleVersion cannot be empty.";

            if (string.IsNullOrEmpty(_buildFileName))
                return "Build file name cannot be empty.";

            if (string.IsNullOrEmpty(BuildPath))
                return "Build path cannot be empty.";

            if (!Directory.Exists(BuildPath))
                return $"Build path does not exist:\n{BuildPath}";

#if UNITY_ANDROID
        if (_androidVersionCode <= 0)
            return "Android version code must be a positive integer.";
#endif
#if UNITY_IOS
        if (string.IsNullOrEmpty(_iOSBuildNumber))
            return "iOS build number cannot be empty.";
#endif

            return ValidateScenes();
        }

        private string ValidateScenes()
        {
            var scenes = EditorBuildSettings.scenes;
            var enabled = scenes.Where(s => s.enabled).ToArray();

            if (enabled.Length == 0)
                return "No scenes are enabled in Build Settings.";

            var missing = enabled.Where(s => !File.Exists(s.path)).ToArray();
            if (missing.Length > 0)
                return $"Scene file not found on disk:\n{missing[0].path}";

            var duplicates = enabled
                .GroupBy(s => s.path)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToArray();
            if (duplicates.Length > 0)
                return $"Duplicate scene in build settings:\n{duplicates[0]}";

            var dirtyScenes = new List<string>();
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.isDirty)
                    dirtyScenes.Add(scene.name);
            }

            if (dirtyScenes.Count > 0)
            {
                bool proceed = EditorUtility.DisplayDialog(
                    "Unsaved Scenes",
                    $"The following scenes have unsaved changes:\n• {string.Join("\n• ", dirtyScenes)}\n\nProceed anyway?",
                    "Build Anyway",
                    "Cancel"
                );
                if (!proceed) return "Build cancelled — unsaved scenes.";
            }

            return null;
        }
    }
}
#endif
