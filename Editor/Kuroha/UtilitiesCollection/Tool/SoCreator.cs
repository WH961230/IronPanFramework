using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal class SoCreator : ToolBase
    {
        private readonly Dictionary<string, Type> typeDic = new Dictionary<string, Type>();
        private readonly List<Assembly> assemblyList = new List<Assembly>();

        private string[] nameArrayShow;
        private readonly List<string> nameListShow = new List<string>();

        private int currentIndex;
        private Type currentType;
        private Searcher searcher;

        public const string TOOL_NAME = "SO 生成器";
        private string savePath = "Assets";

        public override void OnGUI()
        {
            EditorGUILayout.Space(5);

            searcher?.DoLayout();

            EditorGUILayout.Space(10);

            savePath = folderUI.OnGUI(savePath, "选择保存目录: ");

            EditorGUILayout.Space(10);

            DrawPopup();

            EditorGUILayout.Space(5);

            var btnCreate = GUILayout.Button("创建", GUILayout.Height(24));

            if (btnCreate)
            {
                OnButtonClickCreateSo();
            }
        }

        public override void OnOpen()
        {
            InitUI();
            GetAllAssembly();
            GetAllSo();
            UpdateShowNames(string.Empty);
        }

        public override void OnClose()
        {
            typeDic.Clear();
            assemblyList.Clear();
            nameListShow.Clear();
        }

        private void InitUI()
        {
            if (searcher == null)
            {
                searcher = new Searcher(OnSearcherChanged);
            }
        }

        private void DrawPopup()
        {
            GUILayout.BeginHorizontal();

            currentIndex = EditorGUILayout.Popup("当前选择的类型: ", currentIndex, nameArrayShow);

            GUILayout.EndHorizontal();
        }

        private void GetAllAssembly()
        {
            assemblyList.Clear();

            var allAssembly = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in allAssembly)
            {
                var name = assembly.GetName().FullName;
                if (name.Contains("Kuroha") || name.Contains("Sword"))
                {
                    assemblyList.Add(assembly);
                }
                else if (name.Contains("Assembly-CSharp,") || name.Contains("Assembly-CSharp-Editor,"))
                {
                    assemblyList.Add(assembly);
                }
            }
        }

        private void GetAllSo()
        {
            typeDic.Clear();
            foreach (var assembly in assemblyList)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(UnityEngine.ScriptableObject)))
                    {
                        if (!type.IsSubclassOf(typeof(UnityEditor.Editor)))
                        {
                            if (!type.IsSubclassOf(typeof(UnityEditor.EditorWindow)))
                            {
                                if (!string.IsNullOrEmpty(type.FullName))
                                {
                                    typeDic[type.FullName] = type;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateShowNames(string searcherText)
        {
            nameListShow.Clear();

            if (string.IsNullOrEmpty(searcherText))
            {
                foreach (var name in typeDic.Keys)
                {
                    nameListShow.Add(name);
                }
            }
            else
            {
                searcherText = searcherText.ToLower();

                foreach (var name in typeDic.Keys)
                {
                    if (name.ToLower().Contains(searcherText))
                    {
                        nameListShow.Add(name);
                    }
                }
            }

            nameArrayShow = nameListShow.ToArray();
        }

        private void OnButtonClickCreateSo()
        {
            if (AssetDatabase.IsValidFolder(savePath))
            {
                var type = typeDic[nameArrayShow[currentIndex]];

                var asset = ScriptableObject.CreateInstance(type);

                var assetPath = $"{savePath}/{type.Name}.asset";

                if (File.Exists(assetPath))
                {
                    var isCreate = EditorUtility.DisplayDialog("提示", "文件已存在, 是否覆盖?", "确定", "取消");
                    if (!isCreate)
                    {
                        return;
                    }
                }

                AssetDatabase.CreateAsset(asset, assetPath);
                EditorUtility.DisplayDialog("提示", "创建成功", "确定");
            }
        }

        private void OnSearcherChanged(string searcherText)
        {
            UpdateShowNames(searcherText);
        }
    }
}
