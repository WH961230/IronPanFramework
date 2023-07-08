using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动行为 - 位移控制 只关注位移相关的行为
/// </summary>
[CreateAssetMenu(menuName = "Evoreek/Behaviour/MoveBehaviour")]
public class MoveBehaviour : GameBehaviour {
    private List<MoveSettingInfo> infoList = new List<MoveSettingInfo>();
    private InputConditionOutput input;
    private GameSystem MyGameSystem;

    public override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        MyGameSystem = gameSystem;
    }

    public override void Register<T>(T arg1) {
        base.Register(arg1);
        var info = (B<int, AudioClip, Transform, CharacterController, float, float, Animator>) Convert.ChangeType(arg1, typeof(B<int, AudioClip, Transform, CharacterController, float, float, Animator>));
        infoList.Add(new MoveSettingInfo() {
            id = info.t1,
            moveClip = info.t2,
            bodyTran = info.t3,
            cc = info.t4,
            runRatio = info.t5,
            ratio = info.t6,
            animator = info.t7,
        });
    }

    public override void GetControl<T>(T t) {
        this.input = (InputConditionOutput)Convert.ChangeType(t, typeof(InputConditionOutput));
    }

    public override void Update() {
        base.Update();
        Move();
    }

    private void Move() {
        for (int i = 0; i < infoList.Count; i++) {
            var list = infoList[i];
            var vec = new Vector3(input.x, input.y, input.z) * list.ratio * Time.deltaTime;
            infoList[i] = MoveCharacterControllerEvent(list, vec);
        }
    }

    private MoveSettingInfo MoveCharacterControllerEvent(MoveSettingInfo list, Vector3 vec) {
        float ratio = 0;
        if (input.Shift) {
            ratio = list.runRatio;
        } else {
            ratio = list.ratio;
        }

        list.cc.Move(list.bodyTran.transform.forward * input.z * ratio * Time.deltaTime);
        list.cc.Move(list.bodyTran.transform.right * input.x * ratio * Time.deltaTime);

        var deltaPosition = list.bodyTran.position - list.perviousPosition;
        if (deltaPosition.x != 0 || deltaPosition.z != 0) {
            list.perviousPosition = list.bodyTran.position;
            // 此处释放音效 后期音效需要重构
            // MyGameSystem.messageCenter.Dispatcher(MessageConstant.ConsoleLogMsg, $"id: {list.id} 玩家移动中", true);
        }
        return list;
    }
}

public struct MoveSettingInfo {
    public int id;
    public AudioClip moveClip; // 移动音效
    public Vector3 perviousPosition;
    public Transform bodyTran; // 身体
    public CharacterController cc; // 控制器
    public float ratio; // 移动系数
    public float runRatio; // 跑步系数
    public Animator animator; // 状态机
}

public enum MoveType {
    CharacterController,
}