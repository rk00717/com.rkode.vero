using UnityEditor;
using UnityEngine;

namespace RKode.VERO.Editor {
public class ConfigTab : TabBase {
    public override string Name => "Config";
    public ConfigTab(TabContext ctx) : base(ctx) { }

    public override void Draw() {
        EditorGUILayout.LabelField("Project", _ctx.style.HeadingLabel);
        GUILayout.Space(4f);

        string companyName = PlayerSettings.companyName;
        string newCompanyName = EditorGUILayout.TextField("Company Name", companyName, _ctx.style.InputField);
        if(newCompanyName != companyName) {
            PlayerSettings.companyName = newCompanyName;
            _ctx.builder.SyncBundleIdFromProjectSettings();
        }

        GUILayout.Space(2);

        string productName = PlayerSettings.productName;
        string newProductName = EditorGUILayout.TextField("Product Name", productName, _ctx.style.InputField);
        if (newProductName != productName) {
            PlayerSettings.productName = newProductName;
            _ctx.builder.SyncBundleIdFromProjectSettings();
        }

        _ctx.guiLibrary.DrawDivider();
        EditorGUILayout.LabelField("Versioning", _ctx.style.HeadingLabel);
        GUILayout.Space(4f);

        string major = _ctx.guiLibrary.DrawVersionField("Major", _ctx.builder.Major, "0");
        GUILayout.Space(2);
        string minor = _ctx.guiLibrary.DrawVersionField("Minor", _ctx.builder.Minor, "1");
        GUILayout.Space(2);
        string patch = _ctx.guiLibrary.DrawVersionField("Patch", _ctx.builder.Patch, "0");
        GUILayout.Space(4);

        var bgcolor = GUI.backgroundColor;
        GUI.backgroundColor = _ctx.style.Colors.EnumField;
        VersionSuffix suffixType = (VersionSuffix)EditorGUILayout.EnumPopup("Version Type", _ctx.builder.SuffixType, GUILayout.Width(_ctx.windowPosition.width - 47));
        GUI.backgroundColor = bgcolor;

        string suffixMeta = _ctx.guiLibrary.DrawVersionField("Version Suffix", _ctx.builder.SuffixMeta, "");

        string newVersion = Utility.BuildVersionString(major, minor, patch, suffixType, suffixMeta);
        if (newVersion != _ctx.builder.BundleVersion) {
            _ctx.builder.SetBundleVersion(major, minor, patch, suffixType, suffixMeta);
            _ctx.builder.RefreshFileName();
        }

        _ctx.guiLibrary.DrawDivider(_ctx.windowPosition.width - 48, 2, true);

#if UNITY_ANDROID
        int newCode = _ctx.guiLibrary.DrawVersionField("Android Version Code", _ctx.builder.AndroidVersionCode);
        if (newCode != _ctx.builder.AndroidVersionCode)
            _ctx.builder.SetAndroidVersionCode(newCode);
#endif
#if UNITY_IOS
        string newBuild = _ctx.guiLibrary.DrawVersionField("iOS Build Number", _ctx.builder.IOSBuildNumber);
        if (newBuild != _ctx.builder.IOSBuildNumber)
            _ctx.builder.SetIOSBuildNumber(newBuild);
#endif

        GUILayout.Space(4f);

        EditorGUILayout.BeginHorizontal();
        string newFileName = EditorGUILayout.TextField("Build File Name", _ctx.builder.BuildFileName, _ctx.style.InputField);
        if (newFileName != _ctx.builder.BuildFileName)
            _ctx.builder.SetBuildFileName(newFileName);

        if (GUILayout.Button("Update", _ctx.style.ResetButton, GUILayout.Width(80)))
            _ctx.builder.RefreshFileName();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        string newFilePath = EditorGUILayout.TextField("Build File Path", _ctx.builder.BuildPath, _ctx.style.InputField);

        if (GUILayout.Button("Browse...", _ctx.style.ResetButton, GUILayout.Width(80))) {
            string selected = EditorUtility.OpenFolderPanel("Select Build Folder", _ctx.builder.BuildPath, "");
            if (!string.IsNullOrEmpty(selected))
                newFilePath = selected;
        }

        if (newFilePath != _ctx.builder.BuildPath)
            _ctx.builder.SetBuildPath(newFilePath);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(4f);
    }
}
}
