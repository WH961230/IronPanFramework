using Kuroha.QHierarchy.RunTime;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyComponentMonoBehavior : QHierarchyBaseComponent
    {
        private const float TREE_STEP_WIDTH = 14.0f;

        private readonly Texture2D monoBehaviourIconTexture;
        private bool ignoreUnityMonoBehaviour;
        private Color iconColor;

        /// <summary>
        /// 构造函数
        /// </summary>
        internal QHierarchyComponentMonoBehavior()
        {
            rect.width = TREE_STEP_WIDTH;
            rect.height = GAME_OBJECT_HEIGHT;

            monoBehaviourIconTexture = QHierarchyResources.Instance().GetTexture(EM_QHierarchyTexture.QMonoBehaviourIcon);

            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.MonoBehaviourIconIgnoreUnityMonoBehaviour, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.MonoBehaviourIconShow, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.MonoBehaviourIconShowDuringPlayMode, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.MonoBehaviourIconColor, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TreeMapShow, SettingsChanged);

            SettingsChanged();
        }

        /// <summary>
        /// 更改设置
        /// </summary>
        private void SettingsChanged()
        {
            ignoreUnityMonoBehaviour = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.MonoBehaviourIconIgnoreUnityMonoBehaviour);
            enabled = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.MonoBehaviourIconShow);
            showComponentDuringPlayMode = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.MonoBehaviourIconShowDuringPlayMode);
            iconColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.MonoBehaviourIconColor);

            EditorApplication.RepaintHierarchyWindow();
        }

        /// <summary>
        /// 绘制 GUI
        /// </summary>
        internal override void Draw(GameObject gameObjectToDraw, QHierarchyObjectList hierarchyObjectList, Rect selectionRect)
        {
            var isCustomComponent = false;

            if (ignoreUnityMonoBehaviour)
            {
                var monoBehaviours = gameObjectToDraw.GetComponents<MonoBehaviour>();
                foreach (var monoBehaviour in monoBehaviours)
                {
                    if (monoBehaviour != null)
                    {
                        var fullName = monoBehaviour.GetType().FullName;
                        if (fullName != null)
                        {
                            if (!fullName.Contains("UnityEngine") &&
                                !fullName.Contains("MeshPro") &&
                                !fullName.Contains("DOTween"))
                            {
                                isCustomComponent = true;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                isCustomComponent = gameObjectToDraw.GetComponent<MonoBehaviour>() != null;
            }

            if (isCustomComponent)
            {
                var ident = Mathf.FloorToInt(selectionRect.x / TREE_STEP_WIDTH) - 1;

                rect.x = ident * TREE_STEP_WIDTH;
                rect.y = selectionRect.y;
                rect.width = GAME_OBJECT_HEIGHT;

                rect.x += TREE_STEP_WIDTH + 1;
                rect.width += 1;

                UnityEngine.GUI.color = iconColor;
                UnityEngine.GUI.DrawTexture(rect, monoBehaviourIconTexture);
                UnityEngine.GUI.color = QHierarchyColorUtils.DefaultColor;
            }
        }
    }
}
