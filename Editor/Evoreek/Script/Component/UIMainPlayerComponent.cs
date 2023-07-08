using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMainPlayerComponent : GameComp, IPointerDownHandler, IPointerUpHandler {
    #region 单位信息

    #region 单个单位信息

    public GameObject BaseValueFlyOutObj;//数值飞出物体
    public Transform PlayerIcon;//玩家图标
    public Button PlayerButton;//玩家图标按钮 1、单击该按钮切换到该角色位置 2、按住该按钮跟踪该角色
    public Slider PlayerExpSlider;//玩家经验值滑动条
    public TextMeshProUGUI PlayerExpValue;//玩家经验值
    public Slider PlayerHPSlider;//玩家血量滑动条
    public Slider PlayerPhysicPowerSlider;//玩家体力条
    public TextMeshProUGUI PlayerHPValue;//玩家血量
    public TextMeshProUGUI PlayerMPValue;//玩家魔法值

    public Transform BeginnerTutorialPanelTran;//新手教程面板
    public Button CloseBeginnerTutorialBtn;//关闭新手教程

    public Transform SkillAcquireTipTran;//技能获取提示
    public TextMeshProUGUI SkillAcquireTipText;//技能获取提示内容

    public Slider CentralSlider;//建筑血条
    public TextMeshProUGUI CentralSliderText;//建筑血条数值
    
    #endregion

    #region 技能面板

    [HideInInspector]
    public int currentEmptySlotIndex = 0;
    public List<Image> SkillSlots;
    public List<SkillInfo> SkillInfos = new List<SkillInfo>();//技能信息列表

    #endregion

    #region 增益面板三选一

    public Transform BuffRootTran;
    [HideInInspector]
    public int currentEmptyBuffSlotIndex = 0;
    public List<Image> BuffPanelIcons;//状态
    public List<BuffIconInfo> BuffIconInfos;
    public List<BuffDisplayInfo> BuffDisplayInfos;
    public List<FunctionType> BuffTypes = new List<FunctionType>();

    #endregion

    #region 游戏结束面板

    public Transform GameResultPanelTran;
    public Transform WinPanelTran;
    public Transform DefeatedPanelTran;

    #endregion
    
    #region 怪物波数面板

    public Transform EnemyWavePanelTran;
    public TextMeshProUGUI EnemyWaveTextTran;

    #endregion

    #endregion

    #region 倒计时

    public TextMeshProUGUI gameCountDownText;

    #endregion

    #region 按钮事件

    private bool isPressDown;
    private bool isMouseOver;
    public Action PressButtonAction;//按下事件
    public Action FreeButtonAction;//松开事件
    public Action MouseOverAction;//鼠标持续进入区域事件
    public Action MouseExitAction;
    public Action MouseDownAction;

    public void OnPointerDown(PointerEventData eventData) {
        isPressDown = true;
        PressButtonAction?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData) {
        isPressDown = false;
        FreeButtonAction?.Invoke();
    }

    private void OnMouseOver() {
        isMouseOver = true;
        MouseOverAction?.Invoke();
    }

    private void OnMouseExit() {
        isMouseOver = false;
        MouseExitAction?.Invoke();
    }

    private void OnMouseDown() {
        MouseDownAction?.Invoke();
    }

    #endregion
}

[Serializable]
public class BuffDisplayInfo {
    public Image BuffImage;
    public TextMeshProUGUI BuffDescription;
    public Button BuffClickBtn;
}

[Serializable]
public class BuffIconInfo {
    public Image BuffImage;
}
