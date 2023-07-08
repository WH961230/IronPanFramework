using System;
using System.Collections.Generic;
using Kuroha.QHierarchy.RunTime;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyComponentComponents : QHierarchyBaseComponent
    {
        #region 变量

        /// <summary>
        /// 单个游戏物体上全部的 Component (剔除忽略组件后剩余的组件)
        /// </summary>
        private readonly List<Component> components = new List<Component>();

        /// <summary>
        /// 忽略组件的名称关键字列表
        /// </summary>
        private List<string> ignoreKeyList;

        /// <summary>
        /// 鼠标悬浮提示样式
        /// </summary>
        private readonly GUIStyle mouseTipLabelStyle;

        /// <summary>
        /// 鼠标悬浮提示的背景色
        /// </summary>
        private readonly Color backgroundDarkColor;

        /// <summary>
        /// 组件的默认图标
        /// </summary>
        private readonly Texture2D componentDefaultIcon;

        /// <summary>
        /// 可出发点击事件的区域
        /// </summary>
        private Rect eventRect = new Rect(0, 0, GAME_OBJECT_HEIGHT, GAME_OBJECT_HEIGHT);

        /// <summary>
        /// 绘制的组件图标数量
        /// </summary>
        private int countToDraw;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public QHierarchyComponentComponents()
        {
            backgroundDarkColor = QHierarchyResources.Instance().GetColor(EM_QHierarchyColor.BackgroundDark);
            componentDefaultIcon = QHierarchyResources.Instance().GetTexture(EM_QHierarchyTexture.QComponentUnknownIcon);

            mouseTipLabelStyle = new GUIStyle
            {
                normal =
                {
                    textColor = QHierarchyResources.Instance().GetColor(EM_QHierarchyColor.Gray)
                },
                fontSize = 11,
                clipping = TextClipping.Clip
            };

            rect.width = rect.height = GAME_OBJECT_HEIGHT;

            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ComponentsShow, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ComponentsShowDuringPlayMode, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ComponentsIgnore, SettingsChanged);

            SettingsChanged();
        }

        /// <summary>
        /// 更改设置
        /// </summary>
        private void SettingsChanged()
        {
            // 获取设置: 是否显示组件
            enabled = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ComponentsShow);

            // 获取设置: 是否在播放模式下显示
            showComponentDuringPlayMode = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ComponentsShowDuringPlayMode);

            // 获取设置: 忽略的组件
            var ignoreString = QHierarchySettings.Instance().Get<string>(EM_QHierarchySettings.ComponentsIgnore);
            if (!string.IsNullOrEmpty(ignoreString))
            {
                ignoreKeyList = new List<string>(ignoreString.Split(',', ';', '.', ' '));
                ignoreKeyList.RemoveAll(item => item == string.Empty);
            }
            else
            {
                ignoreKeyList = null;
            }
        }

        /// <summary>
        /// 布局
        /// </summary>
        internal override EM_QHierarchyLayoutStatus Layout(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            // 1 个 Component 都显示不了
            if (maxWidth < rect.width + COMPONENT_SPACE)
            {
                return EM_QHierarchyLayoutStatus.Failed;
            }

            // 获取组件
            GetAllComponentWithoutWhiteList(gameObject);

            // 计算可以显示的组件数量
            var maxComponentsCount = Mathf.FloorToInt((maxWidth - COMPONENT_SPACE) / rect.width);
            countToDraw = Math.Min(maxComponentsCount, components.Count - 1);

            // 计算总宽度
            var totalWidth = COMPONENT_SPACE + rect.width * countToDraw;

            // 向左移动: 总宽度
            curRect.x -= totalWidth;

            // 计算整个功能的显示矩形, Y 需要居中
            rect.x = curRect.x;
            rect.y = curRect.y - (rect.height - GAME_OBJECT_HEIGHT) / 2;

            // 记录下事件处理用的矩形, 记录的数据是全部的组件图标显示区域
            eventRect.width = totalWidth;
            eventRect.x = rect.x;
            eventRect.y = rect.y;

            // 可显示全部组件, 返回 Success
            if (maxComponentsCount >= components.Count - 1)
            {
                return EM_QHierarchyLayoutStatus.Success;
            }

            // 当前宽度仅可显示部分组件图标, 返回 Partly
            // 当前宽度无法显示任何图标, 返回 Failed
            return maxComponentsCount == 0 ? EM_QHierarchyLayoutStatus.Failed : EM_QHierarchyLayoutStatus.Partly;
        }

        /// <summary>
        /// 获取物体的全部组件, 并筛选掉白名单中的组件
        /// </summary>
        private void GetAllComponentWithoutWhiteList(GameObject gameObject)
        {
            var allComponents = gameObject.transform.GetComponents<Component>();

            components.Clear();

            if (ignoreKeyList != null)
            {
                foreach (var component in allComponents)
                {
                    var componentName = component.GetType().FullName;
                    if (componentName == null)
                    {
                        continue;
                    }

                    var ignore = false;
                    foreach (var keyWorld in ignoreKeyList)
                    {
                        if (componentName.Contains(keyWorld))
                        {
                            ignore = true;
                        }
                    }

                    if (!ignore)
                    {
                        components.Add(component);
                    }
                }
            }
            else
            {
                components.AddRange(allComponents);
            }
        }

        /// <summary>
        /// 绘制组件图标
        /// </summary>
        internal override void Draw(GameObject gameObjectToDraw, QHierarchyObjectList hierarchyObjectList, Rect selectionRect)
        {
            // 仅绘制能显示的最大数量的组件
            // 比如物体挂载了 10 个组件, 但是只能显示 5 个, 则只显示最后 5 个
            var allCount = components.Count;
            for (var index = components.Count - countToDraw; index < allCount; index++)
            {
                var component = components[index];
                if (component is Transform)
                {
                    continue;
                }

                // 获取组件的图标
                var content = component == null ? EditorGUIUtility.IconContent("console.warnIcon.sml") : EditorGUIUtility.ObjectContent(component, component.GetType());
                if (component is ParticleSystemRenderer)
                {
                    content = EditorGUIUtility.IconContent("BillboardRenderer Icon");
                }

                // 反射获取组件的激活标志
                var objectEnabled = true;
                if (component != null)
                {
                    var type = component.GetType();
                    var property = type.GetProperty("enabled");
                    if (property != null)
                    {
                        objectEnabled = (bool) property.GetValue(component);
                    }
                }

                // 确定颜色
                var color = UnityEngine.GUI.color;
                color.a = objectEnabled ? 1f : 0.2f;
                UnityEngine.GUI.color = color;

                // 绘制组件图标
                var icon = content.image == null ? componentDefaultIcon : content.image;
                UnityEngine.GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);

                // 颜色复原
                color.a = 1;
                UnityEngine.GUI.color = color;

                // 绘制鼠标悬浮提示
                if (rect.Contains(Event.current.mousePosition))
                {
                    // 确定组件名称
                    var componentName = "Missing Script";
                    if (component != null)
                    {
                        componentName = component.GetType().Name;
                    }

                    // 计算组件名称所占的宽度 (此 API 获取的宽度大约会小 8 像素)
                    var labelWidth = Mathf.CeilToInt(mouseTipLabelStyle.CalcSize(new GUIContent(componentName)).x);
                    labelWidth += 8;

                    // 计算矩形框 (X 居中)
                    selectionRect.x = rect.x - (labelWidth - rect.width) / 2f;
                    selectionRect.width = labelWidth;

                    // 加上分割线的高度
                    selectionRect.height += 1;

                    // 第 1 行的提示显示在左侧
                    if (selectionRect.y <= GAME_OBJECT_HEIGHT)
                    {
                        selectionRect.x -= (labelWidth + rect.width) / 2f;
                    }

                    // 后续行的提示显示在上一行
                    if (selectionRect.y > GAME_OBJECT_HEIGHT)
                    {
                        selectionRect.y -= GAME_OBJECT_HEIGHT;
                    }

                    // 绘制提示框背景
                    EditorGUI.DrawRect(selectionRect, backgroundDarkColor);

                    // 绘制提示框的文字
                    selectionRect.x += 4;
                    selectionRect.y += (EditorGUIUtility.singleLineHeight - rect.height) * 0.5f;
                    selectionRect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.LabelField(selectionRect, componentName, mouseTipLabelStyle);
                }

                rect.x += rect.width;
            }
        }

        /// <summary>
        /// 事件处理
        /// </summary>
        internal override void EventHandler(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Event currentEvent)
        {
            if (currentEvent.isMouse && currentEvent.button == 0 && currentEvent.type == EventType.MouseDown && eventRect.Contains(currentEvent.mousePosition))
            {
                currentEvent.Use();

                // GetAllComponentWithoutWhiteList(gameObject);

                // 计算单击的是第几个图标
                var clickIndex = Mathf.FloorToInt((currentEvent.mousePosition.x - eventRect.x) / rect.width) + components.Count - countToDraw;

                // 反射获取组件的 enabled 字段
                var component = components[clickIndex];
                if (component == null)
                {
                    return;
                }

                var type = component.GetType();
                var property = type.GetProperty("enabled");
                if (property == null)
                {
                    return;
                }

                var componentEnabled = (bool) property.GetValue(component);

                // 在撤销栈中记录下操作
                Undo.RecordObject(components[clickIndex], componentEnabled ? "Disable Component" : "Enable Component");

                // 反射 Set enabled 字段具体的值
                componentEnabled = !componentEnabled;
                property.GetSetMethod().Invoke(components[clickIndex], new object[] {componentEnabled});

                EditorUtility.SetDirty(gameObject);
            }
        }
    }
}
