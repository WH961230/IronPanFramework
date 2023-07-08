using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 条件
/// </summary>
[CreateAssetMenu(menuName = "Evoreek/Condition/PickCondition")]
public class PickCondition : GameCondition {
    private List<PickConditionData> datas = new List<PickConditionData>();
    private PickConditionOutput output = new PickConditionOutput();
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
        
        PickConditionData tempData = new PickConditionData() {
            id = id,
            go = obj,
            detectDistance = data.detectDistance,
            objectTypes = data.pickObjectTypes,
        };

        PickConditionOutputChild tempOutput = new PickConditionOutputChild() {
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

    public override IOutput GetResult() {
        base.GetResult();
        return output;
    }

    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
            Pick(tempData);
        }
    }

    private void Pick(PickConditionData data) {
        PickConditionOutputChild tempOutputChild = null;
        foreach (var tempChild in output.outputChild) {//获取当前角色子数据
            if (tempChild.id == data.id) {
                tempOutputChild = tempChild;
                break;
            }
        }

        if (tempOutputChild == null) {
            return;
        }

        if (!data.go.activeSelf) {
            return;
        }

        if (data.objectTypes == null || data.objectTypes.Count == 0) {
            return;
        }

        tempOutputChild.pickId = 0;
        //主体检测周围所有范围内的游戏物体，如果他是可拾取类型则记录ID
        foreach (var tempType in data.objectTypes) {
            List<GameObj> tempSameObjTypeGoList = gameSystem.MyGameObjFeature.GetObjByType(tempType);
            foreach (var tempTypeGo in tempSameObjTypeGoList) {
                Data tempData = tempTypeGo.GetData();
                int tempId = tempData.InstanceID;
                Vector3 goVec = tempTypeGo.GetObj().transform.position;
                ObjectType type = tempData.ObjectType;
                float distance = Vector3.Distance(data.go.transform.position, goVec);
                if (distance < data.detectDistance && type == ObjectType.PLAYER) {
                    tempOutputChild.pickId = tempId;
                    return;
                }
            }
        }
    }
}

public class PickConditionData {
    public int id;
    public GameObject go;
    public float detectDistance;//检测距离
    public List<ObjectType> objectTypes;
}

public class PickConditionOutput : IOutput {
    public List<PickConditionOutputChild> outputChild = new List<PickConditionOutputChild>();
}

public class PickConditionOutputChild {
    public int id;//主动检测的角色
    public int pickId;//拾取的角色
}