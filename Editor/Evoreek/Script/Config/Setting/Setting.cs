using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Setting : GameSetting {
    [Header("预制标识")] public string PrefabSign;
    [Header("待注册功能类型")] public List<FunctionType> FunctionTypes;
    [Header("物体类型")] public ObjectType ObjectType;
    [Header("寻路数据")] public List<DestinationInfo> DestinationInfo;
    [Header("基础数值")] public BaseValueData BaseValueData;
    [Header("物体层级")] public string ObjectLayerMask;
    [Header("物体攻击检测敌人的层级")] public List<string> EnemyObjectLayerMasks;
    [Header("出生点")] public List<PointInfo> StartPoint;
    [Header("出生点数量")] public int StartPointNum;
    [Header("重生时间")] public float RebornTime;
    [Header("数值飞溅的偏移量")] public Vector3 BaseValueFlyOutOffVec;//数值飞溅的偏移量
    [Header("重力系数")] public float GravityRatio;
    [Header("经验量")] public EXPSet ExpSet;
    [Header("寻路点停止距离")] public float DestinationNearDistance;
    [Header("寻路速度")] public float MoveSpeed;
    [Header("是否可以重生")] public bool CanReborn;

    #region 怪物

    public EnemyInfo EnemyInfo;

    #endregion

    #region 基础

    //冲刺
    public float SprintDuration;
    public float SprintSpeed;
    public int SprintConsume;//冲刺消耗

    #endregion
    
    #region 技能
    
    public SkillInfo SkillInfo;
    
    #endregion

    #region 增益

    public BuffInfo BuffInfo;

    #endregion

    #region 掉落物

    public List<GradeInfo> GradeInfos;//不同品阶对应信息
    public List<ObjectType> PickObjectTypes;

    #endregion

    public override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        var systemClassType = typeof(Game).Assembly.GetType(SystemSign);
        var systemInstance = (GameSys)Activator.CreateInstance(systemClassType, gameSystem);
        gameSystem.AddSystem(systemInstance);
    }
    
    public PointInfo GetRand() {
        if (StartPoint.Count > 0) {
            int index = UnityEngine.Random.Range(0, StartPoint.Count);
            return StartPoint[index];
        }

        return default;
    }

    public List<PointInfo> GetRandList(int num) {
        List<PointInfo> points = new List<PointInfo>(num);
        if (num > StartPoint.Count) {
            Logger.Print("需求点位数量超过实际配置的配置个数");
            return null;
        }

        List<int> indexList = new List<int>(num);
        while (indexList.Count < num) {
            int randIndex = UnityEngine.Random.Range(0, StartPoint.Count);//随机序列
            if (indexList.Contains(randIndex)) {
                continue;
            }
            indexList.Add(randIndex);
            points.Add(StartPoint[randIndex]);
        }

        return points;
    }
}

[Serializable]
public struct InstancePointInfo {
    public GameObject obj;
    public Vector3 pos;
    public Vector3 engle;
}

[Serializable]
public enum ObjectType {//物体类型
    NONE,//空
    PLAYER,//玩家
    MONSTER,//怪物
    CENTRAL,//中心建筑
}

[Serializable]
public enum GradeType {//品阶
    S,
    A,
    B,
    C,
    D,
}

[Serializable]
public class GradeInfo {//品阶信息
    public GradeType gradeType;
    public Material gradeTypeMat;//品阶颜色材质
    public int OutputRate; //产出率 （百位制）
}

[Serializable]
public class DestinationInfo {//寻路信息
    public int weight;//权重
    public ObjectType destinationObjectType;//寻路物体类型
    public float detectedMoveDistance;//检测距离
}

[Serializable]
public struct PointInfo {
    public Vector3 vec;
    public Vector3 engle;
}

[Serializable]
public class SkillInfo {
    public float skillCD;
    public float skillDuration;
    public int skillAnimIndex;
    public int onceDamageValue;//一次性伤害
    public GradeInfo gradeInfo;
    public Sprite skillSprite;
    public Image skillImage;
    public string description;
    public string skillName;
    public SkillType skillType;
    public FunctionType functionType;
}

[Serializable]
public class BuffInfo {
    public string buffName;
    public string description;
    public GradeInfo gradeInfo;
    public Sprite buffSprite;
    public FunctionType functionType;
    public int speedIncrease;
    public int attackIncrease;
    public int attackSpeedIncrease;
    public int attackRangeIncrease;
}

[Serializable]
public enum SkillType {
    WHIRLWINDSKILL,//旋风斩
    WARTRAMPLING,//战争践踏
}

[Serializable]
public class EnemyInfo {//怪物信息
    public string enemyName;//怪物名称
    public EnemyType enemyType;//怪物类型
    public int instanceWaveNum;//怪物产出波数
    public int gradeOutputRate; //物品产出百位制
}

[Serializable]
public enum EnemyType {//怪物类型
    NORMAL,
    BOSS,
}