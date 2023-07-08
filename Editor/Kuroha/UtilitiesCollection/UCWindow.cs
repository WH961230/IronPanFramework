using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal class UCWindow : EditorWindow
    {
        private Vector2 scrollView;
        private Splitter splitter;
        private GUIStyle buttonStyle;

        private readonly Dictionary<string, ToolBase> toolDictionary = new Dictionary<string, ToolBase>();

        private string currentID;

        private string CurrentID
        {
            get => currentID;
            set
            {
                if (currentID != value)
                {
                    currentID = value;
                    toolDictionary[value].OnOpen();
                }
            }
        }

        [MenuItem("Kuroha/Tool/批处理工具集", false, 102)]
        public static void Open()
        {
            GetWindow<UCWindow>("批处理工具集");
        }

        private void InitData()
        {
            toolDictionary.Clear();
            toolDictionary.Add(AssetOperateTool.TOOL_NAME, new AssetOperateTool());
            toolDictionary.Add(AssetRenameTool.TOOL_NAME, new AssetRenameTool());
            toolDictionary.Add(SoCreator.TOOL_NAME, new SoCreator());
            toolDictionary.Add(About.TOOL_NAME, new About());

            CurrentID = "资源批处理";
        }

        public void OnGUI()
        {
            InitStyle();
            splitter.OnGUI(new Rect(0, 0, position.width, position.height));
        }

        private void OnEnable()
        {
            InitData();
            InitSplitter();
        }

        private void OnDestroy()
        {
            foreach (var tool in toolDictionary.Values)
            {
                tool.OnClose();
            }
        }

        private void InitStyle()
        {
            if (buttonStyle == null)
            {
                buttonStyle = new GUIStyle("Button")
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal =
                    {
                        textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black
                    }
                };
            }
        }

        private void InitSplitter()
        {
            var freezeList = new[] {false};
            var rectSizeList = new List<float> {122, 400};
            var GUIList = new Action[] {SplitterGUI1, SplitterGUI2};
            splitter = new Splitter(SplitMode.Vertical, freezeList, rectSizeList, GUIList);
        }

        private void SplitterGUI1()
        {
            scrollView = GUILayout.BeginScrollView(scrollView);

            GUILayout.Space(10);

            var counter = 0;
            foreach (var key in toolDictionary.Keys)
            {
                var oldColor = UnityEngine.GUI.backgroundColor;

                if (CurrentID == key)
                {
                    UnityEngine.GUI.backgroundColor = new Color(92 / 255f, 223 / 255f, 240 / 255f);
                }

                if (GUILayout.Button(key, buttonStyle, GUILayout.Width(splitter.Rect[0].width - 6), GUILayout.Height(30)))
                {
                    CurrentID = key;
                }

                UnityEngine.GUI.backgroundColor = oldColor;

                counter++;

                if (counter < toolDictionary.Count)
                {
                    GUILayout.Space(10);
                }
            }

            GUILayout.EndScrollView();
        }

        private void SplitterGUI2()
        {
            toolDictionary[CurrentID].OnGUI();
        }
    }
}
