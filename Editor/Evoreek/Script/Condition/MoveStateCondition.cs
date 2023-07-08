using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 条件
/// </summary>
[CreateAssetMenu(menuName = "Evoreek/Condition/MoveStateCondition")]
public class MoveStateCondition : GameCondition {
    private List<MoveStateConditionData> datas = new List<MoveStateConditionData>();
    private MoveStateConditionOutput output = new MoveStateConditionOutput();
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
        
        MoveStateConditionData tempData = new MoveStateConditionData() {
            id = id,
            go = obj,
        };

        MoveStateConditionOutputChild tempOutput = new MoveStateConditionOutputChild() {
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
    
    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
        }
    }
    
    public override IOutput GetResult() {
        base.GetResult();
        return output;
    }
}

public class MoveStateConditionData {
    public int id;
    public GameObject go;
}

public class MoveStateConditionOutput : IOutput {
    public List<MoveStateConditionOutputChild> outputChild = new List<MoveStateConditionOutputChild>();
}

public class MoveStateConditionOutputChild {
    public int id;//主动检测的角色
    public bool canEnter;//进入状态
}