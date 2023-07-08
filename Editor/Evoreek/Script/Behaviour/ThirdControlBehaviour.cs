using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
[CreateAssetMenu(menuName = "Evoreek/Behaviour/ControlBehaviour")]
public class ThirdControlBehaviour : GameBehaviour {
    private List<ThirdControlBehaviourData> datas = new List<ThirdControlBehaviourData>();
    private InputConditionOutput input;
    public override void GetControl<T>(T t) {
        base.GetControl(t);
        if (t is InputConditionOutput) {
            input = t as InputConditionOutput;
        }
    }

    public override void Register<T>(T arg1) { // 注册
        base.Register(arg1);
        var info = arg1 as B<int>;
        int id = info.t1;
        LoadData(id);
    }

    public override void UnRegister<T>(T arg1) {
        base.UnRegister(arg1);
    }

    public override void LoadData(int id) {
        base.LoadData(id);
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        GameComp comp = go.GetComp();
        Data data = go.GetData();
        GameObject obj = go.GetObj();

        ThirdControlBehaviourData tempData = new ThirdControlBehaviourData() {
            id = id,
            go = obj,
            controller = comp.CC,
            playerViewType = data.viewType,
            anim = comp.animator,
            headTran = comp.cameraViewTarget,
            playerMod = data.PlayerMod,
            runTimeBaseValueData = data.runtimeBaseValueData,
        };

        tempData.moveData = new MoveData() {
            slideRatio = data.SlideRatio,
            flyRatio = data.FlyRatio,
            fastFlyRatio = data.FastFlyRatio,
            backRatio = data.BackRatio,
            firstTurnRatio = data.FirstTurnRatio,
            thirdTurnRatio = data.ThirdTurnRatio,
            walkAnim = data.WalkAnim,
            runAnim = data.RunAnim,
        };

        tempData.rotateData = new RotateData() {
            xRotateSpeed = data.XRotateSpeed,
            yRotateSpeed = data.YRotateSpeed,
            xRotateMin = data.xRotateMin,
            xRotateMax = data.xRotateMax,
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

    /// <summary>
    /// 数据更新
    /// </summary>
    public override void UpdateData() {
        base.UpdateData();
        foreach (var temp in datas) {
            GameObj go = gameSystem.MyGameObjFeature.Get(temp.id);
            Data data = go.GetData();
            temp.playerMod = data.PlayerMod;
            temp.runTimeBaseValueData = data.runtimeBaseValueData;
        }
    }
    
    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
            Move(tempData);
        }
    }

    #region 移动

    private void Move(ThirdControlBehaviourData data) {
        if (input == null) {
            return;
        }

        if (data.controller == null) {
            return;
        }

        string HORIZONTAL = "Horizontal";
        string VERTICAL = "Vertical";
        data.controller.enabled = true;
        
        data.rotateData.yRotate += input.mouseX * Time.deltaTime * data.rotateData.yRotateSpeed;//旋转
        data.rotateData.xRotate -= input.mouseY * Time.deltaTime * data.rotateData.xRotateSpeed;
        data.go.transform.rotation = Quaternion.Euler(0, data.rotateData.yRotate, 0);
        data.rotateData.xRotate = Mathf.Clamp(data.rotateData.xRotate, data.rotateData.xRotateMin, data.rotateData.xRotateMax);
        data.headTran.localRotation = Quaternion.Euler(data.rotateData.xRotate, 0, 0);
        
        AnimatorStateInfo info = gameSystem.MyAnimationFeature.GetCurrentAnimatorStateInfo(data.anim, 1);//移动
        bool canMove = !info.IsName("MeleeAttack");
        if (canMove) {
            Vector3 dir = CheckInput(data);
            Vector3 vNormal = Vector3.Project(dir, data.go.transform.forward); //计算向量在竖直向的投影
            Vector3 hNormal = Vector3.Project(dir, data.go.transform.right);
            Vector3 yNormal = Vector3.Project(dir, data.go.transform.up);
            if (Vector3.Dot(vNormal, data.go.transform.forward) > 0) {
                vNormal *= data.runTimeBaseValueData.MOVESPEED;
            } else {
                vNormal *= data.runTimeBaseValueData.MOVESPEED / 2;
            }

            hNormal *= data.runTimeBaseValueData.MOVESPEED / 2;
            yNormal *= data.runTimeBaseValueData.MOVESPEED * 5;
            dir = vNormal + hNormal + yNormal;
            data.controller.Move(dir * Time.deltaTime); //控制器移动
            float vTargetAnimParam = 0;
            float hTargetAnimParam = 0;
            if (vNormal != Vector3.zero) {
                //竖直方向有速度
                float vRet = Vector3.Dot(vNormal, data.go.transform.forward); //上步结果和前方向进行点积，判断正负
                vTargetAnimParam = vRet > 0 ? 1 : -1;
                vTargetAnimParam *= (input.Shift ? data.moveData.runAnim : data.moveData.walkAnim);
            }

            if (hNormal != Vector3.zero) {
                float hRet = Vector3.Dot(hNormal, data.go.transform.right);
                hTargetAnimParam = hRet > 0 ? 1 : -1;
            }

            data.anim.SetFloat(HORIZONTAL, hTargetAnimParam);
            data.anim.SetFloat(VERTICAL, vTargetAnimParam);
        } else {
            Vector3 dir = CheckInput(data);
            if (dir != Vector3.zero) {
                data.anim.Play("Empty");
            }
        }
    }

    private Vector3 CheckInput(ThirdControlBehaviourData data) {
        Transform go = data.go.transform;
        Vector3 forward = input.W ? go.forward : Vector3.zero;
        Vector3 right = input.D ? go.right : Vector3.zero;
        Vector3 back = input.S ? -go.forward : Vector3.zero;
        Vector3 left = input.A ? -go.right : Vector3.zero;
        Vector3 up = Vector3.zero;
        Vector3 down = Vector3.zero;
        if (data.playerMod == PlayerMode.GODMODE) {
            up = input.SpaceLong ? go.up : Vector3.zero;
            down = input.C ? -go.up : Vector3.zero;
        }
        return forward + right + back + left + up + down;
    }

    #endregion
}

public class ThirdControlBehaviourData {
    public int id;
    public GameObject go;
    public CharacterController controller;
    public PlayerViewType playerViewType;//玩家视角
    public Animator anim;
    public MoveData moveData;
    public RotateData rotateData;
    public Transform headTran;
    public PlayerMode playerMod;
    public BaseValueData runTimeBaseValueData;
}

public class RotateData {
    public float xRotateSpeed;
    public float xRotateMin;
    public float xRotateMax;
    public float xRotate;
    public float yRotateSpeed;
    public float yRotate;
    public float zRotate;
}

public class MoveData {
    public float backRatio;//后退速度
    public float slideRatio;//侧向移动速度
    public float thirdTurnRatio;//第三人才转向速度
    public float firstTurnRatio;//第一人称转向速度
    public float runAnim;//跑步参数
    public float walkAnim;//行走参数
    public float flyRatio;//正常飞行模式
    public float fastFlyRatio;//快速飞行模式
}