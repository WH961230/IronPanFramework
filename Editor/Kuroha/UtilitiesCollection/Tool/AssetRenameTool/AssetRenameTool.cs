using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    using Rename = SoRenameStep.RenameStep;

    internal class AssetRenameTool : ToolBase
    {
        private class OrderList : GenericList<Rename>
        {
            private int selectedIndex;
            private OrderList selectedList;

            public OrderList(IList<Rename> list) : base(list, 24) { }

            public override void Add() => List.Add(new Rename());

            public override void Insert(int index) => List.Insert(index, new Rename());

            public override void DrawItemBackground(Rect rect, int index)
            {
                if (selectedList == this && index == selectedIndex)
                {
                    var restoreColor = GUI.color;
                    GUI.color = ReorderableListStyles.SelectionBackgroundColor;
                    GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
                    GUI.color = restoreColor;
                }
            }

            public override void DrawItem(Rect rect, int index)
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    var totalItemPosition = ListUI.CurrentItemTotalPosition;
                    if (totalItemPosition.Contains(Event.current.mousePosition))
                    {
                        selectedList = this;
                        selectedIndex = index;
                    }
                }

                if (rect.width >= 100)
                {
                    EditorGUI.LabelField(new Rect(rect) {width = 100}, $"步骤 {index + 1} - 操作类型:");
                    DrawElementOpera_Type(rect, List[index]);
                }
            }

            private void DrawElementOpera_Type(Rect rect, SoRenameStep.RenameStep step)
            {
                if (rect.width >= 180)
                {
                    var curRect = new Rect(rect.x + 100 + 3, rect.y + 3, 80, rect.height);
                    step.operaType = (SoRenameStep.OperaType) EditorGUI.EnumPopup(curRect, step.operaType);
                    curRect.y -= 3;

                    if (rect.width >= 565)
                    {
                        switch (step.operaType)
                        {
                            case SoRenameStep.OperaType.Delete:
                                DrawElementOpera_Delete(curRect, step);
                                break;
                            case SoRenameStep.OperaType.Remove:
                                DrawElementOpera_Remove(curRect, step);
                                break;
                            case SoRenameStep.OperaType.Insert:
                                DrawElementOpera_Insert(curRect, step);
                                break;
                            case SoRenameStep.OperaType.Replace:
                                DrawElementOpera_Replace(curRect, step);
                                break;
                            case SoRenameStep.OperaType.Order:
                                DrawElementOpera_Order(curRect, step);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }

            private void DrawElementOpera_Delete(Rect rect, SoRenameStep.RenameStep step)
            {
                var curRect = new Rect(rect.x + rect.width + 5, rect.y, 55, rect.height);
                EditorGUI.LabelField(curRect, "开始索引:");

                curRect = new Rect(curRect.x + curRect.width + 3, rect.y + 3, 130, rect.height - 5);
                step.deleteStep.beginIndex = EditorGUI.IntField(curRect, step.deleteStep.beginIndex);

                curRect = new Rect(curRect.x + curRect.width + 5, rect.y, 55, rect.height);
                EditorGUI.LabelField(curRect, "删除长度:");

                curRect = new Rect(curRect.x + curRect.width + 3, rect.y + 3, 130, rect.height - 5);
                step.deleteStep.length = EditorGUI.IntField(curRect, step.deleteStep.length);
            }

            private void DrawElementOpera_Remove(Rect rect, SoRenameStep.RenameStep step)
            {
                var curRect = new Rect(rect.x + rect.width + 5, rect.y, 55, rect.height);
                EditorGUI.LabelField(curRect, "正则匹配:");

                curRect = new Rect(curRect.x + curRect.width + 3, rect.y + 3, 323, rect.height - 5);
                step.removeStep.regex = EditorGUI.TextField(curRect, step.removeStep.regex);
            }

            private void DrawElementOpera_Replace(Rect rect, SoRenameStep.RenameStep step)
            {
                var curRect = new Rect(rect.x + rect.width + 5, rect.y, 55, rect.height);
                EditorGUI.LabelField(curRect, "正则匹配:");

                curRect = new Rect(curRect.x + curRect.width + 3, rect.y + 3, 130, rect.height - 5);
                step.replaceStep.regex = EditorGUI.TextField(curRect, step.replaceStep.regex);

                curRect = new Rect(curRect.x + curRect.width + 5, rect.y, 55, rect.height);
                EditorGUI.LabelField(curRect, "新字符串:");

                curRect = new Rect(curRect.x + curRect.width + 3, rect.y + 3, 130, rect.height - 5);
                step.replaceStep.newString = EditorGUI.TextField(curRect, step.replaceStep.newString);
            }

            private void DrawElementOpera_Insert(Rect rect, SoRenameStep.RenameStep step)
            {
                var curRect = new Rect(rect.x + rect.width + 5, rect.y, 30, rect.height);
                EditorGUI.LabelField(curRect, "位置:");

                curRect = new Rect(curRect.x + curRect.width + 3, rect.y + 3, 55, rect.height);
                step.insertStep.paramType = (SoRenameStep.PositionType) EditorGUI.EnumPopup(curRect, step.insertStep.paramType);

                if (step.insertStep.paramType == SoRenameStep.PositionType.Index)
                {
                    curRect = new Rect(curRect.x + curRect.width + 5, rect.y, 55, rect.height);
                    EditorGUI.LabelField(curRect, "位置索引:");

                    curRect = new Rect(curRect.x + curRect.width + 3, rect.y + 3, 37, rect.height - 5);
                    step.deleteStep.beginIndex = EditorGUI.IntField(curRect, step.deleteStep.beginIndex);
                }

                curRect = new Rect(curRect.x + curRect.width + 5, rect.y, 55, rect.height);
                EditorGUI.LabelField(curRect, "插入内容:");

                var width = step.insertStep.paramType == SoRenameStep.PositionType.Index ? 130 : 230;
                curRect = new Rect(curRect.x + curRect.width + 3, rect.y + 3, width, rect.height - 5);
                step.insertStep.content = EditorGUI.TextField(curRect, step.insertStep.content);
            }

            private void DrawElementOpera_Order(Rect rect, SoRenameStep.RenameStep step)
            {
                var curRect = new Rect(rect.x + rect.width + 5, rect.y, 30, rect.height);
                EditorGUI.LabelField(curRect, "前缀:");

                curRect = new Rect(curRect.x + curRect.width + 3, rect.y + 3, 69, rect.height - 5);
                step.orderStep.suffix = EditorGUI.TextField(curRect, step.orderStep.suffix);

                curRect = new Rect(curRect.x + curRect.width + 5, rect.y, 55, rect.height);
                EditorGUI.LabelField(curRect, "开始数字:");

                curRect = new Rect(curRect.x + curRect.width + 3, rect.y + 3, 69, rect.height - 5);
                step.orderStep.beginNumber = EditorGUI.IntField(curRect, step.orderStep.beginNumber);

                curRect = new Rect(curRect.x + curRect.width + 5, rect.y, 70, rect.height);
                EditorGUI.LabelField(curRect, "前置 0 数量:");

                curRect = new Rect(curRect.x + curRect.width + 3, rect.y + 3, 69, rect.height - 5);
                step.orderStep.zeroCount = EditorGUI.IntField(curRect, step.orderStep.zeroCount);
            }
        }

        public const string TOOL_NAME = "批量重命名";
        private const string SETTING_SO_NAME = "RenameStepSetting";

        private Vector2 scrollView;
        private OrderList orderList;
        private SoRenameStep renameSetting;

        private readonly List<UnityEngine.Object> selectedObjectList = new List<UnityEngine.Object>(50);
        private readonly Dictionary<string, UnityEngine.Object> selectedCache = new Dictionary<string, UnityEngine.Object>(100);

        public override void OnGUI()
        {
            ConfigUtil.CheckConfig(ref renameSetting, nameof(SoRenameStep), "RenameSetting", SETTING_SO_NAME);

            GUILayout.Space(5);

            ShowList();

            GUILayout.Space(5);

            RenameButton();

            GUILayout.Space(5);

            PreViewSelectedObjectList();
        }

        public override void OnOpen()
        {
            Selection.selectionChanged -= SelectChangeEventHandler;
            Selection.selectionChanged += SelectChangeEventHandler;
        }

        public override void OnClose()
        {
            Selection.selectionChanged -= SelectChangeEventHandler;
        }

        private void ShowList()
        {
            if (orderList == null)
            {
                orderList = new OrderList(renameSetting.steps);
            }

            if (orderList != null)
            {
                ListUI.Title("批量重命名工具, 将依次按照以下步骤进行重命名");
                ListUI.List(orderList, () => GUILayout.Label("先添加重命名步骤, 之后点击 Rename 按钮"));
            }
        }

        private void RenameButton()
        {
            GUI.enabled = renameSetting.steps.Count > 0;

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            var btnRename = GUILayout.Button("Rename Assets", GUILayout.Height(25), GUILayout.Width(120));
            GUILayout.EndHorizontal();

            GUI.enabled = true;

            if (btnRename)
            {
                EditorUtility.SetDirty(renameSetting);
                DoRename(selectedObjectList);
                AssetDatabase.SaveAssets();
                selectedCache.Clear();
                AssetDatabase.Refresh();
            }
        }

        private void SelectChangeEventHandler()
        {
            selectedObjectList.Clear();

            var guids = Selection.assetGUIDs;
            if (guids != null)
            {
                foreach (var guid in guids)
                {
                    if (!selectedCache.TryGetValue(guid, out var selectedObject))
                    {
                        selectedObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(guid));

                        selectedCache[guid] = selectedObject;
                    }

                    selectedObjectList.Add(selectedObject);
                }
            }

            if (EditorWindow.focusedWindow != null)
            {
                EditorWindow.focusedWindow.Repaint();
            }
        }

        private void PreViewSelectedObjectList()
        {
            if (selectedObjectList != null && selectedObjectList.Count > 0)
            {
                scrollView = GUILayout.BeginScrollView(scrollView, GUILayout.Height(527));

                ListUI.Title("当前选择的资源");
                ListUI.List(selectedObjectList,
                    (rect, item) => EditorGUI.ObjectField(rect, item, typeof(UnityEngine.Object), false),
                    () => GUILayout.Label("当前选择为空!"),
                    flags: ReorderableListFlags.ShowIndices | ReorderableListFlags.HideAddButton | ReorderableListFlags.HideRemoveButtons | ReorderableListFlags.DisableContextMenu);

                GUILayout.EndScrollView();
            }
        }

        private void DoRename(List<UnityEngine.Object> assets)
        {
            foreach (var step in renameSetting.steps)
            {
                switch (step.operaType)
                {
                    case SoRenameStep.OperaType.Delete:
                        DoRename(assets, step.deleteStep);
                        break;
                    case SoRenameStep.OperaType.Remove:
                        DoRename(assets, step.removeStep);
                        break;
                    case SoRenameStep.OperaType.Insert:
                        DoRename(assets, step.insertStep);
                        break;
                    case SoRenameStep.OperaType.Replace:
                        DoRename(assets, step.replaceStep);
                        break;
                    case SoRenameStep.OperaType.Order:
                        DoRename(assets, step.orderStep);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void DoRename(in List<UnityEngine.Object> assets, SoRenameStep.DeleteStep info)
        {
            foreach (var asset in assets)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                var newName = asset.name.Remove(info.beginIndex, info.length);
                AssetDatabase.RenameAsset(path, newName);
            }
        }

        private void DoRename(in List<UnityEngine.Object> assets, SoRenameStep.RemoveStep info)
        {
            var regex = new Regex(info.regex);

            foreach (var asset in assets)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                var newName = regex.Replace(asset.name, "");
                if (newName != asset.name)
                {
                    AssetDatabase.RenameAsset(path, newName);
                }
            }
        }

        private void DoRename(in List<UnityEngine.Object> assets, SoRenameStep.ReplaceStep info)
        {
            var regex = new Regex(info.regex);

            foreach (var asset in assets)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                var newName = regex.Replace(asset.name, info.newString);
                if (newName != asset.name)
                {
                    AssetDatabase.RenameAsset(path, newName);
                }
            }
        }

        private void DoRename(in List<UnityEngine.Object> assets, SoRenameStep.InsertStep info)
        {
            foreach (var asset in assets)
            {
                var newName = string.Empty;
                var path = AssetDatabase.GetAssetPath(asset);

                switch (info.paramType)
                {
                    case SoRenameStep.PositionType.Begin:
                        newName = asset.name.Insert(0, info.content);
                        break;

                    case SoRenameStep.PositionType.End:
                        newName = asset.name.Insert(asset.name.Length, info.content);
                        break;

                    case SoRenameStep.PositionType.Index:
                        newName = asset.name.Insert(info.index, info.content);
                        break;
                }

                AssetDatabase.RenameAsset(path, newName);
            }
        }

        private void DoRename(in List<UnityEngine.Object> assets, SoRenameStep.OrderStep info)
        {
            // 开始编号
            var beginNumber = info.beginNumber;

            // "前导零数量" 格式化
            var zeroFormat = string.Empty;
            for (var counter = 0; counter < info.zeroCount; counter++)
            {
                zeroFormat += "0";
            }

            // 资源处理
            for (var index = 0; index < assets.Count; index++)
            {
                var path = AssetDatabase.GetAssetPath(assets[index]);
                var number = beginNumber + index;
                var numberString = number.ToString(zeroFormat);
                var newName = $"{assets[index].name}{info.suffix}{numberString}";

                AssetDatabase.RenameAsset(path, newName);
            }
        }
    }
}
