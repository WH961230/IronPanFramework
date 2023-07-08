using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
[CreateAssetMenu(menuName = "Evoreek/Behaviour/BuffWarGodBlessedBehaviour")]
public class BuffWarGodBlessedBehaviour : GameBehaviour {
    private List<BuffWarGodBlessedBehaviourData> datas = new List<BuffWarGodBlessedBehaviourData>();
    public override void Register<T>(T arg1) {
        base.Register(arg1);
        B<int> info = arg1 as B<int>;
        LoadData(info.t1);
        WarGodBlessed();
    }
    
    public override void UnRegister<T>(T arg1) {
        base.UnRegister(arg1);
    }
    
    private void WarGodBlessed() {
        foreach (var tempData in datas) {
            
            Data data = gameSystem.MyEntityFeature.Get(tempData.id).GetData();//数据给玩家
            data.runtimeBaseValueData.ChangeATK(tempData.id, tempData.attackIncrease);
            
            // Logger.Print($"攻击力增加 {tempData.attackIncrease} 增加后攻击力 {data.runtimeBaseValueData.ATK}");
        }
    }

    public override void LoadData(int id) {
        base.LoadData(id);
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        GameObject obj = go.GetObj();
        GameComp comp = go.GetComp();
        Data data = go.GetData();
        
        BuffInfo buffInfo = GetBuffInfo(data.BuffIds);
        BuffWarGodBlessedBehaviourData tempData = new BuffWarGodBlessedBehaviourData() {
            id = id,
            go = obj,
            attackIncrease = buffInfo.attackIncrease,
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
    
    private BuffInfo GetBuffInfo(List<int> buffIds) {
        BuffInfo retInfo = null;
        foreach (var tempId in buffIds) {
            Data tempSkillData = gameSystem.MyEntityFeature.Get(tempId).GetData();
            if (tempSkillData.buffInfo.functionType == FunctionType.BuffWarGodBlessedFunction) {
                retInfo = tempSkillData.buffInfo;
            }
        }
        return retInfo;
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

public class BuffWarGodBlessedBehaviourData {
    public int id;
    public GameObject go;
    public int attackIncrease;
}