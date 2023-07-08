using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/SOSceneObjectSetting")]
public class SOSceneObjectSetting : Setting {
    [Header("场景游戏物体列表")] public List<GameObject> SceneObjectGoList;
    [Header("场景物体信息列表")] public List<InstancePointInfo> SceneObjectInfoList;
    [Header("场景物体工具物体")] public GameObject SceneObjectDebugObj;
}