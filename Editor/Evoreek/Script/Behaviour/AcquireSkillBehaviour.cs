using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName = "Evoreek/Behaviour/AcquireSkillBehaviour")]
public class AcquireSkillBehaviour : GameBehaviour {
    public bool isPreloadingSkill;
    private List<AcquireSkillBehaviourData> datas = new List<AcquireSkillBehaviourData>();
    private PickConditionOutput output;
    public override void Register<T>(T arg1) {
        base.Register(arg1);
        B<int> info = arg1 as B<int>;
        LoadData(info.t1);
    }

    public override void UnRegister<T>(T arg1) {
        base.UnRegister(arg1);
    }

    public override void LoadData(int id) {
        base.LoadData(id);
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        GameObject obj = go.GetObj();
        GameComp comp = go.GetComp();
        Data data = go.GetData();
        
        AcquireSkillBehaviourData tempData = new AcquireSkillBehaviourData() {
            id = id,
            go = obj,
            skillType = data.SkillType,
            grade = data.gradeType,
        };
        
        if (BeRegisteredID.Contains(id)) {
            var reflectedType = MethodBase.GetCurrentMethod().ReflectedType;
            if (reflectedType != null) {
                Logger.PrintELog($"{reflectedType.Name} 重复注册 id: {id}");
            }
        } else {
            BeRegisteredID.Add(id);
            datas.Add(tempData);
        }
    }
    
    public override void GetControl<T>(T t) {
        base.GetControl(t);
        if (t is PickConditionOutput) {
            output = t as PickConditionOutput;
        }
    }

    public override void UpdateData() {
        base.UpdateData();
    }

    public override void Update() {
        base.Update();
        UpdateData();
        if (datas != null && datas.Count > 0) {
            foreach (var tempData in datas) {
                PreloadingSkill(tempData);
                GetPickId(tempData);
                AcquireSkill(tempData);
            }
        }
    }

    private void PreloadingSkill(AcquireSkillBehaviourData tempData) {
        if (isPreloadingSkill) {
            if (tempData.skillSetting == null) {
                tempData.skillSetting = GetSkillSetting(tempData.skillType);
            }
        }
    }

    private void GetPickId(AcquireSkillBehaviourData tempData) {
        if (!tempData.go.activeSelf) {
            return;
        }

        foreach (var tempChild in output.outputChild) {
            if (tempChild.id == tempData.id) {
                tempData.pickId = tempChild.pickId;
                return;
            }
        }
    }

    /// <summary>
    /// 装载技能
    /// </summary>
    /// <param name="tempData"></param>
    private void AcquireSkill(AcquireSkillBehaviourData tempData) {//获取技能
        if (tempData.pickId == 0 || !tempData.go.activeSelf) {//不存在拾取者 或者 物体未激活
            return;
        }
        //获取拾取者
        Entity pickEntity = gameSystem.MyEntityFeature.Get(tempData.pickId);
        tempData.pickId = 0;

        //判断拾取者是否已存在该技能类型
        bool ifPickContainSkill = pickEntity.CheckSkillType(tempData.skillType);
        if (ifPickContainSkill) {
            Logger.PrintELog($"角色已拥有技能类型 : {tempData.skillType.ToString()}");
            return;
        }
    
        //获取技能配置
        if (tempData.skillSetting == null) {
            tempData.skillSetting = GetSkillSetting(tempData.skillType);
        }

        //实例化技能
        int skillID = gameSystem.GetSystem<SkillSystem>().Instance(tempData.skillSetting);
        //装载到角色技能数据中
        bool ifAcquireSkill = pickEntity.AcquireSkill(skillID);
        if (ifAcquireSkill) {//将当前技能书回收 目前先销毁处理
            GameData.AudioSystem.AddSoundPlay(new AudioData() {
                InstanceID = tempData.id,
                tempAudioName = "本地玩家拾取技能",
                isLoop = false,
                is3D = true,
                isFollow = false,
            });
            Logger.Print(tempData.go.name + "销毁 获取技能");
            tempData.go.SetActive(false);
        }
    }

    private Setting GetSkillSetting(SkillType type) {
        return gameSystem.GetSystem<SkillSystem>().GetSettingBySkillType(type);
    }
}

public class AcquireSkillBehaviourData {
    public int id;
    public SkillType skillType;//技能类型
    public Setting skillSetting;
    public int pickId;//拾取者的ID
    public GameObject go;
    public GradeType grade;
}