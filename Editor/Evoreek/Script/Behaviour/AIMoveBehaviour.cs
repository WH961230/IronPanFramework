using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AI移动行为
/// </summary>
[CreateAssetMenu(menuName = "Evoreek/Behaviour/AIMoveBehaviour")]
public class AIMoveBehaviour : GameBehaviour {
    private List<AIMoveBehaviourData> datas = new List<AIMoveBehaviourData>();
    private DetectedMoveConditionOutput detectedMoveOutput;
    private MoveStateConditionOutput moveStateOutpout;
    public override void Register<T>(T arg1) {
        base.Register(arg1);
        B<int> info = arg1 as B<int>;
        LoadData(info.t1);
    }

    public override void UnRegister<T>(T arg1) {
        base.UnRegister(arg1);
        
        B<int> info = arg1 as B<int>;//反注册
        if (BeRegisteredID.Contains(info.t1)) {
            BeRegisteredID.Remove(info.t1);
            
            foreach (var tempData in datas) {
                if (tempData.id == info.t1) {
                    tempData.nav.isStopped = true;
                    tempData.anim.SetFloat("Vertical", 0);
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
        Data bata = go.GetData();
        
        AIMoveBehaviourData tempData = new AIMoveBehaviourData() {
            id = id,
            go = obj,
            nav = comp.agent,
            anim = comp.animator,
            runtimeBaseValueData = bata.runtimeBaseValueData,
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
        } else if (t is MoveStateConditionOutput) {
            moveStateOutpout = t as MoveStateConditionOutput;
        }
    }

    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
            // bool stateCheck = StateCheck(tempData);
            // if (!stateCheck) {
            //     continue;
            // }
            DetectedDestination(tempData);
            AIMove(tempData);
        }
    }

    private bool StateCheck(AIMoveBehaviourData tempData) {
        foreach (var tempChild in moveStateOutpout.outputChild) {
            if (tempChild.id == tempData.id && tempChild.canEnter) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 检测目的地
    /// </summary>
    private void DetectedDestination(AIMoveBehaviourData tempData) {
        if (detectedMoveOutput == null) {
            return;
        }

        foreach (var tempChild in detectedMoveOutput.outputChild) {
            if (tempChild.id != tempData.id) {
                continue;
            }

            if (tempChild.DetectedMoveId != 0) {
                tempData.destinationId = tempChild.DetectedMoveId;
                tempData.destinationVec = gameSystem.MyGameObjFeature.Get(tempChild.DetectedMoveId).GetObj().transform.position; //获取最近检测到的角色的位置
            } else {
                tempData.destinationId = 0;
                tempData.destinationVec = Vector3.zero;
            }

            break;
        }
    }

    /// <summary>
    /// 机器人移动行为
    /// </summary>
    private void AIMove(AIMoveBehaviourData tempData) {
        if (detectedMoveOutput == null) {
            return;
        }

        if (tempData.nav == null || !tempData.nav.gameObject.activeSelf) {
            return;
        }

        bool isDetected = false;
        foreach (var tempChild in detectedMoveOutput.outputChild) {
            if (tempChild.id == tempData.id && tempChild.DetectedMoveId != 0) {
                isDetected = true;
            }
        }

        if (!isDetected) {//如果没有检测到物体 禁止不动
            return;
        }

        tempData.nav.destination = tempData.destinationVec;//敌人向目标打射线，检测到的点为目的地
        if (Vector3.Distance(tempData.go.transform.position, tempData.destinationVec) < tempData.runtimeBaseValueData.ATKRANGE) {
            StopTargetMove(tempData);
        } else {
            if (tempData.id != GameData.MainPlayerID && !tempData.runtimeData.isMoving) {
                Logger.Print($"id:{tempData.id} 开始移动");
            }
            Debug.DrawLine(tempData.go.transform.position, tempData.nav.destination, Color.red);
            tempData.runtimeData.isMoving = true;
            tempData.nav.isStopped = false;
            tempData.nav.speed = tempData.runtimeBaseValueData.MOVESPEED;
            tempData.anim.SetFloat("Vertical", tempData.runtimeBaseValueData.MOVESPEED);
        }
    }

    private static void StopTargetMove(AIMoveBehaviourData tempData) {
        tempData.nav.isStopped = true;
        tempData.runtimeData.isMoving = false;
        tempData.anim.SetFloat("Vertical", 0);
    }
}

public class AIMoveBehaviourData {
    public int id;
    public GameObject go;
    public NavMeshAgent nav;
    public Animator anim;
    public Vector3 destinationVec;//目标点
    public int destinationId;//目标标识
    public BaseValueData runtimeBaseValueData;//基础信息
    public AIMoveBehaviourRuntimeData runtimeData = new AIMoveBehaviourRuntimeData();
}

public class AIMoveBehaviourRuntimeData {
    public bool isMoving;
}
