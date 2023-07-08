using Kuroha.QHierarchy.RunTime;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyComponentPrefab : QHierarchyBaseComponent
    {
        private Color activeColor;
        private Color inactiveColor;
        private bool onlyShowBroken;
        private readonly Texture2D prefabTexture;

        /// <summary>
        /// 构造函数
        /// </summary>
        internal QHierarchyComponentPrefab()
        {
            rect.width = 9;

            prefabTexture = QHierarchyResources.Instance().GetTexture(EM_QHierarchyTexture.QPrefabIcon);

            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.PrefabShowBrakedPrefabsOnly, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.PrefabShow, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.AdditionalActiveColor, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.AdditionalInactiveColor, SettingsChanged);
            SettingsChanged();
        }

        /// <summary>
        /// 更改设置
        /// </summary>
        private void SettingsChanged()
        {
            enabled = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.PrefabShow);
            activeColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.AdditionalActiveColor);
            inactiveColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.AdditionalInactiveColor);
            onlyShowBroken = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.PrefabShowBrakedPrefabsOnly);
        }

        /// <summary>
        /// 计算布局
        /// </summary>
        internal override EM_QHierarchyLayoutStatus Layout(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if (maxWidth < 9)
            {
                return EM_QHierarchyLayoutStatus.Failed;
            }

            curRect.x -= 9;
            rect.x = curRect.x;
            rect.y = curRect.y;
            return EM_QHierarchyLayoutStatus.Success;
        }

        /// <summary>
        /// 绘制 GUI
        /// </summary>
        internal override void Draw(GameObject gameObjectToDraw, QHierarchyObjectList hierarchyObjectList, Rect selectionRect)
        {
            var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(gameObjectToDraw);

            // Missing Prefab
            if (prefabStatus == PrefabInstanceStatus.MissingAsset)
            {
                UnityEngine.GUI.color = inactiveColor;
                UnityEngine.GUI.DrawTexture(rect, prefabTexture);
                UnityEngine.GUI.color = QHierarchyColorUtils.DefaultColor;
            }

            // 正常 Prefab
            else if (!onlyShowBroken && prefabStatus != PrefabInstanceStatus.NotAPrefab)
            {
                UnityEngine.GUI.color = activeColor;
                UnityEngine.GUI.DrawTexture(rect, prefabTexture);
                UnityEngine.GUI.color = QHierarchyColorUtils.DefaultColor;
            }
        }
    }
}
