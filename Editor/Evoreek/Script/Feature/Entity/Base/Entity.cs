using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 游戏实体
/// </summary>
public class Entity : IEntity {
    public Data MyData;
    protected GameSystem gameSystem;
    public virtual void Init(Game game, Data data) {
        MyData = data;
        gameSystem = game.gameSystem;
        
        MyData.runtimeBaseValueData.HP = MyData.baseValueData.HPMAX;
        MyData.runtimeBaseValueData.ATK = MyData.baseValueData.ATK;
        MyData.runtimeBaseValueData.ATKSPEED = MyData.baseValueData.ATKSPEED;
        MyData.runtimeBaseValueData.EXPVAL = 0;
        MyData.runtimeBaseValueData.EXPLEVEL = 0;
        MyData.runtimeBaseValueData.PHYSICALPOWER = MyData.baseValueData.PHYSICALPOWERMAX;

        StackTrace stackTrace = new StackTrace();
        StackFrame[] stackFrames = stackTrace.GetFrames();
        if (stackFrames.Length > 1) {
            // 获取调用该方法的方法信息
            MethodBase callingMethod = stackFrames[1].GetMethod();

            // 输出调用该方法的类名
            Logger.PrintType(MyData.InstanceID, callingMethod.DeclaringType.FullName, LoggerType.ENTITY);
        }
    }

    #region 血量

