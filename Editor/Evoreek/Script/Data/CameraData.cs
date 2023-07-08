using System.Collections.Generic;
using UnityEngine;

public class CameraData : Data {
    public float lerpRatio;
    public string cameraTag;
    public CameraType cameraType;
    public List<string> cullingMaskLayers;
    public int cameraDepth;
    public CameraClearFlags cameraClearFlags;
    public bool orthographic;
    public float size;
    
    //相机参数
    // public Vector3 offEngle;
    // public float defaultRelativeHeight;//默认相对高度
    // public float curHeight;//相机实际高度
    // public float minFirstHeight;
    // public float maxFirstHeight;
    // public float firstFollowRatio;
    //
    // public float minThirdHeight;
    // public float maxThirdHeight;
    // public float scrollWheelRatio;
    // public float thirdFollowRatio;
    // public float changeRatio;

    // //视角类型
    // public PlayerViewType viewType;
    // public float edgeMoveRatio;
    public float checkObjRange;
}