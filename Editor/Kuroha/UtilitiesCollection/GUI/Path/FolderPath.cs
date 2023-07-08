using System.IO;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal sealed class FolderPath : BasePath
    {
        public string OnGUI(string inputPath, string title, int titleWidth = 150)
        {
            return OnPathGUI(inputPath, title, titleWidth);
        }

        protected override void OnButtonClickFolder()
        {
            if (btnFolder)
            {
                btnFolder = false;
                var absolute = EditorUtility.OpenFolderPanel("Select Assets Folder", "Assets", "Scenes");
                if (!string.IsNullOrEmpty(absolute))
                {
                    path = absolute.Replace('\\', '/');
                }
            }
        }

        protected override void OnButtonClickPick()
        {
            if (btnPick)
            {
                btnPick = false;

                var tempPath = path;
                if (tempPath.Contains(":"))
                {
                    tempPath = tempPath.ToAssetPath();
                }

                var folderObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(tempPath);
                if (folderObject == null)
                {
                    EditorUtility.DisplayDialog("提示", $"Assets 文件夹中不存在此目录 {path}, 无法定位!", "OK");
                }
                else
                {
                    EditorGUIUtility.PingObject(folderObject);
                    Selection.activeObject = folderObject;
                }
            }
        }

        protected override void OnMouseDragOver(Rect textRect)
        {
            // 拖拽结束时添加资源
            if (Event.current.type == EventType.DragExited)
            {
                if (textRect.Contains(Event.current.mousePosition))
                {
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        if (DragAndDrop.paths.Length == 1)
                        {
                            var dragPath = DragAndDrop.paths[0];
                            if (File.Exists(dragPath))
                            {
                                EditorUtility.DisplayDialog("提示", "只能拖拽文件夹", "OK");
                            }
                            else if (Directory.Exists(dragPath))
                            {
                                path = dragPath;
                            }
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("提示", "只能拖拽一个文件夹", "OK");
                        }
                    }
                }
            }
        }
    }
}
