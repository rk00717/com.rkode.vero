// using UnityEditor;
// using UnityEngine;

// namespace RKode.VERO.Editor {
// public class SceneTab : TabBase {
//     public override string Name => "Scenes";
//     public SceneTab(TabContext ctx) : base(ctx) { }

//     public override void Draw() {
//         EditorGUILayout.LabelField("Build Scenes", _ctx.style.HeadingLabel);
//         GUILayout.Space(4f);

//         Rect windowSize = _ctx.windowPosition;
//         windowSize.height = 200;

//         SceneBuildManager.Draw(windowSize);
//         SceneBuildManager.CheckPickerSelection();
//     }
// }
// }
