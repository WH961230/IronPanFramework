using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal class AssetOperateTool : ToolBase
    {
        public const string TOOL_NAME = "资源批处理";

        private EmptyDelete emptyDelete;
        private AssetsDelete assetsDelete;
        private AssetsMove assetsMove;

        private int selectedIndex;
        private readonly string[] operateType = new string[] {"批量删除空文件夹", "批量删除资源", "批量移动资源"};

        public override void OnGUI()
        {
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();

            GUILayout.Space(5);

            OnToolGUI();

            GUILayout.Space(5);

            GUILayout.EndHorizontal();
        }

        public override void OnOpen()
        {
            emptyDelete = new EmptyDelete(folderUI, progressBar);
            assetsDelete = new AssetsDelete(progressBar, objectUI);
            assetsMove = new AssetsMove(progressBar, folderUI, objectUI);
        }

        public override void OnClose()
        {
            emptyDelete = null;
            assetsDelete = null;
            assetsMove = null;
        }

        private void OnToolGUI()
        {
            GUILayout.BeginVertical();
            selectedIndex = GUILayout.Toolbar(selectedIndex, operateType);

            GUILayout.Space(5);

            switch (selectedIndex)
            {
                case 0:
                    emptyDelete.OnGUI();
                    break;
                case 1:
                    assetsDelete.OnGUI();
                    break;
                case 2:
                    assetsMove.OnGUI();
                    break;
            }

            GUILayout.EndVertical();
        }
    }
}
