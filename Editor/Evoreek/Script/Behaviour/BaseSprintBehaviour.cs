using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
[CreateAssetMenu(menuName = "Evoreek/Behaviour/BaseSprintBehaviour")]
public class BaseSprintBehaviour : GameBehaviour {
    private List<BaseSprintBehaviourData> datas = new List<BaseSprintBehaviourData>();
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
        
        BaseSprintBehaviourData tempData = new BaseSprintBehaviourData() {
            id = id,
            go = obj,
            sprintDuration = data.SprintDuration,
            animator = comp.animator,
            controller = comp.CC,
            sprintSpeed = data.SprintSpeed,
            runtimeBaseValueData = data.runtimeBaseValueData,
            SprintConsume  = data.SprintConsume,
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
        if (t is InputConditionOutput) {
            input = t as InputConditionOutput;
        }
    }

    public override void UpdateData() {
        base.UpdateData();
        foreach (var tempData in datas) {
            GameObj go = gameSystem.MyGameObjFeature.Get(tempData.id);
            Data data = go.GetData();
            tempData.runtimeBaseValueData = data.runtimeBaseValueData;
        }
    }

    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
            BaseSprint(tempData);
        }
    }

    private void BaseSprint(BaseSprintBehaviourData tempData) {
        if (input == null) {
            return;
        }

        if (tempData.isSprinting) {
            if (tempData.sprintRuntimeDuration > 0) {
                tempData.sprintRuntimeDuration -= Time.deltaTime;
                Sprint(tempData);
            } else {
                Logger.Print("释放冲刺技能完成");
                tempData.sprintRuntimeDuration = tempData.sprintDuration;
                tempData.isSprinting = false;
                tempData.animator.SetBool("Sprint", false);
            }
            
            return;
        }

        if (input.Space) {
            Entity entity = gameSystem.MyEntityFeature.Get(tempData.id);
            if (entity.PHYSICALPOWERChange(tempData.SprintConsume)) {
                Logger.Print("释放冲刺技能");
                tempData.isSprinting = true;
                tempData.animator.SetBool("Sprint", true);
            }
        }
    }

    private void Sprint(BaseSprintBehaviourData tempData) {
        tempData.controller.Move(tempData.go.transform.forward * tempData.sprintSpeed * Time.deltaTime);
    }
}

public class BaseSprintBehaviourData {
    public int id;
    public GameObject go;
    public CharacterController controller;
    public Animator animator;
    public BaseValueData runtimeBaseValueData;
    public bool isSprinting;//冲刺
    public float sprintDuration;
    public float sprintRuntimeDuration;//冲刺时间
    public float sprintSpeed;
    public int SprintConsume;//冲刺消耗
}