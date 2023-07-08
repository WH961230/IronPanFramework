using System.Collections.Generic;
using Kuroha.QHierarchy.RunTime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyObjectListManager
    {
        private static QHierarchyObjectListManager instance;

        internal static QHierarchyObjectListManager Instance()
        {
            if (instance == null)
            {
                instance = new QHierarchyObjectListManager();
            }

            return instance;
        }

        /// <summary>
        /// 上次选择的数量
        /// </summary>
        private int lastSelectionCount;

        /// <summary>
        /// 上次选择的物体
        /// </summary>
        private GameObject lastSelectionGameObject;

        /// <summary>
        /// 是否显示物品列表
        /// </summary>
        internal bool showObjectList;

        /// <summary>
        /// 显示特殊物体列表
        /// </summary>
        private bool showList;

        /// <summary>
        /// 播放模式下也显示特殊物体列表
        /// </summary>
        private bool showListPlayMode;

        /// <summary>
        /// 特殊物体脚本所在的游戏物体名称
        /// </summary>
        private const string Q_OBJECT_LIST_NAME = "QHierarchyObjectList";

        /// <summary>
        /// QHierarchyObjectList 字典
        /// 不同的场景分开保存
        /// </summary>
        private readonly Dictionary<UnityEngine.SceneManagement.Scene, QHierarchyObjectList> objectListDictionary = new Dictionary<UnityEngine.SceneManagement.Scene, QHierarchyObjectList>();

        /// <summary>
        /// 上次打开的场景
        /// </summary>
        private UnityEngine.SceneManagement.Scene lastActiveScene;

        /// <summary>
        /// 上次选择的场景数量
        /// </summary>
        private int lastSceneCount;

        /// <summary>
        /// 构造函数
        /// </summary>
        private QHierarchyObjectListManager()
        {
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.AdditionalShowHiddenQHierarchyObjectList, OnSettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.LockPreventSelectionOfLockedObjects, OnSettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.LockShow, OnSettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.LockShowDuringPlayMode, OnSettingsChanged);

            OnSettingsChanged();
        }

        /// <summary>
        /// 更改设置
        /// </summary>
        private void OnSettingsChanged()
        {
            showObjectList = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.AdditionalShowHiddenQHierarchyObjectList);
            showList = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.LockShow) && QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.LockPreventSelectionOfLockedObjects);
            showListPlayMode = showList && QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.LockShowDuringPlayMode);
        }

        /// <summary>
        /// 检测是否选择发生了更改
        /// </summary>
        private bool IsSelectionChanged()
        {
            if (lastSelectionGameObject != Selection.activeGameObject || lastSelectionCount != Selection.gameObjects.Length)
            {
                lastSelectionGameObject = Selection.activeGameObject;
                lastSelectionCount = Selection.gameObjects.Length;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 验证
        /// </summary>
        internal void Validate()
        {
            QHierarchyObjectList.instances.RemoveAll(item => item == null);

            foreach (var objectList in QHierarchyObjectList.instances)
            {
                objectList.CheckIntegrity();
            }

            objectListDictionary.Clear();

            foreach (var objectList in QHierarchyObjectList.instances)
            {
                objectListDictionary.Add(objectList.gameObject.scene, objectList);
            }
        }

        /// <summary>
        /// 编辑器更新
        /// </summary>
        internal void OnEditorUpdate()
        {
            var objectListInstance = QHierarchyObjectList.instances;
            var objectListCount = objectListInstance.Count;
            if (objectListCount > 0)
            {
                for (var index = 0; index < objectListCount; ++index)
                {
                    var objectList = objectListInstance[index];
                    var objectListScene = objectList.gameObject.scene;

                    if (objectListDictionary.ContainsKey(objectListScene))
                    {
                        // 移除字典中的空值
                        if (objectListDictionary[objectListScene] == null)
                        {
                            objectListDictionary.Remove(objectListScene);
                        }
                        else if (objectListDictionary[objectListScene] != objectList)
                        {
                            objectListDictionary[objectListScene].Merge(objectList);
                            Object.DestroyImmediate(objectList.gameObject);
                        }
                    }
                    else
                    {
                        objectListDictionary.Add(objectListScene, objectList);
                    }
                }

                foreach (var objectList in objectListDictionary.Values)
                {
                    if (objectList != null)
                    {
                        SetupObjectList(objectList);

                        var objectListGameObject = objectList.gameObject;
                        var flag1 = showObjectList && (objectListGameObject.hideFlags & HideFlags.HideInHierarchy) > 0;
                        var flag2 = !showObjectList && (objectListGameObject.hideFlags & HideFlags.HideInHierarchy) == 0;

                        if (flag1 || flag2)
                        {
                            objectList.gameObject.hideFlags ^= HideFlags.HideInHierarchy;
                            EditorApplication.DirtyHierarchyWindowSorting();
                        }
                    }
                    else
                    {
                        Validate();
                        return;
                    }
                }

                if (showList && !Application.isPlaying || Application.isPlaying && showListPlayMode && IsSelectionChanged())
                {
                    var selectionGameObjects = Selection.gameObjects;
                    var actual = new List<GameObject>(selectionGameObjects.Length);
                    var found = false;

                    foreach (var obj in selectionGameObjects)
                    {
                        if (objectListDictionary.ContainsKey(obj.scene))
                        {
                            if (objectListDictionary[obj.scene].lockedObjects.Contains(obj))
                            {
                                found = true;
                            }
                            else
                            {
                                actual.Add(obj);
                            }
                        }
                    }

                    if (found)
                    {
                        var array = actual.ToArray();
                        if (array is Object[] objArray)
                        {
                            Selection.objects = objArray;
                        }
                    }
                }

                lastActiveScene = SceneManager.GetActiveScene();
                lastSceneCount = SceneManager.loadedSceneCount;
            }
        }

        /// <summary>
        /// 得到特殊物体列表
        /// </summary>
        internal QHierarchyObjectList GetObjectList(GameObject gameObject, bool createIfNotExist = true)
        {
            objectListDictionary.TryGetValue(gameObject.scene, out var hierarchyObjectList);

            if (hierarchyObjectList == null && createIfNotExist)
            {
                hierarchyObjectList = CreateObjectList();

                if (gameObject.scene != hierarchyObjectList.gameObject.scene)
                {
                    SceneManager.MoveGameObjectToScene(hierarchyObjectList.gameObject, gameObject.scene);
                }

                objectListDictionary.Add(gameObject.scene, hierarchyObjectList);
            }

            return hierarchyObjectList;
        }

        /// <summary>
        /// 创建特殊物体列表
        /// </summary>
        private static QHierarchyObjectList CreateObjectList()
        {
            var gameObjectList = new GameObject(Q_OBJECT_LIST_NAME);

            var hierarchyObjectList = gameObjectList.AddComponent<QHierarchyObjectList>();

            SetupObjectList(hierarchyObjectList);

            return hierarchyObjectList;
        }

        /// <summary>
        /// 设置特殊物体列表脚本: QHierarchyObjectList
        /// </summary>
        /// <param name="hierarchyObjectList"></param>
        private static void SetupObjectList(MonoBehaviour hierarchyObjectList)
        {
            if (hierarchyObjectList.CompareTag("EditorOnly"))
            {
                hierarchyObjectList.tag = "Untagged";
            }

            var monoScript = MonoScript.FromMonoBehaviour(hierarchyObjectList);
            if (MonoImporter.GetExecutionOrder(monoScript) != -10000)
            {
                MonoImporter.SetExecutionOrder(monoScript, -10000);
            }
        }

        /// <summary>
        /// 判断场景是否变化
        /// </summary>
        /// <returns></returns>
        internal bool IsSceneChanged()
        {
            var flag1 = lastActiveScene != SceneManager.GetActiveScene();
            var flag2 = lastSceneCount != SceneManager.loadedSceneCount;

            return flag1 || flag2;
        }
    }
}
