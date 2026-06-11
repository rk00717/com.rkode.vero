#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RKode.VERO.Editor {
public static class SceneBuildManager {
    private static Vector2 _scrollPos;

    /// <summary>
    /// Renders the Scene Management UI. 
    /// </summary>
    public static void Draw(Rect windowRect) {
        // Fetch current build scenes directly from Unity's Global Config
        var scenes = EditorBuildSettings.scenes.ToList();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("BUILD SCENES", EditorStyles.boldLabel);
        EditorGUILayout.Space(2);

        // ScrollView with a flexible height
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.MaxHeight(200));

        if (scenes.Count == 0) {
            EditorGUILayout.HelpBox("No scenes in build settings.", MessageType.Info);
        }

        for (int i = 0; i < scenes.Count; i++) {
            DrawSceneRow(scenes, i);
        }

        EditorGUILayout.EndScrollView();

        // Footer Actions
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Active Scene", EditorStyles.miniButtonLeft)) {
            AddActiveScene(scenes);
        }
        
        // This allows adding ANY scene without opening Build Settings
        if (GUILayout.Button("Add Scene from Assets...", EditorStyles.miniButtonRight)) {
            ShowScenePicker();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private static void DrawSceneRow(List<EditorBuildSettingsScene> scenes, int index) {
        var scene = scenes[index];
        // Load the asset to show the nice Icon and Name
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);

        EditorGUILayout.BeginHorizontal();
        
        // Enabled Toggle
        bool toggled = EditorGUILayout.Toggle(scene.enabled, GUILayout.Width(20));
        if (toggled != scene.enabled) {
            scenes[index].enabled = toggled;
            Save(scenes);
        }

        // Scene Object Display (Read-only for path safety)
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField(sceneAsset, typeof(SceneAsset), false);
        EditorGUI.EndDisabledGroup();

        // Ordering Logic
        if (GUILayout.Button("▲", EditorStyles.miniButton, GUILayout.Width(22))) Move(scenes, index, -1);
        if (GUILayout.Button("▼", EditorStyles.miniButton, GUILayout.Width(22))) Move(scenes, index, 1);
        
        // Remove Logic
        GUI.color = new Color(1f, 0.4f, 0.4f);
        if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(22))) {
            scenes.RemoveAt(index);
            Save(scenes);
        }
        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();
    }

    private static void Move(List<EditorBuildSettingsScene> scenes, int index, int dir) {
        int target = index + dir;
        if (target < 0 || target >= scenes.Count) return;

        var item = scenes[index];
        scenes.RemoveAt(index);
        scenes.Insert(target, item);
        Save(scenes);
    }

    private static void AddActiveScene(List<EditorBuildSettingsScene> scenes) {
        string path = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
        if (string.IsNullOrEmpty(path) || scenes.Any(s => s.path == path)) return;

        scenes.Add(new EditorBuildSettingsScene(path, true));
        Save(scenes);
    }

    private static void ShowScenePicker() {
        // Opens the standard Unity Object Picker filtered to Scenes
        EditorGUIUtility.ShowObjectPicker<SceneAsset>(null, false, "", 0);
    }

    // Call this to handle the Object Picker selection
    public static void CheckPickerSelection() {
        if (Event.current.commandName == "ObjectSelectorUpdated") {
            var selected = EditorGUIUtility.GetObjectPickerObject() as SceneAsset;
            if (selected != null) {
                string path = AssetDatabase.GetAssetPath(selected);
                var scenes = EditorBuildSettings.scenes.ToList();
                if (!scenes.Any(s => s.path == path)) {
                    scenes.Add(new EditorBuildSettingsScene(path, true));
                    Save(scenes);
                }
            }
        }
    }

    private static void Save(List<EditorBuildSettingsScene> scenes) {
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
}
#endif