using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal class AssetsDelete
    {
        private string txtPath;

        private readonly ObjectPath objectUI;
        private readonly ProgressBar progressBar;
        private readonly List<string> pathList = new List<string>(20);
        private readonly List<UnityEngine.Object> objectList = new List<Object>(20);

        public AssetsDelete(ProgressBar progressBar, ObjectPath objectUI)
        {
            this.objectUI = objectUI;
            this.progressBar = progressBar;
        }

        public void OnGUI()
        {
            GUILayout.Label("1. 选择路径集合文件. 文件中的路径必须以 Assets 路径");
            txtPath = objectUI.OnGUI(txtPath, "路径集合文件 (txt)", "txt");

            GUILayout.Space(5);

            GUILayout.Label("2. 点击按钮, 读取文件");
            var btnLoad = GUILayout.Button("读取", GUILayout.Width(100), GUILayout.Height(25));

            ListUI.Title("待删除资源预览");
            ListUI.List(objectList, ItemDrawer, EmptyDrawer);

            GUILayout.Label("3. 点击按钮, 删除资源");
            var btnDelete = GUILayout.Button("删除", GUILayout.Width(100), GUILayout.Height(25));

            if (btnLoad)
            {
                LoadTxt();
            }

            if (btnDelete)
            {
                DeleteAllAsset();
            }
        }

        private void EmptyDrawer()
        {
            GUILayout.Label("无待删除资源");
        }

        private UnityEngine.Object ItemDrawer(Rect rect, UnityEngine.Object obj)
        {
            obj = EditorGUI.ObjectField(rect, obj, typeof(UnityEngine.Object), false);
            return obj;
        }

        private void DeleteAllAsset()
        {
            var counter = 0;
            for (var index = 0; index < pathList.Count; index++)
            {
                progressBar.Show("提示", $"正在删除资源: {index + 1}/{pathList.Count}", index + 1, pathList.Count);

                if (!AssetDatabase.DeleteAsset(pathList[index]))
                {
                    ToolDebug.Log($"资源 <{pathList[index]}> 删除失败", ToolDebug.橙);
                }
                else
                {
                    counter++;
                }
            }

            ToolDebug.Log($"成功删除 {counter}/{pathList.Count} 个文件", ToolDebug.浅绿);
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
