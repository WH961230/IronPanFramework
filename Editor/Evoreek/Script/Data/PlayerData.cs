using System;
using UnityEngine;

public class PlayerData : Data {
}

// 玩家状态
public enum PlayerState {
    Walk,
    Run,
    Idle,
}

public enum PlayerViewType {
    NONE,
    FPS,
    TPS,
    RTS //RTS 分为全局视角和锁定视角 取决于是否有锁定的对象
}

[Serializable]
public enum PlayerMode {
    NORMALMODE,//普通模式
    GODMODE,//上帝模式
}

public enum PlayerValue {
    HP,
    HPMAX,
    EXPVALMAX,
    EXPVAL,
    EXPLEVEL,
    EXPLEVELMAX,
    PHYSICALPOWER,
    PHYSICALPOWERMAX,
    ATK,//攻击力
    DEF,//防御力
    MOVESPEED,//速度
}