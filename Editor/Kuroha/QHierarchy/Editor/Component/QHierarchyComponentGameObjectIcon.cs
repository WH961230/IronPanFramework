using System;
using System.Reflection;
using Kuroha.QHierarchy.RunTime;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyComponentGameObjectIcon : QHierarchyBaseComponent
    {
        private readonly MethodInfo getIconMethodInfo;
        private readonly object[] getIconMethodParams;

        /// <summary>
        /// 构造函数
        /// </summary>
        public QHierarchyComponentGameObjectIcon()
        {
            getIconMethodInfo = typeof(EditorGUIUtility).GetMethod("GetIconForObject", BindingFlags.Public | BindingFlags.Static);
            getIconMethodParams = new object[1];

            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.GameObjectIconShow, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.GameObjectIconShowDuringPlayMode, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.GameObjectIconSize, SettingsChanged);

            SettingsChanged();
        }

        /// <summary>
        /// 修改设置
        /// </summary>
        private void SettingsChanged()
        {
            enabled = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.GameObjectIconShow);
            showComponentDuringPlayMode = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.GameObjectIconShowDuringPlayMode);
            var size = (EM_QHierarchySizeAll) QHierarchySettings.Instance().Get<int>(EM_QHierarchySettings.GameObjectIconSize);
            switch (size)
            {
                case EM_QHierarchySizeAll.Small:
                    rect.height = 14;
                    break;
                case EM_QHierarchySizeAll.Normal:
                    rect.height = 17;
                    break;
                case EM_QHierarchySizeAll.Big:
                    rect.height = 20;
                    break;
                default:
                    rect.height = 14;
                    break;
            }

            rect.width = rect.height;
        }

        /// <summary>
        /// 计算布局
        /// </summary>
        internal override EM_QHierarchyLayoutStatus Layout(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if (maxWidth < rect.width + COMPONENT_SPACE)
            {
                return EM_QHierarchyLayoutStatus.Failed;
            }

            curRect.x -= rect.width + COMPONENT_SPACE;
            rect.x = curRect.x;
            rect.y = curRect.y - (rect.height - GAME_OBJECT_HEIGHT) / 2;

            return EM_QHierarchyLayoutStatus.Success;
        }

        /// <summary>
        /// 绘制 GUI
        /// </summary>
        internal override void Draw(GameObject gameObjectToDraw, QHierarchyObjectList hierarchyObjectList, Rect selectionRect)
        {
            getIconMethodParams[0] = gameObjectToDraw;

            if (getIconMethodInfo == null)
            {
                QHierarchyDebugger.LogError("当前 Unity 版本不支持显示 GameObject Icon", QHierarchyDebugger.山吹);
            }
            else
            {
                var icon = (Texture2D) getIconMethodInfo?.Invoke(null, getIconMethodParams);
                if (icon != null)
                {
                    UnityEngine.GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit, true);
                }
            }
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        internal override void EventHandler(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Event currentEvent)
        {
            // 左键点击图标
            if (currentEvent.isMouse && currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition))
            {
                currentEvent.Use();

                var assembly = Assembly.Load("UnityEditor");
                var type = assembly.GetType("IconSelector");

                // 由于目标方法有 2 个重载 (下面 2 行), 所以需要使用参数类型进行区分
                // private internal static bool ShowAtPosition(Object   targetObj, Rect activatorRect, bool showLabelIcons)
                // private internal static bool ShowAtPosition(Object[] targetObj, Rect activatorRect, bool showLabelIcons)
                var paramsTypeArray = new[] {typeof(UnityEngine.Object), typeof(Rect), typeof(bool)};
                var methodInfo = type.GetMethod("ShowAtPosition", BindingFlags.Static | BindingFlags.NonPublic, Type.DefaultBinder, paramsTypeArray, null);
                if (methodInfo == null)
                {
                    QHierarchyDebugger.LogError("获取 ShowAtPosition 方法失败!", QHierarchyDebugger.红);
                }
                else
                {
                    methodInfo.Invoke(null, new object[] {gameObject, rect, true});
                }
            }
        }
    }
}
