using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
[CreateAssetMenu(menuName = "Evoreek/Behaviour/BuffFleetFootedBehaviour")]
public class BuffFleetFootedBehaviour : GameBehaviour {
    private List<BuffFleetFootedBehaviourData> datas = new List<BuffFleetFootedBehaviourData>();
    public override void Register<T>(T arg1) {
        base.Register(arg1);
        B<int> info = arg1 as B<int>;
        LoadData(info.t1);
        FleetFooted();
    }

    public override void UnRegister<T>(T arg1) {
        base.UnRegister(arg1);
    }

    public override void LoadData(int id) {
        base.LoadData(id);
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        GameObject obj = go.GetObj();
        GameComp comp = go.GetComp();
        Data data = go.GetData();

        BuffInfo buffInfo = GetBuffInfo(data.BuffIds);
        BuffFleetFootedBehaviourData tempData = new BuffFleetFootedBehaviourData() {
            id = id,
            go = obj,
            speedIncrease = buffInfo.speedIncrease,
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
        
        Logger.Print($"注册飞毛腿！");
    }
    
    private BuffInfo GetBuffInfo(List<int> buffIds) {
        BuffInfo retInfo = null;
        foreach (var tempId in buffIds) {
            Data tempSkillData = gameSystem.MyEntityFeature.Get(tempId).GetData();
            if (tempSkillData.buffInfo.functionType == FunctionType.BuffFleetFootedFunction) {
                retInfo = tempSkillData.buffInfo;
            }
        }
        return retInfo;
    }

    private void FleetFooted() {
        foreach (var tempData in datas) {
            Data data = gameSystem.MyEntityFeature.Get(tempData.id).GetData();//数据给玩家
            data.runtimeBaseValueData.ChangeMOVESPEED(tempData.id, tempData.speedIncrease);
        }
    }

    public override void GetControl<T>(T t) {
        base.GetControl(t);
    }

    public override void UpdateData() {
        base.UpdateData();
    }

    public override void Update() {
        base.Update();
        UpdateData();
    }
}

public class BuffFleetFootedBehaviourData {
    public int id;
    public GameObject go;
    public int speedIncrease;
}