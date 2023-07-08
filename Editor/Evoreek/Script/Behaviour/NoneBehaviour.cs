using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
[CreateAssetMenu(menuName = "Evoreek/Behaviour/NoneBehaviour")]
public class NoneBehaviour : GameBehaviour {
    private List<NoneBehaviourData> datas = new List<NoneBehaviourData>();
    public override void Register<T>(T arg1) {
        base.Register(arg1);
        B<int> info = arg1 as B<int>;
        LoadData(info.t1);
    }
    
    public override void UnRegister<T>(T arg1) {
        base.UnRegister(arg1);
        var info = arg1 as B<int>;
    }

    public override void LoadData(int id) {
        base.LoadData(id);
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        GameObject obj = go.GetObj();
        GameComp comp = go.GetComp();
        Data data = go.GetData();
        
        NoneBehaviourData tempData = new NoneBehaviourData() {
            id = id,
            go = obj,
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
            
        }
    }
}

public class NoneBehaviourData {
    public int id;
    public GameObject go;
}