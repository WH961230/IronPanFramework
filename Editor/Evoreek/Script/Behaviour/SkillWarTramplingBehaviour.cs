using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Evoreek/Behaviour/SkillWarTramplingBehaviour")]
public class SkillWarTramplingBehaviour : GameBehaviour {
    private List<SkillWarTramplingBehaviourData> datas = new List<SkillWarTramplingBehaviourData>();
    private DetectedWithinRangeConditionOutput detected;
    private InputConditionOutput input;
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

        SkillInfo skillInfo = GetSkillInfo(data.SkillIds);
        SkillWarTramplingBehaviourData tempData = new SkillWarTramplingBehaviourData() {
            id = id,
            go = obj,
            skillInfo = skillInfo,//获取拥有者的技能列表中为 当前技能类型的
            runtimeData = new SkillWarTramplingBehaviourRuntimeData() {
                skillCD = skillInfo.skillCD,
                skillDuration = skillInfo.skillDuration,
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
            if (tempSkillData.skillInfo.functionType == FunctionType.SkillWarTramplingFunction) {
                retInfo = tempSkillData.skillInfo;
            }
        }
        return retInfo;
    }

    public override void GetControl<T>(T t) {
        base.GetControl(t);
        if (t is DetectedWithinRangeConditionOutput) {
            detected = t as DetectedWithinRangeConditionOutput;
        } else if (t is InputConditionOutput) {
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
            if (input == null || detected == null) {
                return;
            }
            
            if (tempData.skillInfo == null) {
                continue;
            }
            SkillDuringDown(tempData);//技能倒计时
            SkillCDDown(tempData);//技能CD倒计时
            GetRangeIds(tempData);
            WarTrampling(tempData);
        }
    }

    private void GetRangeIds(SkillWarTramplingBehaviourData tempData) {
        foreach (var tempChild in detected.outputChild) {
            if (tempChild.id == tempData.id) {
                tempData.rangeIds = tempChild.rangeIds;
            }
        }
    }
    
    private void SkillDuringDown(SkillWarTramplingBehaviourData tempData) {
        if (tempData.runtimeData.skillDuration > 0) {
            tempData.runtimeData.skillDuration -= Time.deltaTime;
            SetImageAlpha(tempData.runtimeData.skillImage, 0.2f);
        } else {
            tempData.runtimeData.skillDuration = 0;
            SetImageAlpha(tempData.runtimeData.skillImage, 0.75f);
        }
    }

    private void SetImageAlpha(Image image, float a) {
        Color tempColor = image.color;
        tempColor.a = a;
        image.color = tempColor;
    }

    private void SkillCDDown(SkillWarTramplingBehaviourData tempData) {//技能持续时间为冷却倒计时
        if (tempData.runtimeData.skillCD > 0) {
            tempData.runtimeData.skillCD -= Time.deltaTime;
            tempData.runtimeData.skillImage.fillAmount = 1- (tempData.runtimeData.skillCD / tempData.skillInfo.skillCD);
        } else {
            tempData.runtimeData.skillCD = 0;
        }
    }

    private void WarTrampling(SkillWarTramplingBehaviourData tempData) {
        if (tempData.rangeIds == null || tempData.rangeIds.Count <= 0) {
            return;
        }

        if (!tempData.runtimeData.IsSkillPlaying && !tempData.runtimeData.IsSkillCoolDowning) {
            if (input.alpha2) {
                tempData.runtimeData.skillDuration = tempData.skillInfo.skillDuration;
                //获取范围内的物体
                List<GameObj> rangeGameObjs = GetGameObjListById(tempData.rangeIds);
            
                //临时取消物体的移动行为（AIMoveBehaviour）
                foreach (var tempGameObj in rangeGameObjs) {
                    gameSystem.MyEntityFeature.Get(tempGameObj.GetData().InstanceID).HPChange(tempData.id, tempData.skillInfo.onceDamageValue);
                    tempGameObj.UnRegister(FunctionType.AIMoveFunction, new B<int>{t1 = tempGameObj.GetData().InstanceID});//反注册移动行为
                    tempGameObj.UnRegister(FunctionType.AIAttackFunction, new B<int>{t1 = tempGameObj.GetData().InstanceID});//反注册移动行为

                    GameObject weaponDetectGo = gameSystem.MyPoolFeature.Get("Dizziness");
                    weaponDetectGo.transform.parent = tempGameObj.GetObj().transform;
                    weaponDetectGo.transform.localPosition = Vector3.up * 2;
                    weaponDetectGo.transform.forward = tempData.go.transform.forward;
                    tempData.dizzinessObjList.Add(weaponDetectGo);
                }
                
                tempData.treadOnObj = gameSystem.MyPoolFeature.Get("TreadOn");
                tempData.treadOnObj.transform.parent = tempData.go.transform;
                tempData.treadOnObj.transform.localPosition = Vector3.zero;

                tempData.treadOnAudioObj = GameData.AudioSystem.AddSoundPlay(new AudioData() {
                    isLoop = false,
                    is3D = true,
                    isFollow = true,
                    tempAudioName = "技能战争践踏",
                    InstanceID = tempData.id,
                });

                //n 秒 后重新注册
                ClockUtil.Instance.AlarmAfter(tempData.runtimeData.skillDuration, () => {
                    foreach (var tempGameObj in rangeGameObjs) {
                        tempGameObj.Register(FunctionType.AIMoveFunction, new B<int>{t1 = tempGameObj.GetData().InstanceID});//注册移动行为
                        tempGameObj.Register(FunctionType.AIAttackFunction, new B<int>{t1 = tempGameObj.GetData().InstanceID});//注册移动行为
                    }

                    foreach (var tempObj in tempData.dizzinessObjList) {
                        gameSystem.MyPoolFeature.Release("Dizziness", tempObj);
                    }

                    gameSystem.MyPoolFeature.Release("TreadOn", tempData.treadOnObj);
                    
                    GameData.AudioSystem.RemoveSoundPlay(tempData.treadOnAudioObj);

                    tempData.dizzinessObjList.Clear();
                    tempData.runtimeData.skillCD = tempData.skillInfo.skillCD;//设置 CD
                });
            }
        }
    }

    private List<GameObj> GetGameObjListById(List<int> ids) {
        List<GameObj> retList = new List<GameObj>();
        foreach (var tempId in ids) {
            GameObj tempGameObj = gameSystem.MyGameObjFeature.Get(tempId);
            retList.Add(tempGameObj);
        }

        return retList;
    }
}

public class SkillWarTramplingBehaviourData {
    public int id;
    public GameObject go;
    public List<int> rangeIds;
    public SkillInfo skillInfo;
    public GameObject treadOnObj;//地震特效
    public List<GameObject> dizzinessObjList = new List<GameObject>();
    public SkillWarTramplingBehaviourRuntimeData runtimeData;
    public GameObject treadOnAudioObj;
}

public class SkillWarTramplingBehaviourRuntimeData {
    public float skillCD;
    public float skillDuration;//持续时间
    public Image skillImage;
    public bool IsSkillPlaying => skillDuration > 0;
    public bool IsSkillCoolDowning => skillCD > 0;
}