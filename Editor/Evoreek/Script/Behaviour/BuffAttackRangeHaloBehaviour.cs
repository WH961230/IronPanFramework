using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
[CreateAssetMenu(menuName = "Evoreek/Behaviour/BuffAttackRangeHaloBehaviour")]
public class BuffAttackRangeHaloBehaviour : GameBehaviour {
    private List<BuffAttackRangeHaloBehaviourData> datas = new List<BuffAttackRangeHaloBehaviourData>();
    public override void Register<T>(T arg1) {
        base.Register(arg1);
        B<int> info = arg1 as B<int>;
        LoadData(info.t1);
        AttackRange();
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
        BuffAttackRangeHaloBehaviourData tempData = new BuffAttackRangeHaloBehaviourData() {
            id = id,
            go = obj,
            attackRangeIncrease = buffInfo.attackRangeIncrease,
            boxCollider = comp.attackWeaponCollider,
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
            if (tempSkillData.buffInfo.functionType == FunctionType.BuffAttackRangeHaloFunction) {
                retInfo = tempSkillData.buffInfo;
            }
        }
        return retInfo;
    }

    private void AttackRange() {
        foreach (var tempData in datas) {
            Vector3 boxSizeVec = tempData.boxCollider.size;
            boxSizeVec.x += tempData.attackRangeIncrease;
            tempData.boxCollider.size = boxSizeVec;
            Logger.Print($"增加攻击范围 {tempData.attackRangeIncrease} 增加后 {boxSizeVec.x}");
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

public class BuffAttackRangeHaloBehaviourData {
    public int id;
    public GameObject go;
    public float attackRangeIncrease;
    public BoxCollider boxCollider;
}