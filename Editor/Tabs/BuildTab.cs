using UnityEditor;
using UnityEngine;
using System.Linq;

namespace RKode.VERO.Editor {
public class BuildTab : TabBase {
    public override string Name => "Build";
    private readonly System.Action _onClose;

    public BuildTab(TabContext ctx, System.Action onClose) : base(ctx) {
        _onClose = onClose;
    }

    public override void Draw() {
        DrawBuildAwareness();

        GUILayout.Space(6f);
        DrawSummary();

        GUILayout.Space(8);
        _ctx.guiLibrary.DrawDivider();

        DrawAutoIncrementOption();

        GUILayout.Space(6);
        if (_ctx.builder.IsValidBuildPath()) {
            EditorGUILayout.HelpBox("Click the path below to open the build folder.", MessageType.Info);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Open Output Folder...", _ctx.style.LinkButton)) {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo() {
                    FileName = _ctx.builder.BuildPath,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            GUILayout.EndHorizontal();

            _ctx.guiLibrary.DrawDivider();
            GUILayout.Space(8);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("✕  Cancel", _ctx.style.CancelButton, GUILayout.Width(110f), GUILayout.Height(28f)))
                _onClose?.Invoke();

            GUILayout.Space(8f);

            if (GUILayout.Button("▶  Build", _ctx.style.CTAButton, GUILayout.Width(110f), GUILayout.Height(28f)))
                _ctx.builder.RequestBuild();

            EditorGUILayout.EndHorizontal();
        } else {
            EditorGUILayout.HelpBox("The specified path does not exist.", MessageType.Warning);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset Path (Default)", _ctx.style.CTAButton))
                _ctx.builder.ResetBuildPath();
            GUILayout.EndHorizontal();
        }
    }

    private void DrawBuildAwareness() {
        string companyName = Application.companyName;
        string bundleId = PlayerSettings.applicationIdentifier;

        bool defaultCompany = companyName == "DefaultCompany";
        bool defaultBundle = bundleId == "com.Company.ProductName";

        if (defaultCompany)
            EditorGUILayout.HelpBox("⚠  Company Name is still 'DefaultCompany'. Update it in Player Settings.", MessageType.Warning);

        if (defaultBundle)
            EditorGUILayout.HelpBox("⚠  Bundle ID is still the Unity default. Update it in Player Settings.", MessageType.Warning);
    }

    private void DrawSummary() {
        EditorGUILayout.LabelField("Summary", _ctx.style.HeadingLabel);
        GUILayout.Space(2f);

        EditorGUILayout.BeginVertical(_ctx.style.Box);
        _ctx.guiLibrary.DrawTableRow("Platform", EditorUserBuildSettings.activeBuildTarget.ToString());
        _ctx.guiLibrary.DrawDivider(useSpacing: false);
        _ctx.guiLibrary.DrawTableRow("Version", _ctx.builder.BundleVersion);
        _ctx.guiLibrary.DrawDivider(useSpacing: false);
        _ctx.guiLibrary.DrawTableRow("Filename", _ctx.builder.BuildFileName);
        _ctx.guiLibrary.DrawDivider(useSpacing: false);
        _ctx.guiLibrary.DrawTableRow("Filepath", _ctx.builder.BuildPath);

#if UNITY_ANDROID
        _ctx.guiLibrary.DrawDivider(useSpacing: false);
        _ctx.guiLibrary.DrawTableRow("Android Version Code", _ctx.builder.AndroidVersionCode.ToString());
#endif
#if UNITY_IOS
        _ctx.guiLibrary.DrawDivider(useSpacing: false);
        _ctx.guiLibrary.DrawTableRow("iOS Build Number", _ctx.builder.IOSBuildNumber);
#endif

        _ctx.guiLibrary.DrawDivider(useSpacing: false);
        _ctx.guiLibrary.DrawTableRow("Scenes Included", EditorBuildSettings.scenes.Count(p => p.enabled).ToString());

        if (_ctx.builder.AutoIncrementEnabled) {
            _ctx.guiLibrary.DrawDivider(useSpacing: false);
            string preview = _ctx.builder.CanAutoIncrement(out _)
                ? _ctx.builder.GetNextVersionPreview()
                : "—  (pattern not supported)";
            _ctx.guiLibrary.DrawTableRow("After Build →", preview);
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawAutoIncrementOption() {
        bool newAutoInc = EditorGUILayout.Toggle("Auto-Increment Patch", _ctx.builder.AutoIncrementEnabled);
        if (newAutoInc != _ctx.builder.AutoIncrementEnabled)
            _ctx.builder.SetAutoIncrement(newAutoInc);

        if(_ctx.builder.AutoIncrementEnabled) {
            _ctx.builder.CanAutoIncrement(out string incrementBlockReason);

            if(!string.IsNullOrEmpty(incrementBlockReason))
                EditorGUILayout.HelpBox($"⚠  Auto-increment disabled: {incrementBlockReason}", MessageType.Warning);
        }
    }
}
}
