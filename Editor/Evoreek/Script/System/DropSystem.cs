using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DropSystem : GameSys {
    private SODropSetting setting;
    private GameSystem gameSystem;
    public DropSystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        Init(gameSystem);
    }

    protected override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        setting = gameSystem.SoData.SOGameSetting.GetSetting<SODropSetting>();
        base.setting = this.setting;
    }

    public override void Update() {
        base.Update();
    }

    protected override void Clear() {
        base.Clear();
    }

    #region 不同实例创建接口
    
    /// <summary>
    /// 计算掉落概率
    /// </summary>
    /// <param name="info"></param>
    public void Probability(List<GradeInfo> info, Vector3 point) {
        int rand = Random.Range(0, 100);
        int tempRand = 0;
        bool isOutput = false;
        GradeType type = default;
        foreach (var tempInfo in info) {
            tempRand += tempInfo.OutputRate;
            if (rand <= tempRand) {
                isOutput = true;
                type = tempInfo.gradeType;
                break;
            }
        }

        if (isOutput) {
            Instance(type, point);
        }
    }

    /// <summary>
    /// 实例化掉落物 随机品阶 品阶随机技能类型
    /// </summary>
    public int Instance(GradeType type, Vector3 point) {
        GameData.GetTerrainPos(point, out Vector3 dropPos);
        GradeType gradeType = GetRandGradeType();//获取随机品阶
        return Instance<DropGameObj, DropEntity, DropData>((data) => {
            data.MyObjName = "掉落物";
            data.MyStartPointInfo = new PointInfo() {
                vec = dropPos,
                engle = Vector3.zero,
            };
            data.GravityRatio = setting.GravityRatio;
            data.detectDistance = setting.DetectDistance;
            data.gradeType = type;
            data.SkillType = gameSystem.GetSystem<SkillSystem>().GetGradeSetting(type).SkillInfo.skillType;//获取技能配置
            data.gradeInfo = setting.GradeInfos;
            data.pickObjectTypes = setting.PickObjectTypes;//可是
        });
    }

    /// <summary>
    /// 实例化掉落物 指定品阶 品阶随机技能类型
    /// </summary>
    public int Instance(Vector3 point, GradeType gradeType){
        GameData.GetTerrainPos(point, out Vector3 dropPos);
        return Instance<DropGameObj, DropEntity, DropData>((data) => {
            data.MyObjName = "掉落物";
            data.MyStartPointInfo = new PointInfo() {
                vec = dropPos,
                engle = Vector3.zero,
            };
            data.GravityRatio = setting.GravityRatio;
            data.detectDistance = setting.DetectDistance;
            data.gradeType = gradeType;
            data.SkillType = gameSystem.GetSystem<SkillSystem>().GetGradeSetting(gradeType).SkillInfo.skillType;//获取技能配置
            data.gradeInfo = setting.GradeInfos;
            data.pickObjectTypes = setting.PickObjectTypes;//可是
        });
    }

    #endregion

    /// <summary>
    /// 获取随机品质类型
    /// </summary>
    /// <returns></returns>
    private GradeType GetRandGradeType() {
        int num = Random.Range(0, 2);
        if (num == 0) {
            return GradeType.S;
        }
        return GradeType.B;
        // GradeType[] gradeTypes = Enum.GetValues(typeof(GradeType)) as GradeType[];
        // return gradeTypes[Random.Range(0, gradeTypes.Length)];
    }
}