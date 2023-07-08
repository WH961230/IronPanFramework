using Kuroha.QHierarchy.RunTime;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyComponentRenderer : QHierarchyBaseComponent
    {
        private Color activeColor;
        private Color inactiveColor;

        private const int RECT_WIDTH = 12;
        private readonly Texture2D rendererButtonTexture;

        /// <summary>
        /// 构造函数
        /// </summary>
        internal QHierarchyComponentRenderer()
        {
            rect.width = RECT_WIDTH;

            rendererButtonTexture = QHierarchyResources.Instance().GetTexture(EM_QHierarchyTexture.QRendererButton);

            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.RendererShow, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.RendererShowDuringPlayMode, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.AdditionalActiveColor, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.AdditionalInactiveColor, SettingsChanged);

            SettingsChanged();
        }

        /// <summary>
        /// 修改设置
        /// </summary>
        private void SettingsChanged()
        {
            enabled = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.RendererShow);
            showComponentDuringPlayMode = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.RendererShowDuringPlayMode);
            activeColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.AdditionalActiveColor);
            inactiveColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.AdditionalInactiveColor);
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
            rect.y = curRect.y;
            return EM_QHierarchyLayoutStatus.Success;
        }

        /// <summary>
        /// 绘制
        /// </summary>
        internal override void Draw(GameObject gameObjectToDraw, QHierarchyObjectList hierarchyObjectList, Rect selectionRect)
        {
            var renderer = gameObjectToDraw.GetComponent<Renderer>();
            if (renderer != null)
            {
                UnityEngine.GUI.color = renderer.enabled ? activeColor : inactiveColor;
                UnityEngine.GUI.DrawTexture(rect, rendererButtonTexture);
                UnityEngine.GUI.color = QHierarchyColorUtils.DefaultColor;
            }
        }

        /// <summary>
        /// 左键单击事件
        /// </summary>
        internal override void EventHandler(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Event currentEvent)
        {
            if (currentEvent.isMouse && currentEvent.button == 0 && currentEvent.type == EventType.MouseDown && rect.Contains(currentEvent.mousePosition))
            {
                var renderer = gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    currentEvent.Use();

                    var isEnabled = renderer.enabled;

                    Undo.RecordObject(renderer, isEnabled ? "Disable Component" : "Enable Component");
                    renderer.enabled = !isEnabled;
                    SceneView.RepaintAll();

                    EditorUtility.SetDirty(gameObject);
                }
            }
        }
    }
}
