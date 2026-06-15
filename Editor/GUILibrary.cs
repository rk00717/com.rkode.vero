#if UNITY_EDITOR 
using System;
using RKode.Utils.Editor;
using UnityEditor;
using UnityEngine;
using ColorUtility = RKode.Utils.ColorUtility;

namespace RKode.VERO.Editor {
public class GUILibrary {
    private readonly Style _style;

    public GUILibrary(Style style) {
        _style = style;
    }

    public void DrawDivider(float width = -1f, float x = 0, bool useOffset = false, bool useSpacing = true) {
        if(useSpacing) GUILayout.Space(4f);
        Rect r = EditorGUILayout.GetControlRect(false, 1f);

        if(width > 0) {
            r.x += (r.width - width) / 2;
            r.width = width;
        }

        if(useOffset) {
            r.x += x;
        }

        EditorGUI.DrawRect(r, ColorUtility.WithAlpha(_style.Colors.Primary, .2f));
        if(useSpacing) GUILayout.Space(4f);
    }

    public void DrawCustomTabs(TabBase[] tabs, int selected, Action<int> onSelectCallback) {
        Rect barRect = EditorGUILayout.GetControlRect(false, 34f);
        barRect.x -= 10f;
        barRect.y -= 10f;
        barRect.width += 20f;

        float tabWidth = barRect.width / tabs.Length;
        for (int i = 0; i < tabs.Length; i++) {
            Rect tabRect = new Rect(barRect.x + i * tabWidth, barRect.y, tabWidth, barRect.height);
            bool isActive = (i == selected);
            bool isHovered = tabRect.Contains(Event.current.mousePosition);

            if (isActive) {
                RoundedPanelLayout.DrawRoundedRect(tabRect, _style.ActiveTabPanel, true, true, false, false);
            } else if (isHovered && Event.current.type == EventType.Repaint) {
                EditorGUI.DrawRect(tabRect, ColorUtility.HexToColor("#FFFFFF0A"));
            }

            GUI.Label(tabRect, tabs[i].Name, isActive ? _style.ActiveTab : _style.InactiveTab);

            if (Event.current.type == EventType.MouseDown && tabRect.Contains(Event.current.mousePosition)) {
                onSelectCallback?.Invoke(i);
                Event.current.Use();
            }
        }

        EditorGUI.DrawRect(new Rect(barRect.x, barRect.yMax - 1, barRect.width, 1), ColorUtility.WithAlpha(_style.Colors.Primary, .3f));
        GUILayout.Space(2f);
    }

    public int DrawCounterBtns(int value) {
        GUILayout.Space(2f);
        if (GUILayout.Button("+", _style.SmallButton, GUILayout.Width(26))) value++;
        GUILayout.Space(4f);
        if (GUILayout.Button("-", _style.SmallButton, GUILayout.Width(26))) value--;
        GUILayout.Space(4f);
        return value;
    }

    public string DrawVersionField(string label, string value, string defaultVal, params GUILayoutOption[] options) {
        EditorGUILayout.BeginHorizontal();
        string newValue = EditorGUILayout.TextField(label, value, _style.InputField, options);

        GUI.enabled = int.TryParse(value, out int intVal);
        int newIntVal = DrawCounterBtns(intVal);
        if (newIntVal != intVal) newValue = newIntVal.ToString();

        GUI.enabled = true;
        if (GUILayout.Button("↺", _style.SmallButton, GUILayout.Width(20)))
            newValue = defaultVal;

        EditorGUILayout.EndHorizontal();
        return newValue;
    }

    public int DrawVersionField(string label, int value, params GUILayoutOption[] options) {
        EditorGUILayout.BeginHorizontal();
        int newValue = EditorGUILayout.IntField(label, value, _style.InputField, options);
        newValue = DrawCounterBtns(newValue);

        if (GUILayout.Button("↺", _style.SmallButton, GUILayout.Width(20)))
            newValue = 0;

        EditorGUILayout.EndHorizontal();
        return newValue;
    }

    public void DrawSectionHeader(string title) {
        Rect rect = EditorGUILayout.GetControlRect(false, 22f);
        EditorGUI.DrawRect(rect, ColorUtility.WithAlpha(_style.Colors.Elevated, .8f));
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, 3f, rect.height), _style.Colors.Accent);
        EditorGUI.LabelField(new Rect(rect.x + 10f, rect.y + 2f, rect.width, rect.height - 2f), title, _style.HeadingLabel);
    }

    public VersionSuffix DrawStyledEnumField(string label, VersionSuffix current) {
        var newValue = current;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);

        string displayName = newValue.ToString();
        Rect btnRect = GUILayoutUtility.GetRect(new GUIContent(displayName), _style.InputField, GUILayout.ExpandWidth(true));
        btnRect.x += 2f;

        if (Event.current.type == EventType.Repaint)
            _style.InputField.Draw(btnRect, new GUIContent(displayName), false, false, false, false);

        Rect arrowRect = new Rect(btnRect.xMax - 18f, btnRect.y, 18f, btnRect.height);
        if (Event.current.type == EventType.Repaint) {
            EditorGUI.DrawRect(arrowRect, ColorUtility.WithAlpha(_style.Colors.Primary, 0.15f));
            GUI.Label(arrowRect, "▾", new GUIStyle(EditorStyles.label) {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = _style.Colors.Primary }
            });
        }

        if (Event.current.type == EventType.MouseDown && btnRect.Contains(Event.current.mousePosition)) {
            var menu = new GenericMenu();
            foreach (VersionSuffix val in System.Enum.GetValues(typeof(VersionSuffix))) {
                var captured = val;
                menu.AddItem(new GUIContent(val.ToString()), val == newValue, () => { newValue = captured; });
            }
            menu.DropDown(btnRect);
            Event.current.Use();
        }

        EditorGUILayout.EndHorizontal();
        return newValue;
    }

    public void DrawTableRow(string label, string value, float width = 120f) {
        EditorGUILayout.BeginHorizontal(_style.Box);
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Width(width));
        EditorGUILayout.SelectableLabel(value, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        EditorGUILayout.EndHorizontal();
    }
}
}
#endif
