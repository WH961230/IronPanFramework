using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Evoreek/Behaviour/WhirlwindBehaviour")]
public class SkillWhirlwindBehaviour : GameBehaviour {
    private List<SkillWhirlwindBehaviourData> datas = new List<SkillWhirlwindBehaviourData>();
    private InputConditionOutput input;
    public override void Register<T>(T arg1) {//仅限玩家注册
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
        
        Logger.Print($"恭喜 {data.MyObjName} 注册 旋风斩功能");

        SkillInfo skillInfo = GetSkillInfo(data.SkillIds);
        SkillWhirlwindBehaviourData tempData = new SkillWhirlwindBehaviourData() {
            id = id,
            go = obj,
            skillName = "技能旋风斩",
            anim = comp.animator,
            skillInfo = skillInfo,//获取拥有者的技能列表中为 当前技能类型的
            runtimeData = new SkillWhirlwindBehaviourRuntimeData() {
                skillCD = 0,//初始CD为0
                skillDuration = 0,//初始持续时间
                skillImage = skillInfo.skillImage,
            },
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

    private SkillInfo GetSkillInfo(List<int> skillIds) {
        SkillInfo retInfo = null;
        foreach (var tempId in skillIds) {
            Data tempSkillData = gameSystem.MyEntityFeature.Get(tempId).GetData();
            if (tempSkillData.skillInfo.functionType == FunctionType.WhirlwindSkillFunction) {
                retInfo = tempSkillData.skillInfo;
            }
        }
        return retInfo;
    }
    
    public override void GetControl<T>(T t) {
        base.GetControl(t);
        if (t is InputConditionOutput) {
            input = t as InputConditionOutput;
        }
    }

    public override void UpdateData() {
        base.UpdateData();
    }

    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
            if (tempData.skillInfo == null) {
                continue;
            }
            SkillDuringDown(tempData);//技能倒计时
            SkillCDDown(tempData);//技能CD倒计时
            GetInput(tempData);
            Whirlwind(tempData);
        }
    }

    private void SetImageAlpha(Image image, float a) {
        Color tempColor = image.color;
        tempColor.a = a;
        image.color = tempColor;
    }

    private void SkillDuringDown(SkillWhirlwindBehaviourData tempData) {
        if (tempData.runtimeData.skillDuration > 0) {
            tempData.runtimeData.skillDuration -= Time.deltaTime;
            SetImageAlpha(tempData.runtimeData.skillImage, 0.2f);
        } else {
            tempData.runtimeData.skillDuration = 0;
            SetImageAlpha(tempData.runtimeData.skillImage, 0.75f);
        }
    }

    private void SkillCDDown(SkillWhirlwindBehaviourData tempData) {//技能持续时间为冷却倒计时
        if (tempData.runtimeData.skillCD > 0) {
            tempData.runtimeData.skillCD -= Time.deltaTime;
            tempData.runtimeData.skillImage.fillAmount = 1- (tempData.runtimeData.skillCD / tempData.skillInfo.skillCD);
        } else {
            tempData.runtimeData.skillCD = 0;
        }
    }

    private void GetInput(SkillWhirlwindBehaviourData tempData) {
        if (input.G) {//按下G使用技能 
            if (!tempData.runtimeData.IsSkillCoolDowning && !tempData.runtimeData.IsSkillPlaying) {
                tempData.runtimeData.skillDuration = tempData.skillInfo.skillDuration;
                Logger.Print($"技能 {tempData.skillName} 开始使用！");
            }
        }
    }

    private void Whirlwind(SkillWhirlwindBehaviourData tempData) {
        if (tempData.runtimeData.IsSkillPlaying) {
            if (tempData.runtimeData.weaponDetect == null) {//加载武器检测物体
                GameObject weaponDetectGo = gameSystem.MyPoolFeature.Get("WhirlwindDetect");
                weaponDetectGo.transform.parent = tempData.go.transform;
                weaponDetectGo.transform.localPosition = Vector3.up;
                weaponDetectGo.transform.forward = tempData.go.transform.forward;

                WeaponAttackDetect attackDetect = weaponDetectGo.GetComponentInChildren<WeaponAttackDetect>();
                attackDetect.OpenDetect(false);
                tempData.runtimeData.weaponDetect = attackDetect;
                // Logger.Print($"开启旋风斩");
                tempData.runtimeData.audioClipGo = GameData.AudioSystem.AddSoundPlay(new AudioData() {
                    InstanceID = tempData.id,
                    tempAudioName = "旋风斩技能",
                    isLoop = true,
                    isFollow = true,
                    is3D = false,
                });
                Logger.Print("旋风斩");
            } else {
                tempData.runtimeData.weaponDetect.transform.RotateAround(tempData.runtimeData.weaponDetect.transform.position, Vector3.up, tempData.runtimeData.weaponDetect.roundSpeed);
            }
            tempData.anim.SetInteger("SkillIndex", tempData.skillInfo.skillAnimIndex);
        } else {
            if (tempData.runtimeData.weaponDetect != null) {
                // Logger.Print($"关闭旋风斩");
                tempData.runtimeData.weaponDetect.CloseDetect();
                gameSystem.MyPoolFeature.Release("WhirlwindDetect", tempData.runtimeData.weaponDetect.transform.parent.gameObject);
                tempData.runtimeData.weaponDetect = null;
                tempData.runtimeData.skillCD = tempData.skillInfo.skillCD;//设置 CD
                // Logger.Print($"技能 {tempData.skillName} 使用结束！");
                GameData.AudioSystem.RemoveSoundPlay(tempData.runtimeData.audioClipGo);
            }
        }
    }
}

public class SkillWhirlwindBehaviourData {
    public int id;
    public string skillName;
    public GameObject go;
    public Animator anim;
    public SkillInfo skillInfo;
    public SkillWhirlwindBehaviourRuntimeData runtimeData;
}

public class SkillWhirlwindBehaviourRuntimeData {
    public float skillCD;
    public float skillDuration;//技能持续时间
    public bool IsSkillPlaying => skillDuration > 0;
    public bool IsSkillCoolDowning => skillCD > 0;
    public WeaponAttackDetect weaponDetect;
    public Image skillImage;
    public GameObject audioClipGo;
}