using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
[CreateAssetMenu(menuName = "Evoreek/Behaviour/BuffBloodsuckingHaloBehaviour")]
public class BuffBloodsuckingHaloBehaviour : GameBehaviour {
    private List<BuffBloodsuckingHaloBehaviourData> datas = new List<BuffBloodsuckingHaloBehaviourData>();
    public override void Register<T>(T arg1) {
        base.Register(arg1);
        B<int> info = arg1 as B<int>;
        LoadData(info.t1);
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
        
        BuffBloodsuckingHaloBehaviourData tempData = new BuffBloodsuckingHaloBehaviourData() {
            id = id,
            go = obj,
            runtimeData = data.baseRuntimeData,
            suckBloodValue = data.baseRuntimeData.TotalCausedDamageAmount,
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
    
    public override void GetControl<T>(T t) {
        base.GetControl(t);
    }

    public override void UpdateData() {
        base.UpdateData();
    }

    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
            Bloodsucking(tempData);
        }
    }

    private void Bloodsucking(BuffBloodsuckingHaloBehaviourData data) {
        //如果角色的攻击总量有变化，直接吸血
        if (data.runtimeData.TotalCausedDamageAmount != data.suckBloodValue) {
            int diffVal = data.runtimeData.TotalCausedDamageAmount - data.suckBloodValue;
            float tempVal = diffVal * (float)data.runtimeBaseValueData.HPINTAKE / 100;
            diffVal = (int) tempVal;
            gameSystem.MyEntityFeature.Get(data.id).HPChange(data.id, diffVal);
            data.suckBloodValue = data.runtimeData.TotalCausedDamageAmount;
        }
    }
}

public class BuffBloodsuckingHaloBehaviourData {
    public int id;
    public GameObject go;
    public BaseRuntimeData runtimeData;
    public BaseValueData runtimeBaseValueData;
    public int suckBloodValue;
}