using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerSystem : GameSys {
    public PlayerManager playerManager;
    private SOPlayerSetting playerSetting;
    private GameSystem gameSystem;
    private List<PointInfo> startPoints;
    private int localPlayerId;
    public PlayerSystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        playerManager = new PlayerManager();
        Init(gameSystem);
    }

    protected override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        playerSetting = gameSystem.SoData.SOGameSetting.GetSetting<SOPlayerSetting>();
        setting = playerSetting;
        gameSystem.messageCenter.Reg(MessageConstant.TerrainLoadFinished, () => {
            // Game.Instance.netManager.ClientAddPlayerRequest();
        });

        gameSystem.messageCenter.Reg(MessageConstant.TerrainLoadFinished, () => {
            startPoints = playerSetting.GetRandList(playerSetting.StartPointNum);//获取多个玩家设置点
            foreach (var point in startPoints) {
                bool isPosTouchTerrain = GameData.GetTerrainPos(point.vec, out Vector3 retPos);
                if (isPosTouchTerrain) {
                    gameSystem.GetSystem<CreaterSystem>().Instance(playerSetting.ObjectType, retPos);
                }
            }
        });
    }

    protected override void Clear() {
        gameSystem.messageCenter.UnReg(MessageConstant.TerrainLoadFinished, () => {
            // Game.Instance.netManager.ClientAddPlayerRequest();
        });
        
        gameSystem.messageCenter.UnReg(MessageConstant.TerrainLoadFinished, () => {
            startPoints = playerSetting.GetRandList(playerSetting.StartPointNum);//获取多个玩家设置点
            foreach (var point in startPoints) {
                bool isPosTouchTerrain = GameData.GetTerrainPos(point.vec, out Vector3 retPos);
                if (isPosTouchTerrain) {
                    gameSystem.GetSystem<CreaterSystem>().Instance(playerSetting.ObjectType, retPos);
                }
            }
        });
        base.Clear();
    }

    public PlayerGameObj GetGO() {
        return gameSystem.MyGameObjFeature.Get<PlayerGameObj>(localPlayerId);
    }

    public PlayerEntity GetEntity() {
        return gameSystem.MyEntityFeature.Get<PlayerEntity>(localPlayerId);
    }

    public void InstanceAction(GameObject MyObj, bool isOwner) {
        Instance(MyObj, isOwner);
    }

    public int Instance(GameObject MyObj, bool isOwner) {
         int id = Instance<PlayerGameObj, PlayerEntity, PlayerData>(MyObj, (data) => {
            data.MyObjName = "玩家";
            data.SlideRatio = playerSetting.SlideRatio;
            data.BackRatio = playerSetting.BackRatio;
            data.FlyRatio = playerSetting.FlyRatio;
            data.FastFlyRatio = playerSetting.FastFlyRatio;
            data.FirstTurnRatio = playerSetting.FirstTurnRatio;
            data.XRotateSpeed = playerSetting.XRotateSpeed;
            data.YRotateSpeed = playerSetting.YRotateSpeed;
            data.ThirdTurnRatio = playerSetting.ThirdTurnRatio;
            data.GravityRatio = playerSetting.GravityRatio;
            data.WalkAnim = playerSetting.WalkAnim;
            data.RunAnim = playerSetting.RunAnim;
            data.MyStartPointInfo = startPoints[UnityEngine.Random.Range(0, startPoints.Count)];
            data.RotateType = RotateType.RotateSelf;
            data.SoundClips = playerSetting.SoundClips;
            data.isOwner = isOwner;
            data.baseValueData = playerSetting.BaseValueData;
            data.runtimeBaseValueData = playerSetting.BaseValueData;
            data.expSet = playerSetting.ExpSet;
            data.xRotateMin = playerSetting.XRotateMin;
            data.xRotateMax = playerSetting.XRotateMax;
            data.viewType = PlayerViewType.TPS;
            data.PlayerMod = playerSetting.Mod;
            data.isMainPlayer = isOwner;
            data.ObjectType = setting.ObjectType;
            data.DestinationInfo = setting.DestinationInfo;
            data.ObjectLayerMask = setting.ObjectLayerMask;
            data.EnemyObjectLayerMasks = setting.EnemyObjectLayerMasks;
            data.RebornTime = setting.RebornTime;
            data.canReborn = setting.CanReborn;
            data.SprintDuration = setting.SprintDuration;
            data.SprintSpeed = setting.SprintSpeed;
            data.SprintConsume = setting.SprintConsume;
            data.BaseValueFlyOutOffVec = setting.BaseValueFlyOutOffVec;
            data.baseRuntimeData = new BaseRuntimeData();
            data.DestinationNearDistance = setting.DestinationNearDistance;
            data.MoveSpeed = setting.MoveSpeed;
         });
         Data playerData = gameSystem.MyEntityFeature.Get(id).GetData();
         if (isOwner) {
             GameData.MainPlayer = gameSystem.MyGameObjFeature.Get<PlayerGameObj>(id);
             gameSystem.messageCenter.Dispatcher(MessageConstant.MainPlayerLoadFinished);
         }
         playerData.UIMainPlayerId = gameSystem.GetSystem<UISystem>().Instance<UIMainPlayerWindow, UIMainPlayerGameObj, UIMainPlayerEntity, UIMainPlayerData> ((data) => {// 生成玩家界面
             data.MyObjName = "玩家界面";
         });
         return id;
    }
}