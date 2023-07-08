using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Evoreek/Behaviour/HumanoidBehaviour")]
public class HumanoidBehaviour : GameBehaviour {
    private List<HumanoidBehaviourData> datas = new List<HumanoidBehaviourData>();
    private InputConditionOutput input;
    public override void Register<T>(T arg1) { // 注册
        base.Register(arg1);
        var info = arg1 as B<int>;
        int id = info.t1;
        LoadData(id);
    }

    public override void LoadData(int id) {
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        GameObject goObj = go.GetObj();
        GameComp comp = go.GetComp();
        Data data = go.GetData();

        HumanoidBehaviourData tempHumanoidBehaviourData = new HumanoidBehaviourData();
        tempHumanoidBehaviourData.id = id;
        tempHumanoidBehaviourData.go = goObj;
        tempHumanoidBehaviourData.anim = comp.animator;
        tempHumanoidBehaviourData.controller = comp.CC;
        tempHumanoidBehaviourData.agent = comp.agent;
        tempHumanoidBehaviourData.headTran = comp.cameraViewTarget;
        tempHumanoidBehaviourData.playerViewType = data.viewType;
        tempHumanoidBehaviourData.playerMod = data.PlayerMod;
        tempHumanoidBehaviourData.isMainPlayer = data.isMainPlayer;

        tempHumanoidBehaviourData.moveData = new MoveData() {
            slideRatio = data.SlideRatio,
            flyRatio = data.FlyRatio,
            fastFlyRatio = data.FastFlyRatio,
            // walkRatio = data.WalkRatio,
            // runRatio = data.RunRatio,
            backRatio = data.BackRatio,
            firstTurnRatio = data.FirstTurnRatio,
            thirdTurnRatio = data.ThirdTurnRatio,
            walkAnim = data.WalkAnim,
            runAnim = data.RunAnim,
        };

        tempHumanoidBehaviourData.rotateData = new RotateData() {
            xRotateSpeed = data.XRotateSpeed,
            yRotateSpeed = data.YRotateSpeed,
            xRotateMin = data.xRotateMin,
            xRotateMax = data.xRotateMax,
        };

        if (BeRegisteredID.Contains(id)) {
            var reflectedType = MethodBase.GetCurrentMethod().ReflectedType;
            if (reflectedType != null) {
                Logger.PrintELog($"{reflectedType.Name} 重复注册 id: {id}");
            }
        } else {
            BeRegisteredID.Add(id);
            datas.Add(tempHumanoidBehaviourData);
        }
    }
    
    /// <summary>
    /// 数据更新
    /// </summary>
    public override void UpdateData() {
        base.UpdateData();
        foreach (var temp in datas) {
            GameObj go = gameSystem.MyGameObjFeature.Get(temp.id);
            Data data = go.GetData();
            temp.playerMod = data.PlayerMod;
        }
    }

    public override void GetControl<T>(T t) { // 控制
        base.GetControl(t);
        if (t is InputConditionOutput) {
            input = t as InputConditionOutput;
        }
    }

    public override void Update() {
        base.Update();
        UpdateData();
    }
}

public class HumanoidBehaviourData {
    public int id;
    public GameObject go;
    public Animator anim;
    public CharacterController controller;
    public NavMeshAgent agent;
    public MoveData moveData;
    public RotateData rotateData;
    public Transform headTran;//目标头部
    public PlayerViewType playerViewType;//玩家视角
    public PlayerMode playerMod;//玩家模式
    public bool isMainPlayer;
}