using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
[CreateAssetMenu(menuName = "Evoreek/Behaviour/GravityBehaviour")]
public class GravityBehaviour : GameBehaviour {
    private List<GravityBehaviourData> datas = new List<GravityBehaviourData>();
    public override void Register<T>(T arg1) {
        base.Register(arg1); 
        var info = arg1 as B<int>;//解析 ID
        int id = info.t1;
        LoadData(id);
    }

    public override void UnRegister<T>(T arg1) {
        base.UnRegister(arg1);
    }

    public override void LoadData(int id) {
        base.LoadData(id);
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        GameComp comp = go.GetComp();
        Data data = go.GetData();
        
        GravityBehaviourData tempData = new GravityBehaviourData();
        tempData.id = id;
        tempData.GravityRatio = data.GravityRatio;
        tempData.CC = comp.CC;
        tempData.go = data.MyObj;
        tempData.playerMod = data.PlayerMod;

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

    public override void UpdateData() {
        base.UpdateData();
        foreach (var temp in datas) {
            GameObj go = gameSystem.MyGameObjFeature.Get(temp.id);
            Data data = go.GetData();
            temp.playerMod = data.PlayerMod;
        }
    }

    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
            if (tempData.playerMod == PlayerMode.GODMODE) {
                continue;
            }
            if (tempData.CC == null) {
                continue;
            }
            tempData.CC.Move(Vector3.down * tempData.GravityRatio * Time.deltaTime);
        }
    }
}

public class GravityBehaviourData {
    public int id;
    public GameObject go;//目标物体
    public float GravityRatio;//重力系数
    public CharacterController CC;
    public PlayerMode playerMod;//玩家模式
}