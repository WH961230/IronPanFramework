using Kuroha.QHierarchy.RunTime;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    /// <summary>
    /// QHierarchyComponent : 显示子物体数量
    /// </summary>
    internal class QHierarchyComponentChildrenCount : QHierarchyBaseComponent
    {
        /// <summary>
        /// 标签样式
        /// </summary>
        private readonly GUIStyle labelStyle;

        /// <summary>
        /// 构造函数
        /// </summary>
        public QHierarchyComponentChildrenCount()
        {
            labelStyle = new GUIStyle
            {
                clipping = TextClipping.Clip,
                alignment = TextAnchor.MiddleRight
            };

            rect.height = GAME_OBJECT_HEIGHT;

            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ChildrenCountShow, OnSettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ChildrenCountShowDuringPlayMode, OnSettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ChildrenCountLabelSize, OnSettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ChildrenCountLabelColor, OnSettingsChanged);

            OnSettingsChanged();
        }

        /// <summary>
        /// 当用户更改设置时触发
        /// </summary>
        private void OnSettingsChanged()
        {
            // 取出设置: 是否启用功能
            enabled = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ChildrenCountShow);

            // 取出设置: 是否在播放模式下显示
            showComponentDuringPlayMode = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ChildrenCountShowDuringPlayMode);

            // 取出设置: 数字标签显示大小
            var labelSize = (EM_QHierarchySize) QHierarchySettings.Instance().Get<int>(EM_QHierarchySettings.ChildrenCountLabelSize);

            // 取出设置: 数字标签显示颜色
            labelStyle.normal.textColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.ChildrenCountLabelColor);

            switch (labelSize)
            {
                case EM_QHierarchySize.Normal:
                    labelStyle.fontSize = 11;
                    break;
                case EM_QHierarchySize.Big:
                    labelStyle.fontSize = 13;
                    break;
                default:
                    labelStyle.fontSize = 11;
                    break;
            }

            switch (labelSize)
            {
                case EM_QHierarchySize.Normal:
                    rect.width = 22;
                    break;
                case EM_QHierarchySize.Big:
                    rect.width = 24;
                    break;
                default:
                    rect.width = 22;
                    break;
            }
        }

        /// <summary>
        /// 进行布局
        /// </summary>
        internal override EM_QHierarchyLayoutStatus Layout(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if (maxWidth < rect.width + COMPONENT_SPACE)
            {
                return EM_QHierarchyLayoutStatus.Failed;
            }

            // 从右向左绘制
            curRect.x -= rect.width + COMPONENT_SPACE;
            rect.x = curRect.x;
            rect.y = curRect.y;

            return EM_QHierarchyLayoutStatus.Success;
        }

        /// <summary>
        /// 进行绘制
        /// </summary>
        internal override void Draw(GameObject gameObjectToDraw, QHierarchyObjectList hierarchyObjectList, Rect selectionRect)
        {
            var childrenCount = gameObjectToDraw.transform.childCount;
            if (childrenCount > 0)
            {
                UnityEngine.GUI.Label(rect, childrenCount.ToString("000"), labelStyle);
            }
        }
    }
}
