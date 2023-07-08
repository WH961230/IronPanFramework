using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Kuroha.QHierarchy.RunTime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyComponentError : QHierarchyBaseComponent
    {
        /// <summary>
        /// 有 Error 时图标的颜色
        /// </summary>
        private Color activeColor;

        /// <summary>
        /// 无 Error 时图标的颜色
        /// </summary>
        private Color inactiveColor;

        /// <summary>
        /// Error 提示的图标
        /// </summary>
        private readonly Texture2D errorIconTexture;

        private bool settingsShowErrorOfChildren;
        private bool settingsShowErrorTypeReferenceIsNull;
        private bool settingsShowErrorTypeReferenceIsMissing;
        private bool settingsShowErrorTypeStringIsEmpty;
        private bool settingsShowErrorIconScriptIsMissing;
        private bool settingsShowErrorIconWhenTagIsUndefined;
        private bool settingsShowErrorForDisabledComponents;
        private bool settingsShowErrorIconMissingEventMethod;
        private bool settingsShowErrorForDisabledGameObjects;

        /// <summary>
        /// 忽略错误的关键字列表
        /// </summary>
        private List<string> ignoreErrorOfMonoBehaviours;

        private int errorCount;
        private const int RECT_WIDTH = 7;
        private readonly StringBuilder errorMessageStringBuilder = new StringBuilder();
        private readonly List<string> targetFieldNames = new List<string>(10);

        /// <summary>
        /// 构造函数初始化
        /// </summary>
        public QHierarchyComponentError()
        {
            rect.width = RECT_WIDTH;

            errorIconTexture = QHierarchyResources.Instance().GetTexture(EM_QHierarchyTexture.QErrorIcon);

            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ErrorShowIconOnParent, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ErrorShowReferenceIsNull, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ErrorShowReferenceIsMissing, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ErrorShowStringIsEmpty, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ErrorShowScriptIsMissing, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ErrorShowForDisabledComponents, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ErrorShowForDisabledGameObjects, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ErrorShowMissingEventMethod, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ErrorShowWhenTagOrLayerIsUndefined, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ErrorShow, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ErrorShowDuringPlayMode, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.ErrorIgnoreString, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.AdditionalActiveColor, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.AdditionalInactiveColor, SettingsChanged);

            SettingsChanged();
        }

        /// <summary>
        /// 设置更改
        /// </summary>
        private void SettingsChanged()
        {
            activeColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.AdditionalActiveColor);
            inactiveColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.AdditionalInactiveColor);

            enabled = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ErrorShow);
            settingsShowErrorOfChildren = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ErrorShowIconOnParent);
            settingsShowErrorTypeReferenceIsNull = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ErrorShowReferenceIsNull);
            settingsShowErrorTypeReferenceIsMissing = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ErrorShowReferenceIsMissing);
            settingsShowErrorTypeStringIsEmpty = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ErrorShowStringIsEmpty);
            settingsShowErrorIconScriptIsMissing = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ErrorShowScriptIsMissing);
            settingsShowErrorForDisabledComponents = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ErrorShowForDisabledComponents);
            settingsShowErrorForDisabledGameObjects = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ErrorShowForDisabledGameObjects);
            settingsShowErrorIconMissingEventMethod = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ErrorShowMissingEventMethod);
            settingsShowErrorIconWhenTagIsUndefined = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ErrorShowWhenTagOrLayerIsUndefined);
            showComponentDuringPlayMode = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.ErrorShowDuringPlayMode);

            var ignoreErrorOfMonoBehavioursString = QHierarchySettings.Instance().Get<string>(EM_QHierarchySettings.ErrorIgnoreString);
            if (!string.IsNullOrEmpty(ignoreErrorOfMonoBehavioursString))
            {
                ignoreErrorOfMonoBehaviours = new List<string>(ignoreErrorOfMonoBehavioursString.Split(new char[] {',', ';', '.', ' '}));
                ignoreErrorOfMonoBehaviours.RemoveAll(string.IsNullOrEmpty);
            }
            else
            {
                ignoreErrorOfMonoBehaviours = null;
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
            var components = gameObjectToDraw.GetComponents<MonoBehaviour>();
            if (FindError(gameObjectToDraw, components, false))
            {
                UnityEngine.GUI.color = activeColor;
                UnityEngine.GUI.DrawTexture(rect, errorIconTexture);
                UnityEngine.GUI.color = QHierarchyColorUtils.DefaultColor;
            }
            else if (settingsShowErrorOfChildren)
            {
                var children = gameObjectToDraw.GetComponentsInChildren<MonoBehaviour>(true);
                if (FindError(gameObjectToDraw, children, false))
                {
                    UnityEngine.GUI.color = inactiveColor;
                    UnityEngine.GUI.DrawTexture(rect, errorIconTexture);
                    UnityEngine.GUI.color = QHierarchyColorUtils.DefaultColor;
                }
            }
        }

        /// <summary>
        /// 单击事件处理
        /// </summary>
        internal override void EventHandler(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Event currentEvent)
        {
            // 鼠标左键单击
            if (currentEvent.isMouse && currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition))
            {
                currentEvent.Use();

                errorCount = 0;
                errorMessageStringBuilder.Clear();

                FindError(gameObject, gameObject.GetComponents<MonoBehaviour>(), true);

                if (errorCount > 0)
                {
                    var title = errorCount + (errorCount == 1 ? " error was found" : " errors were found");
                    EditorUtility.DisplayDialog(title, errorMessageStringBuilder.ToString(), "OK", "Cancel");
                }
            }
        }

        /// <summary>
        /// 查找错误
        /// </summary>
        private bool FindError(GameObject gameObject, in MonoBehaviour[] components, bool isFoundAllError)
        {
            if (CheckRenderer(gameObject, isFoundAllError))
            {
                return true;
            }

            if (settingsShowErrorIconWhenTagIsUndefined)
            {
                #region Tag 未定义

                if (string.IsNullOrEmpty(gameObject.tag))
                {
                    // 如果仅仅是为了检查是否有 Error, 而不是打印全部 Error 信息, 可以立刻返回
                    if (!isFoundAllError)
                    {
                        return true;
                    }

                    AppendErrorLine("tag is undefined");
                }

                #endregion

                #region Layer 未定义

                if (string.IsNullOrEmpty(LayerMask.LayerToName(gameObject.layer)))
                {
                    // 如果仅仅是为了检查是否有 Error, 而不是打印全部 Error 信息, 可以立刻返回
                    if (!isFoundAllError)
                    {
                        return true;
                    }

                    AppendErrorLine("layer is undefined");
                }

                #endregion
            }

            for (var i = 0; i < components.Length; i++)
            {
                var monoBehaviour = components[i];

                #region 组件丢失

                if (monoBehaviour == null)
                {
                    if (settingsShowErrorIconScriptIsMissing)
                    {
                        // 如果仅仅是为了检查是否有 Error, 而不是打印全部 Error 信息, 可以立刻返回
                        if (!isFoundAllError)
                        {
                            return true;
                        }

                        AppendErrorLine($"component <{i}> is missing");
                    }
                }

                #endregion

                else
                {
                    #region 白名单过滤

                    if (ignoreErrorOfMonoBehaviours != null)
                    {
                        for (var index = ignoreErrorOfMonoBehaviours.Count - 1; index >= 0; index--)
                        {
                            var classFullName = monoBehaviour.GetType().FullName;
                            if (classFullName != null && classFullName.IndexOf(ignoreErrorOfMonoBehaviours[index], StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                return false;
                            }
                        }
                    }

                    #endregion

                    #region 检查事件方法丢失

                    if (settingsShowErrorIconMissingEventMethod)
                    {
                        if (monoBehaviour.gameObject.activeSelf || settingsShowErrorForDisabledComponents)
                        {
                            if (IsUnityEventsNullOrMissing(monoBehaviour, isFoundAllError))
                            {
                                if (!isFoundAllError)
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    #endregion

                    if (settingsShowErrorTypeReferenceIsNull || settingsShowErrorTypeStringIsEmpty || settingsShowErrorTypeReferenceIsMissing)
                    {
                        // 组件未激活 && 不显示隐藏组件的错误
                        if (!monoBehaviour.enabled && !settingsShowErrorForDisabledComponents)
                        {
                            continue;
                        }

                        // 物体未激活 && 不显示隐藏物体的错误
                        if (!monoBehaviour.gameObject.activeSelf && !settingsShowErrorForDisabledGameObjects)
                        {
                            continue;
                        }

                        // 得到组件的类型
                        var classInfo = monoBehaviour.GetType();
                        while (classInfo != null)
                        {
                            // 如果是 Unity 的组件类, 获取全部的公有非静态字段
                            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

                            // 如果是自己定义的组件类, 那么也获取私有字段
                            if (!classInfo.FullName.Contains("UnityEngine"))
                            {
                                bindingFlags |= BindingFlags.NonPublic;
                            }

                            var fieldInfoArray = classInfo.GetFields(bindingFlags);
                            foreach (var fieldInfo in fieldInfoArray)
                            {
                                #region 不检查在 Inspector 面板不显示的字段

                                if (System.Attribute.IsDefined(fieldInfo, typeof(HideInInspector)) || System.Attribute.IsDefined(fieldInfo, typeof(NonSerializedAttribute)) ||
                                    System.Attribute.IsDefined(fieldInfo, typeof(QHierarchyNullableAttribute)) || fieldInfo.IsStatic)
                                {
                                    continue;
                                }

                                if ((fieldInfo.IsPrivate || !fieldInfo.IsPublic) &&
                                    !System.Attribute.IsDefined(fieldInfo, typeof(SerializeField)))
                                {
                                    continue;
                                }

                                #endregion

                                var fieldValue = fieldInfo.GetValue(monoBehaviour);

                                #region 检查字符串类型变量是否为空

                                if (settingsShowErrorTypeStringIsEmpty)
                                {
                                    if (fieldInfo != null && fieldInfo.FieldType == typeof(string) && string.IsNullOrEmpty(fieldValue as string))
                                    {
                                        // 如果仅仅是为了检查是否有 Error, 而不是打印全部 Error 信息, 可以立刻返回
                                        if (!isFoundAllError)
                                        {
                                            return true;
                                        }

                                        AppendErrorLine(monoBehaviour.GetType().Name + "." + fieldInfo.Name + ": String value is empty");
                                        continue;
                                    }
                                }

                                #endregion

                                #region 检查组件引用是否丢失

                                if (settingsShowErrorTypeReferenceIsMissing)
                                {
                                    if (fieldValue is Component component && component == null)
                                    {
                                        // 如果仅仅是为了检查是否有 Error, 而不是打印全部 Error 信息, 可以立刻返回
                                        if (!isFoundAllError)
                                        {
                                            return true;
                                        }

                                        AppendErrorLine(monoBehaviour.GetType().Name + "." + fieldInfo.Name + ": Reference is missing");
                                        continue;
                                    }
                                }

                                #endregion

                                #region 检查字段的值是否为空

                                if (settingsShowErrorTypeReferenceIsNull)
                                {
                                    if (fieldValue == null || fieldValue.Equals(null))
                                    {
                                        // 如果仅仅是为了检查是否有 Error, 而不是打印全部 Error 信息, 可以立刻返回
                                        if (!isFoundAllError)
                                        {
                                            return true;
                                        }

                                        AppendErrorLine(monoBehaviour.GetType().Name + "." + fieldInfo.Name + ": Reference is null");
                                        continue;
                                    }
                                }

                                #endregion

                                #region 检查可遍历字段的元素值是否为空

                                if (settingsShowErrorTypeReferenceIsNull && fieldValue is IEnumerable enumerable)
                                {
                                    foreach (var item in enumerable)
                                    {
                                        if (item == null || item.Equals(null))
                                        {
                                            // 如果仅仅是为了检查是否有 Error, 而不是打印全部 Error 信息, 可以立刻返回
                                            if (!isFoundAllError)
                                            {
                                                return true;
                                            }

                                            AppendErrorLine(monoBehaviour.GetType().Name + "." + fieldInfo.Name + ": IEnumerable has value with null reference");
                                        }
                                    }
                                }

                                #endregion
                            }

                            classInfo = classInfo.BaseType;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 检查 Renderer
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="isFoundAllError"></param>
        /// <returns></returns>
        private bool CheckRenderer(GameObject gameObject, bool isFoundAllError)
        {
            var renderers = gameObject.GetComponents<Renderer>();
            foreach (var renderer in renderers)
            {
                if (renderer.sharedMaterials == null || renderer.sharedMaterials.Length == 0)
                {
                    if (!isFoundAllError)
                    {
                        return true;
                    }

                    AppendErrorLine($"renderer <{renderer.name}> has not material");
                }
                else if (renderer.sharedMaterials.Length > 0)
                {
                    foreach (var material in renderer.sharedMaterials)
                    {
                        if (material == null)
                        {
                            if (!isFoundAllError)
                            {
                                return true;
                            }

                            AppendErrorLine($"renderer <{renderer.name}> has missing material");
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 检测是否有空的 Unity 事件
        /// </summary>
        private bool IsUnityEventsNullOrMissing(UnityEngine.Object monoBehaviour, bool isFoundAllError)
        {
            targetFieldNames.Clear();

            // 反射得到全部的字段
            var fieldArray = monoBehaviour.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // 筛选出全部的 UnityEventBase 类
            foreach (var fieldInfo in fieldArray)
            {
                if (fieldInfo.FieldType == typeof(UnityEventBase) || fieldInfo.FieldType.IsSubclassOf(typeof(UnityEventBase)))
                {
                    targetFieldNames.Add(fieldInfo.Name);
                }
            }

            if (targetFieldNames.Count > 0)
            {
                var serializedMonoBehaviour = new SerializedObject(monoBehaviour);
                foreach (var fieldName in targetFieldNames)
                {
                    var property = serializedMonoBehaviour.FindProperty(fieldName);
                    var propertyRelativeArray = property.FindPropertyRelative("m_PersistentCalls.m_Calls");

                    for (var index = 0; index < propertyRelativeArray.arraySize; index++)
                    {
                        var propertyRelative = propertyRelativeArray.GetArrayElementAtIndex(index);

                        #region 检查事件物体引用

                        var propertyTarget = propertyRelative.FindPropertyRelative("m_Target");
                        if (propertyTarget.objectReferenceValue == null)
                        {
                            // 如果仅仅是为了检查是否有 Error, 而不是打印全部 Error 信息, 可以立刻返回
                            if (!isFoundAllError)
                            {
                                return true;
                            }

                            AppendErrorLine(monoBehaviour.GetType().Name + ": 事件物体引用为空!");
                        }

                        #endregion

                        #region 检查监听事件

                        var propertyMethodName = propertyRelative.FindPropertyRelative("m_MethodName");
                        if (string.IsNullOrEmpty(propertyMethodName.stringValue))
                        {
                            // 如果仅仅是为了检查是否有 Error, 而不是打印全部 Error 信息, 可以立刻返回
                            if (!isFoundAllError)
                            {
                                return true;
                            }

                            AppendErrorLine(monoBehaviour.GetType().Name + ": 监听事件为空!");
                        }

                        #endregion

                        #region 监听事件

                        var argumentAssemblyName = propertyRelative.FindPropertyRelative("m_Arguments").FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue;
                        var argumentAssemblyType = string.IsNullOrEmpty(argumentAssemblyName) ? typeof(UnityEngine.Object) : System.Type.GetType(argumentAssemblyName, false) ?? typeof(UnityEngine.Object);
                        var typeName = property.FindPropertyRelative("m_TypeName");
                        if (typeName != null)
                        {
                            var propertyTypeName = System.Type.GetType(typeName.stringValue, false);
                            var dummyEvent = propertyTypeName == null ? new UnityEvent() : Activator.CreateInstance(propertyTypeName) as UnityEventBase;
                            if (dummyEvent != null)
                            {
                                var persistentListenerMode = (PersistentListenerMode) propertyRelative.FindPropertyRelative("m_Mode").enumValueIndex;
                                if (!UnityEventDrawer.IsPersistantListenerValid(dummyEvent, propertyMethodName.stringValue, propertyTarget.objectReferenceValue, persistentListenerMode, argumentAssemblyType))
                                {
                                    // 如果仅仅是为了检查是否有 Error, 而不是打印全部 Error 信息, 可以立刻返回
                                    if (!isFoundAllError)
                                    {
                                        return true;
                                    }

                                    AppendErrorLine(monoBehaviour.GetType().Name + ": Event handler function is missing");
                                }
                            }
                        }

                        #endregion
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 增加 1 条错误
        /// </summary>
        private void AppendErrorLine(string error)
        {
            errorCount++;
            errorMessageStringBuilder.Append(errorCount.ToString());
            errorMessageStringBuilder.Append(": ");
            errorMessageStringBuilder.AppendLine(error);
        }
    }
}
