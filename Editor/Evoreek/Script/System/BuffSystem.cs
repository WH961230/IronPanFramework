using System.Collections.Generic;
using System.Reflection;

public class BuffSystem : GameSys {
    private SOBuffSetting setting;
    private GameSystem gameSystem;

    public BuffSystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        Init(gameSystem);
    }

    protected override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        setting = gameSystem.SoData.SOGameSetting.GetSetting<SOBuffSetting>();
        base.setting = this.setting;
    }

    #region 不同实例创建接口

    public int Instance(Setting buffSetting) {
        int buffId = 0;
        switch (buffSetting.BuffInfo.functionType) {
            case FunctionType.BuffBloodsuckingHaloFunction:
                SOBuffBloodsuckingHaloSetting sOBuffBloodsuckingHaloSetting = buffSetting as SOBuffBloodsuckingHaloSetting;
                buffId = InstanceBuffBloodsuckingHalo(sOBuffBloodsuckingHaloSetting);
                break;
            case FunctionType.BuffFleetFootedFunction:
                SOBuffFleetFootedSetting sOBuffFleetFootedSetting = buffSetting as SOBuffFleetFootedSetting;
                buffId = InstanceBuffFleetFooted(sOBuffFleetFootedSetting);
                break;
            case FunctionType.BuffWarGodBlessedFunction:
                SOBuffWarGodBlessedSetting sOBuffWarGodBlessedSetting = buffSetting as SOBuffWarGodBlessedSetting;
                buffId = InstanceBuffWarGodBlessed(sOBuffWarGodBlessedSetting);
                break;
            case FunctionType.BuffAttackSpeedHaloFunction:
                SOBuffAttackSpeedHaloSetting sOBuffAttackSpeedHaloSetting = buffSetting as SOBuffAttackSpeedHaloSetting;
                buffId = InstanceBuffAttackSpeedHalo(sOBuffAttackSpeedHaloSetting);
                break;
            case FunctionType.BuffAttackRangeHaloFunction:
                SOBuffAttackRangeHaloSetting sOBuffAttackRangeHaloSetting = buffSetting as SOBuffAttackRangeHaloSetting;
                buffId = InstanceBuffAttackRangeHalo(sOBuffAttackRangeHaloSetting);
                break;
            default:
                break;
        }

        return buffId;
    }
    
    private int InstanceBuffAttackRangeHalo(SOBuffAttackRangeHaloSetting buffSetting) {
        return Instance<SOBuffAttackRangeHaloSetting, BuffAttackRangeHaloGameObj, BuffAttackRangeHaloEntity, BuffAttackRangeHaloData>(buffSetting, (data) => {
            data.MyObjName = "攻范光环";
            data.buffInfo = buffSetting.BuffInfo;
        });
    }

    private int InstanceBuffAttackSpeedHalo(SOBuffAttackSpeedHaloSetting buffSetting) {
        return Instance<SOBuffAttackSpeedHaloSetting, BuffAttackSpeedHaloGameObj, BuffAttackSpeedHaloEntity, BuffAttackSpeedHaloData>(buffSetting, (data) => {
            data.MyObjName = "攻速光环";
            data.buffInfo = buffSetting.BuffInfo;
        });
    }

    private int InstanceBuffBloodsuckingHalo(SOBuffBloodsuckingHaloSetting buffSetting) {
        return Instance<SOBuffBloodsuckingHaloSetting, BuffBloodsuckingHaloGameObj, BuffBloodsuckingHaloEntity, BuffBloodsuckingHaloData>(buffSetting, (data) => {
            data.MyObjName = "吸血光环";
            data.buffInfo = buffSetting.BuffInfo;
        });
    }
    
    private int InstanceBuffWarGodBlessed(SOBuffWarGodBlessedSetting buffSetting) {
        return Instance<SOBuffWarGodBlessedSetting, BuffWarGodBlessedGameObj, BuffWarGodBlessedEntity, BuffWarGodBlessedData>(buffSetting, (data) => {
            data.MyObjName = "战神护佑";
            data.buffInfo = buffSetting.BuffInfo;
        });
    }
    
    private int InstanceBuffFleetFooted(SOBuffFleetFootedSetting buffSetting) {
        return Instance<SOBuffFleetFootedSetting, BuffFleetFootedGameObj, BuffFleetFootedEntity, BuffFleetFootedData>(buffSetting, (data) => {
            data.MyObjName = "飞毛腿";
            data.buffInfo = buffSetting.BuffInfo;
        });
    }

    #endregion

    /// <summary>
    /// 获取三个不同的增益配置
    /// </summary>
    /// <returns></returns>
    public List<Setting> GetSettingsNotDuplicate(int count) {
        List<Setting> retSetting = new List<Setting>();
        List<int> retIndex = new List<int>();
        List<Setting> tempBuffSettings = new List<Setting>(setting.BuffSettings);

        while (count > 0) {
            int randIndex = UnityEngine.Random.Range(0, tempBuffSettings.Count);
            if (retIndex.Contains(randIndex)) {
                continue;
            }
            retSetting.Add(tempBuffSettings[randIndex]);
            retIndex.Add(randIndex);
            count--;
        }

        return retSetting;
    } 
}