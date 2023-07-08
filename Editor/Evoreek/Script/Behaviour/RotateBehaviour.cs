using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 旋转行为
/// </summary>
[CreateAssetMenu(menuName = "Evoreek/Behaviour/RotateBehaviour")]
public class RotateBehaviour : GameBehaviour {
    private List<RotateSettingInfo> infos = new List<RotateSettingInfo>();
    private InputConditionOutput input;
    //forDebug
    public float scrollWheelRatio;
    public float MaxHeight;
    public float MinHeight;
    public float curHeight;//相机高度
    public Vector3 offEngle;

    public override void GetControl<T>(T t) {
        this.input = (InputConditionOutput)Convert.ChangeType(t, typeof(InputConditionOutput));
    }

    #region 注册旋转方式

    public override void Register<T>(T arg1) {
        base.Register(arg1);
        var info = (B<int, Transform, Transform, Transform, Transform, Vector3, Vector3, RotateType>) Convert.ChangeType(arg1,
            typeof(B<int, Transform, Transform, Transform, Transform, Vector3, Vector3, RotateType>));
        infos.Add(new RotateSettingInfo() {
            id = info.t1,
            headTran = info.t2,
            bodyTran = info.t3,
            targetTran = info.t4,
            targetOffTran = info.t5,
            offVec = info.t6,
            offEngle = info.t7,
            RotateType = info.t8,
        });
    }

    #endregion

    public override void Update() {
        base.Update();
    }

    public override void LateUpdate() {
        base.LateUpdate();
        RotateEvent();
    }

    private void RotateEvent() {
        for (int i = 0; i < infos.Count; i++) {
            RotateSettingInfo info = infos[i];
            switch (info.RotateType) {
                case RotateType.RotateSelf:
                    infos[i] = RotateSelfEvent(info);
                    break;
                case RotateType.RotateAround:
                    infos[i] = RotateAround(info);
                    break;
            }
        }
    }

    private RotateSettingInfo RotateSelfEvent(RotateSettingInfo info) {
        info.xRotate -= input.mouseY;
        info.yRotate += input.mouseX;
        info.bodyTran.rotation = Quaternion.Euler(0, info.yRotate, 0);
        info.headTran.rotation = Quaternion.Euler(info.xRotate, info.yRotate, 0);
        return info;
    }

    private RotateSettingInfo RotateAround(RotateSettingInfo info) {
        Quaternion q = Quaternion.Euler(offEngle);
        curHeight = Mathf.Lerp(curHeight, curHeight + input.ScrollWheel, Time.deltaTime * scrollWheelRatio);
        curHeight = Mathf.Max(curHeight, MinHeight);
        curHeight = Mathf.Min(curHeight, MaxHeight);
        Vector3 finalOffVec = new Vector3(0, curHeight, -Mathf.Tan(offEngle.x) * curHeight); // 通过相机高度与相机偏移角度计算出 forward 方向的 z
        Vector3 v = info.targetTran.position + finalOffVec;

        if (info.offEngle == default) {
            info.xRotate -= input.mouseY;
            info.yRotate += input.mouseX;
            q = Quaternion.Euler(info.xRotate, info.yRotate, 0);
        }


        info.targetOffTran.rotation = q;
        info.targetOffTran.position = v;

        return info;
    }
}

public struct RotateSettingInfo {
    public int id;
    public Transform headTran;
    public Transform bodyTran;
    public Transform targetTran; // 围绕旋转物体
    public Transform targetOffTran; // 偏移物体
    public Vector3 offVec;
    public Vector3 offEngle;
    public float xRotate;
    public float yRotate;
    public RotateType RotateType;
}

public enum RotateType {
    RotateSelf, // 类似头部旋转方式 -> 自旋转
    RotateAround, // 观察某个物体旋转方式 -> 环绕旋转
}