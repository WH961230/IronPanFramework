using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMainPlayerWindow : Window {
    private UIMainPlayerComponent uIMainPlayerComp;
    private UIMainPlayerGameObj uIMainPlayerGameObj;
    private UIMainPlayerData uIMainPlayerData;

    public override void Init(Game game, GameObj gameObj) {
        base.Init(game, gameObj);
        uIMainPlayerComp = gameObj.GetObj().GetComponent<UIMainPlayerComponent>();
        uIMainPlayerGameObj = (UIMainPlayerGameObj) gameObj;
        uIMainPlayerData = (UIMainPlayerData) uIMainPlayerGameObj.GetData();
        uIMainPlayerComp.BuffRootTran.gameObject.SetActive(false);
        uIMainPlayerComp.GameResultPanelTran.gameObject.SetActive(false);
        uIMainPlayerComp.EnemyWavePanelTran.gameObject.SetActive(false);
        uIMainPlayerComp.BeginnerTutorialPanelTran.gameObject.SetActive(true);
        uIMainPlayerComp.CloseBeginnerTutorialBtn.gameObject.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        uIMainPlayerComp.SkillAcquireTipTran.gameObject.SetActive(false);
        uIMainPlayerComp.CentralSlider.gameObject.SetActive(false);
        gameSystem.messageCenter.Reg<PlayerValue, int, int, int>(MessageConstant.ValueSingleChangeMsg,
            (valueType, id, value, MaxValue) => {
                if (gameSystem.MyEntityFeature.Get(id).GetData().ObjectType == ObjectType.CENTRAL) {
                    uIMainPlayerComp.CentralSlider.gameObject.SetActive(true);
                    uIMainPlayerComp.CentralSliderText.text = value + " / " + MaxValue;
                    uIMainPlayerComp.CentralSlider.value = (float) value / MaxValue;
                }

                if (id == GameData.MainPlayer.GetData().InstanceID) {
                    if (valueType == PlayerValue.PHYSICALPOWER) {
                        if (uIMainPlayerComp.PhysicalPowerSlider != null) {
                            uIMainPlayerComp.PhysicalPowerSlider.value = (float) value / MaxValue;
                        }
                    } else if (valueType == PlayerValue.ATK) {
                        if (uIMainPlayerComp.AttackText != null) {
                            uIMainPlayerComp.AttackText.text = value.ToString();
                        }
                    } else if (valueType == PlayerValue.DEF) {
                        if (uIMainPlayerComp.DefendText != null) {
                            uIMainPlayerComp.DefendText.text = value.ToString();
                        }
                    } else if (valueType == PlayerValue.MOVESPEED) {
                        if (uIMainPlayerComp.SpeedText != null) {
                            uIMainPlayerComp.SpeedText.text = value.ToString();
                        }
                    }
                }
            });
        gameSystem.messageCenter.Reg<int, BaseRoleInfo>(MessageConstant.BarInfoMsg, (id, info) => {
            if (id == GameData.MainPlayer.GetData().InstanceID) {
                if (uIMainPlayerComp.PlayerNameInfoText != null) {
                    //本地玩家信息数据
                    uIMainPlayerComp.PlayerNameInfoText.text = info.Name;
                }

                if (uIMainPlayerComp.PlayerEXPLevelInfoText != null) {
                    uIMainPlayerComp.PlayerEXPLevelInfoText.text = info.ExpLevel;
                }
            }
        });
        gameSystem.messageCenter.Dispatcher(MessageConstant.MainPlayerWindowLoadFinished);
        uIMainPlayerComp.CloseBeginnerTutorialBtn.onClick.AddListener(() => {
            uIMainPlayerComp.BeginnerTutorialPanelTran.gameObject.SetActive(false);
            Time.timeScale = 1;
        });
        gameSystem.messageCenter.Reg<float, string>(MessageConstant.EnemyWaveTipMsg, (continueTime, tipTxt) => {
            uIMainPlayerComp.EnemyWavePanelTran.gameObject.SetActive(true);
            uIMainPlayerComp.EnemyWaveTextTran.text = tipTxt;
        });
        gameSystem.messageCenter.Reg<int, bool>(MessageConstant.RegionSingleMsg, (id, isRegion) => {
            //单个框选 更新玩家消息
            if (isRegion) {
                //框选中
                uIMainPlayerData.BaseData = gameSystem.MyEntityFeature.Get<PlayerEntity>(id).GetData<PlayerData>()
                    .baseValueData;
                SetDataToComp(uIMainPlayerData.BaseData, uIMainPlayerData.runtimeBaseValueData);
            } else {
                uIMainPlayerData.BaseData = default;
                ClearSingleDataComp();
            }
        });
        gameSystem.messageCenter.Reg<bool>(MessageConstant.GameOverMsg, (result) => {
            uIMainPlayerComp.GameResultPanelTran.gameObject.SetActive(true);
            bool isWin = result;
            if (isWin) {
                uIMainPlayerComp.WinPanelTran.gameObject.SetActive(true);
            } else {
                uIMainPlayerComp.DefeatedPanelTran.gameObject.SetActive(true);
            }
        });
        gameSystem.messageCenter.Reg<PlayerValue, int, int, int>(MessageConstant.ValueSingleChangeMsg,
            (valueType, id, value, valueMax) => {
                if (id == GameData.MainPlayer.GetData().InstanceID) {
                    SetValueToComp(valueType, value, valueMax);
                }
            });
        gameSystem.messageCenter.Reg(MessageConstant.BackToMainMsg,
            () => { uIMainPlayerGameObj.GetObj().SetActive(false); });
        gameSystem.messageCenter.Reg<SkillInfo>(MessageConstant.SkillAcquireSucMsg, (skillInfo) => {
            uIMainPlayerComp.SkillAcquireTipText.text = $"获取技能: {skillInfo.skillName} 技能说明: {skillInfo.description}";
            uIMainPlayerComp.SkillAcquireTipTran.gameObject.SetActive(true);
            ClockUtil.Instance.AlarmAfter(5,
                () => { uIMainPlayerComp.SkillAcquireTipTran.gameObject.SetActive(false); });
            uIMainPlayerComp.SkillInfos.Add(skillInfo);
            Image image = uIMainPlayerComp.SkillSlots[uIMainPlayerComp.currentEmptySlotIndex];
            image.gameObject.SetActive(true);
            image.sprite = skillInfo.skillSprite;
            skillInfo.skillImage = image; //存储技能 UI
            Color tempColor = image.color;
            tempColor = Color.white;
            tempColor.a = 0.75f;
            image.color = tempColor;
            uIMainPlayerComp.currentEmptySlotIndex++;

            Button tempImageButton = image.transform.parent.gameObject.GetComponent<Button>();
            EventTrigger trigger = tempImageButton.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            //鼠标悬浮
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => {
                OnPointerEnterDelegate((PointerEventData) data, skillInfo.skillType);
            });
            trigger.triggers.Add(entry);
            //鼠标悬浮结束
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerExit;
            entry.callback.AddListener((data) => {
                OnPointerExitDelegate((PointerEventData) data, tempImageButton);
            });
            trigger.triggers.Add(entry);
        });
        gameSystem.messageCenter.Reg<List<Setting>>(MessageConstant.BuffDisplayMsg, (buffSettings) => {
            uIMainPlayerComp.BuffRootTran.gameObject.SetActive(true);
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            DisplayBuff(buffSettings);
        });
        gameSystem.messageCenter.Reg<SkillType, Vector3>(MessageConstant.ShowSkillInfo, (skillType, offVec) => { ShowSkillInfo(skillType, offVec); });
        gameSystem.messageCenter.Reg(MessageConstant.CloseSkillInfo, () => {
            if (uIMainPlayerData.SkillInfoGO == null) {
                return;
            }

            gameSystem.MyPoolFeature.Release("SkillInfo", uIMainPlayerData.SkillInfoGO);
            uIMainPlayerData.SkillInfoGO = null;
        });
    }

    public override void Clear() {
        gameSystem.messageCenter.UnReg(MessageConstant.CloseSkillInfo, () => {
        });
        gameSystem.messageCenter.UnReg<SkillType, Vector3>(MessageConstant.ShowSkillInfo, (skillType, offVec) => {
            
        });
        gameSystem.messageCenter.UnReg<PlayerValue, int, int, int>(MessageConstant.ValueSingleChangeMsg, (valueType, id, HPValue, MaxHPValue) => {
            if (gameSystem.MyEntityFeature.Get(id).GetData().ObjectType == ObjectType.CENTRAL) {
                uIMainPlayerComp.CentralSlider.gameObject.SetActive(true);
                uIMainPlayerComp.CentralSliderText.text = HPValue + " / " + MaxHPValue;
                uIMainPlayerComp.CentralSlider.value = (float) HPValue / MaxHPValue;
            }
        });

        gameSystem.messageCenter.UnReg<float, string>(MessageConstant.EnemyWaveTipMsg, (continueTime, tipTxt) => {
            uIMainPlayerComp.EnemyWavePanelTran.gameObject.SetActive(true);
            uIMainPlayerComp.EnemyWaveTextTran.text = tipTxt;
            ClockUtil.Instance.AlarmAfter(continueTime, () => {
                uIMainPlayerComp.EnemyWavePanelTran.gameObject.SetActive(false);
            });
        });
        
        gameSystem.messageCenter.UnReg<bool>(MessageConstant.GameOverMsg, (result) => {
            uIMainPlayerComp.GameResultPanelTran.gameObject.SetActive(true);
            bool isWin = result;
            if (isWin) {
                uIMainPlayerComp.WinPanelTran.gameObject.SetActive(true);
            } else {
                uIMainPlayerComp.DefeatedPanelTran.gameObject.SetActive(true);
            }
        });

        gameSystem.messageCenter.UnReg<int, bool>(MessageConstant.RegionSingleMsg, (id, isRegion) => {//单个框选 更新玩家消息
            if (isRegion) {//框选中
                uIMainPlayerData.BaseData = gameSystem.MyEntityFeature.Get<PlayerEntity>(id).GetData<PlayerData>().baseValueData;
                SetDataToComp(uIMainPlayerData.BaseData, uIMainPlayerData.runtimeBaseValueData);
            } else {
                uIMainPlayerData.BaseData = default;
                ClearSingleDataComp();
            }
        });
        
        gameSystem.messageCenter.UnReg<PlayerValue, int, int, int>(MessageConstant.ValueSingleChangeMsg, (valueType, id, value, valueMax) => {
            if (id == GameData.MainPlayer.GetData().InstanceID) {
                SetValueToComp(valueType, value, valueMax);
            }
        });
        
        gameSystem.messageCenter.UnReg(MessageConstant.BackToMainMsg, () => {
            uIMainPlayerGameObj.GetObj().SetActive(false);
        });

        gameSystem.messageCenter.UnReg<List<Setting>>(MessageConstant.BuffDisplayMsg, (buffSettings) => {
            uIMainPlayerComp.BuffRootTran.gameObject.SetActive(true);
            DisplayBuff(buffSettings);
        });
        
        base.Clear();
    }
    
    private void OnPointerEnterDelegate(PointerEventData data, SkillType  skillType) {
        ShowSkillInfo(skillType, Vector3.zero);
        Logger.Print("悬浮展示技能信息");
    }

    private void OnPointerExitDelegate(PointerEventData data, Button areaBtn) {
        if (uIMainPlayerData.SkillInfoGO == null) {
            return;
        }

        Logger.Print("悬浮取消展示技能信息");
        gameSystem.MyPoolFeature.Release("SkillInfo", uIMainPlayerData.SkillInfoGO);
        uIMainPlayerData.SkillInfoGO = null;
    }
    
    private void ShowSkillInfo(SkillType skillType, Vector3 offVec) {
        if (uIMainPlayerData.SkillInfoGO != null) {
            return;
        }
        uIMainPlayerData.SkillInfoGO = GameData.gameSystem.MyPoolFeature.Get("SkillInfo"); //创建UI
        if (uIMainPlayerData.SkillInfoGO == null) {
            Logger.PrintE("skillInfo == null");
            return;
        }

        DisplaySkillInfo info = uIMainPlayerData.SkillInfoGO.GetComponent<DisplaySkillInfo>();
        Setting setting = GameData.gameSystem.GetSystem<SkillSystem>().GetSettingBySkillType(skillType);
        if (setting == null) {
            Logger.PrintE("setting == null");
            return;
        }

        info.SkillDescription.text = setting.SkillInfo.description;
        Color tempColor = info.SkillIcon.color;
        tempColor.a = 1;
        info.SkillIcon.color = tempColor;
        info.SkillIcon.sprite = setting.SkillInfo.skillSprite;
        uIMainPlayerData.SkillInfoGO.transform.parent = uIMainPlayerData.MyObj.transform;
        uIMainPlayerData.SkillInfoGO.transform.localRotation = Quaternion.identity;
        if (offVec == Vector3.zero) {//未赋值视为技能信息 赋值则为掉落物物体 赋值位置
            uIMainPlayerData.SkillInfoGO.transform.localPosition = new Vector3(15.95f, -127f, 0);
        } else {
            uIMainPlayerData.SkillInfoGO.transform.position = offVec;
        }
    }

    private void DisplayBuff(List<Setting> buffSettings) {
        foreach (var tempBuff in uIMainPlayerComp.BuffDisplayInfos) {//遍历增益面板
            Setting tempSetting = buffSettings[uIMainPlayerComp.BuffDisplayInfos.IndexOf(tempBuff)];
            tempBuff.BuffImage.sprite = tempSetting.BuffInfo.buffSprite;//获取配置的技能图片到组件图片中
            tempBuff.BuffDescription.text = tempSetting.BuffInfo.description;//获取配置 Buff 说明
            
            Color tempColor = tempBuff.BuffImage.color;//修改颜色透明度
            tempColor.a = 0.75f;
            tempBuff.BuffImage.color = tempColor;
            
            tempBuff.BuffClickBtn.onClick.RemoveAllListeners();//移除所有监听
            tempBuff.BuffClickBtn.onClick.AddListener(() => {//选择增益
                int id = gameSystem.GetSystem<BuffSystem>().Instance(tempSetting);//实例化 Buff 配置
                GameData.MainPlayer.GetData().BuffIds.Add(id);//增益列表
                
                Data buffData = gameSystem.MyEntityFeature.Get(id).GetData();
                GameData.MainPlayer.Register(buffData.buffInfo.functionType, new B<int> {//玩家注册增益效果功能
                    t1 = GameData.MainPlayer.GetData().InstanceID,
                });

                if (!uIMainPlayerComp.BuffTypes.Contains(buffData.buffInfo.functionType)) {
                    uIMainPlayerComp.BuffTypes.Add(buffData.buffInfo.functionType);
                    int index = uIMainPlayerComp.currentEmptyBuffSlotIndex;
                    uIMainPlayerComp.BuffPanelIcons[index].sprite = buffData.buffInfo.buffSprite;//buff icon 图片
                    Color color = uIMainPlayerComp.BuffPanelIcons[index].color;//颜色
                    color.a = 0.75f;
                    uIMainPlayerComp.BuffPanelIcons[index].color = color;
                    uIMainPlayerComp.currentEmptyBuffSlotIndex++;
                }

                GameData.AudioSystem.AddSoundPlay(new AudioData() {
                    InstanceID = uIMainPlayerData.InstanceID,
                    tempAudioName = "选择增益按钮",
                    is3D = false,
                    isLoop = false,
                    isFollow = false,
                });

                uIMainPlayerComp.BuffRootTran.gameObject.SetActive(false);//Buff 组件三选一物体隐藏
                Time.timeScale = 1;
            });
        }
    }

    /// <summary>
    /// 设置玩家数据到组件
    /// </summary>
    /// <param name="data"></param>
    private void SetDataToComp(BaseValueData data, BaseValueData runtimeData) {
        string middle = " / ";
        SetText(uIMainPlayerComp.PlayerHPValue, string.Concat(runtimeData.HP, middle, data.HPMAX));
        SetText(uIMainPlayerComp.PlayerExpValue, string.Concat(runtimeData.EXPVAL, middle, data.EXPVALMAX));
        SetText(uIMainPlayerComp.PlayerExpLevel, runtimeData.EXPLEVEL.ToString());
    }

    /// <summary>
    /// 清除单个玩家数据
    /// </summary>
    private void ClearSingleDataComp() {
        uIMainPlayerComp.PlayerHPValue.text = "";
        uIMainPlayerComp.PlayerMPValue.text = "";
        uIMainPlayerComp.PlayerExpValue.text = "";
        uIMainPlayerComp.PlayerExpLevel.text = "";
    }

    /// <summary>
    /// 设置单个数值到组件
    /// </summary>
    private void SetValueToComp(PlayerValue valueType, int curValue, int maxValue) {
        TextMeshProUGUI text = null;
        if (valueType == PlayerValue.HP) {
            text = uIMainPlayerComp.PlayerHPValue;
            uIMainPlayerComp.PlayerHPSlider.value = (float) curValue / maxValue;
        } else if (valueType == PlayerValue.EXPVAL) {
            text = uIMainPlayerComp.PlayerExpValue;
            uIMainPlayerComp.PlayerExpSlider.value = (float) curValue / maxValue;
        } else if (valueType == PlayerValue.EXPLEVEL) {
            SetText(uIMainPlayerComp.PlayerExpLevel, curValue.ToString());
            return;
        }

        if (text != null) {
            string middle = " / ";
            SetText(text, String.Concat(curValue, middle, maxValue));
        }
    }

    private void SetText(TextMeshProUGUI text, string content) {
        if (text == null) {
            return;
        }
        text.text = content;
    }

    private float time = 900;
    public override void Update() {
        base.Update();
        time -= Time.deltaTime;
        uIMainPlayerComp.gameCountDownText.text = time.ToString("F2") + '`' + " 后胜利";
        if (time <= 0) {
            gameSystem.messageCenter.Dispatcher(MessageConstant.GameOverMsg, true);//当坚持到最后一波之后胜利
        }
    }
}