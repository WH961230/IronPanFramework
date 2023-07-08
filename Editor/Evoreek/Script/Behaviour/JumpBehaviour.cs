using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动行为 - 位移控制 只关注位移相关的行为
/// </summary>
[CreateAssetMenu(menuName = "Evoreek/Behaviour/JumpBehaviour")]
public class JumpBehaviour : GameBehaviour {
    private List<JumpSettingInfo> infoList = new List<JumpSettingInfo>();
    private InputConditionOutput input;

    public override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
    }

    public override void Register<T>(T arg1) {
        base.Register(arg1);
        var info = (B<int, AudioClip, Transform, CharacterController, float, float>) Convert.ChangeType(
            arg1, typeof(B<int, AudioClip, Transform, CharacterController, float, float>));
        infoList.Add(new JumpSettingInfo() {
            id = info.t1,
            moveClip = info.t2,
            bodyTran = info.t3,
            cc = info.t4,
            runRatio = info.t5,
            ratio = info.t6,
        });
    }

    public override void GetControl<T>(T t) {
        this.input = (InputConditionOutput)Convert.ChangeType(t, typeof(InputConditionOutput));
    }

    public override void Update() {
        base.Update();
        JumpEvent();
    }

    private void JumpEvent() {
        for (int i = 0; i < infoList.Count; i++) {
            var info = infoList[i];
            var controller = info.cc;
            var moveDirection = info.moveDirection;
            if (GameData.GetTerrainPos(info.bodyTran.position, out var pos)) {
                if (input.Space) {
                    moveDirection.y = 300f;
                }
            }

            controller.Move(moveDirection * Time.deltaTime);
        }
    }
}

public struct JumpSettingInfo {
    public int id;
    public AudioClip moveClip; // 移动音效
    public Vector3 perviousPosition;
    public Transform bodyTran; // 身体
    public CharacterController cc; // 控制器
    public float ratio; // 移动系数
    public float runRatio; // 跑步系数
    public Vector3 moveDirection;
}