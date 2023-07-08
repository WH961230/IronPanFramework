using System.Collections.Generic;
using Kuroha.QHierarchy.RunTime;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyComponentLayerIcon : QHierarchyBaseComponent
    {
        private List<QHierarchyLayerTexture> layerTextureList;

        /// <summary>
        /// 构造方法
        /// </summary>
        internal QHierarchyComponentLayerIcon()
        {
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.LayerIconShow, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.LayerIconShowDuringPlayMode, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.LayerIconSize, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.LayerIconList, SettingsChanged);

            SettingsChanged();
        }

        /// <summary>
        /// 设置更改
        /// </summary>
        private void SettingsChanged()
        {
            enabled = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.LayerIconShow);
            showComponentDuringPlayMode = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.LayerIconShowDuringPlayMode);
            var size = (EM_QHierarchySizeAll) QHierarchySettings.Instance().Get<int>(EM_QHierarchySettings.LayerIconSize);

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

            layerTextureList = QHierarchyLayerTexture.LoadLayerTextureList();
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
            var gameObjectLayerName = LayerMask.LayerToName(gameObjectToDraw.layer);

            var layerTexture = layerTextureList.Find(texture => texture.layer == gameObjectLayerName);
            if (layerTexture != null && layerTexture.texture != null)
            {
                UnityEngine.GUI.DrawTexture(rect, layerTexture.texture, ScaleMode.ScaleToFit, true);
            }
        }
    }
}
