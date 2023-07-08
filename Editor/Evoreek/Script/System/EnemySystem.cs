
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EnemySystem : GameSys {
    private SOEnemySetting setting;
    private GameSystem gameSystem;
    private List<PointInfo> startPoints;

    public EnemySystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        Init(gameSystem);
    }

    protected override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        setting = gameSystem.SoData.SOGameSetting.GetSetting<SOEnemySetting>();
        base.setting = this.setting;

        gameSystem.messageCenter.Reg(MessageConstant.TerrainLoadFinished, () => {
            startPoints = setting.GetRandList(setting.StartPointNum);
            foreach (var point in startPoints) {
                bool isPosTouchTerrain = GameData.GetTerrainPos(point.vec, out Vector3 retPos);
                if (isPosTouchTerrain) {
                    gameSystem.GetSystem<CreaterSystem>().Instance(setting.ObjectType, retPos);
                }
            }
        });
    }
    
    protected override void Clear() {
        
        gameSystem.messageCenter.UnReg(MessageConstant.TerrainLoadFinished, () => {
            startPoints = setting.GetRandList(setting.StartPointNum);
            foreach (var point in startPoints) {
                bool isPosTouchTerrain = GameData.GetTerrainPos(point.vec, out Vector3 retPos);
                if (isPosTouchTerrain) {
                    gameSystem.GetSystem<CreaterSystem>().Instance(setting.ObjectType, retPos);
                }
            }
        });
        base.Clear();
    }

    #region 不同实例创建接口

    /// <summary>
    /// 实例化 GameObject
    /// </summary>
    public int Instance() {
        return Instance<EnemyGameObj, EnemyEntity, EnemyData>((data) => {
            data.MyObjName = "怪物";
            data.MyStartPointInfo = startPoints[Random.Range(0, startPoints.Count)];
            data.viewType = PlayerViewType.TPS;
            data.GravityRatio = setting.GravityRatio;
            data.DestinationInfo = setting.DestinationInfo;
            data.ObjectType = setting.ObjectType;
            data.baseValueData = setting.BaseValueData;
            data.runtimeBaseValueData = setting.BaseValueData;
            data.ObjectLayerMask = setting.ObjectLayerMask;
            data.EnemyObjectLayerMasks = setting.EnemyObjectLayerMasks;
            data.RebornTime = setting.RebornTime;
            data.canReborn = setting.CanReborn;
            data.BaseValueFlyOutOffVec = setting.BaseValueFlyOutOffVec;
            data.expSet = setting.ExpSet;
            data.baseRuntimeData = new BaseRuntimeData();
            data.EnemyInfo = setting.EnemyInfo;
            data.gradeInfo = setting.GradeInfos;
        });
    }

    public int InstanceBoss(int waveNum) {
        SOEnemySetting enemySetting = null;
        foreach (var tempSetting in setting.BossSettings) {//根据波数确定怪物数量 怪物种类
            if (tempSetting.EnemyInfo.instanceWaveNum == waveNum) {
                enemySetting = tempSetting as SOEnemySetting;
            }
        }

        if (enemySetting == null) {
            Logger.PrintELog($"找不到第 {waveNum} 波的怪物配置");
            return 0;
        }
        
        return Instance<SOEnemySetting, EnemyGameObj, EnemyEntity, EnemyData>(enemySetting, (data) => {
            data.MyObjName = "怪物";
            data.MyStartPointInfo = startPoints[Random.Range(0, startPoints.Count)];
            data.viewType = PlayerViewType.TPS;
            data.GravityRatio = enemySetting.GravityRatio;
            data.DestinationInfo = enemySetting.DestinationInfo;
            data.ObjectType = enemySetting.ObjectType;
            data.baseValueData = enemySetting.BaseValueData;
            data.runtimeBaseValueData = enemySetting.BaseValueData;
            data.ObjectLayerMask = enemySetting.ObjectLayerMask;
            data.EnemyObjectLayerMasks = enemySetting.EnemyObjectLayerMasks;
            data.RebornTime = enemySetting.RebornTime;
            data.BaseValueFlyOutOffVec = enemySetting.BaseValueFlyOutOffVec;
            data.expSet = enemySetting.ExpSet;
            data.baseRuntimeData = new BaseRuntimeData();
            data.EnemyInfo = enemySetting.EnemyInfo;
            data.gradeInfo = setting.GradeInfos;
        });
    }

    #endregion
}