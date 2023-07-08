using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏数据
/// </summary>
public class Data : IData {
    public int InstanceID;
    public GameObject MyObj;
    public string MyObjName; //名称
    public PointInfo MyStartPointInfo;
    public RotateType RotateType;
    public List<SoundClip> SoundClips;
    public List<FunctionType> FunctionTypes;
    public string ObjectLayerMask;
    public List<string> EnemyObjectLayerMasks;

    #region 本地玩家

    public bool isOwner;
    public int UIMainPlayerId;
    public BaseRuntimeData baseRuntimeData;

    #endregion

    #region HumanoidData 类人类数据
    
    public float SlideRatio;//侧向移动系数
    public float BackRatio;//后退速度
    public float FlyRatio;//正常飞行模式
    public float FastFlyRatio;//快速飞行模式
    public float FirstTurnRatio;
    public float ThirdTurnRatio;
    public float xRotateMin;//相机Y向最小阈值
    public float xRotateMax;//相机Y向最大阈值
    public float XRotateSpeed;//相机Y向系数
    public float YRotateSpeed;//相机X向系数
    public float WalkAnim;//移动状态机参数
    public float RunAnim;//奔跑状态机参数
    public float GravityRatio;//重力系数
    public PlayerViewType viewType;//视角类型
    public PlayerMode PlayerMod;//玩家模式
    public bool isMainPlayer;//是否主玩家
    public BaseValueData baseValueData;
    public BaseValueData runtimeBaseValueData;//运行中的角色基础数据
    public BaseRuntimeState baseRuntimeState = new BaseRuntimeState();//运行状态
    public EXPSet expSet;
    public float RebornTime;//重生时间
    public bool canReborn;//是否可以重生
    public Vector3 BaseValueFlyOutOffVec;//数值飞溅的偏移量
    public float DestinationNearDistance;//寻路停止距离 到达目标点多远停止寻路 
    public float MoveSpeed;
    public BaseRoleInfo BaseRoleInfo;//角色信息 

    #endregion

    #region 相机行为数据

    public Camera camera;

    public float minFirstHeight;
    public float maxFirstHeight;
    public float minThirdHeight;
    public float maxThirdHeight;
    public Transform viewTargetGo;//第一人称目标物体
    public Transform viewTPSTargetGo;//三人称目标物体
    public float edgeMoveRatio;//边缘移动系数
    public Vector3 FPSRelationVec;//
    public Vector3 TPSRelationVec;
    public Vector3 offEngle;//相机偏移角度
    public float defaultRelativeHeight;//默认相对高度

    public float scrollWheelRatio;//滑轮滚动系数
    public float firstFollowRatio;//第一人称跟随系数
    public float thirdFollowRatio;//第三人称跟随系数
    public float changeRatio;

    #endregion

    #region 机器人行为

    public List<DestinationInfo> DestinationInfo;//寻路信息
    public ObjectType ObjectType;//当前物体类型

    #endregion

    #region 掉落物行为
    
    public float detectDistance;//检测距离
    public int skillId;//绑定的技能 ID
    public GradeType gradeType;//品阶类型 Drop 使用
    public List<ObjectType> pickObjectTypes;//可拾取物体类型
    public List<GradeInfo> gradeInfo;//品阶信息

    #endregion

    #region 技能
    
    public SkillInfo skillInfo;//技能信息

    #region 战争践踏

    public float detectedRange;//检测距离
    public List<ObjectType> detectedObjectType;//检测物体类型列表

    #endregion
    
    #endregion
    
    #region 增益

    public BuffInfo buffInfo; //增益列表

    #endregion

    #region 玩家信息

    #region 基础

    public float SprintDuration;
    public float SprintSpeed;
    public int SprintConsume;

    #endregion

    #region 技能

    public List<int> SkillIds = new List<int>(); //技能拥有者 //技能 ID 列表 每个玩家每种类型 比如 旋风斩 只能有一个后续可以增加技能段位升阶，
    public int SkillSlotMaxCount = 6;//技能槽位数量上限
    public List<int> SkillSlots = new List<int>();
    public SkillType SkillType;

    #endregion

    #region 增益

    public List<int> BuffIds = new List<int>(); //增益列表
    public int BuffMaxCount = 10;

    #endregion

    #endregion

    #region 怪物信息

    public EnemyInfo EnemyInfo;

    #endregion
}

/// <summary>
/// 角色基础数据
/// </summary>
[Serializable]
public struct BaseValueData {
    public int HP;//血量
    public int HPMAX;//血量上限

    public int EXPLEVEL;//经验等级
    public int EXPLEVELMAX;//经验等级上限
    public int EXPVAL;//经验值
    public int EXPVALMAX;//经验上限

    public int PHYSICALPOWER;//体力值
    public int PHYSICALPOWERMAX;//体力值上限

    public int ATK;//攻击力
    public int DEF;//防御力
    public int ATKSPEED;//攻击速度
    public int ATKRANGE;//攻击范围
    public int MOVESPEED;//移动速度
    public int HITRATE;//暴击率
    public int HITDAMAGE;//暴击伤害
    public int HPINTAKE;//吸血量(吸血光环暂用，吸血率按照 100% 计算)
    public int EXPINTAKE;//经验吸取量
    public int HPRECOVERSPEED;//血量回复速度
    public int PHYSICALPOWERRECOVERSPEED;//体力恢复速度

    public void ChangeATK(int id, int valueChange) {
        if (valueChange == 0) {
            return;
        }
        ATK += valueChange;
        Logger.Print($"id: {id} 攻击力提升: {valueChange} 攻击力提升后：{ATK}");
        GameData.gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.ATK, id, ATK, 1);
    }
    
    public void ChangeDEF(int id, int valueChange) {
        if (valueChange == 0) {
            return;
        }
        DEF += valueChange;
        Logger.Print($"id: {id} 防御力提升: {valueChange} 防御力提升后：{DEF}");
        GameData.gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.DEF, id, DEF, 1);
    }

    public void ChangeMOVESPEED(int id, int valueChange) {
        if (valueChange == 0) {
            return;
        }
        MOVESPEED += valueChange;
        Logger.Print($"id: {id} 移动速度提升: {valueChange} 移动速度提升后：{MOVESPEED}");
        GameData.gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.MOVESPEED, id, MOVESPEED, 1);
    }
}

[Serializable]
public class EXPSet {//该角色包含的经验值
    public int EXP;
}

public class BaseRuntimeData {//运行时数据
    public int TotalCausedDamageAmount; //造成的总伤害
}

public class BaseRuntimeState {//运行时基础状态
    public bool isDead;//死亡
    public bool isMove;//移动
    public bool isAttack;//攻击
    public bool isIdle;//静止
}

public class BaseRoleInfo {//角色信息
    public string Name;//名称
    public string ExpLevel;//等级
}