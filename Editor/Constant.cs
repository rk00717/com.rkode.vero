using System.IO;
using UnityEditor;
using UnityEngine;

namespace RKode.VERO {
public static class Constant {
    public static readonly string settingsPath = "ProjectSettings/VEROSettings.json";

    // Editor Prefs Key
    public const string tintPrefKey = "RKode.VERO.TintColor";
    public const string buildPathKey = "RKode.VERO.BuildPath";

    public const string preferencePath = "Preferences/RKode/VERO";

    public static readonly string defaultTint = "#9B8EC4FF";

    // [MenuItem("RKode/Reset Prefs")]
    // public static void ResetPrefs() {
    //     EditorPrefs.DeleteKey(buildPathKey);
    // }
}
}
