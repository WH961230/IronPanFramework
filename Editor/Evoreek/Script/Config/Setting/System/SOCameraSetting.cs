using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Evoreek/Setting/SOCameraSetting")]
public class SOCameraSetting : Setting {
    [Header("平滑速率")] public float LerpRatio;
    [Header("相机参数")] public List<CameraSetting> cameraSettings;
    [Header("相机相对初始高度")] public float DefaultRelativeHeight;
    [Header("相机偏移角度")] public Vector3 OffEngle;
    [Header("相机滚动缩放速率")] public float ScrollWheelRatio;
    [Header("相机跟随平滑速率")] public float FollowRatio;
    [Header("人称切换速率")] public float ChangeRatio;
    
    [Header("第一人称跟随速度")] public float firstFollowRatio;
    [Header("第一人称相机最低高度")] public float MinFirstHeight;
    [Header("第一人称相机最高高度")] public float MaxFirstHeight;
    
    [Header("第三人称跟随速度")] public float thirdFollowRatio;
    [Header("第三人称相机最高高度")] public float MaxThirdHeight;
    [Header("第三人称相机最低高度")] public float MinThirdHeight;
    
    [Header("相机边缘移动速率")] public float EdgeMoveRatio;
    [Header("单击检测物体范围")] public float CheckObjRange;

    public CameraSetting GetSetting(CameraType type) {
        foreach (var setting in cameraSettings) {
            if (setting.cameraType == type) {
                return setting;
            }
        }

        return default;
    }
}

public enum CameraType {
    MainCamera,
}

[Serializable]
public struct CameraSetting {
    [Header("相机渲染层级")] public List<string> cullingMaskLayers;
    [Header("相机渲染深度")] public int cameraDepth;
    [Header("相机清除标志")] public CameraClearFlags cameraClearFlags;
    [Header("相机类型")] public CameraType cameraType;
    [Header("相机是否正交")] public bool orthographic;
    [Header("相机尺寸")] public float size;
}