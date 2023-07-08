using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
[CreateAssetMenu(menuName = "Evoreek/Behaviour/BuffAttackSpeedHaloBehaviour")]
public class BuffAttackSpeedHaloBehaviour : GameBehaviour {
    private List<BuffAttackSpeedHaloBehaviourData> datas = new List<BuffAttackSpeedHaloBehaviourData>();
    public override void Register<T>(T arg1) {
        base.Register(arg1);
        B<int> info = arg1 as B<int>;
        LoadData(info.t1);
        AttackSpeedHalo();//攻击速度光环
    }

    public override void UnRegister<T>(T arg1) {
        base.UnRegister(arg1);
    }

    private void AttackSpeedHalo() {
        foreach (var tempData in datas) {
            tempData.runtimeBaseValueData.ATKSPEED += tempData.attackSpeedIncrease;
            
            Data data = gameSystem.MyEntityFeature.Get(tempData.id).GetData();//数据给玩家
            data.runtimeBaseValueData.ATKSPEED = tempData.runtimeBaseValueData.ATKSPEED;
            
            Logger.Print($"攻击速度增加 {tempData.attackSpeedIncrease} 增加后攻击速度 {data.runtimeBaseValueData.ATKSPEED}");
        }
    }

    public override void LoadData(int id) {
        base.LoadData(id);
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        GameObject obj = go.GetObj();
        GameComp comp = go.GetComp();
        Data data = go.GetData();
        
        BuffInfo buffInfo = GetBuffInfo(data.BuffIds);
        BuffAttackSpeedHaloBehaviourData tempData = new BuffAttackSpeedHaloBehaviourData() {
            id = id,
            go = obj,
            attackSpeedIncrease = buffInfo.attackSpeedIncrease,
            runtimeBaseValueData = data.runtimeBaseValueData,
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
            if (tempSkillData.buffInfo.functionType == FunctionType.BuffAttackSpeedHaloFunction) {
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

public class BuffAttackSpeedHaloBehaviourData {
    public int id;
    public GameObject go;
    public int attackSpeedIncrease;
    public BaseValueData runtimeBaseValueData;
}