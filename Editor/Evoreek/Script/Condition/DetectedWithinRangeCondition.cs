using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 条件
/// </summary>
[CreateAssetMenu(menuName = "Evoreek/Condition/DetectedWithinRangeCondition")]
public class DetectedWithinRangeCondition : GameCondition {
    private List<DetectedWithinRangeConditionData> datas = new List<DetectedWithinRangeConditionData>();
    private DetectedWithinRangeConditionOutput output = new DetectedWithinRangeConditionOutput();
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
        
        DetectedWithinRangeConditionData tempData = new DetectedWithinRangeConditionData() {
            id = id,
            go = obj,
            detectedRange = data.detectedRange,
            detectedObjectType = data.detectedObjectType,
        };

        DetectedWithinRangeConditionOutputChild tempOutput = new DetectedWithinRangeConditionOutputChild() {
            id = id,
            rangeIds = new List<int>(),
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
    
    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
            DetectedWithinRange(tempData);
        }
    }

    private void DetectedWithinRange(DetectedWithinRangeConditionData tempData) {
        DetectedWithinRangeConditionOutputChild retChild = null;
        foreach (var tempChild in output.outputChild) {//获取当前角色子数据
            if (tempChild.id == tempData.id) {
                retChild = tempChild;
                break;
            }
        }

        if (retChild == null) {
            return;
        }

        retChild.rangeIds.Clear();
        foreach (var tempType in tempData.detectedObjectType) {
            List<GameObj> tempSamGameObjList = gameSystem.MyGameObjFeature.GetObjByType(tempType);
            foreach (var tempSameGameObj in tempSamGameObjList) {//检测所有的可检测物体
                float distance = Vector3.Distance(tempData.go.transform.position, tempSameGameObj.GetObj().transform.position);
                if (distance <= tempData.detectedRange) {//检测所有在范围内的角色
                    retChild.rangeIds.Add(tempSameGameObj.GetData().InstanceID);
                }
            }
        }
    }

    public override IOutput GetResult() {
        base.GetResult();
        return output;
    }
}

public class DetectedWithinRangeConditionData {
    public int id;
    public GameObject go;
    public float detectedRange;
    public List<ObjectType> detectedObjectType;
}

public class DetectedWithinRangeConditionOutput : IOutput {
    public List<DetectedWithinRangeConditionOutputChild> outputChild = new List<DetectedWithinRangeConditionOutputChild>();
}

public class DetectedWithinRangeConditionOutputChild {
    public int id;//主动检测的角色
    public List<int> rangeIds;
}