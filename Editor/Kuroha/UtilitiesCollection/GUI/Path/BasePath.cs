using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal abstract class BasePath
    {
        protected string path;

        protected bool btnPick;
        protected bool btnFolder;
        private bool btnConvert;

        private GUIContent iconPick;
        private GUIContent iconFolder;
        private GUIContent iconConvert;

        private readonly GUIContent iconPickCache;
        private readonly GUIContent iconFolderCache;
        private readonly GUIContent iconConvertCache;
        private readonly GUIContent iconFolderOpenedCache;

        protected BasePath()
        {
            iconPickCache = EditorGUIUtility.IconContent("d_Search Icon");
            iconFolderCache = EditorGUIUtility.IconContent("Folder Icon");
            iconConvertCache = EditorGUIUtility.IconContent("d_Refresh");
            iconFolderOpenedCache = EditorGUIUtility.IconContent("d_FolderOpened Icon");
        }

        protected string OnPathGUI(string inputPath, string title, int titleWidth = 150)
        {
            GUILayout.Space(3);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(title, GUILayout.Width(titleWidth));
                inputPath = OnPathGUI(inputPath);
            }
            GUILayout.EndHorizontal();

            return inputPath;
        }

        private string OnPathGUI(string inputPath)
        {
            path = inputPath;

            GUILayout.BeginHorizontal();
            {
                SetIcon();
                DrawIcon();
                DrawPath();
            }
            GUILayout.EndHorizontal();

            var pathTextRect = GUILayoutUtility.GetLastRect();
            OnMouseDrag(pathTextRect);
            OnMouseDragOver(pathTextRect);

            OnButtonClickPick();
            OnButtonClickFolder();
            OnButtonClickTransform();

            return path;
        }

        private void SetIcon()
        {
            if (string.IsNullOrEmpty(path))
            {
                iconPick = null;
                iconConvert = null;
                iconFolder = iconFolderCache;
            }
            else
            {
                iconPick = iconPickCache;
                iconConvert = iconConvertCache;
                iconFolder = iconFolderOpenedCache;
            }
        }

        private void DrawIcon()
        {
            var btnRect = GUILayoutUtility.GetAspectRect(10, GUILayout.Width(string.IsNullOrEmpty(path) ? 26 : 74), GUILayout.Height(22));

            if (iconFolder != null)
            {
                btnFolder = UnityEngine.GUI.Button(new Rect(btnRect.x, btnRect.y, 22, 22), iconFolder, GUIStyle.none);
            }

            if (iconPick != null)
            {
                btnPick = UnityEngine.GUI.Button(new Rect(btnRect.x + 27, btnRect.y, 22, 22), iconPick, GUIStyle.none);
            }

            if (iconConvert != null)
            {
                btnConvert = UnityEngine.GUI.Button(new Rect(btnRect.x + 52, btnRect.y + 3, 24, 24), iconConvert, GUIStyle.none);
            }
        }

        private void DrawPath()
        {
            var textRect = GUILayoutUtility.GetRect(1, 1, GUILayout.Height(24), GUILayout.ExpandWidth(true));
            textRect.x += 1;
            textRect.y += 1;
            textRect.height = 20;
            textRect.width -= 5;

            var originSize = UnityEngine.GUI.skin.textField.fontSize;
            var alignment = UnityEngine.GUI.skin.textField.alignment;
            UnityEngine.GUI.skin.textField.fontSize = 14;
            UnityEngine.GUI.skin.textField.alignment = TextAnchor.MiddleLeft;

            path = EditorGUI.TextField(textRect, path);

            UnityEngine.GUI.skin.textField.fontSize = originSize;
            UnityEngine.GUI.skin.textField.alignment = alignment;
        }

        private void OnButtonClickTransform()
        {
            if (btnConvert)
            {
                btnConvert = false;

                var tempPath = path;
                tempPath = tempPath.Contains(":") ? tempPath.ToAssetPath() : tempPath.ToFullPath();
                if (!string.IsNullOrEmpty(tempPath))
                {
                    path = tempPath;
                    GUIUtility.keyboardControl = 0;
                }
            }
        }

        private void OnMouseDrag(Rect textRect)
        {
            // 拖拽时更改鼠标样式
            if (Event.current.type == EventType.DragUpdated)
            {
                if (textRect.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                }
            }
        }

        protected abstract void OnButtonClickFolder();

        protected abstract void OnButtonClickPick();

        protected abstract void OnMouseDragOver(Rect textRect);
    }
}