    public void HPChange(int attackId, int value) {// value 变化值
        if (GameData.MainPlayer == null) {
            return;
        }
        if (MyData.PlayerMod == PlayerMode.GODMODE) {//上帝模式无敌
            return;
        }
        int preciousHp = MyData.runtimeBaseValueData.HP;
        MyData.runtimeBaseValueData.HP += value;
        MyData.runtimeBaseValueData.HP = Mathf.Min(MyData.runtimeBaseValueData.HP, MyData.baseValueData.HPMAX);
        MyData.runtimeBaseValueData.HP = Mathf.Max(MyData.runtimeBaseValueData.HP, 0);
        if (MyData.runtimeBaseValueData.HP != preciousHp) { // 攻击生效
            Data tempData = gameSystem.MyEntityFeature.Get(attackId).GetData();
            tempData.baseRuntimeData.TotalCausedDamageAmount += Mathf.Abs(value);
            
            gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.HP, MyData.InstanceID, MyData.runtimeBaseValueData.HP, MyData.runtimeBaseValueData.HPMAX);
            if (GameData.MainPlayer.GetData().InstanceID == MyData.InstanceID) {
                gameSystem.messageCenter.Dispatcher(MessageConstant.ShakeCamera, 0.3f, new Vector3(0.05f, 0.05f, 0), 0.3f, 2f);
            }

            if (MyData.ObjectType == ObjectType.MONSTER) {
                gameSystem.messageCenter.Dispatcher(MessageConstant.BaseValueFlyOut, MyData.MyObj.transform.position + MyData.BaseValueFlyOutOffVec, value);
            }

            if (MyData.runtimeBaseValueData.HP == 0) {//该角色死亡
                MyData.baseRuntimeState.isDead = true;
                if (MyData.ObjectType == ObjectType.CENTRAL) {
                    gameSystem.messageCenter.Dispatcher(MessageConstant.GameOverMsg, false);
                    return;
                }
                
                Entity attackEntity = gameSystem.MyEntityFeature.Get(attackId);
                if (attackEntity.GetData().ObjectType == ObjectType.PLAYER) {
                    attackEntity.EXPVALChange(MyData.expSet.EXP);
                }

                if (GameData.MainPlayer.GetData().InstanceID == attackId) {//攻击者是本地玩家
                    gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.EXPVAL,
                        MyData.InstanceID, MyData.runtimeBaseValueData.EXPVAL, MyData.baseValueData.EXPVALMAX);
                }

                MyData.MyObj.SetActive(false);//死亡，触发重生
                if (MyData.canReborn) {//是否允许重生
                    ClockUtil.Instance.AlarmAfter(MyData.RebornTime, () => {
                        MyData.baseRuntimeState.isDead = false;
                        MyData.MyObj.SetActive(true);//模型激活
                        MyData.MyObj.transform.position = MyData.MyStartPointInfo.vec; //到出生点
                        MyData.MyObj.transform.rotation = Quaternion.Euler(MyData.MyStartPointInfo.engle);
                        MyData.runtimeBaseValueData.HP = MyData.baseValueData.HPMAX;
                        gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.HP, MyData.InstanceID,MyData.baseValueData.HPMAX, MyData.baseValueData.HPMAX);
                    });
                } else {
                    gameSystem.GetSystem<DropSystem>().Probability(MyData.gradeInfo, MyData.MyObj.transform.position);
                }
            }
        }
    }
    
    private float HPInterval;//恢复间隔
    private void HPRecover() {
        if (HPInterval > 0) {
            HPInterval -= Time.deltaTime;
        } else {
            HPInterval = 1;
            int precious = MyData.runtimeBaseValueData.HP;
            MyData.runtimeBaseValueData.HP += MyData.baseValueData.HPRECOVERSPEED;
            MyData.runtimeBaseValueData.HP = Mathf.Min(MyData.runtimeBaseValueData.HP, MyData.baseValueData.HPMAX);
            MyData.runtimeBaseValueData.HP = Mathf.Max(MyData.runtimeBaseValueData.HP, 0);
            if (precious != MyData.runtimeBaseValueData.HP) {
                if (MyData.runtimeBaseValueData.HP != 0) {
                    // Logger.Print($"血量：{MyData.runtimeBaseValueData.HP}");
                }
                gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.HP, MyData.InstanceID, MyData.runtimeBaseValueData.HP, MyData.runtimeBaseValueData.HPMAX);
            }
        }
    }

    #endregion

    #region 经验值

    public void EXPVALChange(int value) {
        int preciousEXPLEVE = MyData.baseValueData.EXPLEVEL;
        int preciousEXPVAL = MyData.baseValueData.EXPVAL;

        MyData.baseValueData.EXPVAL += value;
        int expDiff = MyData.baseValueData.EXPVAL - MyData.baseValueData.EXPVALMAX;//经验高于上限差值
        if (expDiff >= 0) { //差值大等0达到经验值上限升级
            int expLevelNum = 1;//升级等级1
            while (expDiff >= MyData.baseValueData.EXPVALMAX) {//差值依旧大于经验上限 累计升级等级+1
                expDiff -= MyData.baseValueData.EXPVALMAX;
                expLevelNum++;
            }

            if (MyData.baseValueData.EXPLEVEL < MyData.baseValueData.EXPLEVELMAX) {
                MyData.baseValueData.EXPLEVEL += expLevelNum;
                if (preciousEXPLEVE != MyData.baseValueData.EXPLEVEL) {
                    GameObject levelUpEffect = gameSystem.MyPoolFeature.Get("LevelUp");
                    levelUpEffect.transform.parent = GetData().MyObj.transform;
                    levelUpEffect.transform.localPosition = Vector3.zero;
                    levelUpEffect.transform.localRotation = Quaternion.identity;
                    GameData.AudioSystem.AddSoundPlay(new AudioData() {
                        InstanceID = MyData.InstanceID,
                        tempAudioName = "本地玩家升级",
                        is3D = false,
                        isLoop = false,
                        isFollow = false,
                    });
                    ClockUtil.Instance.AlarmAfter(2, () => {
                        List<Setting> settings = gameSystem.GetSystem<BuffSystem>().GetSettingsNotDuplicate(3);//获取三种不同的Buff
                        gameSystem.messageCenter.Dispatcher(MessageConstant.BuffDisplayMsg, settings);
                        gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.EXPLEVEL, MyData.InstanceID, MyData.baseValueData.EXPLEVEL, MyData.baseValueData.EXPLEVELMAX);
                        gameSystem.messageCenter.Dispatcher(MessageConstant.BarInfoMsg, MyData.InstanceID, new BaseRoleInfo() {
                            Name = MyData.MyObjName,
                            ExpLevel = MyData.baseValueData.EXPLEVEL.ToString(),
                        });
                        gameSystem.MyPoolFeature.Release("LevelUp", levelUpEffect);
                    });
                }
            }

            MyData.baseValueData.EXPVAL = expDiff;//当前经验值
            if (MyData.baseValueData.EXPVAL != preciousEXPVAL) {
                gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.EXPVAL, MyData.InstanceID, MyData.baseValueData.EXPVAL, MyData.baseValueData.EXPVALMAX);
            }
        } else {
            gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.EXPVAL, MyData.InstanceID, MyData.baseValueData.EXPVAL, MyData.baseValueData.EXPVALMAX);
        }
    }

    #endregion

    #region 体力 技能和移动消耗体力值

    public bool PHYSICALPOWERChange(int value) {
        //体力
        if (value == 0) {
            Logger.PrintELog("消耗体力：0 无需调用");
            return false;
        }

        if (Mathf.Abs(value) > MyData.runtimeBaseValueData.PHYSICALPOWER) {
            return false;
        }

        MyData.runtimeBaseValueData.PHYSICALPOWER += value; //消耗体力值
        MyData.runtimeBaseValueData.PHYSICALPOWER = Mathf.Min(MyData.runtimeBaseValueData.PHYSICALPOWER, MyData.baseValueData.PHYSICALPOWERMAX);
        MyData.runtimeBaseValueData.PHYSICALPOWER = Mathf.Max(MyData.runtimeBaseValueData.PHYSICALPOWER, 0);
        gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.PHYSICALPOWER, MyData.InstanceID, MyData.runtimeBaseValueData.PHYSICALPOWER, MyData.runtimeBaseValueData.PHYSICALPOWERMAX);
        return true;
    }

    private float RecoverInterval;//恢复间隔
    private void PHYSICALPOWERRecover() {
        if (RecoverInterval > 0) {
            RecoverInterval -= Time.deltaTime;
        } else {
            RecoverInterval = 1;
            int precious = MyData.runtimeBaseValueData.PHYSICALPOWER;
            MyData.runtimeBaseValueData.PHYSICALPOWER += MyData.baseValueData.PHYSICALPOWERRECOVERSPEED; //如果体力值没有达到上限，每秒恢复体力值
            MyData.runtimeBaseValueData.PHYSICALPOWER = Mathf.Min(MyData.runtimeBaseValueData.PHYSICALPOWER, MyData.baseValueData.PHYSICALPOWERMAX);
            MyData.runtimeBaseValueData.PHYSICALPOWER = Mathf.Max(MyData.runtimeBaseValueData.PHYSICALPOWER, 0);
            if (precious != MyData.runtimeBaseValueData.PHYSICALPOWER) {
                gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.PHYSICALPOWER, MyData.InstanceID, MyData.runtimeBaseValueData.PHYSICALPOWER, MyData.runtimeBaseValueData.PHYSICALPOWERMAX);
            }
        }
    }

    #endregion
    

    #region 技能

    /// <summary>
    /// 装载技能
    /// </summary>
    public bool AcquireSkill(int skillId) {
        Data skillData = gameSystem.MyEntityFeature.Get(skillId).GetData();
        if (MyData.SkillIds.Contains(skillId)) {//已存在该技能则不处理
            Logger.PrintELog($"已存在该技能 : id : {skillId}");
            return false;
        }

        if (MyData.SkillSlots.Count >= MyData.SkillSlotMaxCount) {
            Logger.PrintELog($"玩家已装载技能达到最高上限 : max : {MyData.SkillSlotMaxCount} id : {skillId}");
            return false;
        }

        MyData.SkillSlots.Add(skillId);//装载入背包
        MyData.SkillIds.Add(skillId);//装载入所获取的所有技能列表
        gameSystem.messageCenter.Dispatcher(MessageConstant.SkillAcquireSucMsg, skillData.skillInfo);
        
        gameSystem.MyGameObjFeature.Get(MyData.InstanceID).Register(skillData.skillInfo.functionType, new B<int>() {//注册功能到实体
            t1 = MyData.InstanceID,
        });//功能中有依赖 UI 的图片  所以先上一步发送消息 再执行注册功能
        Logger.Print($"成功装载技能 : id : {skillId} name: {skillData.skillInfo.skillName}");
        return true;
    }

    /// <summary>
    /// 检测该技能类型是否已存在
    /// </summary>
    public bool CheckSkillType(SkillType skillType) {
        foreach (var tempSkillID in MyData.SkillIds) {
            Data tempSkillData = gameSystem.MyEntityFeature.Get(tempSkillID).GetData();
            SkillType tempSkillType = tempSkillData.skillInfo.skillType;
            if (skillType == tempSkillType) {
                return true;
            }
        }

        return false;
    }

    #endregion

    public virtual void Update() {
        PHYSICALPOWERRecover();
        HPRecover();
        if (Input.GetKeyDown(KeyCode.F)) {
            PHYSICALPOWERChange(-5);
        }
    }

    public virtual void FixedUpdate() {
    }

    public virtual void LateUpdate() {
    }

    public virtual void Clear() {
    }

    public virtual T GetData<T>() where T : Data {
        return (T)MyData;
    }

    public Data GetData() {
        return MyData;
    }
}