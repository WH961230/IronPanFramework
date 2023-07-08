using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Evoreek/Setting/SOPlayerSetting")]
public class SOPlayerSetting : Setting {
    // [Header("移动系数")] public float MoveRatio;
    // [Header("跑步系数")] public float RunRatio;
    [Header("侧向移动系数")] public float SlideRatio;
    [Header("后退移动系数")] public float BackRatio;

    [Header("第一人称转向系数")] public float FirstTurnRatio;
    [Header("第三人称转向系数")] public float ThirdTurnRatio;
    [Header("移动状态机参数")] public float WalkAnim;
    [Header("跑步状态机参数")] public float RunAnim;
    [Header("音效")] public List<SoundClip> SoundClips;
    
    //玩家数值
    [Header("玩家血量上限")] public int HPMAX;
    [Header("玩家魔法上限")] public int MPMAX;
    [Header("玩家经验等级上限")] public int EXPLEVELMAX;
    [Header("玩家经验值上限")] public int EXPVALMAX;

    //玩家视角参数
    [Header("玩家第一人称物体相对位置")] public Vector3 FPSRelationVec;
    [Header("玩家第三人称物体相对位置")] public Vector3 TPSRelationVec;
    
    [Header("玩家 X 轴速度系数")] public float XRotateSpeed;
    [Header("玩家 X 轴旋转下阈值")] public float XRotateMin;
    [Header("玩家 X 轴旋转上阈值")] public float XRotateMax;

    [Header("玩家 Y 轴速度系数")] public float YRotateSpeed;
    [Header("玩家模式")] public PlayerMode Mod;
    [Header("普通飞行系数")] public float FlyRatio;
    [Header("普通飞行系数")] public float FastFlyRatio;
}

[Serializable]
public struct SoundClip {
    public PlayerState state;
    public AudioClip clip;
}
