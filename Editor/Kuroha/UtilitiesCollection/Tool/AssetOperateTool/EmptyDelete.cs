using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal class EmptyDelete
    {
        private string detectPath = "Assets";
        private readonly FolderPath pathUI;
        private readonly ProgressBar progressBar;
        private readonly List<string> whiteList = new List<string> {".git", "Assets/Packages"};

        public EmptyDelete(FolderPath pathUI, ProgressBar progressBar)
        {
            this.pathUI = pathUI;
            this.progressBar = progressBar;
        }

        public void OnGUI()
        {
            GUILayout.Label("1. 选择待检测目录, 会自动检测该目录下的空文件夹并移除, 默认检测 Assets 文件夹");
            detectPath = pathUI.OnGUI(detectPath, "  检测目录");

            GUILayout.Label("2. 设置文件夹排除规则");
            ListUI.Title("白名单规则");
            ListUI.List(whiteList, (rect, str) =>
            {
                str = GUI.TextField(rect, str);
                return str;
            }, () => GUILayout.Label("当前没有配置文件夹忽略规则"));

            GUILayout.Label("3. 点击按钮, 开始检测");
            var btnDetect = GUILayout.Button("开始检测", GUILayout.Width(100), GUILayout.Height(25));

            if (btnDetect)
            {
                RemoveEmptyFolder();
            }
        }

        private void RemoveEmptyFolder()
        {
            var counter = 0;

            var dirList = new List<DirectoryInfo>();
            var tempDir = new DirectoryInfo(detectPath);
            dirList.Add(tempDir);

            for (var index = 0; index < dirList.Count; index++)
            {
                var tempDirs = dirList[index].GetDirectories();
                dirList.AddRange(tempDirs);
            }

            ToolDebug.Log($"一共检测了 {dirList.Count} 个目录!", ToolDebug.浅绿);

            for (var index = dirList.Count - 1; index >= 0; index--)
            {
                var total = dirList.Count - 1;
                var current = total - index;

                progressBar.Show("目录检测中", $"进度: {current}/{total}", current, total);

                if (CheckWhite(dirList[index].FullName))
                {
                    continue;
                }

                if (RemoveFolder(dirList[index]))
                {
                    counter++;
                }
            }

            ToolDebug.Log($"一共移除了 {counter} 个空目录!", ToolDebug.浅绿);
        }

        private bool CheckWhite(string fullName)
        {
            var ignore = false;
            foreach (var white in whiteList)
            {
                if (fullName.Replace('\\', '/').Contains(white))
                {
                    ignore = true;
                    break;
                }
            }

            return ignore;
        }

        private bool RemoveFolder(DirectoryInfo dir)
        {
            var dirCount = dir.GetDirectories().Length;
            var files = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);

            var metaCount = files.Count(f => f.Name.EndsWith(".meta"));
            var fileCount = files.Count(f => !f.Name.EndsWith(".meta"));

            if (dirCount == 0 && fileCount == 0 && metaCount == 0)
            {
                ToolDebug.Log($"目录 {dir.FullName} 是空的!", ToolDebug.橙);
                AssetDatabase.DeleteAsset(dir.FullName.ToAssetPath());
                return true;
            }

            return false;
        }
    }
}
