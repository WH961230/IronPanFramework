using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 场景编辑器管理器
/// </summary>
public class SceneEditorManager : MonoBehaviour {
#if UNITY_EDITOR
    [Header("非运行数据")]
    public SOSceneObjectSetting Sceneobjsetting;
    public GameObject sceneobjectRoot;
    public GameObject sceneobjectDebugRoot;

    [Header("运行数据")]
    public bool isEditorMode;
    public List<GameObject> sceneObjectList = new List<GameObject>();
    public GameObject TerrainGO;//地形物体
    public List<GameObject> sceneObjectDebugList = new List<GameObject>();

    private void Start() {
    }

    private void Update() {
    }

    /// <summary>
    /// 加载地形
    /// </summary>
    public void LoadTerrain() {
        if (TerrainGO != null) {
            Logger.Print("地形存在重复创建 - 此操作无效");
            return;
        }
        Logger.Print("加载地形");
        var go = AssetsLoad.GetAsset<GameObject>("Evoreek_", "地形", AssetsLoad.AssetType.Prefab);
        TerrainGO = Instantiate(go);
    }

    /// <summary>
    /// 销毁地形
    /// </summary>
    public void DestroyTerrain() {
        Logger.Print("销毁地形");
        if (TerrainGO != null) {
            DestroyImmediate(TerrainGO);
            TerrainGO = null;
        }
    }

    /// <summary>
    /// 加载所有的场景物体
    /// </summary>
    public void LoadAllSceneObject() {
        if (sceneObjectList != null && sceneObjectList.Count > 0) {
            DestroyAllSceneObject();
        }

        Logger.Print("加载所有的场景物体");
        //根据场景物体设置的物体列表创建所有场景物品
        foreach (var info in Sceneobjsetting.SceneObjectInfoList) {
            var go = Instantiate(info.obj, sceneobjectRoot.transform, true);
            go.transform.position = info.pos;
            go.transform.rotation = Quaternion.Euler(info.engle);
            sceneObjectList.Add(go);
        }
    }

    /// <summary>
    /// 销毁场景物体
    /// </summary>
    public void DestroyAllSceneObject() {
        Logger.Print("销毁场景物体");
        if (sceneObjectList != null && sceneObjectList.Count > 0) {
            foreach (var go in sceneObjectList) {
                DestroyImmediate(go);
            }

            sceneObjectList.Clear();
        }
    }

    /// <summary>
    /// 加载所有场景调试物体
    /// </summary>
    public void LoadAllSceneObjectDebug() {
        if (sceneObjectDebugList != null && sceneObjectDebugList.Count > 0) {
            DestroyAllSceneObjectDebug();
        }

        foreach (var o in Sceneobjsetting.SceneObjectInfoList) {
            GameObject tempGO = Instantiate(Sceneobjsetting.SceneObjectDebugObj);
            tempGO.transform.SetParent(sceneobjectDebugRoot.transform);
            tempGO.transform.position = o.pos;
            tempGO.transform.rotation = Quaternion.Euler(o.engle);
            sceneObjectDebugList.Add(tempGO);
        }
    }

    public void DestroyAllSceneObjectDebug() {
        Logger.Print("销毁场景调试物体");
        if (sceneObjectDebugList != null && sceneObjectDebugList.Count > 0) {
            foreach (var go in sceneObjectDebugList) {
                DestroyImmediate(go);
            }

            sceneObjectDebugList.Clear();
        }
    }

    /// <summary>
    /// 保存所有场景调试物体到配置
    /// </summary>
    public void SaveAllSceneObjDebug() {
        Sceneobjsetting.SceneObjectInfoList = null;
        Sceneobjsetting.SceneObjectInfoList = new List<InstancePointInfo>();
        for (int i = 0; i < sceneobjectDebugRoot.transform.childCount; i++) {
            var child = sceneobjectDebugRoot.transform.GetChild(i).gameObject;
            Sceneobjsetting.SceneObjectInfoList.Add(new InstancePointInfo() {
                obj = Sceneobjsetting.SceneObjectGoList[Random.Range(0, Sceneobjsetting.SceneObjectGoList.Count)],
                pos = child.transform.position,
                engle = child.transform.rotation.eulerAngles,
            });
            Logger.Print($"保存点位信息{child.name}");
        }
        EditorUtility.SetDirty(Sceneobjsetting);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }    
#endif
}