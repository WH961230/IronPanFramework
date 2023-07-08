using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// 游戏组件
/// </summary>
public class GameComp : MonoBehaviour, IComp {
    #region 公共

    public int InstanceID;

    #endregion
    
    #region 类人类组件

    public Animator animator;
    public CharacterController CC;
    public NavMeshAgent agent; //AI代理
    public Transform cameraViewTarget;
    public Transform CameraTPSViewTarget; //相机视角目标
    public Slider HPSlider;//血条
    public Transform bloodTran;//血条及玩家信息
    public BoxCollider attackWeaponCollider;//攻击范围使用
    public Transform dizzinessTran;//眩晕图标
    public Slider PhysicalPowerSlider;//体力条
    public Transform PhysicalPowerTran;//体力条物体
    public Transform RegionTran;//选中物体
    public SpriteRenderer SpriteRenderer;

    public TextMeshProUGUI PlayerExpLevel;//玩家等级
    public TextMeshProUGUI PlayerName;//玩家名称
    
    #endregion
    
    #region 基础数据

    public TextMeshProUGUI PlayerNameInfoText;//玩家信息名称
    public TextMeshProUGUI PlayerEXPLevelInfoText;//玩家等级信息名称
    public TextMeshProUGUI AttackText;
    public TextMeshProUGUI DefendText;
    public TextMeshProUGUI SpeedText;

    #endregion
}