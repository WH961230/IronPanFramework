using System;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal class Searcher
    {
        private string showText;

        private string ShowText
        {
            get => showText;
            set
            {
                if (showText != value)
                {
                    showText = value;
                    OnChanged?.Invoke(value);
                }
            }
        }

        private event Action<string> OnChanged;

        private readonly GUIStyle boxStyle;
        private readonly GUIStyle tipStyle;
        private readonly GUIStyle textStyle;
        private readonly GUIContent emptyContent;
        private readonly GUIContent tipContent;
        private readonly GUIContent searchIcon;
        private readonly GUIContent clearIcon;

        public Searcher(Action<string> onChanged)
        {
            OnChanged += onChanged;

            emptyContent = new GUIContent("");
            tipContent = new GUIContent("Input Search Text");
            boxStyle = new GUIStyle("TextField");
            tipStyle = new GUIStyle("Label")
            {
                normal =
                {
                    textColor = new Color(1f, 1f, 1f, 0.4f)
                },
                fontSize = 20,
                alignment = TextAnchor.MiddleLeft
            };
            searchIcon = EditorGUIUtility.IconContent("Search Icon");
            #if Kuroha
            clearIcon = EditorGUIUtility.IconContent("close@2x");
            #else
            clearIcon = EditorGUIUtility.IconContent("winbtn_mac_close_h@2x");
            #endif

            textStyle = new GUIStyle(EditorStyles.whiteLabel)
            {
                normal =
                {
                    textColor = EditorStyles.textField.normal.textColor
                },
                fontSize = 20,
                alignment = TextAnchor.MiddleLeft
            };
        }

        public void DoLayout()
        {
            var showRect = GUILayoutUtility.GetRect(1, 1, GUILayout.Height(30), GUILayout.ExpandWidth(true));
            showRect.x += 3;
            showRect.width -= 6;

            DrawBox(showRect);
            DrawTip(showRect);
            DrawSearchIcon(showRect);
            DrawClearIcon(showRect);
            DrawTextField(showRect);
        }

        private void DrawBox(Rect showRect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                boxStyle.Draw(showRect, emptyContent, 0);
            }
        }

        private void DrawTip(Rect showRect)
        {
            if (Event.current.type == EventType.Repaint && string.IsNullOrEmpty(ShowText))
            {
                showRect.x += 38;
                showRect.width -= 38;

                tipStyle.Draw(showRect, tipContent, 0);
            }
        }

        private void DrawSearchIcon(Rect showRect)
        {
            if (Event.current.type == EventType.Repaint)
            {
                showRect.x += 3;
                showRect.width = 30;
                showRect.height = 30;

                UnityEngine.GUI.DrawTexture(showRect, searchIcon.image);
            }
        }

        private void DrawClearIcon(Rect showRect)
        {
            showRect.x = showRect.width - 30;
            showRect.width = 30;
            showRect.height = 30;

            if (string.IsNullOrEmpty(ShowText))
            {
                UnityEngine.GUI.Button(showRect, emptyContent, GUIStyle.none);
            }
            else if (UnityEngine.GUI.Button(showRect, clearIcon, GUIStyle.none))
            {
                ShowText = string.Empty;

                // 让文本框失去焦点, 以让文本框更新文本, 不然会导致点击 clear 之后文本框没有清空
                GUIUtility.keyboardControl = 0;
            }
        }

        private void DrawTextField(Rect showRect)
        {
            showRect.x += 33;
            showRect.width -= 66;

            ShowText = EditorGUI.TextField(showRect, ShowText, textStyle);
        }
    }
}
