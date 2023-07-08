using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
///  自定义窗口 Demo
/// </summary>
public class LFDemo : EditorWindow {
    [MenuItem("点这里/Demo/自定义窗口Demo _F2")]
    public static void GenerateScriptWindow() {
        LFDemo window = (LFDemo) GetWindow(typeof(LFDemo), false, "CustomWindow", true);
        window.Show();
    }

    private bool foldOut = true;
    private float scale = 0.0f;
    private float value = 0.0f;
    private int layer = 0;
    private string dropDown = "";
    private int flags = 0;
    private Color color;

    private string[] values = {
        "1)基础组件", "2)图标", "3)画板"
    };

    private int index;

    private void OnGUI() {
        index = GUILayout.Toolbar(index, values);
        if (index == 0) {
            EditorGUILayout.LabelField("1 - Toggle");
            GUIStyle style = new GUIStyle();
            style.hover = new GUIStyleState();
            EditorGUILayout.Toggle("style", true);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("2 - Foldout");
            foldOut = EditorGUILayout.Foldout(foldOut, "foldOut");
            if (foldOut) {
                EditorGUILayout.LabelField("Foldout - Show");
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("3 - Slider");
            scale = EditorGUILayout.Slider(scale, 1, 100);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("4 - ColorField");
            color = EditorGUILayout.ColorField(color, null);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("5 - BeginHorizontal");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Button")) {
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("6 - BoundsField");
            Bounds b = new Bounds();
            EditorGUILayout.BoundsField(b);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("7 - Knob");
            value = EditorGUILayout.Knob(new Vector2(50, 50), value, 0, 360, "", Color.black, Color.red, true, null);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("8 - Layer");
            layer = EditorGUILayout.LayerField(layer);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("9 - DropdownButton");
            if (EditorGUILayout.DropdownButton(new GUIContent(dropDown), FocusType.Keyboard)) {
                var alls = new string[4] {
                    "A/A", "A/B", "C", "D"
                };
                GenericMenu menu = new GenericMenu();
                foreach (var item in alls) {
                    if (string.IsNullOrEmpty(item)) {
                        continue;
                    }

                    menu.AddItem(new GUIContent(item), dropDown.Equals(item), OnValueSelected, item);
                }

                menu.ShowAsContext();
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("10 - MaskField");
            flags = EditorGUILayout.MaskField("MaskField", flags, options);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("11 - HelpBox");
            EditorGUILayout.HelpBox("HelpBox", MessageType.Warning);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("12 - CurveField");
            curve = EditorGUILayout.CurveField(curve);
        } else if (index == 1) {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(650), GUILayout.Height(650));
            var method = typeof(EditorGUIUtility).GetMethod("GetEditorAssetBundle", BindingFlags.Static | BindingFlags.NonPublic);
            var bundle = method?.Invoke(null, null) as AssetBundle;
            var allAsset = bundle.LoadAllAssets();
            List<Texture2D> allTexture2 = new List<Texture2D>();
            for (int i = 0; i < allAsset.Length; i++) {
                if (allAsset[i] is Texture2D texture2D) {
                    // 这些图标非法（版本原因）
                    if (texture2D.name != "DialArrow_Texture" &&
                        texture2D.name != "ProgressBar" &&
                        texture2D.name != "builtin_brush_1" &&
                        texture2D.name != "builtin_brush_2" &&
                        texture2D.name != "builtin_brush_3" &&
                        texture2D.name != "builtin_brush_4" &&
                        texture2D.name != "builtin_brush_5" &&
                        texture2D.name != "builtin_brush_6" &&
                        texture2D.name != "builtin_brush_7" &&
                        texture2D.name != "builtin_brush_8" &&
                        texture2D.name != "builtin_brush_9" &&
                        texture2D.name != "builtin_brush_10") {
                        allTexture2.Add(texture2D);
                    }
                }
            }

            Texture2D[] textureArray = allTexture2.ToArray();

            // 所有图标 row 个一组
            for (int i = 0; i < textureArray.Length; i += 15) {
                EditorGUILayout.BeginHorizontal();

                // 横向排列
                for (int j = i; j < i + 15; j++) {
                    if (j < textureArray.Length) {
                        var name = textureArray[j].name;
                        GUIContent content = EditorGUIUtility.IconContent(name);
                        if (GUILayout.Button(content, GUILayout.Height(30), GUILayout.Width(30))) {
                            logName = name;
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.SelectableLabel(logName);
        } else if (index == 2) {
        }
    }

    private Vector2 scrollPos;
    private string logName = "";

    static string[] options = new string[] {
        "CanJump", "CanShoot", "CanSwim"
    };

    private AnimationCurve curve = new AnimationCurve(new Keyframe(5, 5));

    private void OnValueSelected(object value) {
        dropDown = value.ToString();
    }
}