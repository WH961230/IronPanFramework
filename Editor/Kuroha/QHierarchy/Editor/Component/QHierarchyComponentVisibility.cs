using System.Collections.Generic;
using Kuroha.QHierarchy.RunTime;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyComponentVisibility : QHierarchyBaseComponent
    {
        private Color activeColor;
        private Color inactiveColor;
        private Color specialColor;
        private readonly Texture2D visibilityButtonTexture;
        private readonly Texture2D visibilityOffButtonTexture;
        private const int RECT_WIDTH = 18;

        private const string TIP_TITLE = "变更编辑时物体可见性";

        private const string CTRL_SHIFT_TIP_ON = "要仅在编辑器模式下递归激活此物体吗? (可以在设置中关闭此提示)";
        private const string CTRL_SHIFT_TIP_OFF = "要仅在编辑器模式下递归取消激活此物体吗? (可以在设置中关闭此提示)";
        private const string CTRL_ALT_TIP_ON = "要仅在编辑器模式下同时激活此物体以及全部同级物体吗? (可以在设置中关闭此提示)";
        private const string CTRL_ALT_TIP_OFF = "要仅在编辑器模式下同时取消激活此物体以及全部同级物体吗? (可以在设置中关闭此提示)";

        private const string SHIFT_TIP_ON = "要递归激活此物体吗? (可以在设置中关闭此提示)";
        private const string SHIFT_TIP_OFF = "要递归取消激活此物体吗? (可以在设置中关闭此提示)";
        private const string ALT_TIP_ON = "要同时激活此物体以及全部同级物体吗? (可以在设置中关闭此提示)";
        private const string ALT_TIP_OFF = "要同时取消激活此物体以及全部同级物体吗? (可以在设置中关闭此提示)";

        /// <summary>
        /// 构造函数
        /// </summary>
        internal QHierarchyComponentVisibility()
        {
            rect.width = RECT_WIDTH;

            visibilityButtonTexture = QHierarchyResources.Instance().GetTexture(EM_QHierarchyTexture.QVisibilityButton);
            visibilityOffButtonTexture = QHierarchyResources.Instance().GetTexture(EM_QHierarchyTexture.QVisibilityOffButton);

            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.VisibilityShow, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.VisibilityShowDuringPlayMode, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.AdditionalActiveColor, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.AdditionalInactiveColor, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.AdditionalSpecialColor, SettingsChanged);

            SettingsChanged();
        }

        /// <summary>
        /// 修改设置
        /// </summary>
        private void SettingsChanged()
        {
            enabled = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.VisibilityShow);
            showComponentDuringPlayMode = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.VisibilityShowDuringPlayMode);

            // "自身激活, 父级激活"
            activeColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.AdditionalActiveColor);
            // "自身激活, 父级未激活"
            specialColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.AdditionalSpecialColor);
            // "自身未激活
            inactiveColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.AdditionalInactiveColor);
        }

        /// <summary>
        /// 计算布局
        /// </summary>
        internal override EM_QHierarchyLayoutStatus Layout(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if (maxWidth < rect.width - 1)
            {
                return EM_QHierarchyLayoutStatus.Failed;
            }

            curRect.x -= rect.width - 1;
            rect.x = curRect.x;
            rect.y = curRect.y - 1;
            return EM_QHierarchyLayoutStatus.Success;
        }

        /// <summary>
        /// 当直接在引擎中切换激活状态时
        /// </summary>
        internal override void DisabledHandler(GameObject gameObject, QHierarchyObjectList hierarchyObjectList)
        {
            if (hierarchyObjectList != null)
            {
                if (gameObject.activeSelf && hierarchyObjectList.editModeVisibleObjects.Contains(gameObject))
                {
                    hierarchyObjectList.editModeVisibleObjects.Remove(gameObject);
                    gameObject.SetActive(false);
                    EditorUtility.SetDirty(gameObject);
                }
                else if (!gameObject.activeSelf && hierarchyObjectList.editModeInvisibleObjects.Contains(gameObject))
                {
                    hierarchyObjectList.editModeInvisibleObjects.Remove(gameObject);
                    gameObject.SetActive(true);
                    EditorUtility.SetDirty(gameObject);
                }
            }
        }

        /// <summary>
        /// 绘制
        /// </summary>
        internal override void Draw(GameObject gameObjectToDraw, QHierarchyObjectList hierarchyObjectList, Rect selectionRect)
        {
            var visibility = gameObjectToDraw.activeSelf ? 1 : 0;

            var editModeVisibleObjectsContains = IsEditModeVisible(gameObjectToDraw, hierarchyObjectList);
            var editModeInvisibleObjectsContains = IsEditModeInvisible(gameObjectToDraw, hierarchyObjectList);

            // 检查列表和可见性是否不一致
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (!gameObjectToDraw.activeSelf && editModeVisibleObjectsContains || gameObjectToDraw.activeSelf && editModeInvisibleObjectsContains)
                {
                    gameObjectToDraw.SetActive(!gameObjectToDraw.activeSelf);
                }
            }

            // 检查情况是否为: "自身激活 父级未激活"
            var transform = gameObjectToDraw.transform;
            while (transform.parent != null)
            {
                transform = transform.parent;
                if (!transform.gameObject.activeSelf)
                {
                    visibility = 2;
                    break;
                }
            }

            if (!EditorApplication.isPlayingOrWillChangePlaymode && (editModeVisibleObjectsContains || editModeInvisibleObjectsContains))
            {
                switch (visibility)
                {
                    case 0:
                        UnityEngine.GUI.color = specialColor;
                        UnityEngine.GUI.DrawTexture(rect, visibilityOffButtonTexture);
                        break;
                    case 1:
                        UnityEngine.GUI.color = specialColor;
                        UnityEngine.GUI.DrawTexture(rect, visibilityButtonTexture);
                        break;
                    default:
                        UnityEngine.GUI.color = QHierarchyColorUtils.GetCustomColor(specialColor, 1.0f, 0.4f);
                        UnityEngine.GUI.DrawTexture(rect, editModeVisibleObjectsContains ? visibilityButtonTexture : visibilityOffButtonTexture);
                        break;
                }
            }
            else
            {
                switch (visibility)
                {
                    case 0:
                        UnityEngine.GUI.color = inactiveColor;
                        UnityEngine.GUI.DrawTexture(rect, visibilityOffButtonTexture);
                        break;
                    case 1:
                        UnityEngine.GUI.color = activeColor;
                        UnityEngine.GUI.DrawTexture(rect, visibilityButtonTexture);
                        break;
                    default:
                    {
                        if (gameObjectToDraw.activeSelf)
                        {
                            UnityEngine.GUI.color = QHierarchyColorUtils.GetCustomColor(activeColor, 0.65f, 0.65f);
                            UnityEngine.GUI.DrawTexture(rect, visibilityButtonTexture);
                        }
                        else
                        {
                            UnityEngine.GUI.color = QHierarchyColorUtils.GetCustomColor(inactiveColor, 0.85f, 0.85f);
                            UnityEngine.GUI.DrawTexture(rect, visibilityOffButtonTexture);
                        }

                        break;
                    }
                }
            }

            UnityEngine.GUI.color = QHierarchyColorUtils.DefaultColor;
        }

        /// <summary>
        /// 单击事件处理
        /// </summary>
        internal override void EventHandler(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Event currentEvent)
        {
            if (currentEvent.isMouse && currentEvent.button == 0 && currentEvent.type == EventType.MouseDown && rect.Contains(currentEvent.mousePosition))
            {
                var showWarning = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.AdditionalShowModifierWarning);

                var targetGameObjects = new List<GameObject>();

                // Shift
                if (currentEvent.shift)
                {
                    var message = gameObject.activeSelf ? SHIFT_TIP_OFF : SHIFT_TIP_ON;
                    if (currentEvent.control || currentEvent.command)
                    {
                        message = gameObject.activeSelf ? CTRL_SHIFT_TIP_OFF : CTRL_SHIFT_TIP_ON;
                    }

                    if (!showWarning || EditorUtility.DisplayDialog(TIP_TITLE, message, "Yes", "Cancel"))
                    {
                        GetGameObjectListRecursive(gameObject, ref targetGameObjects);
                    }
                }

                // Alt
                else if (currentEvent.alt)
                {
                    var parent = gameObject.transform.parent;
                    if (parent != null)
                    {
                        var message = gameObject.activeSelf ? ALT_TIP_OFF : ALT_TIP_ON;
                        if (currentEvent.control || currentEvent.command)
                        {
                            message = gameObject.activeSelf ? CTRL_ALT_TIP_OFF : CTRL_ALT_TIP_ON;
                        }

                        if (!showWarning || EditorUtility.DisplayDialog(TIP_TITLE, message, "Yes", "Cancel"))
                        {
                            GetGameObjectListRecursive(parent.gameObject, ref targetGameObjects, 1);
                            targetGameObjects.Remove(parent.gameObject);
                        }
                    }
                }
                else
                {
                    // 实现多选后同时执行显隐操作
                    if (Selection.Contains(gameObject))
                    {
                        targetGameObjects.AddRange(Selection.gameObjects);
                    }
                    else
                    {
                        GetGameObjectListRecursive(gameObject, ref targetGameObjects, 0);
                    }
                }

                SetVisibility(targetGameObjects, hierarchyObjectList, !gameObject.activeSelf, currentEvent.control || currentEvent.command);
                currentEvent.Use();
            }
        }

        /// <summary>
        /// 判断是否编辑器模式下可见
        /// </summary>
        private static bool IsEditModeVisible(GameObject gameObject, QHierarchyObjectList hierarchyObjectList)
        {
            return hierarchyObjectList != null && hierarchyObjectList.editModeVisibleObjects.Contains(gameObject);
        }

        /// <summary>
        /// 判断是否编辑器模式下不可见
        /// </summary>
        private static bool IsEditModeInvisible(GameObject gameObject, QHierarchyObjectList hierarchyObjectList)
        {
            return hierarchyObjectList != null && hierarchyObjectList.editModeInvisibleObjects.Contains(gameObject);
        }

        /// <summary>
        /// 设置编辑器下的可见性
        /// </summary>
        private static void SetVisibility(in List<GameObject> gameObjects, QHierarchyObjectList hierarchyObjectList, bool targetVisibility, bool editMode)
        {
            if (gameObjects.Count == 0)
            {
                return;
            }

            if (hierarchyObjectList == null && editMode)
            {
                hierarchyObjectList = QHierarchyObjectListManager.Instance().GetObjectList(gameObjects[0]);
            }

            if (hierarchyObjectList != null)
            {
                Undo.RecordObject(hierarchyObjectList, "visibility change");
            }

            foreach (var curGameObject in gameObjects)
            {
                Undo.RecordObject(curGameObject, "visibility change");

                if (editMode)
                {
                    if (!targetVisibility)
                    {
                        hierarchyObjectList.editModeVisibleObjects.Remove(curGameObject);
                        if (!hierarchyObjectList.editModeInvisibleObjects.Contains(curGameObject))
                        {
                            hierarchyObjectList.editModeInvisibleObjects.Add(curGameObject);
                        }
                    }
                    else
                    {
                        hierarchyObjectList.editModeInvisibleObjects.Remove(curGameObject);
                        if (!hierarchyObjectList.editModeVisibleObjects.Contains(curGameObject))
                        {
                            hierarchyObjectList.editModeVisibleObjects.Add(curGameObject);
                        }
                    }
                }
                else if (hierarchyObjectList != null)
                {
                    hierarchyObjectList.editModeVisibleObjects.Remove(curGameObject);
                    hierarchyObjectList.editModeInvisibleObjects.Remove(curGameObject);
                }

                curGameObject.SetActive(targetVisibility);
                EditorUtility.SetDirty(curGameObject);
            }
        }
    }
}
