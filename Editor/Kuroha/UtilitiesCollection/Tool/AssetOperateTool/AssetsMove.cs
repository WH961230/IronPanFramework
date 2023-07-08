using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal class AssetsMove
    {
        private string txtPath;
        private string targetFolder;

        private readonly ProgressBar progressBar;
        private readonly FolderPath folderUI;
        private readonly ObjectPath objectUI;
        private readonly List<string> pathList = new List<string>(20);
        private readonly List<UnityEngine.Object> objectList = new List<UnityEngine.Object>(20);

        public AssetsMove(ProgressBar progressBar, FolderPath folderUI, ObjectPath objectUI)
        {
            this.objectUI = objectUI;
            this.folderUI = folderUI;
            this.progressBar = progressBar;
        }

        public void OnGUI()
        {
            GUILayout.Label("1. 选择目标文件夹.");
            targetFolder = folderUI.OnGUI(targetFolder, "目标文件夹");
            GUILayout.Space(5);

            GUILayout.Label("2. 选择路径集合文件.");
            txtPath = objectUI.OnGUI(txtPath, "路径集合文件 (txt)", "txt");
            GUILayout.Space(5);

            GUILayout.Label("3. 点击按钮, 读取文件");
            var btnLoad = GUILayout.Button("读取", GUILayout.Width(100), GUILayout.Height(25));
            GUILayout.Space(5);

            ListUI.Title("待移动资源预览");
            ListUI.List(objectList, ItemDrawer, EmptyDrawer);

            GUILayout.Label("4. 点击按钮, 移动资源");
            var btnDelete = GUILayout.Button("移动", GUILayout.Width(100), GUILayout.Height(25));
            GUILayout.Space(5);

            if (btnLoad)
            {
                LoadTxt();
            }

            if (btnDelete)
            {
                MoveAllAsset();
            }
        }

        private void EmptyDrawer()
        {
            GUILayout.Label("无待移动资源");
        }

        private UnityEngine.Object ItemDrawer(Rect rect, UnityEngine.Object obj)
        {
            obj = EditorGUI.ObjectField(rect, obj, typeof(UnityEngine.Object), false);
            return obj;
        }

        private void MoveAllAsset()
        {
            var counter = 0;
            for (var index = 0; index < pathList.Count; index++)
            {
                progressBar.Show("提示", $"正在移动资源: {index + 1}/{pathList.Count}", index + 1, pathList.Count);

                pathList[index] = pathList[index].Replace('\\', '/');
                var assetName = pathList[index].Substring(pathList[index].LastIndexOf("/", StringComparison.Ordinal));
                var error = AssetDatabase.MoveAsset(pathList[index], targetFolder + assetName);
                if (string.IsNullOrEmpty(error))
                {
                    counter++;
                }
                else
                {
                    ToolDebug.Log($"资源 <{pathList[index]}> 移动失败: {error}", ToolDebug.橙);
                }
            }

            ToolDebug.Log($"成功移动 {counter}/{pathList.Count} 个文件", ToolDebug.浅绿);
        }

        private void LoadTxt()
        {
            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(txtPath);
            if (textAsset == null)
            {
                ToolDebug.Log("未找到路径集合文件! 请检查路径配置!", ToolDebug.橙);
                return;
            }

            var text = textAsset.text;
            text = text.Replace("\r\n", "\n");
            var files = text.Split('\n');

            objectList.Clear();
            foreach (var file in files)
            {
                if (!string.IsNullOrEmpty(file))
                {
                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(file);
                    if (asset == null)
                    {
                        ToolDebug.Log($"未找到资源 <{file}>", ToolDebug.橙);
                    }
                    else
                    {
                        pathList.Add(file);
                        objectList.Add(asset);
                    }
                }
            }
        }
    }
}
