using System.Reflection;
using UnityEngine;

public class CentralSystem : GameSys {
    private SOCentralSetting setting;
    private GameSystem gameSystem;
    public CentralSystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        Init(gameSystem);
    }

    protected override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        setting = gameSystem.SoData.SOGameSetting.GetSetting<SOCentralSetting>();
        base.setting = this.setting;
        gameSystem.messageCenter.Reg(MessageConstant.TerrainLoadFinished, InstanceCentral);
    }

    protected override void Clear() {
        gameSystem.messageCenter.UnReg(MessageConstant.TerrainLoadFinished, InstanceCentral);
        base.Clear();
    }

    private void InstanceCentral() {
        Instance();
    }

    #region 不同实例创建接口

    public int Instance() {
        return Instance<CentralGameObj, CentralEntity, CentralData>((data) => {
            data.MyObjName = "中心建筑";
            data.MyStartPointInfo = setting.GetRand();
            data.DestinationInfo = setting.DestinationInfo;
            data.ObjectType = setting.ObjectType;
            data.baseValueData = setting.BaseValueData;
            data.runtimeBaseValueData = setting.BaseValueData;
            data.ObjectLayerMask = setting.ObjectLayerMask;
            data.EnemyObjectLayerMasks = setting.EnemyObjectLayerMasks;
            data.BaseValueFlyOutOffVec = setting.BaseValueFlyOutOffVec;
            data.baseRuntimeData = new BaseRuntimeData();
        });
    }

    #endregion
}