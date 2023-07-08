using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 条件
/// </summary>
[CreateAssetMenu(menuName = "Evoreek/Condition/DetectedMoveCondition")]
public class DetectedMoveCondition : GameCondition {
    
    private List<DetectedMoveConditionData> datas = new List<DetectedMoveConditionData>();
    private DetectedMoveConditionOutput output = new DetectedMoveConditionOutput();
    public override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
    }
    
    public override void Clear() {
        base.Clear();
    }

    public override void Register<T>(T arg1) {
        base.Register(arg1);
        B<int> info = arg1 as B<int>;
        int id = info.t1;
        LoadData(id);
    }

    protected override void LoadData(int id) {
        base.LoadData(id);
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        GameObject obj = go.GetObj();
        Data data = go.GetData();

        DetectedMoveConditionData tempData = new DetectedMoveConditionData() {//获取需要检测的敌人数据
            id = id,
            go = obj,
            destinationInfo = data.DestinationInfo,
        };

        DetectedMoveConditionOutputChild tempOutput = new DetectedMoveConditionOutputChild() {
            id = id,
        };
        
        if (BeRegisteredID.Contains(id)) {
            var reflectedType = MethodBase.GetCurrentMethod().ReflectedType;
            if (reflectedType != null) {
                Logger.PrintELog($"{reflectedType.Name} 重复注册 id: {id}");
            }
        } else {
            BeRegisteredID.Add(id);
            datas.Add(tempData);
            output.outputChild.Add(tempOutput);
        }
    }

    protected override void UpdateData() {
        base.UpdateData();
    }

    public override IOutput GetResult() {
        base.GetResult();
        return output;
    }

    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
            DetectedWithWeightAndDistance(tempData);
        }
    }

    private void DetectedWithWeightAndDistance(DetectedMoveConditionData tempData) {
        DetectedMoveConditionOutputChild tempOutputChild = null;
        foreach (var tempChild in output.outputChild) {//获取当前角色子数据
            if (tempChild.id == tempData.id) {
                tempOutputChild = tempChild;
                break;
            }
        }

        if (tempOutputChild == null) {
            return;
        }

        Data data = gameSystem.MyEntityFeature.Get(tempData.id).GetData();
        if (data.baseRuntimeState.isDead) {//当检测的物体死亡 停止检测
            return;
        }

        tempOutputChild.DetectedMoveId = 0;
        //权重距离判断的玩法：优先遍历降序的权重，将权重高的类型的物体全部取出没有先后之分，判断是否在距离内，
        //如果该类型的都没有，则下一类型，直到穷举所有的配置物体，如果都没有则无操作结束。
        tempData.destinationInfo.Sort((x, y) => -x.weight.CompareTo(y.weight));
        foreach (var tempInfo in tempData.destinationInfo) {
            List<GameObj> tempSameObjTypeGoList = gameSystem.MyGameObjFeature.GetObjByType(tempInfo.destinationObjectType);
            foreach (var tempObj in tempSameObjTypeGoList) {
                BaseRuntimeState runtimeState = tempObj.GetData().baseRuntimeState;
                if (runtimeState.isDead) {
                    continue;
                }
                float tempDistance = Vector3.Distance(tempData.go.transform.position, tempObj.GetObj().transform.position);
                if (tempDistance < tempInfo.detectedMoveDistance) {
                    tempOutputChild.DetectedMoveId = tempObj.GetData().InstanceID;
                    // Logger.Print($"{tempData.go.transform.name} 检测到目标： {tempObj.GetData().MyObjName}");
                    return;
                }
            }
        }
    }
}

public class DetectedMoveConditionData {
    public int id;
    public GameObject go;
    public List<DestinationInfo> destinationInfo;
}

public class DetectedMoveConditionOutput : IOutput {
    public List<DetectedMoveConditionOutputChild> outputChild = new List<DetectedMoveConditionOutputChild>();
}

public class DetectedMoveConditionOutputChild {
    public int id;//主动检测的角色
    public int DetectedMoveId;//检测到的角色ID
}