using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
[CreateAssetMenu(menuName = "Evoreek/Behaviour/AIAttackBehaviour")]
public class AIAttackBehaviour : GameBehaviour {
    private List<AIAttackBehaviourData> datas = new List<AIAttackBehaviourData>();
    private DetectedMoveConditionOutput detectedMoveOutput;
    
    public override void Register<T>(T arg1) {
        base.Register(arg1);
        B<int> info = arg1 as B<int>;
        LoadData(info.t1);
    }

    public override void UnRegister<T>(T arg1) {
        base.UnRegister(arg1);
        
        B<int> info = arg1 as B<int>;
        if (BeRegisteredID.Contains(info.t1)) {
            BeRegisteredID.Remove(info.t1);

            foreach (var tempData in datas) {
                if (tempData.id == info.t1) {
                    tempData.animator.SetBool("Attack", false);
                }
            }

            int index = -1;
            foreach (var tempData in datas) {
                if (tempData.id == info.t1) {
                    index = datas.IndexOf(tempData);
                    break;
                }
            }

            if (index != -1) {
                datas.RemoveAt(index);
            }
        }
    }

    public override void LoadData(int id) {
        base.LoadData(id);
        
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        GameObject obj = go.GetObj();
        GameComp comp = go.GetComp();
        Data data = go.GetData();
        
        AIAttackBehaviourData tempData = new AIAttackBehaviourData(gameSystem) {
            id = id,
            go = obj,
            animator = comp.animator,
            runtimeBaseValueData = data.runtimeBaseValueData,
            runtimeBaseStateData = data.baseRuntimeState,
        };
        
        if (BeRegisteredID.Contains(id)) {
            var reflectedType = MethodBase.GetCurrentMethod().ReflectedType;
            if (reflectedType != null) {
                Logger.PrintELog($"{reflectedType.Name} 重复注册 id: {id}");
            }
        } else {
            BeRegisteredID.Add(id);
            datas.Add(tempData);
        }
    }

    public override void UpdateData() {
        base.UpdateData();
    }

    public override void GetControl<T>(T t) {
        base.GetControl(t);
        if (t is DetectedMoveConditionOutput) {
            detectedMoveOutput = t as DetectedMoveConditionOutput;
        }
    }

    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
            tempData.OnUpdate();
            AIAttack(tempData);
        }
    }

    private void AIAttack(AIAttackBehaviourData data) {
        DetectedMoveConditionOutputChild child = GetDetectedData(data);
        AIAttack(data, child);
    }

    private DetectedMoveConditionOutputChild GetDetectedData(AIAttackBehaviourData data) {
        List<DetectedMoveConditionOutputChild> child = detectedMoveOutput.outputChild;
        foreach (var tempChild in child) {
            if (tempChild.id == data.id) {
                return tempChild;
            }
        }

        return null;
    }

    private void AIAttack(AIAttackBehaviourData data, DetectedMoveConditionOutputChild child) {
        int id = child.DetectedMoveId;//攻击目标id
        
        //玩家优先选中的怪物
        if (GameData.MainPlayerID == data.id) {
            if (GameData.DetectedAttackTargetID.ContainsKey(data.id) && GameData.DetectedAttackTargetID[data.id] != 0) {
                id = GameData.DetectedAttackTargetID[data.id];
            }
        }

        GameObj detectedGameObj = gameSystem.MyGameObjFeature.Get(id);
        if (detectedGameObj == null) {//检测物体找不到
            StopAttack(data);
            return;
        }
        GameObject detectedObj = detectedGameObj.GetObj();
        bool isDead = detectedGameObj.GetData().baseRuntimeState.isDead;
        if (isDead) {//死亡停止攻击
            StopAttack(data);
            return;
        }

        float distance = Vector3.Distance(data.go.transform.position, detectedObj.transform.position);
        if (distance <= data.runtimeBaseValueData.ATKRANGE && !data.runtimeBaseStateData.isMove) {//攻击范围内且处于攻击状态
            if (data.id == GameData.MainPlayerID) {
                data.go.transform.forward = (detectedObj.transform.position - data.go.transform.position).normalized;//朝向目标
            }

            if (GameData.DetectedAttackTargetID.ContainsKey(data.id)) {
                GameData.DetectedAttackTargetID[data.id] = id;
            } else {
                GameData.DetectedAttackTargetID.Add(data.id, id);
            }
            
            StartAttack(data);
        } else {
            StopAttack(data);
        }
    }

    private void StartAttack(AIAttackBehaviourData data) {
        data.animator.speed = (float)data.runtimeBaseValueData.ATKSPEED / 10;
        data.animator.SetBool("Attack", true);
        data.runtimeBaseStateData.isAttack = true;
    }

    private void StopAttack(AIAttackBehaviourData data) {
        data.animator.speed = 1;
        data.animator.SetBool("Attack", false);
        data.runtimeBaseStateData.isAttack = false;
        if (GameData.DetectedAttackTargetID.ContainsKey(data.id)) {
            GameData.DetectedAttackTargetID[data.id] = 0;
        }
    }
}

public class AIAttackBehaviourData {
    public int id;
    public GameObject go;
    public Data data;
    public Animator animator;
    public BaseValueData runtimeBaseValueData;
    public BaseRuntimeState runtimeBaseStateData;
    private GameSystem gameSystem;
    public GameObject attackMonsterAudio;

    public AIAttackBehaviourData (GameSystem gameSystem) {
        this.gameSystem = gameSystem;
    }

    public void OnUpdate() {
        if (data != null) {
            data.baseRuntimeState.isAttack = runtimeBaseStateData.isAttack;//更新
        } else {
            data = gameSystem?.MyEntityFeature.Get(id).GetData();
        }
    }
}