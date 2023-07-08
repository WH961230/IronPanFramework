using System.Collections.Generic;
using System.Reflection;
using Kuroha.QHierarchy.RunTime;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyComponentTagLayerName : QHierarchyBaseComponent
    {
        private readonly GUIStyle labelStyle;
        private EM_QHierarchyTagAndLayerShowType showType;
        private Color layerColor;
        private Color tagColor;
        private bool showAlways;
        private bool sizeIsPixel;
        private int pixelSize;
        private float percentSize;
        private GameObject[] gameObjects;
        private float labelAlpha;
        private EM_QHierarchyTagAndLayerLabelSize labelSize;
        private Rect tagRect;
        private Rect layerRect;
        private bool needDrawTag;
        private bool needDrawLayer;
        private int layer;
        private string tag;

        /// <summary>
        /// 构造方法
        /// </summary>
        internal QHierarchyComponentTagLayerName()
        {
            labelStyle = new GUIStyle
            {
                clipping = TextClipping.Clip,
                alignment = TextAnchor.MiddleLeft
            };

            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagAndLayerSizeShowType, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagAndLayerType, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagAndLayerSizeValueType, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagAndLayerSizeValuePixel, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagAndLayerSizeValuePercent, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagAndLayerLabelSize, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagAndLayerShow, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagAndLayerShowDuringPlayMode, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagAndLayerTagLabelColor, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagAndLayerLayerLabelColor, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagAndLayerAlignment, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.TagAndLayerLabelAlpha, SettingsChanged);

            SettingsChanged();
        }

        /// <summary>
        /// 更改设置
        /// </summary>
        private void SettingsChanged()
        {
            showAlways = QHierarchySettings.Instance().Get<int>(EM_QHierarchySettings.TagAndLayerType) == (int) EM_QHierarchyTagAndLayerType.总是显示全部名称;
            showType = (EM_QHierarchyTagAndLayerShowType) QHierarchySettings.Instance().Get<int>(EM_QHierarchySettings.TagAndLayerSizeShowType);
            sizeIsPixel = QHierarchySettings.Instance().Get<int>(EM_QHierarchySettings.TagAndLayerSizeValueType) == (int) EM_QHierarchyTagAndLayerSizeType.像素值;
            pixelSize = QHierarchySettings.Instance().Get<int>(EM_QHierarchySettings.TagAndLayerSizeValuePixel);
            percentSize = QHierarchySettings.Instance().Get<float>(EM_QHierarchySettings.TagAndLayerSizeValuePercent);
            labelSize = (EM_QHierarchyTagAndLayerLabelSize) QHierarchySettings.Instance().Get<int>(EM_QHierarchySettings.TagAndLayerLabelSize);
            enabled = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.TagAndLayerShow);
            tagColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.TagAndLayerTagLabelColor);
            layerColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.TagAndLayerLayerLabelColor);
            labelAlpha = QHierarchySettings.Instance().Get<float>(EM_QHierarchySettings.TagAndLayerLabelAlpha);
            showComponentDuringPlayMode = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.TagAndLayerShowDuringPlayMode);

            var alignment = (EM_QHierarchyTagAndLayerAlignment) QHierarchySettings.Instance().Get<int>(EM_QHierarchySettings.TagAndLayerAlignment);
            switch (alignment)
            {
                case EM_QHierarchyTagAndLayerAlignment.Left:
                    labelStyle.alignment = TextAnchor.MiddleLeft;
                    break;
                case EM_QHierarchyTagAndLayerAlignment.Center:
                    labelStyle.alignment = TextAnchor.MiddleCenter;
                    break;
                case EM_QHierarchyTagAndLayerAlignment.Right:
                    labelStyle.alignment = TextAnchor.MiddleRight;
                    break;
                default:
                    labelStyle.alignment = labelStyle.alignment;
                    break;
            }
        }

        /// <summary>
        /// 计算布局
        /// </summary>
        internal override EM_QHierarchyLayoutStatus Layout(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            var textWidth = sizeIsPixel ? pixelSize : percentSize * rect.x;

            rect.width = textWidth + 4;

            if (maxWidth < rect.width)
            {
                return EM_QHierarchyLayoutStatus.Failed;
            }

            curRect.x -= rect.width + 2;
            rect.x = curRect.x;
            rect.y = curRect.y;

            layer = gameObject.layer;
            tag = gameObject.tag;

            needDrawTag = showType != EM_QHierarchyTagAndLayerShowType.仅显示层级 && showAlways || tag != "Untagged";
            needDrawLayer = showType != EM_QHierarchyTagAndLayerShowType.仅显示标签 && showAlways || layer != 0;

            if (labelSize == EM_QHierarchyTagAndLayerLabelSize.BigIfOnlyShowTagOrLayer && needDrawTag != needDrawLayer)
            {
                labelStyle.fontSize = 14;
            }
            else if (labelSize == EM_QHierarchyTagAndLayerLabelSize.Big)
            {
                labelStyle.fontSize = 10;
            }
            else
            {
                labelStyle.fontSize = 8;
            }

            if (needDrawTag)
            {
                tagRect.Set(rect.x, rect.y - (needDrawLayer ? 4 : 0), rect.width, rect.height);
            }

            if (needDrawLayer)
            {
                layerRect.Set(rect.x, rect.y + (needDrawTag ? 4 : 0), rect.width, rect.height);
            }

            return EM_QHierarchyLayoutStatus.Success;
        }

        /// <summary>
        /// 绘制 GUI
        /// </summary>
        internal override void Draw(GameObject gameObjectToDraw, QHierarchyObjectList hierarchyObjectList, Rect selectionRect)
        {
            if (needDrawTag)
            {
                tagColor.a = tag == "Untagged" ? labelAlpha : 1.0f;
                labelStyle.normal.textColor = tagColor;
                EditorGUI.LabelField(tagRect, tag, labelStyle);
            }

            if (needDrawLayer)
            {
                layerColor.a = layer == 0 ? labelAlpha : 1.0f;
                labelStyle.normal.textColor = layerColor;
                EditorGUI.LabelField(layerRect, GetLayerName(layer), labelStyle);
            }
        }

        /// <summary>
        /// 鼠标点击事件
        /// </summary>
        internal override void EventHandler(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Event currentEvent)
        {
            if (Event.current.isMouse && currentEvent.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (needDrawTag && needDrawLayer)
                {
                    tagRect.height = 8;
                    layerRect.height = 8;
                    tagRect.y += 4;
                    layerRect.y += 4;
                }

                // 显示 Tag 菜单
                if (needDrawTag && tagRect.Contains(Event.current.mousePosition))
                {
                    gameObjects = Selection.Contains(gameObject) ? Selection.gameObjects : new[] {gameObject};
                    ShowTagsContextMenu();
                    Event.current.Use();
                }

                // 显示 Layer 菜单
                if (needDrawLayer && layerRect.Contains(Event.current.mousePosition))
                {
                    gameObjects = Selection.Contains(gameObject) ? Selection.gameObjects : new[] {gameObject};
                    var layerName = LayerMask.LayerToName(layer);
                    ShowLayersContextMenu(layerName);
                    Event.current.Use();
                }
            }
        }

        /// <summary>
        /// 将 Layer 从 int 转为 string
        /// </summary>
        private static string GetLayerName(int layer)
        {
            var layerName = LayerMask.LayerToName(layer);

            if (layerName.Equals(""))
            {
                layerName = "Undefined";
            }

            return layerName;
        }

        /// <summary>
        /// 显示 Tag 菜单
        /// </summary>
        private void ShowTagsContextMenu()
        {
            var tags = new List<string>(UnityEditorInternal.InternalEditorUtility.tags);

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Untagged"), false, TagChangedHandler, "Untagged");

            for (int i = 0, n = tags.Count; i < n; i++)
            {
                var curTag = tags[i];
                menu.AddItem(new GUIContent(curTag), tag == curTag, TagChangedHandler, curTag);
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add Tag..."), false, AddTagOrLayerHandler, "Tags");
            menu.ShowAsContext();
        }

        /// <summary>
        /// 显示 Layer 菜单
        /// </summary>
        private void ShowLayersContextMenu(string layerName)
        {
            var layers = new List<string>(UnityEditorInternal.InternalEditorUtility.layers);

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Default"), false, LayerChangedHandler, "Default");

            for (int i = 0, n = layers.Count; i < n; i++)
            {
                var curLayer = layers[i];
                menu.AddItem(new GUIContent(curLayer), layerName == curLayer, LayerChangedHandler, curLayer);
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add Layer..."), false, AddTagOrLayerHandler, "Layers");
            menu.ShowAsContext();
        }

        /// <summary>
        /// 修改 Tag 事件
        /// </summary>
        private void TagChangedHandler(object newTag)
        {
            foreach (var gameObject in gameObjects)
            {
                Undo.RecordObject(gameObject, "Change Tag");
                gameObject.tag = (string) newTag;
                EditorUtility.SetDirty(gameObject);
            }
        }

        /// <summary>
        /// Layer 修改事件
        /// </summary>
        private void LayerChangedHandler(object newLayer)
        {
            var newLayerId = LayerMask.NameToLayer((string) newLayer);

            foreach (var gameObject in gameObjects)
            {
                Undo.RecordObject(gameObject, "Change Layer");
                gameObject.layer = newLayerId;
                EditorUtility.SetDirty(gameObject);
            }
        }

        /// <summary>
        /// 增加 Tag Layer 事件
        /// </summary>
        private static void AddTagOrLayerHandler(object value)
        {
            var propertyInfo = typeof(EditorApplication).GetProperty("tagManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty);

            var obj = (UnityEngine.Object) propertyInfo?.GetValue(null, null);

            if (obj != null)
            {
                obj.GetType().GetField("m_DefaultExpandedFoldout").SetValue(obj, value);
                Selection.activeObject = obj;
            }
        }
    }
}
