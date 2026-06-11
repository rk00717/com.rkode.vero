#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using ColorUtility = RKode.Utils.ColorUtility;

namespace RKode.VERO.Editor {
public static class Preferences {
    [SettingsProvider]
    public static SettingsProvider CreateProvider() {
        return new SettingsProvider(Constant.preferencePath, SettingsScope.User) {
            label = "VERO",
            guiHandler = _ => {
                DrawBuildSettings();
                DrawAppearanceSettings();
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Reset to Default", GUILayout.Width(140))) {
                    OnClickReset();
                }
                EditorGUILayout.EndHorizontal();
            },
            keywords = new System.Collections.Generic.HashSet<string> { "tint", "theme", "color", "vero" }
        };
    }

    private static void DrawBuildSettings() {
        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("Build Settings", EditorStyles.boldLabel);
        string buildPath = EditorPrefs.GetString(Constant.buildPathKey, Utility.GetDefaultBuildPath());

        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        string newFilePath = EditorGUILayout.TextField("Build File Path", buildPath);

        if(GUILayout.Button("Browse...", GUILayout.Width(80))) {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Build Folder", buildPath, "");
            if(!string.IsNullOrEmpty(selectedPath)) {
                newFilePath = selectedPath;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck()) {
            EditorPrefs.SetString(Constant.buildPathKey, newFilePath);
        }
    }

    private static void DrawAppearanceSettings() {
        EditorGUILayout.Space(4f);
        EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);
        EditorGUILayout.Space(2f);

        string hex = EditorPrefs.GetString(Constant.tintPrefKey, Constant.defaultTint.ToString());
        Color current = ColorUtility.HexToColor(hex);

        EditorGUI.BeginChangeCheck();
        Color picked = EditorGUILayout.ColorField("Theme Tint", current);
        if (EditorGUI.EndChangeCheck()) {
            EditorPrefs.SetString(Constant.tintPrefKey, "#" + UnityEngine.ColorUtility.ToHtmlStringRGBA(picked));
        }

        EditorGUILayout.Space();
    }

    private static void OnClickReset() {
        EditorPrefs.SetString(Constant.tintPrefKey, Constant.defaultTint);
        EditorPrefs.SetString(Constant.buildPathKey, Utility.GetDefaultBuildPath());
    }
}
}
#endif