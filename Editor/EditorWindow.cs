#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using RKode.Utils.Editor;

using ColorUtility = RKode.Utils.ColorUtility;
using System.Collections.Generic;

namespace RKode.VERO.Editor {
public class EditorWindow : UnityEditor.EditorWindow, ISerializationCallbackReceiver {
    private Controller _builder;
    private Style _builderStyle;
    private GUILibrary _guiLibrary;

    private float _windowHeight = 1f;
    private float _minWindowWidth = 540f;
    private float _maxWindowWidth = 780f;

    private bool _needsStyleRebuild = true;

    private TabBase[] _tabs;
    private int _tabIndex;
    private TabContext _tabCtx;

    private Color _lastTint;
    private Color TintColor {
        get {
            string hex = EditorPrefs.GetString(Constant.tintPrefKey, Constant.defaultTint);
            return ColorUtility.HexToColor(hex);
        }
        set {
            EditorPrefs.SetString(Constant.tintPrefKey, "#" + UnityEngine.ColorUtility.ToHtmlStringRGBA(value));
        }
    }

    [SerializeField] private string _savedBundleVersion;
    [SerializeField] private string _savedBuildFileName;
#if UNITY_ANDROID
    [SerializeField] private int _savedAndroidVersionCode;
#endif
#if UNITY_IOS
    [SerializeField] private string _savedIOSBuildNumber;
#endif

    [MenuItem("RKode/Build Project", false, 100)]
    public static void BuildProject() {
        var scenes = EditorBuildSettings.scenes;
        List<string> enabledScenes = new List<string>();
        foreach (var s in scenes) {
            if (s.enabled)
                enabledScenes.Add(s.path);
        }

        if (enabledScenes.Count == 0) {
            EditorUtility.DisplayDialog("No Scenes",
                "No scenes are enabled in Build Settings.", "OK");
            return;
        }

        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);
        BuildPlayerOptions options = new BuildPlayerOptions {
            scenes = enabledScenes.ToArray(),
            target = target,
            targetGroup = group,
            locationPathName = ""
        };

        Show(options);
    }

