using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace RKode.VERO {
public static class Utility {
    public static string GetDefaultFileName(BuildTarget target, string version) {
        BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
        string bundleId = PlayerSettings.GetApplicationIdentifier(group).ToLower();

        if (string.IsNullOrEmpty(bundleId))
            bundleId = "com.company.product";

        string cleanVersion = string.IsNullOrEmpty(version)
            ? "0.0.0"
            : version.Trim().Replace(" ", "_");

        return $"{bundleId}.v{cleanVersion}";
    }

    public static string GetDefaultExtension(BuildTarget target) {
        return target switch {
            BuildTarget.StandaloneWindows => "exe",
            BuildTarget.StandaloneWindows64 => "exe",
            BuildTarget.StandaloneOSX => "app",
#if UNITY_ANDROID
            BuildTarget.Android => EditorUserBuildSettings.buildAppBundle ? "aab" : "apk",
#endif
            _ => ""
        };
    }

    public static string ResolveBuildDirectory(BuildTarget target) {
        string defaultDir = "Builds";
        string previousPath = EditorUserBuildSettings.GetBuildLocation(target);

        if (string.IsNullOrEmpty(previousPath)) return defaultDir;

        try {
            string dir = Path.GetDirectoryName(previousPath);
            return string.IsNullOrEmpty(dir) ? defaultDir : dir;
        }
        catch { return defaultDir; }
    }

    public static BuildResult Build(BuildPlayerOptions options) {
        BuildReport report = BuildPipeline.BuildPlayer(options);

#if UNITY_EDITOR
        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log($"[CustomBuilderService] Build succeeded: {report.summary.outputPath}");
        else
            Debug.LogError($"[CustomBuilderService] Build failed: {report.summary.result}");
#endif

        return report.summary.result;
    }

    public static string BuildVersionString(string major, string minor, string patch, VersionSuffix suffixType, string suffixMeta) {
        var version = $"{major}.{minor}.{patch}";
        var hasSuffixMeta = !string.IsNullOrEmpty(suffixMeta);

        if(suffixType != VersionSuffix.none) {
            version += $"-{suffixType}";
            if(hasSuffixMeta) {
                version += $".{suffixMeta}";
            }
        }else if(hasSuffixMeta) {
            version += $"-{suffixMeta}";
        }

        return version;
    }

    public static string GetDefaultBuildPath() {
        var projectRoot = Directory.GetParent(Application.dataPath).FullName;
        var buildPath = Path.Combine(projectRoot, "Build");

        if(!Directory.Exists(buildPath)) {
            Directory.CreateDirectory(buildPath);
        }

        return buildPath;
    }

    public static bool TryParseVersionComponent(string input, out int numericPart, out string trailingSuffix) {
        numericPart = 0;
        trailingSuffix = "";

        if (string.IsNullOrEmpty(input)) return false;

        int i = 0;
        while (i < input.Length && char.IsDigit(input[i])) 
            i++;

        if (i == 0) return false; 

        numericPart = int.Parse(input.Substring(0, i));
        trailingSuffix = input.Substring(i);
        return true;
    }
}
}