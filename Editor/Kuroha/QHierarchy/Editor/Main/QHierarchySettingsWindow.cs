using System;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchySettingsWindow : EditorWindow
    {
        /// <summary>
        /// 初始化标志
        /// </summary>
        private bool initFlag;

        /// <summary>
        /// 是否为黑色风格
        /// </summary>
        private bool isProSkin;

        /// <summary>
        /// 折叠框
        /// </summary>
        private bool isOpenComponentsSettings;

        /// <summary>
        /// 折叠框
        /// </summary>
        private bool isOpenComponentsOrders;

        /// <summary>
        /// 折叠框
        /// </summary>
        private bool isOpenAdditionalSettings;

        /// <summary>
        /// 缩进像素数
        /// </summary>
        private int indentLevel;

        /// <summary>
        /// 窗口可绘制界面区域的总宽度
        /// 使用 window.position.width 获取的宽度无法自动适配右侧可能会出现的滑动条, 因此手动控制总宽度
        /// </summary>
        private float paintWidth;

        /// <summary>
        /// 记录上次绘制使用的 Rect
        /// </summary>
        private Rect lastRect;

        /// <summary>
        /// 当前可绘制区域的 Y 值
        /// </summary>
        private float currentRectY;

        /// <summary>
        /// 用于滑动条计算
        /// </summary>
        private Vector2 scrollPosition;

        /// <summary>
        /// 窗口
        /// </summary>
        private static QHierarchySettingsWindow window;

        /// <summary>
        /// 菜单按钮颜色
        /// </summary>
        private Color menuButtonColor;

        /// <summary>
        /// 黄色区域的颜色
        /// </summary>
        private Color yellowColor;

        /// <summary>
        /// 分隔器的颜色
        /// </summary>
        private Color separatorColor;

        private const int UP_DOWN_SPACE = 4;
        private const int ITEM_SETTING_HEIGHT = 18;
        private Texture2D checkBoxGray;
        private Texture2D checkBoxGreen;
        private Texture2D checkBoxOrange;
        private Texture2D restoreButtonTexture;

        private QHierarchyOrderListWindow orderListWindow;

        [MenuItem("Kuroha/Tool/QHierarchySetting", false, 101)]
        internal static void Open()
        {
            window = GetWindow<QHierarchySettingsWindow>("QHierarchyMain");
            window.minSize = new Vector2(500, 75);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            // 未初始化或者编辑器更换了皮肤, 则进行初始化
            if (initFlag && isProSkin == EditorGUIUtility.isProSkin)
            {
                return;
            }

            initFlag = true;

            isProSkin = EditorGUIUtility.isProSkin;
            yellowColor = isProSkin ? new Color(1.00f, 0.90f, 0.40f) : new Color(0.31f, 0.31f, 0.31f);
            separatorColor = isProSkin ? new Color(0.18f, 0.18f, 0.18f) : new Color(0.59f, 0.59f, 0.59f);
            menuButtonColor = isProSkin ? new Color(0.7f, 0.7f, 0.7f) : new Color(0.9f, 0.9f, 0.9f);

            checkBoxGray = EditorGUIUtility.IconContent("d_lightRim").image as Texture2D;
            checkBoxGreen = EditorGUIUtility.IconContent("d_greenLight").image as Texture2D;
            checkBoxOrange = EditorGUIUtility.IconContent("d_orangeLight").image as Texture2D;
            restoreButtonTexture = EditorGUIUtility.IconContent("Refresh@2x").image as Texture2D;
            orderListWindow = new QHierarchyOrderListWindow(this);
        }

        /// <summary>
        /// 绘制界面
        /// </summary>
        private void OnGUI()
        {
            // 初始化
            Init();

            // 绘制前初始化 indentLevel
            indentLevel = 10;

            // 绘制前初始化 lastRect
            lastRect = new Rect(0, 1, 0, 0);

            // 绘制
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                // 计算前景区域的宽度
                // EditorGUILayout.GetControlRect 获取到的高度默认是 EditorGUIUtility.singleLineHeight, 即 18
                // 获取的宽度在 Layout 状态下为 1, 在 Repaint 状态下为当前坐标尽可能延展的宽度 - 6 像素的留白
                // 这里不需要告诉 Layout 高度, 所以传 0, 如果使用默认值会导致使用了 Layout 系统的滑动条的范围高度多出部分像素
                paintWidth = EditorGUILayout.GetControlRect(GUILayout.Height(0)).width;
                paintWidth += 6;

                // 绘制菜单框 COMPONENTS SETTINGS
                DrawMenuBox("COMPONENTS SETTINGS", ref isOpenComponentsSettings);
                currentRectY = lastRect.y + lastRect.height;
                if (isOpenComponentsSettings)
                {
                    DrawSettingsHierarchyTree();
                    DrawSeparator();
                    DrawSettingsMonoBehaviourIcon();
                    DrawSeparator();
                    DrawSettingsSeparator();
                    DrawSeparator();
                    DrawSettingsVisibility();
                    DrawSeparator();
                    DrawSettingsLock();
                    DrawSeparator();
                    DrawSettingsStaticInfo();
                    DrawSeparator();
                    DrawSettingsError();
                    DrawSeparator();
                    DrawSettingsRenderer();
                    DrawSeparator();
                    DrawPrefabComponentSettings();
                    DrawSeparator();
                    DrawTagLayerNameComponentSettings();
                    DrawSeparator();
                    DrawColorComponentSettings();
                    DrawSeparator();
                    DrawGameObjectIconComponentSettings();
                    DrawSeparator();
                    DrawTagIconComponentSettings();
                    DrawSeparator();
                    DrawLayerIconComponentSettings();
                    DrawSeparator();
                    DrawChildrenCountComponentSettings();
                    DrawSeparator();
                    DrawVerticesAndTrianglesCountComponentSettings();
                    DrawSeparator();
                    DrawComponentsComponentSettings();
                    DrawLine(currentRectY, lastRect.y + lastRect.height, separatorColor);
                }

                // 开始绘制
                DrawMenuBox("ORDER OF COMPONENTS", ref isOpenComponentsOrders);
                currentRectY = lastRect.y + lastRect.height;
                if (isOpenComponentsOrders)
                {
                    DrawSpace(6);
                    DrawOrderSettings();
                    DrawSpace(6);
                    DrawLine(currentRectY, lastRect.y + lastRect.height, separatorColor);
                }

                // 开始绘制
                DrawMenuBox("ADDITIONAL SETTINGS", ref isOpenAdditionalSettings);
                currentRectY = lastRect.y + lastRect.height;
                if (isOpenAdditionalSettings)
                {
                    DrawSpace(3);
                    DrawAdditionalSettings();
                    DrawLine(currentRectY, lastRect.y + lastRect.height + 4, separatorColor);
                }

                indentLevel -= 1;
            }
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 获得一个新的 UI 绘制区域
        /// 1. 自动通知 Layout 系统的组件
        /// 2. 自动更新 lastRect
        /// </summary>
        /// <param name="width">区域的宽度</param>
        /// <param name="rectHeight">区域的高度</param>
        /// <param name="leftIndent">左侧缩进</param>
        /// <param name="rightIndent">右侧缩进</param>
        /// <returns></returns>
        private Rect GetNewRect(float width, float rectHeight, float leftIndent = 0, float rightIndent = 0)
        {
            // 为了使 GUILayout 的滑动条检测正确生效, 必须有这一句
            EditorGUILayout.GetControlRect(false, rectHeight, GUIStyle.none, GUILayout.ExpandWidth(true));

            // Rect Width
            var rectWidth = width == 0 ? paintWidth - indentLevel - leftIndent - rightIndent : width;

            // Position X
            var positionX = indentLevel + leftIndent;

            // Position Y
            var positionY = lastRect.y + lastRect.height;

            var rect = new Rect(positionX, positionY, rectWidth, rectHeight);

            // Update lastRect
            lastRect = rect;

            return rect;
        }

        /// <summary>
        /// 绘制菜单
        /// </summary>
        private void DrawMenuBox(string sectionTitle, ref bool buttonFlag)
        {
            // 定义数据
            const float ICON_WIDTH = 28;
            const float MENU_HEIGHT = 24;
            const float MENU_WIDTH = 240;

            // 告诉 Layout 要绘制一个默认宽度, 指定高度的区域
            // 默认宽度就是可绘制区域宽度 (比窗口宽度小 24 像素)
            var rect = GetNewRect(0, MENU_HEIGHT);

            // 覆盖右侧留白
            rect.width *= 2;

            // 绘制底色 Box (最左侧绘制)
            rect.x = 0;
            UnityEngine.GUI.Box(rect, string.Empty);

            // 绘制左侧竖线
            DrawLine(rect.y, rect.y + MENU_HEIGHT, yellowColor, indentLevel);
            rect.x += indentLevel;

            // 绘制折叠图标
            var oldColor = UnityEngine.GUI.backgroundColor;
            UnityEngine.GUI.backgroundColor = menuButtonColor;
            var oldAlignment = UnityEngine.GUI.skin.button.alignment;
            UnityEngine.GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            var autoCheckIcon = EditorGUIUtility.IconContent(buttonFlag ? "sv_icon_dot11_pix16_gizmo" : "sv_icon_dot8_pix16_gizmo");
            if (UnityEngine.GUI.Button(new Rect(rect.x, rect.y, MENU_WIDTH, rect.height), autoCheckIcon))
            {
                buttonFlag = !buttonFlag;
            }

            UnityEngine.GUI.backgroundColor = oldColor;
            UnityEngine.GUI.skin.button.alignment = oldAlignment;
            rect.x += ICON_WIDTH;

            // 绘制设置图标
            EditorGUI.LabelField(rect, EditorGUIUtility.IconContent("GameManager Icon"));
            rect.x += ICON_WIDTH;

            // 绘制名称
            EditorGUI.LabelField(rect, sectionTitle);
        }

        #region Draw Components

        /// <summary>
        /// 绘制 HierarchyTree
        /// </summary>
        private void DrawSettingsHierarchyTree()
        {
            if (DrawCheckBox("Hierarchy Tree", "在 Hierarchy 面板的左侧绘制树形结构\r\n体现游戏物体的父子级关系", EM_QHierarchySettings.TreeMapShow))
            {
                var rect = GetNewRect(0, 0);

                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TreeMapColor);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TreeMapEnhanced);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TreeMapTransparentBackground);
                }

                // 绘制背景色
                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT + UP_DOWN_SPACE * 2);

                // 绘制空白
                DrawSpace(UP_DOWN_SPACE);

                DrawColorPicker("树形结构的颜色", EM_QHierarchySettings.TreeMapColor);

                // 绘制空白
                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 MonoBehaviourIcon
        /// </summary>
        private void DrawSettingsMonoBehaviourIcon()
        {
            if (DrawCheckBox("Highlight MonoBehaviour", "高亮 MonoBehaviour 游戏物体", EM_QHierarchySettings.MonoBehaviourIconShow))
            {
                var rect = GetNewRect(0, 0);

                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.MonoBehaviourIconShowDuringPlayMode);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.MonoBehaviourIconColor);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.MonoBehaviourIconIgnoreUnityMonoBehaviour);
                }

                // 绘制底色背景
                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT * 3 + UP_DOWN_SPACE * 2);

                DrawSpace(UP_DOWN_SPACE);

                DrawColorPicker("高亮的颜色", EM_QHierarchySettings.MonoBehaviourIconColor);
                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.MonoBehaviourIconShowDuringPlayMode);
                DrawCheckBoxRight("忽略 Unity 原生 MonoBehaviour 组件", EM_QHierarchySettings.MonoBehaviourIconIgnoreUnityMonoBehaviour);

                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 Separator
        /// </summary>
        private void DrawSettingsSeparator()
        {
            if (DrawCheckBox("Separator", "Hierarchy 分隔器功能", EM_QHierarchySettings.SeparatorShow))
            {
                var rect = GetNewRect(0, 0);
                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.SeparatorColor);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.SeparatorShowRowShading);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.SeparatorOddRowShadingColor);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.SeparatorEvenRowShadingColor);
                }

                var rowShading = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.SeparatorShowRowShading);

                // 绘制空行
                DrawSpace(UP_DOWN_SPACE);

                // 绘制底色背景
                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT * 4 + UP_DOWN_SPACE * 2);

                // 是否对比奇偶行
                DrawCheckBoxRight("奇偶行颜色区分", EM_QHierarchySettings.SeparatorShowRowShading);
                UnityEngine.GUI.enabled = rowShading;
                DrawColorPicker("奇数行底色", EM_QHierarchySettings.SeparatorEvenRowShadingColor);
                DrawColorPicker("偶数行底色", EM_QHierarchySettings.SeparatorOddRowShadingColor);
                UnityEngine.GUI.enabled = true;

                // 绘制分隔线
                DrawColorPicker("游戏物体间分隔线的颜色", EM_QHierarchySettings.SeparatorColor);

                // 绘制空行
                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 Visibility
        /// </summary>
        private void DrawSettingsVisibility()
        {
            if (DrawCheckBox("Visibility", "游戏物体可见性功能", EM_QHierarchySettings.VisibilityShow))
            {
                var rect = GetNewRect(0, 0);

                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.VisibilityShowDuringPlayMode);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT + UP_DOWN_SPACE * 2);

                DrawSpace(UP_DOWN_SPACE);

                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.VisibilityShowDuringPlayMode);

                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 Lock
        /// </summary>
        private void DrawSettingsLock()
        {
            if (DrawCheckBox("Lock", "锁定游戏物体, 锁定后的游戏物体各项数值变为只读", EM_QHierarchySettings.LockShow))
            {
                var rect = GetNewRect(0, 0);
                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.LockShowDuringPlayMode);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.LockPreventSelectionOfLockedObjects);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT * 2 + UP_DOWN_SPACE * 2);

                DrawSpace(UP_DOWN_SPACE);

                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.LockShowDuringPlayMode);

                DrawCheckBoxRight("锁定物体禁止选中 (无法删除, 无法查看 Inspector)", EM_QHierarchySettings.LockPreventSelectionOfLockedObjects);

                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 Static Info
        /// </summary>
        private void DrawSettingsStaticInfo()
        {
            if (DrawCheckBox("Static Info", "显示游戏物体的静态选项信息", EM_QHierarchySettings.StaticShow))
            {
                var rect = GetNewRect(0, 0);
                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.StaticShowDuringPlayMode);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT + UP_DOWN_SPACE * 2);

                DrawSpace(UP_DOWN_SPACE);

                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.StaticShowDuringPlayMode);

                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 Error 显示
        /// </summary>
        private void DrawSettingsError()
        {
            if (DrawCheckBox("Error", "", EM_QHierarchySettings.ErrorShow))
            {
                var rect = GetNewRect(0, 0);
                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ErrorShowDuringPlayMode);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ErrorShowIconOnParent);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ErrorShowForDisabledComponents);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ErrorShowForDisabledGameObjects);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ErrorShowScriptIsMissing);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ErrorShowReferenceIsMissing);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ErrorShowReferenceIsNull);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ErrorShowStringIsEmpty);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ErrorShowMissingEventMethod);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ErrorShowWhenTagOrLayerIsUndefined);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ErrorIgnoreString);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT * 12 + UP_DOWN_SPACE * 2);
                DrawSpace(UP_DOWN_SPACE);

                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.ErrorShowDuringPlayMode);
                DrawCheckBoxRight("是否在错误物体的父级也显示错误标志", EM_QHierarchySettings.ErrorShowIconOnParent);
                DrawCheckBoxRight("是否检查未激活的组件", EM_QHierarchySettings.ErrorShowForDisabledComponents);
                DrawCheckBoxRight("是否检查未激活的游戏物体", EM_QHierarchySettings.ErrorShowForDisabledGameObjects);

                DrawLabel("检测以下错误:");
                indentLevel += 16;
                DrawCheckBoxRight("-- Component Is Missing", EM_QHierarchySettings.ErrorShowScriptIsMissing);
                DrawCheckBoxRight("-- Reference Is Missing", EM_QHierarchySettings.ErrorShowReferenceIsMissing);
                DrawCheckBoxRight("-- Reference Is Null", EM_QHierarchySettings.ErrorShowReferenceIsNull);
                DrawCheckBoxRight("-- String Is Empty", EM_QHierarchySettings.ErrorShowStringIsEmpty);
                DrawCheckBoxRight("-- Event Is Missing (性能消耗较大)", EM_QHierarchySettings.ErrorShowMissingEventMethod);
                DrawCheckBoxRight("-- Tag | Layer Is Undefined", EM_QHierarchySettings.ErrorShowWhenTagOrLayerIsUndefined);
                indentLevel -= 16;

                DrawTextField("类名白名单", EM_QHierarchySettings.ErrorIgnoreString);

                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 Renderer
        /// </summary>
        private void DrawSettingsRenderer()
        {
            if (DrawCheckBox("Renderer", "显示 Renderer 组件图标", EM_QHierarchySettings.RendererShow))
            {
                var rect = GetNewRect(0, 0);
                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.RendererShowDuringPlayMode);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT + UP_DOWN_SPACE * 2);

                DrawSpace(UP_DOWN_SPACE);

                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.RendererShowDuringPlayMode);

                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 Prefab
        /// </summary>
        private void DrawPrefabComponentSettings()
        {
            if (DrawCheckBox("Prefab", "", EM_QHierarchySettings.PrefabShow))
            {
                var rect = GetNewRect(0, 0);

                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.PrefabShowBrakedPrefabsOnly);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT + UP_DOWN_SPACE * 2);
                DrawSpace(UP_DOWN_SPACE);
                DrawCheckBoxRight("仅显示 Missing Prefabs", EM_QHierarchySettings.PrefabShowBrakedPrefabsOnly);
                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 TagLayerName
        /// </summary>
        private void DrawTagLayerNameComponentSettings()
        {
            if (DrawCheckBox("Tag And Layer", "显示 Tag 和 Layer 的名称", EM_QHierarchySettings.TagAndLayerShow))
            {
                var rect = GetNewRect(0, 0);
                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TagAndLayerShowDuringPlayMode);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TagAndLayerSizeShowType);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TagAndLayerType);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TagAndLayerSizeValueType);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TagAndLayerSizeValuePixel);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TagAndLayerSizeValuePercent);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TagAndLayerAlignment);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TagAndLayerLabelSize);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TagAndLayerLabelAlpha);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TagAndLayerTagLabelColor);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TagAndLayerLayerLabelColor);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT * 10 + UP_DOWN_SPACE * 2);
                DrawSpace(UP_DOWN_SPACE);
                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.TagAndLayerShowDuringPlayMode);
                DrawEnum("显示内容", EM_QHierarchySettings.TagAndLayerSizeShowType, typeof(EM_QHierarchyTagAndLayerShowType));
                DrawEnum("显示模式", EM_QHierarchySettings.TagAndLayerType, typeof(EM_QHierarchyTagAndLayerType));

                var newTagAndLayerSizeValueType = (EM_QHierarchyTagAndLayerSizeType) DrawEnum("显示区域宽度", EM_QHierarchySettings.TagAndLayerSizeValueType, typeof(EM_QHierarchyTagAndLayerSizeType));
                if (newTagAndLayerSizeValueType == EM_QHierarchyTagAndLayerSizeType.像素值)
                {
                    DrawIntSlider("像素值", EM_QHierarchySettings.TagAndLayerSizeValuePixel, 5, 300);
                }
                else
                {
                    DrawFloatSlider("百分比", EM_QHierarchySettings.TagAndLayerSizeValuePercent, 0, 0.5f);
                }

                DrawEnum("对齐方式", EM_QHierarchySettings.TagAndLayerAlignment, typeof(EM_QHierarchyTagAndLayerAlignment));
                DrawEnum("字体大小", EM_QHierarchySettings.TagAndLayerLabelSize, typeof(EM_QHierarchyTagAndLayerLabelSize));
                DrawFloatSlider("默认名称的透明度", EM_QHierarchySettings.TagAndLayerLabelAlpha, 0, 1.0f);
                DrawColorPicker("标签文本颜色", EM_QHierarchySettings.TagAndLayerTagLabelColor);
                DrawColorPicker("层级文本颜色", EM_QHierarchySettings.TagAndLayerLayerLabelColor);
                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 Color
        /// </summary>
        private void DrawColorComponentSettings()
        {
            if (DrawCheckBox("Color", "", EM_QHierarchySettings.ColorShow))
            {
                var rect = GetNewRect(0, 0);
                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ColorShowDuringPlayMode);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT + UP_DOWN_SPACE * 2);
                DrawSpace(UP_DOWN_SPACE);
                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.ColorShowDuringPlayMode);
                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 GameObject Icon
        /// </summary>
        private void DrawGameObjectIconComponentSettings()
        {
            if (DrawCheckBox("GameObject Icon", "", EM_QHierarchySettings.GameObjectIconShow))
            {
                var rect = GetNewRect(0, 0);
                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.GameObjectIconShowDuringPlayMode);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.GameObjectIconSize);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT * 2 + UP_DOWN_SPACE * 2);
                DrawSpace(UP_DOWN_SPACE);
                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.GameObjectIconShowDuringPlayMode);
                DrawEnum("图标尺寸大小", EM_QHierarchySettings.GameObjectIconSize, typeof(EM_QHierarchySizeAll));
                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 Tag Icon
        /// </summary>
        private void DrawTagIconComponentSettings()
        {
            if (DrawCheckBox("Tag Icon", "", EM_QHierarchySettings.TagIconShow))
            {
                var tags = UnityEditorInternal.InternalEditorUtility.tags;

                var showTagIconList = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.TagIconListFoldout);

                var rect = GetNewRect(0, 0);
                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TagIconShowDuringPlayMode);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.TagIconSize);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT * 3 + (showTagIconList ? ITEM_SETTING_HEIGHT * tags.Length : 0) + 4 + UP_DOWN_SPACE * 2);

                DrawSpace(UP_DOWN_SPACE);
                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.TagIconShowDuringPlayMode);
                DrawEnum("图标尺寸", EM_QHierarchySettings.TagIconSize, typeof(EM_QHierarchySizeAll));
                if (DrawFoldout("标签图标列表", EM_QHierarchySettings.TagIconListFoldout))
                {
                    var tagTextureList = QHierarchyTagTexture.LoadTagTextureList();

                    var changed = false;
                    foreach (var tag in tags)
                    {
                        var tag1 = tag;
                        var tagTexture = tagTextureList.Find(t => t.tag == tag1);
                        var newTexture = (Texture2D) EditorGUI.ObjectField(GetNewRect(0, 16, 34 + 16, 6), tag, tagTexture?.texture, typeof(Texture2D), false);
                        if (newTexture != null && tagTexture == null)
                        {
                            var newTagTexture = new QHierarchyTagTexture(tag, newTexture);
                            tagTextureList.Add(newTagTexture);

                            changed = true;
                        }
                        else if (newTexture == null && tagTexture != null)
                        {
                            tagTextureList.Remove(tagTexture);
                            changed = true;
                        }
                        else if (tagTexture != null && tagTexture.texture != newTexture)
                        {
                            tagTexture.texture = newTexture;
                            changed = true;
                        }

                        DrawSpace(2);
                    }

                    if (changed)
                    {
                        QHierarchyTagTexture.SaveTagTextureList(EM_QHierarchySettings.TagIconList, tagTextureList);
                        EditorApplication.RepaintHierarchyWindow();
                    }
                }

                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 Layer Icon
        /// </summary>
        private void DrawLayerIconComponentSettings()
        {
            if (DrawCheckBox("Layer Icon", "", EM_QHierarchySettings.LayerIconShow))
            {
                var layers = UnityEditorInternal.InternalEditorUtility.layers;

                var showLayerIconList = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.LayerIconListFoldout);

                var rect = GetNewRect(0, 0);
                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.LayerIconShowDuringPlayMode);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.LayerIconSize);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT * 3 + (showLayerIconList ? ITEM_SETTING_HEIGHT * layers.Length : 0) + 4 + UP_DOWN_SPACE * 2);

                DrawSpace(UP_DOWN_SPACE);
                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.LayerIconShowDuringPlayMode);
                DrawEnum("图标尺寸", EM_QHierarchySettings.LayerIconSize, typeof(EM_QHierarchySizeAll));
                if (DrawFoldout("层级图标列表", EM_QHierarchySettings.LayerIconListFoldout))
                {
                    var layerTextureList = QHierarchyLayerTexture.LoadLayerTextureList();

                    var changed = false;
                    foreach (var layer in layers)
                    {
                        var layer1 = layer;
                        var layerTexture = layerTextureList.Find(t => t.layer == layer1);
                        var newTexture = (Texture2D) EditorGUI.ObjectField(GetNewRect(0, 16, 34 + 16, 6), layer, layerTexture?.texture, typeof(Texture2D), false);
                        if (newTexture != null && layerTexture == null)
                        {
                            var newLayerTexture = new QHierarchyLayerTexture(layer, newTexture);
                            layerTextureList.Add(newLayerTexture);

                            changed = true;
                        }
                        else if (newTexture == null && layerTexture != null)
                        {
                            layerTextureList.Remove(layerTexture);
                            changed = true;
                        }
                        else if (layerTexture != null && layerTexture.texture != newTexture)
                        {
                            layerTexture.texture = newTexture;
                            changed = true;
                        }

                        DrawSpace(2);
                    }

                    if (changed)
                    {
                        QHierarchyLayerTexture.SaveLayerTextureList(EM_QHierarchySettings.LayerIconList, layerTextureList);
                        EditorApplication.RepaintHierarchyWindow();
                    }
                }

                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 Child Count
        /// </summary>
        private void DrawChildrenCountComponentSettings()
        {
            if (DrawCheckBox("Children Count", "", EM_QHierarchySettings.ChildrenCountShow))
            {
                var rect = GetNewRect(0, 0);
                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ChildrenCountShowDuringPlayMode);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ChildrenCountLabelSize);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ChildrenCountLabelColor);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT * 3 + UP_DOWN_SPACE * 2);
                DrawSpace(UP_DOWN_SPACE);
                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.ChildrenCountShowDuringPlayMode);
                DrawEnum("数字大小", EM_QHierarchySettings.ChildrenCountLabelSize, typeof(EM_QHierarchySize));
                DrawColorPicker("数字颜色", EM_QHierarchySettings.ChildrenCountLabelColor);
                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 Vertices And Triangles Count
        /// </summary>
        private void DrawVerticesAndTrianglesCountComponentSettings()
        {
            if (DrawCheckBox("Vertices And Triangles Count", "", EM_QHierarchySettings.VerticesAndTrianglesShow))
            {
                var rect = GetNewRect(0, 0);
                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.VerticesAndTrianglesShowDuringPlayMode);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.VerticesAndTrianglesShowVertices);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.VerticesAndTrianglesShowTriangles);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.VerticesAndTrianglesCalculateTotalCount);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.VerticesAndTrianglesLabelSize);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.VerticesAndTrianglesVerticesLabelColor);
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.VerticesAndTrianglesTrianglesLabelColor);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT * 7 + UP_DOWN_SPACE * 2);
                DrawSpace(UP_DOWN_SPACE);
                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.VerticesAndTrianglesShowDuringPlayMode);
                if (DrawCheckBoxRight("显示网格顶点数", EM_QHierarchySettings.VerticesAndTrianglesShowVertices))
                {
                    if (!QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.VerticesAndTrianglesShowVertices) &&
                        !QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.VerticesAndTrianglesShowTriangles))
                    {
                        QHierarchySettings.Instance().Set(EM_QHierarchySettings.VerticesAndTrianglesShowTriangles, true);
                    }
                }

                if (DrawCheckBoxRight("显示网格三角面数", EM_QHierarchySettings.VerticesAndTrianglesShowTriangles))
                {
                    if (!QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.VerticesAndTrianglesShowVertices) &&
                        !QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.VerticesAndTrianglesShowTriangles))
                    {
                        QHierarchySettings.Instance().Set(EM_QHierarchySettings.VerticesAndTrianglesShowVertices, true);
                    }
                }

                DrawCheckBoxRight("计算时是否包含子物体", EM_QHierarchySettings.VerticesAndTrianglesCalculateTotalCount);
                DrawEnum("字体大小", EM_QHierarchySettings.VerticesAndTrianglesLabelSize, typeof(EM_QHierarchySize));
                DrawColorPicker("顶点数颜色", EM_QHierarchySettings.VerticesAndTrianglesVerticesLabelColor);
                DrawColorPicker("三角面数颜色", EM_QHierarchySettings.VerticesAndTrianglesTrianglesLabelColor);
                DrawSpace(UP_DOWN_SPACE);
            }
        }

        /// <summary>
        /// 绘制 Components
        /// </summary>
        private void DrawComponentsComponentSettings()
        {
            if (DrawCheckBox("Components", "", EM_QHierarchySettings.ComponentsShow))
            {
                var rect = GetNewRect(0, 0);
                if (DrawRestore(28))
                {
                    QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ComponentsShowDuringPlayMode);
                }

                DrawBackground(rect.x, rect.y, rect.width, ITEM_SETTING_HEIGHT * 3 + UP_DOWN_SPACE * 2);
                DrawSpace(UP_DOWN_SPACE);
                DrawCheckBoxRight("播放模式是否启用", EM_QHierarchySettings.ComponentsShowDuringPlayMode);
                DrawTextField("类名白名单", EM_QHierarchySettings.ComponentsIgnore);
                DrawSpace(UP_DOWN_SPACE);
            }
        }

        #endregion

        private void DrawOrderSettings()
        {
            if (DrawRestore(24))
            {
                QHierarchySettings.Instance().Restore(EM_QHierarchySettings.ComponentsOrder);
            }

            indentLevel += 4;

            var componentOrder = QHierarchySettings.Instance().Get<string>(EM_QHierarchySettings.ComponentsOrder);
            var componentIds = componentOrder.Split(';');

            var rect = GetNewRect(position.width, QHierarchyOrderListWindow.ITEM_HEIGHT * componentIds.Length);

            if (orderListWindow == null)
            {
                orderListWindow = new QHierarchyOrderListWindow(this);
            }

            orderListWindow.Draw(rect, componentIds);

            indentLevel -= 4;
        }

        private void DrawAdditionalSettings()
        {
            if (DrawRestore(24))
            {
                QHierarchySettings.Instance().Restore(EM_QHierarchySettings.AdditionalShowHiddenQHierarchyObjectList);
                QHierarchySettings.Instance().Restore(EM_QHierarchySettings.AdditionalHideIconsIfNotFit);
                QHierarchySettings.Instance().Restore(EM_QHierarchySettings.AdditionalIndentation);
                QHierarchySettings.Instance().Restore(EM_QHierarchySettings.AdditionalShowModifierWarning);
                QHierarchySettings.Instance().Restore(EM_QHierarchySettings.AdditionalBackgroundColor);
                QHierarchySettings.Instance().Restore(EM_QHierarchySettings.AdditionalActiveColor);
                QHierarchySettings.Instance().Restore(EM_QHierarchySettings.AdditionalInactiveColor);
                QHierarchySettings.Instance().Restore(EM_QHierarchySettings.AdditionalSpecialColor);
            }

            DrawSpace(UP_DOWN_SPACE);

            DrawCheckBoxRight("显示 QHierarchyObjectList 物体", EM_QHierarchySettings.AdditionalShowHiddenQHierarchyObjectList);
            DrawCheckBoxRight("如果图标比例无法适配则不显示图标", EM_QHierarchySettings.AdditionalHideIconsIfNotFit);
            DrawCheckBoxRight("特殊操作时是否显示弹窗", EM_QHierarchySettings.AdditionalShowModifierWarning);
            DrawIntSlider("右侧缩进", EM_QHierarchySettings.AdditionalIndentation, 0, 500);
            DrawColorPicker("背景色", EM_QHierarchySettings.AdditionalBackgroundColor);
            DrawColorPicker("表示激活的图标颜色", EM_QHierarchySettings.AdditionalActiveColor);
            DrawColorPicker("表示未激活的图标颜色", EM_QHierarchySettings.AdditionalInactiveColor);
            DrawColorPicker("表示特殊状态的图标颜色", EM_QHierarchySettings.AdditionalSpecialColor);

            DrawSpace(UP_DOWN_SPACE);
        }

        #region 界面组件

        /// <summary>
        /// 绘制线
        /// </summary>
        private void DrawLine(float fromY, float toY, Color color, float width = 0)
        {
            var lineWidth = width == 0 ? indentLevel : width;
            var lineHeight = toY - fromY;
            EditorGUI.DrawRect(new Rect(0, fromY, lineWidth, lineHeight), color);
        }

        /// <summary>
        /// 绘制单选框
        /// </summary>
        private bool DrawCheckBox(string labelText, string toolTip, EM_QHierarchySettings hierarchySettings)
        {
            // 定义
            const int SECOND_INDENT = 10;
            const float CHECKED_ICON = 14;

            // 进行 2 级缩进
            indentLevel += SECOND_INDENT;

            // 通知 Layout 绘制指定高度的矩形
            var rect = GetNewRect(0, CHECKED_ICON * 2);

            // 缓存总宽度
            var rectTotalWidth = rect.width;

            // 计算单选框的绘制区域
            rect.y += CHECKED_ICON * 0.5f;
            rect.width = CHECKED_ICON;
            rect.height = CHECKED_ICON;
            var isChecked = QHierarchySettings.Instance().Get<bool>(hierarchySettings);
            var icon = isChecked ? checkBoxOrange : checkBoxGray;
            if (UnityEngine.GUI.Button(rect, icon, GUIStyle.none))
            {
                QHierarchySettings.Instance().Set(hierarchySettings, !isChecked);
            }

            // 计算标题的绘制区域
            rect.x += CHECKED_ICON + 10;
            // Y 坐标上调 (LabelHeight / 2) - (CheckBoxHeight / 2) => (LabelHeight - CheckBoxHeight) / 2
            rect.y -= (EditorGUIUtility.singleLineHeight - rect.height) * 0.5f;
            // 确保覆盖右侧全部留白
            rect.width = rectTotalWidth * 2;
            // 默认高度: 18
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, new GUIContent(labelText, toolTip));

            // 取消 2 级缩进
            indentLevel -= SECOND_INDENT;

            return isChecked;
        }

        /// <summary>
        /// 绘制重置按钮
        /// </summary>
        /// <returns></returns>
        private bool DrawRestore(float restoreMenuHeight)
        {
            const float RIGHT_SPACE = 10;
            const float RESTORE_ICON_WIDTH_HEIGHT = 16;

            var positionX = lastRect.x + lastRect.width - RESTORE_ICON_WIDTH_HEIGHT - RIGHT_SPACE;
            var positionY = lastRect.y - (restoreMenuHeight + RESTORE_ICON_WIDTH_HEIGHT) * 0.5f;
            var rect = new Rect(positionX, positionY, RESTORE_ICON_WIDTH_HEIGHT, RESTORE_ICON_WIDTH_HEIGHT);

            if (UnityEngine.GUI.Button(rect, restoreButtonTexture, GUIStyle.none))
            {
                if (EditorUtility.DisplayDialog("Restore", "Restore default settings?", "Ok", "Cancel"))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 绘制指定高度的空白区域
        /// </summary>
        private void DrawSpace(int height)
        {
            GetNewRect(0, height);
        }

        /// <summary>
        /// 绘制底色背景
        /// </summary>
        private void DrawBackground(float x, float y, float width, float height)
        {
            EditorGUI.DrawRect(new Rect(x, y, width, height), separatorColor);
        }

        /// <summary>
        /// 绘制取色器
        /// </summary>
        private void DrawColorPicker(string label, EM_QHierarchySettings hierarchySettings)
        {
            const int COLOR_PICKER_HEIGHT = 16;
            const int LEFT_INDENT = 34;
            const int RIGHT_INDENT = 10;

            var newRect = GetNewRect(0, COLOR_PICKER_HEIGHT, LEFT_INDENT, RIGHT_INDENT);
            var currentColor = QHierarchySettings.Instance().GetColor(hierarchySettings);
            var newColor = EditorGUI.ColorField(newRect, label, currentColor);
            if (currentColor != newColor)
            {
                QHierarchySettings.Instance().SetColor(hierarchySettings, newColor);
            }

            DrawSpace(2);
        }

        /// <summary>
        /// 绘制附带 Label 的右侧单选框
        /// </summary>
        private bool DrawCheckBoxRight(string label, EM_QHierarchySettings hierarchySettings)
        {
            const int CHECK_BOX_HEIGHT_WIDTH = 16;
            const int LEFT_INDENT = 10 * 2 + CHECK_BOX_HEIGHT_WIDTH;
            const int RIGHT_INDENT = 10;
            const int SPACE_LABEL_CHECKBOX = 4;

            var result = false;
            var isChecked = QHierarchySettings.Instance().Get<bool>(hierarchySettings);
            var isCheckedIcon = isChecked ? checkBoxGreen : checkBoxGray;
            var rect = GetNewRect(0, ITEM_SETTING_HEIGHT, LEFT_INDENT, RIGHT_INDENT);

            // 绘制左侧 Label
            rect.width = rect.width - CHECK_BOX_HEIGHT_WIDTH - SPACE_LABEL_CHECKBOX;
            EditorGUI.LabelField(rect, label);

            // 绘制右侧单选框
            rect.x += rect.width + SPACE_LABEL_CHECKBOX;
            rect.y += 1;
            rect.width = CHECK_BOX_HEIGHT_WIDTH;
            rect.height = CHECK_BOX_HEIGHT_WIDTH;
            if (UnityEngine.GUI.Button(rect, isCheckedIcon, GUIStyle.none))
            {
                QHierarchySettings.Instance().Set(hierarchySettings, !isChecked);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// 绘制分隔符
        /// </summary>
        private void DrawSeparator(int spaceBefore = 0, int spaceAfter = 0, int height = 1)
        {
            if (spaceBefore > 0)
            {
                DrawSpace(spaceBefore);
            }

            var rect = GetNewRect(0, height);

            // 确保覆盖全部留白
            rect.width += 8;

            EditorGUI.DrawRect(rect, separatorColor);

            if (spaceAfter > 0)
            {
                DrawSpace(spaceAfter);
            }
        }

        /// <summary>
        /// 绘制 Int 滑动条
        /// </summary>
        private void DrawIntSlider(string label, EM_QHierarchySettings hierarchySettings, int minValue, int maxValue)
        {
            var rect = GetNewRect(0, 16, 34, 4);

            var currentValue = QHierarchySettings.Instance().Get<int>(hierarchySettings);

            var newValue = EditorGUI.IntSlider(rect, label, currentValue, minValue, maxValue);

            if (!currentValue.Equals(newValue))
            {
                QHierarchySettings.Instance().Set(hierarchySettings, newValue);
            }

            DrawSpace(2);
        }

        /// <summary>
        /// 绘制 Float 滑动条
        /// </summary>
        private void DrawFloatSlider(string label, EM_QHierarchySettings hierarchySettings, float minValue, float maxValue)
        {
            var rect = GetNewRect(0, 16, 34, 4);

            var currentValue = QHierarchySettings.Instance().Get<float>(hierarchySettings);

            var newValue = EditorGUI.Slider(rect, label, currentValue, minValue, maxValue);

            if (!currentValue.Equals(newValue))
            {
                QHierarchySettings.Instance().Set(hierarchySettings, newValue);
            }

            DrawSpace(2);
        }

        /// <summary>
        /// 绘制枚举选单
        /// </summary>
        private Enum DrawEnum(string label, EM_QHierarchySettings hierarchySettings, Type enumType)
        {
            var currentEnum = (Enum) Enum.ToObject(enumType, QHierarchySettings.Instance().Get<int>(hierarchySettings));

            var newEnumValue = EditorGUI.EnumPopup(GetNewRect(0, 16, 34, 6), label, currentEnum);

            if (!newEnumValue.Equals(currentEnum))
            {
                QHierarchySettings.Instance().Set(hierarchySettings, Convert.ToInt32(newEnumValue));
            }

            DrawSpace(2);

            return newEnumValue;
        }

        /// <summary>
        /// 绘制折叠框
        /// </summary>
        private bool DrawFoldout(string label, EM_QHierarchySettings hierarchySettings)
        {
            var foldoutRect = GetNewRect(0, 16, 19, 6);

            var foldoutValue = QHierarchySettings.Instance().Get<bool>(hierarchySettings);

            var newFoldoutValue = EditorGUI.Foldout(foldoutRect, foldoutValue, label);

            if (foldoutValue != newFoldoutValue)
            {
                QHierarchySettings.Instance().Set(hierarchySettings, newFoldoutValue);
            }

            DrawSpace(2);

            return newFoldoutValue;
        }

        /// <summary>
        /// 绘制文本输入框
        /// </summary>
        private void DrawTextField(string label, EM_QHierarchySettings hierarchySettings)
        {
            var currentValue = QHierarchySettings.Instance().Get<string>(hierarchySettings);

            var newValue = EditorGUI.TextField(GetNewRect(0, 16, 34, 6), label, currentValue);

            if (!currentValue.Equals(newValue))
            {
                QHierarchySettings.Instance().Set(hierarchySettings, newValue);
            }

            DrawSpace(2);
        }

        /// <summary>
        /// 绘制文本标签
        /// </summary>
        private void DrawLabel(string label)
        {
            var rect = GetNewRect(0, 16, 34, 6);

            var offset = (EditorGUIUtility.singleLineHeight - rect.height) * 0.5f;

            rect.y -= offset;

            EditorGUI.LabelField(rect, label);

            DrawSpace(2);
        }

        #endregion
    }
}
