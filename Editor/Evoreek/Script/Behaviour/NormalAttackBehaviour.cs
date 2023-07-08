using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
[CreateAssetMenu(menuName = "Evoreek/Behaviour/NormalAttackBehaviour")]
public class NormalAttackBehaviour : GameBehaviour {
    private List<NormalAttackBehaviourData> datas = new List<NormalAttackBehaviourData>();
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

        List<WeaponAttackDetect> tempWeapons = new List<WeaponAttackDetect>();
        tempWeapons = obj.transform.GetComponentsInChildren<WeaponAttackDetect>().ToList();
        
        NormalAttackBehaviourData tempData = new NormalAttackBehaviourData() {
            id = id,
            go = obj,
            runtimeBaseValueData = data.runtimeBaseValueData,
            AttackHash = Animator.StringToHash("MeleeAttack"),
            detects = tempWeapons,
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
        foreach (var temp in datas) {
            GameObj go = gameSystem.MyGameObjFeature.Get(temp.id);
            Data data = go.GetData();
            temp.runtimeBaseValueData = data.runtimeBaseValueData;
        }
    }

    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
            if (input == null) {
                continue;
            }

            Attack(tempData);

        }
    }

    private void Attack(NormalAttackBehaviourData data) {
        Animator animator = gameSystem.MyAnimationFeature.GetAnimator(data.id);
        AnimatorStateInfo info = gameSystem.MyAnimationFeature.GetCurrentAnimatorStateInfo(animator, 1);
        
        if (input.LeftClick) {//按下空格后
            animator.SetTrigger("Attack");
        }
        if (info.IsName("MeleeAttack")) {
            animator.speed = 1 + (float)data.runtimeBaseValueData.ATKSPEED / 10;
        } else {
            animator.speed = 1;
        }
    }
}

public class NormalAttackBehaviourData {
    public int id;
    public GameObject go;
    public int AttackHash;
    public BaseValueData runtimeBaseValueData;
    public List<WeaponAttackDetect> detects;
}