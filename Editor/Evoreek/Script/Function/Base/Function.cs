using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Evoreek/Function")]
public class Function : GameFunc {
    
    /// <summary>
    /// 功能注册行为
    /// </summary>
    /// <param name="arg1"></param>
    /// <typeparam name="T"></typeparam>
    public override void Register<T>(T arg1) {
        base.Register(arg1);
        //注册功能 - 提供 ID 参数 遍历功能块注册
        foreach (var f in funcBlocks) {
            foreach (var tempCond in f.condition) {
                tempCond.Register(arg1);//条件注册
            }
            f.gameBehaviour.Register(arg1);//行为注册
        }
    }

    /// <summary>
    /// 反注册行为
    /// </summary>
    /// <param name="arg1"></param>
    /// <typeparam name="T"></typeparam>
    public override void UnRegister<T>(T arg1) {
        base.UnRegister(arg1);
        //卸载功能 - 根据提供的 ID 卸载功能
        foreach (var f in funcBlocks) {
            f.gameBehaviour.UnRegister(arg1);
        }
    }
}

[Serializable]
public enum FunctionType {
    HumanoidFunction,//类人类功能
    DebugFunction,//控制台功能
    GravityFunction,//重力功能
    AIMoveFunction,//机器人移动
    AIAttackFunction,//机器人攻击
    AcquireSkillFunction,//获取技能功能

    #region 基础
    
    ThirdControlFunction,//第三人称控制功能（本地玩家使用）
    NormalAttackFunction,//普攻功能（本地玩家使用）
    BaseSprintFunction,//基础冲刺功能
    RealTimeStrategyControlFunction,//即时战略控制功能
    RealTimeStrategyViewFunction,//即时战略视角功能

    #endregion
    
    #region 技能

    WhirlwindSkillFunction,//旋风斩技能
    SkillWarTramplingFunction,//战争践踏技能

    #endregion

    #region 增益

    BuffBloodsuckingHaloFunction,//吸血光环
    BuffFleetFootedFunction,//飞毛腿
    BuffWarGodBlessedFunction,//战神护佑
    BuffAttackSpeedHaloFunction,//攻击速度光环
    BuffAttackRangeHaloFunction,//攻击范围光环

    #endregion
}