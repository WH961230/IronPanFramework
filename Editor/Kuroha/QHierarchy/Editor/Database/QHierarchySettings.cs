using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal delegate void QSettingChangedHandler();

    internal class QHierarchySettings
    {
        private const string SETTINGS_FILE_NAME = "QHierarchySettingsObjectAsset";
        private const string PREFS_PREFIX = "QHierarchy_";
        private const string PREFS_DARK = "Dark_";
        private const string PREFS_LIGHT = "Light_";
        internal const string DEFAULT_ORDER = "0;1;2;3;4;5;6;7;8;9;10;11;12";

        /// <summary>
        /// 默认情况下参与排序的功能的数量
        /// </summary>
        internal const int DEFAULT_ORDER_COUNT = 13;

        private readonly SoQHierarchy settingsObject;
        private readonly Dictionary<EM_QHierarchySettings, object> defaultSettings = new Dictionary<EM_QHierarchySettings, object>();
        private readonly HashSet<int> skinDependedSettings = new HashSet<int>();
        private readonly Dictionary<int, QSettingChangedHandler> settingChangedHandlerList = new Dictionary<int, QSettingChangedHandler>();

        /// <summary>
        /// 单例
        /// </summary>
        private static QHierarchySettings instance;

        internal static QHierarchySettings Instance()
        {
            if (instance == null)
            {
                instance = new QHierarchySettings();
            }

            return instance;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private QHierarchySettings()
        {
            ConfigUtil.CheckConfig(ref settingsObject, nameof(SoQHierarchy), "QHSetting", SETTINGS_FILE_NAME);

            InitSetting(EM_QHierarchySettings.TreeMapShow, false);
            InitSetting(EM_QHierarchySettings.TreeMapColor, "39FFFFFF", "905D5D5D");
            InitSetting(EM_QHierarchySettings.TreeMapEnhanced, true);
            InitSetting(EM_QHierarchySettings.TreeMapTransparentBackground, true);

            InitSetting(EM_QHierarchySettings.MonoBehaviourIconShow, false);
            InitSetting(EM_QHierarchySettings.MonoBehaviourIconShowDuringPlayMode, true);
            InitSetting(EM_QHierarchySettings.MonoBehaviourIconIgnoreUnityMonoBehaviour, true);
            InitSetting(EM_QHierarchySettings.MonoBehaviourIconColor, "A01B6DBB");

            InitSetting(EM_QHierarchySettings.SeparatorShow, false);
            InitSetting(EM_QHierarchySettings.SeparatorShowRowShading, true);
            InitSetting(EM_QHierarchySettings.SeparatorColor, "FF303030", "48666666");
            InitSetting(EM_QHierarchySettings.SeparatorEvenRowShadingColor, "13000000", "08000000");
            InitSetting(EM_QHierarchySettings.SeparatorOddRowShadingColor, "00000000", "00FFFFFF");

            InitSetting(EM_QHierarchySettings.VisibilityShow, false);
            InitSetting(EM_QHierarchySettings.VisibilityShowDuringPlayMode, true);

            InitSetting(EM_QHierarchySettings.LockShow, false);
            InitSetting(EM_QHierarchySettings.LockShowDuringPlayMode, false);
            InitSetting(EM_QHierarchySettings.LockPreventSelectionOfLockedObjects, false);

            InitSetting(EM_QHierarchySettings.StaticShow, false);
            InitSetting(EM_QHierarchySettings.StaticShowDuringPlayMode, false);

            InitSetting(EM_QHierarchySettings.ErrorShow, false);
            InitSetting(EM_QHierarchySettings.ErrorShowDuringPlayMode, false);
            InitSetting(EM_QHierarchySettings.ErrorShowIconOnParent, false);
            InitSetting(EM_QHierarchySettings.ErrorShowScriptIsMissing, true);
            InitSetting(EM_QHierarchySettings.ErrorShowReferenceIsNull, false);
            InitSetting(EM_QHierarchySettings.ErrorShowReferenceIsMissing, true);
            InitSetting(EM_QHierarchySettings.ErrorShowStringIsEmpty, false);
            InitSetting(EM_QHierarchySettings.ErrorShowMissingEventMethod, true);
            InitSetting(EM_QHierarchySettings.ErrorShowWhenTagOrLayerIsUndefined, true);
            InitSetting(EM_QHierarchySettings.ErrorIgnoreString, "");
            InitSetting(EM_QHierarchySettings.ErrorShowForDisabledComponents, true);
            InitSetting(EM_QHierarchySettings.ErrorShowForDisabledGameObjects, true);

            InitSetting(EM_QHierarchySettings.RendererShow, false);
            InitSetting(EM_QHierarchySettings.RendererShowDuringPlayMode, false);

            InitSetting(EM_QHierarchySettings.PrefabShow, false);
            InitSetting(EM_QHierarchySettings.PrefabShowBrakedPrefabsOnly, true);

            InitSetting(EM_QHierarchySettings.TagAndLayerShow, false);
            InitSetting(EM_QHierarchySettings.TagAndLayerShowDuringPlayMode, true);
            InitSetting(EM_QHierarchySettings.TagAndLayerSizeShowType, (int) EM_QHierarchyTagAndLayerShowType.标签和层级都显示);
            InitSetting(EM_QHierarchySettings.TagAndLayerType, (int) EM_QHierarchyTagAndLayerType.仅显示非默认名称);
            InitSetting(EM_QHierarchySettings.TagAndLayerAlignment, (int) EM_QHierarchyTagAndLayerAlignment.Left);
            InitSetting(EM_QHierarchySettings.TagAndLayerSizeValueType, (int) EM_QHierarchyTagAndLayerSizeType.像素值);
            InitSetting(EM_QHierarchySettings.TagAndLayerSizeValuePercent, 0.25f);
            InitSetting(EM_QHierarchySettings.TagAndLayerSizeValuePixel, 75);
            InitSetting(EM_QHierarchySettings.TagAndLayerLabelSize, (int) EM_QHierarchyTagAndLayerLabelSize.Normal);
            InitSetting(EM_QHierarchySettings.TagAndLayerTagLabelColor, "FFCCCCCC", "FF333333");
            InitSetting(EM_QHierarchySettings.TagAndLayerLayerLabelColor, "FFCCCCCC", "FF333333");
            InitSetting(EM_QHierarchySettings.TagAndLayerLabelAlpha, 0.35f);

            InitSetting(EM_QHierarchySettings.ColorShow, false);
            InitSetting(EM_QHierarchySettings.ColorShowDuringPlayMode, true);

            InitSetting(EM_QHierarchySettings.GameObjectIconShow, false);
            InitSetting(EM_QHierarchySettings.GameObjectIconShowDuringPlayMode, true);
            InitSetting(EM_QHierarchySettings.GameObjectIconSize, (int) EM_QHierarchySizeAll.Small);

            InitSetting(EM_QHierarchySettings.TagIconShow, false);
            InitSetting(EM_QHierarchySettings.TagIconShowDuringPlayMode, true);
            InitSetting(EM_QHierarchySettings.TagIconListFoldout, false);
            InitSetting(EM_QHierarchySettings.TagIconList, "");
            InitSetting(EM_QHierarchySettings.TagIconSize, (int) EM_QHierarchySizeAll.Small);

            InitSetting(EM_QHierarchySettings.LayerIconShow, false);
            InitSetting(EM_QHierarchySettings.LayerIconShowDuringPlayMode, true);
            InitSetting(EM_QHierarchySettings.LayerIconListFoldout, false);
            InitSetting(EM_QHierarchySettings.LayerIconList, "");
            InitSetting(EM_QHierarchySettings.LayerIconSize, (int) EM_QHierarchySizeAll.Small);

            InitSetting(EM_QHierarchySettings.ChildrenCountShow, false);
            InitSetting(EM_QHierarchySettings.ChildrenCountShowDuringPlayMode, true);
            InitSetting(EM_QHierarchySettings.ChildrenCountLabelSize, (int) EM_QHierarchySize.Normal);
            InitSetting(EM_QHierarchySettings.ChildrenCountLabelColor, "FFCCCCCC", "FF333333");

            InitSetting(EM_QHierarchySettings.VerticesAndTrianglesShow, false);
            InitSetting(EM_QHierarchySettings.VerticesAndTrianglesShowDuringPlayMode, false);
            InitSetting(EM_QHierarchySettings.VerticesAndTrianglesCalculateTotalCount, false);
            InitSetting(EM_QHierarchySettings.VerticesAndTrianglesShowTriangles, false);
            InitSetting(EM_QHierarchySettings.VerticesAndTrianglesShowVertices, true);
            InitSetting(EM_QHierarchySettings.VerticesAndTrianglesLabelSize, (int) EM_QHierarchySize.Normal);
            InitSetting(EM_QHierarchySettings.VerticesAndTrianglesVerticesLabelColor, "FFCCCCCC", "FF333333");
            InitSetting(EM_QHierarchySettings.VerticesAndTrianglesTrianglesLabelColor, "FFCCCCCC", "FF333333");

            InitSetting(EM_QHierarchySettings.ComponentsShow, false);
            InitSetting(EM_QHierarchySettings.ComponentsShowDuringPlayMode, false);
            InitSetting(EM_QHierarchySettings.ComponentsIconSize, (int) EM_QHierarchySize.Big);
            InitSetting(EM_QHierarchySettings.ComponentsIgnore, "");

            InitSetting(EM_QHierarchySettings.ComponentsOrder, DEFAULT_ORDER);

            InitSetting(EM_QHierarchySettings.AdditionalShowObjectListContent, false);
            InitSetting(EM_QHierarchySettings.AdditionalShowHiddenQHierarchyObjectList, true);
            InitSetting(EM_QHierarchySettings.AdditionalHideIconsIfNotFit, true);
            InitSetting(EM_QHierarchySettings.AdditionalIndentation, 0);
            InitSetting(EM_QHierarchySettings.AdditionalShowModifierWarning, true);

            InitSetting(EM_QHierarchySettings.AdditionalBackgroundColor, "00383838", "00CFCFCF");
            InitSetting(EM_QHierarchySettings.AdditionalActiveColor, "FFFFFF80", "CF363636");
            InitSetting(EM_QHierarchySettings.AdditionalInactiveColor, "FF4F4F4F", "1E000000");
            InitSetting(EM_QHierarchySettings.AdditionalSpecialColor, "FF2CA8CA", "FF1D78D5");
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        internal T Get<T>(EM_QHierarchySettings hierarchySettings)
        {
            return (T) settingsObject.Get<T>(GetSettingName(hierarchySettings));
        }

        /// <summary>
        /// 获取颜色
        /// </summary>
        internal Color GetColor(EM_QHierarchySettings hierarchySettings)
        {
            var stringColor = (string) settingsObject.Get<string>(GetSettingName(hierarchySettings));

            return QHierarchyColorUtils.StringToColor(stringColor);
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        internal void SetColor(EM_QHierarchySettings hierarchySettings, Color color)
        {
            var stringColor = QHierarchyColorUtils.ColorToString(color);

            Set(hierarchySettings, stringColor);
        }

        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="hierarchySettings"></param>
        /// <param name="value"></param>
        /// <param name="invokeChanger"></param>
        /// <typeparam name="T"></typeparam>
        internal void Set<T>(EM_QHierarchySettings hierarchySettings, T value, bool invokeChanger = true)
        {
            var settingId = (int) hierarchySettings;
            settingsObject.Set(GetSettingName(hierarchySettings), value);

            if (invokeChanger && settingChangedHandlerList.ContainsKey(settingId) && settingChangedHandlerList[settingId] != null)
            {
                settingChangedHandlerList[settingId].Invoke();
            }

            EditorApplication.RepaintHierarchyWindow();
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        internal void AddEventListener(EM_QHierarchySettings hierarchySettings, QSettingChangedHandler handler)
        {
            var settingId = (int) hierarchySettings;

            if (!settingChangedHandlerList.ContainsKey(settingId))
            {
                settingChangedHandlerList.Add(settingId, null);
            }

            if (settingChangedHandlerList[settingId] == null)
            {
                settingChangedHandlerList[settingId] = handler;
            }
            else
            {
                settingChangedHandlerList[settingId] += handler;
            }
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        internal void RemoveEventListener(EM_QHierarchySettings hierarchySettings, QSettingChangedHandler handler)
        {
            var settingId = (int) hierarchySettings;

            if (settingChangedHandlerList.ContainsKey(settingId) && settingChangedHandlerList[settingId] != null)
            {
                settingChangedHandlerList[settingId] -= handler;
            }
        }

        /// <summary>
        /// 恢复默认设置
        /// </summary>
        /// <param name="hierarchySettings"></param>
        internal void Restore(EM_QHierarchySettings hierarchySettings)
        {
            Set(hierarchySettings, defaultSettings[hierarchySettings]);
        }

        /// <summary>
        /// 初始化设置
        /// </summary>
        private void InitSetting(EM_QHierarchySettings hierarchySettings, object defaultValueDark, object defaultValueLight)
        {
            skinDependedSettings.Add((int) hierarchySettings);

            InitSetting(hierarchySettings, EditorGUIUtility.isProSkin ? defaultValueDark : defaultValueLight);
        }

        /// <summary>
        /// 初始化设置
        /// </summary>
        private void InitSetting(EM_QHierarchySettings hierarchySettings, object defaultValue)
        {
            var settingName = GetSettingName(hierarchySettings);

            defaultSettings.Add(hierarchySettings, defaultValue);

            var value = settingsObject.Get(settingName, defaultValue);
            if (value == null || value.GetType() != defaultValue.GetType())
            {
                settingsObject.Set(settingName, defaultValue);
            }
        }

        /// <summary>
        /// 获取设置的名称 (枚举 => 字符串)
        /// </summary>
        private string GetSettingName(EM_QHierarchySettings hierarchySettings)
        {
            var settingId = (int) hierarchySettings;
            var settingName = PREFS_PREFIX;

            if (skinDependedSettings.Contains(settingId))
            {
                settingName += EditorGUIUtility.isProSkin ? PREFS_DARK : PREFS_LIGHT;
            }

            settingName += hierarchySettings.ToString("G");

            return settingName;
        }
    }
}