    public static void Show(BuildPlayerOptions options) {
        var window = CreateInstance<EditorWindow>();
        window.titleContent = new GUIContent("⚙ VERO ⚙");
        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 540, 1);
        window._builder = new Controller(options);
        window.Show();
    }

    public void OnBeforeSerialize() {
        if (_builder == null) return;

        _savedBundleVersion = _builder.BundleVersion;
        _savedBuildFileName = _builder.BuildFileName;
#if UNITY_ANDROID
        _savedAndroidVersionCode = _builder.AndroidVersionCode;
#endif
#if UNITY_IOS
        _savedIOSBuildNumber = _builder.IOSBuildNumber;
#endif
    }

    public void OnAfterDeserialize() { }

    private void OnEnable() {
        RebuildControllerIfNeeded();
        _tabIndex = 0;
        _needsStyleRebuild = true;

        _builder.onBuildSucceeded += Close;
        _builder.onBuildCancelled += OnBuildCancelled;
        _builder.onValidationFailed += OnValidationFailed;
    }

    private void OnDisable() {
        if (_builder == null) return;

        _builder.onBuildSucceeded -= Close;
        _builder.onBuildCancelled -= OnBuildCancelled;
        _builder.onValidationFailed -= OnValidationFailed;
    }

    private void OnFocus() {
        _needsStyleRebuild = true;
        _builder?.LoadSettings();
    }

    private void Initialize() {
        if (_needsStyleRebuild || _builderStyle == null || !_builderStyle.IsInitialized) {
            if (_builderStyle == null)
                _builderStyle = new Style();

            _lastTint = Color.clear;
            TryUpdateTint(TintColor);
            _needsStyleRebuild = false;
        }

        if (_guiLibrary == null)
            _guiLibrary = new GUILibrary(_builderStyle);

        if (_tabs == null)
            BuildTabs();
    }

    private void OnGUI() {
        Initialize();
        _tabCtx.windowPosition = position;

        GUILayout.Space(10);
        RoundedPanelLayout.BeginRoundedBox(_builderStyle.HeaderPanel);
        DrawHeader();

        GUILayout.Space(6f);
        RoundedPanelLayout.BeginRoundedBox(_builderStyle.FieldsPanel);
        _guiLibrary.DrawCustomTabs(_tabs, _tabIndex, (selectedIndex)=> {
            _tabIndex = selectedIndex;
            Repaint();
        });

        EditorGUILayout.BeginVertical();
        _tabs[_tabIndex].Draw();
        EditorGUILayout.EndVertical();

        RoundedPanelLayout.EndRoundedBox();

        if (BuildPipeline.isBuildingPlayer) {
            Rect rect = EditorGUILayout.GetControlRect(false, 20);
            EditorGUI.ProgressBar(rect, 0.5f, "Building...");
        }

        RoundedPanelLayout.EndRoundedBox();

        if (Event.current.type == EventType.Repaint) {
            float contentHeight = GUILayoutUtility.GetLastRect().yMax + 10f;

            if (Mathf.Abs(_windowHeight - contentHeight) > 0.5f) {
                _windowHeight = contentHeight;
                minSize = new Vector2(_minWindowWidth, _windowHeight);
                maxSize = new Vector2(_maxWindowWidth, _windowHeight);
                Repaint();
            }
        }

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0) {
            GUI.FocusControl(null);
            Repaint();
        }
    }

    private void TryUpdateTint(Color color) {
        if(_builderStyle == null) {
            return;
        }

        if(_lastTint == color && _builderStyle.IsInitialized) {
            return;
        }

        _builderStyle.Initialize(color);
        _lastTint = color;
    }

    private void BuildTabs() {
        if(_tabCtx == null) {
            _tabCtx = new TabContext() {
                builder = _builder,
                style = _builderStyle,
                guiLibrary = _guiLibrary,
            };
        }

        _tabs = new TabBase[] {
            new BuildTab(_tabCtx, Close),
            new ConfigTab(_tabCtx),
        };
    }

    private void RebuildControllerIfNeeded() {
        if (_builder != null) return;

        var options = new BuildPlayerOptions {
            target = EditorUserBuildSettings.activeBuildTarget,
            targetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget),
            scenes = GetEnabledScenePaths()
        };

        _builder = new Controller(options);

        if (!string.IsNullOrEmpty(_savedBundleVersion))
            _builder.SetBundleVersion(_savedBundleVersion);

        if (!string.IsNullOrEmpty(_savedBuildFileName))
            _builder.SetBuildFileName(_savedBuildFileName);

#if UNITY_ANDROID
        if (_savedAndroidVersionCode > 0)
            _builder.SetAndroidVersionCode(_savedAndroidVersionCode);
#endif
#if UNITY_IOS
        if (!string.IsNullOrEmpty(_savedIOSBuildNumber))
            _builder.SetIOSBuildNumber(_savedIOSBuildNumber);
#endif
    }

    private static string[] GetEnabledScenePaths() {
        var scenes = EditorBuildSettings.scenes;
        var paths = new List<string>();

        foreach (var s in scenes) {
            if (s.enabled) paths.Add(s.path);
        }

        return paths.ToArray();
    }

    private void OnBuildCancelled() { }

    private void OnValidationFailed(string message) =>
        EditorUtility.DisplayDialog("Validation Error", message, "OK");

#region UIElems

    private void DrawHeader() {
        GUILayout.Label("⚙ VERO ⚙", _builderStyle.TitleLabel);   // ⚙
        GUILayout.Space(2f);
        GUILayout.Label("Versioning Extension & Release Optimizer", _builderStyle.SubtitleLabel);
        _guiLibrary.DrawDivider(220);
        GUILayout.Label("Set version details before building.", _builderStyle.SubtitleLabel);
    }

#endregion
}
}
#endif
