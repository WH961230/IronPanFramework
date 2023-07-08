using System.Collections.Generic;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal class About : ToolBase
    {
        private string version = "0.0.1";
        public const string TOOL_NAME = "关于";
        private Dictionary<string, string> updateLogDic;

        public override void OnGUI()
        {
            GUILayout.Space(10);

            GUILayout.Label("批处理工具集", ToolStyle.TxtTitle);

            GUILayout.Space(10);

            GUILayout.Label($"版本: {version}", ToolStyle.TxtSubTitle);

            GUILayout.Space(10);

            DrawUpdateLog();
        }

        public override void OnOpen()
        {
            if (updateLogDic == null)
            {
                updateLogDic = new Dictionary<string, string>
                {
                    ["0.0.1\n2022-11-01"] = "添加重命名工具\n重命名工具添加 Order 功能\n",
                    ["0.0.2\n2022-11-03"] = "新增 UI 控件 ReorderableList\n重命名工具使用 ReorderableList 控件重绘\n",
                    ["0.0.3\n2022-12-07"] = "新增 空文件夹检测工具\n新增 资源批量删除工具\n新增 资源批量移动工具\n"
                };
                version = "0.0.3";
            }
        }

        public override void OnClose() { }

        private void DrawUpdateLog()
        {
            foreach (var pair in updateLogDic)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(pair.Key, GUILayout.Width(100));
                GUILayout.Label(pair.Value);
                GUILayout.EndHorizontal();
            }
        }
    }
}
