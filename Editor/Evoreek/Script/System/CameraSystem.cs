using System;
using System.Reflection;
using UnityEngine;

public class CameraSystem : GameSys {
    private SOCameraSetting cameraSetting;
    private GameSystem gameSystem;

    public CameraSystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        Init(gameSystem);
    }

    protected sealed override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        cameraSetting = gameSystem.SoData.SOGameSetting.GetSetting<SOCameraSetting>();
        setting = cameraSetting;
        gameSystem.messageCenter.Reg(MessageConstant.MainPlayerLoadFinished, MsgInstance);
    }

    protected override void Clear() {
        gameSystem.messageCenter.UnReg(MessageConstant.MainPlayerLoadFinished, MsgInstance);
        base.Clear();
    }

    private void MsgInstance() {
        if (GameData.MainCamera == null) {
            GameData.MainCamera = gameSystem.MyGameObjFeature.Get<CameraGameObj>(Instance());
        }

        if (GameData.BigHeadCamera == null) {
            GameData.BigHeadCamera = gameSystem.MyGameObjFeature.Get<BigHeadCameraGameObj>(InstanceBigHeadCamera());
        }
    }

    public int Instance() {
        return Instance<CameraGameObj, CameraEntity, CameraData>((data) => {
            data.MyObjName = "主相机";
            data.offEngle = cameraSetting.OffEngle;
            data.defaultRelativeHeight = cameraSetting.DefaultRelativeHeight;
            data.minFirstHeight = cameraSetting.MinFirstHeight;
            data.maxFirstHeight = cameraSetting.MaxFirstHeight;
            data.firstFollowRatio = cameraSetting.firstFollowRatio;
            data.minThirdHeight = cameraSetting.MinThirdHeight;
            data.maxThirdHeight = cameraSetting.MaxThirdHeight;
            data.scrollWheelRatio = cameraSetting.ScrollWheelRatio;
            data.thirdFollowRatio = cameraSetting.thirdFollowRatio;
            data.changeRatio = cameraSetting.ChangeRatio;
            data.edgeMoveRatio = cameraSetting.EdgeMoveRatio;
            data.checkObjRange = cameraSetting.CheckObjRange;
            
            data.lerpRatio = cameraSetting.LerpRatio;
            data.cameraTag = "MainCamera";
            data.cameraType = CameraType.MainCamera;
            data.cameraDepth = cameraSetting.GetSetting(CameraType.MainCamera).cameraDepth;
            data.cameraClearFlags = cameraSetting.GetSetting(CameraType.MainCamera).cameraClearFlags;
            data.cullingMaskLayers = cameraSetting.GetSetting(CameraType.MainCamera).cullingMaskLayers;
            data.orthographic = cameraSetting.GetSetting(CameraType.MainCamera).orthographic;
            data.size = cameraSetting.GetSetting(CameraType.MainCamera).size;
        });
    }

    public int InstanceBigHeadCamera() {
        return Instance<SOBigHeadCameraSetting, BigHeadCameraGameObj, BigHeadCameraEntity, BigHeadCameraData>(gameSystem.SoData.SOGameSetting.GetSetting<SOBigHeadCameraSetting>(), data => {
            data.MyObjName = "大头相机";
            data.MyStartPointInfo = new PointInfo() {
                engle = Vector3.zero,
                vec = new Vector3(0, -100, 0),
            };
        });
    }
}