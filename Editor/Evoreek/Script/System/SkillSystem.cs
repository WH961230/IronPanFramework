using System.Collections.Generic;
using System.Reflection;

public class SkillSystem : GameSys {
    private SOSkillSetting setting;
    private GameSystem gameSystem;
    public SkillSystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        Init(gameSystem);
    }

    protected override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        setting = gameSystem.SoData.SOGameSetting.GetSetting<SOSkillSetting>();
        base.setting = this.setting;
    }

    #region 不同实例创建接口

    /// <summary>
    /// 创建技能实例
    /// </summary>
    /// <param name="gradeType"></param>
    /// <returns></returns>
    public int Instance(GradeType gradeType) {
        return Instance(GetGradeSetting(gradeType));
    }
    
    /// <summary>
    /// 实例化具体的技能配置
    /// </summary>
    public int Instance(Setting setting) {
        if (setting == null) {
            Logger.PrintE("找不到配置");
        }
        SkillInfo info = setting.SkillInfo;
        int skillId = 0;
        switch (info.skillType) {
            case SkillType.WHIRLWINDSKILL:
                SOWhirlwindSkillSetting sOWhirlwindSkillSetting = setting as SOWhirlwindSkillSetting;
                skillId = InstanceWhirlwind(sOWhirlwindSkillSetting);
                break;
            case SkillType.WARTRAMPLING:
                SOSkillWarTramplingSetting sOSkillWarTramplingSetting = setting as SOSkillWarTramplingSetting;
                skillId = InstanceSkillWarTrampling(sOSkillWarTramplingSetting);
                break;
            default:
                break;
        }
        return skillId;
    }

    private int InstanceSkillWarTrampling(SOSkillWarTramplingSetting skillSetting) {
        return Instance<SOSkillWarTramplingSetting, SkillWarTramplingGameObj, SkillWarTramplingEntity, SkillWarTramplingData>(skillSetting, (data) => {
            data.MyObjName = skillSetting.SkillInfo.skillName;
            data.detectedRange = skillSetting.DetectedRange;
            data.detectedObjectType = skillSetting.DetectedObjectTypeList;
            data.skillInfo = new SkillInfo() {
                skillName = skillSetting.SkillInfo.skillName,
                skillCD = skillSetting.SkillInfo.skillCD,
                onceDamageValue = skillSetting.SkillInfo.onceDamageValue,
                skillDuration = skillSetting.SkillInfo.skillDuration,
                skillType = skillSetting.SkillInfo.skillType,
                description = skillSetting.SkillInfo.description,
                gradeInfo = skillSetting.SkillInfo.gradeInfo,
                skillSprite = skillSetting.SkillInfo.skillSprite,
                skillAnimIndex = skillSetting.SkillInfo.skillAnimIndex,
                functionType = skillSetting.SkillInfo.functionType,
            };
        });
    }

    /// <summary>
    /// 实例化旋风斩
    /// </summary>
    /// <param name="setting"></param>
    /// <returns></returns>
    private int InstanceWhirlwind(SOWhirlwindSkillSetting skillSetting) {
        return Instance<SOWhirlwindSkillSetting, WhirlwindSkillGameObj, SkillWhirlwindEntity, SkillWhirlwindData>(skillSetting, (data) => {
            data.MyObjName = skillSetting.SkillInfo.skillName;
            data.skillInfo = new SkillInfo() {
                skillName = skillSetting.SkillInfo.skillName,
                skillCD = skillSetting.SkillInfo.skillCD,
                skillDuration = skillSetting.SkillInfo.skillDuration,
                skillType = skillSetting.SkillInfo.skillType,
                description = skillSetting.SkillInfo.description,
                gradeInfo = skillSetting.SkillInfo.gradeInfo,
                skillSprite = skillSetting.SkillInfo.skillSprite,
                skillAnimIndex = skillSetting.SkillInfo.skillAnimIndex,
                functionType = skillSetting.SkillInfo.functionType,
            };
        });
    }

    #endregion
    
    /// <summary>
    /// 获取某品阶的所有技能配置
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Setting GetGradeSetting(GradeType type) {
        List<Setting> retList = new List<Setting>();
        foreach (var tempSkillSetting in setting.SkillSettings) {//遍历所有技能配置
            Setting set = tempSkillSetting;
            SkillInfo skillInfo = set.SkillInfo;
            if (skillInfo.gradeInfo.gradeType == type) {
                retList.Add(set);
            }
        }

        return GetRandSetting(retList);
    }

    /// <summary>
    /// 获取随机配置
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    private Setting GetRandSetting(List<Setting> settings) {
        Setting retSetting = null;
        if (settings.Count > 0) {
            retSetting = settings[UnityEngine.Random.Range(0, settings.Count - 1)];
        }

        return retSetting;
    }

    /// <summary>
    /// 根据技能的类型获取技能配置
    /// </summary>
    /// <param name="skillType"></param>
    /// <returns></returns>
    public Setting GetSettingBySkillType(SkillType skillType) {
        Setting retSetting = null;
        foreach (var tempSkillSetting in setting.SkillSettings) {//遍历所有技能配置
            Setting set = tempSkillSetting;
            SkillInfo skillInfo = set.SkillInfo;
            if (skillInfo.skillType == skillType) {
                retSetting = set;
                break;
            }
        }

        return retSetting;
    }
}