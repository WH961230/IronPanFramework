using System.Collections.Generic;
using Kuroha.QHierarchy.RunTime;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyComponentTagIcon : QHierarchyBaseComponent
    {
        private List<QHierarchyTagTexture> tagTextureList;

        /// <summary>
        /// 构造函数
        /// </summary>
        internal QHierarchyComponentTagIcon()
        {
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagIconShow, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagIconShowDuringPlayMode, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagIconSize, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagIconList, SettingsChanged);

            SettingsChanged();
        }

        /// <summary>
        /// 更改设置
        /// </summary>
        private void SettingsChanged()
        {
            enabled = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.TagIconShow);
            showComponentDuringPlayMode = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.TagIconShowDuringPlayMode);
            var size = (EM_QHierarchySizeAll) QHierarchySettings.Instance().Get<int>(EM_QHierarchySettings.TagIconSize);

            switch (size)
            {
                case EM_QHierarchySizeAll.Small:
                    rect.height = 14;
                    break;
                case EM_QHierarchySizeAll.Normal:
                    rect.height = 15;
                    break;
                case EM_QHierarchySizeAll.Big:
                    rect.height = 16;
                    break;
                default:
                    rect.height = 14;
                    break;
            }

            rect.width = rect.height;

            tagTextureList = QHierarchyTagTexture.LoadTagTextureList();
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
        /// 绘制
        /// </summary>
        internal override void Draw(GameObject gameObjectToDraw, QHierarchyObjectList hierarchyObjectList, Rect selectionRect)
        {
            var gameObjectTag = gameObjectToDraw.tag;
            var tagTexture = tagTextureList.Find(texture => texture.tag == gameObjectTag);
            if (tagTexture != null && tagTexture.texture != null)
            {
                UnityEngine.GUI.DrawTexture(rect, tagTexture.texture, ScaleMode.ScaleToFit, true);
            }
        }
    }
}
