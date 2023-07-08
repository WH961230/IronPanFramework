using UnityEngine;

public class CameraGameObj : GameObj {
    private CameraData cameraData;
    private CameraComponent cameraComponent;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        cameraData = (CameraData)data;

        cameraComponent = MyObj.GetComponent<CameraComponent>();
        cameraData.camera = cameraComponent.camera;
        cameraData.camera.cullingMask = LayerMask.GetMask(cameraData.cullingMaskLayers.ToArray());
        cameraData.camera.depth = cameraData.cameraDepth;
        cameraData.camera.clearFlags = cameraData.cameraClearFlags;
        cameraData.camera.gameObject.tag = cameraData.cameraTag;
        cameraData.camera.orthographic = cameraData.orthographic;
        cameraData.camera.orthographicSize = cameraData.orthographic ? cameraData.size : 0;

        //赋值注视物体的信息
        cameraData.viewTPSTargetGo = GameData.MainPlayer.GetData().viewTPSTargetGo;
        cameraData.viewType = GameData.MainPlayer.GetData().viewType;
        Register();

        gameSystem.messageCenter.Reg<float, Vector3, float, float>(MessageConstant.ShakeCamera, (vecDur, shakeVec, ratDur, strength) => {
            // cameraComponent.camera.DOShakePosition(vecDur, shakeVec);
            // cameraComponent.camera.DOShakeRotation(ratDur, strength);
            ClockUtil.Instance.AlarmAfter(ratDur, () => {
                cameraComponent.camera.transform.localPosition = Vector3.zero;
                cameraComponent.camera.transform.localRotation = Quaternion.Euler(Vector3.zero);
            });
        });
    }

    public override void Clear() {
        gameSystem.messageCenter.UnReg<float, Vector3, float, float>(MessageConstant.ShakeCamera, (vecDur, shakeVec, ratDur, strength) => {
            // cameraComponent.camera.DOShakePosition(vecDur, shakeVec);
            // cameraComponent.camera.DOShakeRotation(ratDur, strength);
            ClockUtil.Instance.AlarmAfter(ratDur, () => {
                cameraComponent.camera.transform.localPosition = Vector3.zero;
                cameraComponent.camera.transform.localRotation = Quaternion.Euler(Vector3.zero);
            });
        });
        base.Clear();
    }

    public override void Update() {
        base.Update();
    }

    private void Register() {
        foreach (var temp in cameraData.FunctionTypes) {
            Register(temp, new B<int> {
                t1 = cameraData.InstanceID
            });
        }
    }

    private void UnRegister(FunctionType type) {
        UnRegister(type, new B<int> {
            t1 = cameraData.InstanceID
        });
    }
}