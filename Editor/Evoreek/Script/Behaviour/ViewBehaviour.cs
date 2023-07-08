using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 视角行为 - 视角行为
/// </summary>
[CreateAssetMenu(menuName = "Evoreek/Behaviour/ViewBehaviour")]
public class ViewBehaviour : GameBehaviour {
    private List<ViewBehaviourData> datas = new List<ViewBehaviourData>();
    private InputConditionOutput input;
    public bool isOpenEdgeMove;

    public override void Register<T>(T arg1) {
        base.Register(arg1);
        var info = arg1 as B<int>;
        LoadData(info.t1);
    }

    public override void LoadData(int id) {
        base.LoadData(id);
        
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        Data data = go.GetData();

        ViewBehaviourData tempData = new ViewBehaviourData();
        tempData.id = id;
        tempData.go = go.GetObj();
        tempData.cameraGo = data.camera.transform;
        tempData.defaultCameraAngle = data.offEngle;
        tempData.defaultCameraHeight = data.defaultRelativeHeight;
        tempData.runtimData.curHeight = data.defaultRelativeHeight;
        tempData.runtimData.curAngle = data.offEngle;
        tempData.scrollWheelRatio = data.scrollWheelRatio;
        tempData.edgeMoveRatio = data.edgeMoveRatio;

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
        this.input = (InputConditionOutput)Convert.ChangeType(t, typeof(InputConditionOutput));
    }

    public override void LateUpdate() {
        base.LateUpdate();
        
        foreach (var tempData in datas) {
            View(tempData);
        }
    }

    #region 相机

    private void View(ViewBehaviourData tempData) {
        EdgeMove(tempData);
        ScrollWheelChangeHeight(tempData);
        SetPointAndEngle(tempData);
    }

    /// <summary>
    /// 设置位置和方向
    /// </summary>
    /// <param name="tempData"></param>
    private void SetPointAndEngle(ViewBehaviourData tempData) {
        Vector3 offVec = new Vector3(0, tempData.runtimData.curHeight, -Mathf.Abs(tempData.runtimData.curHeight / Mathf.Tan(Mathf.Deg2Rad * tempData.runtimData.curAngle.x)));

        if (isOpenEdgeMove) {
            tempData.cameraGo.position = tempData.runtimData.previourVec + offVec;
        } else {
            tempData.cameraGo.position = GameData.MainPlayer.GetObj().transform.position + offVec;
        }
        tempData.cameraGo.rotation = Quaternion.Euler(tempData.runtimData.curAngle);
    }

    /// <summary>
    /// 滚轮改变相机高度 最低到玩家的位置
    /// </summary>
    /// <param name="tempData"></param>
    private void ScrollWheelChangeHeight(ViewBehaviourData tempData) {
        tempData.runtimData.curHeight = Mathf.Lerp(tempData.runtimData.curHeight, tempData.runtimData.curHeight + input.ScrollWheel, Time.deltaTime * tempData.scrollWheelRatio);
        tempData.runtimData.curHeight = Mathf.Max(tempData.runtimData.curHeight, 0);
    }

    /// <summary>
    /// 相机看向目标位置
    /// </summary>
    /// <param name="cameraGo"></param>
    /// <param name="preciousVec"></param>
    /// <param name="vData"></param>
    private void CameraLookAtTarget(ViewBehaviourData tempData) {
        Transform cameraGo = tempData.go.transform;
        Vector3 preciousVec = tempData.runtimData.previourVec;
        ViewData vData = tempData.viewData;
        vData.curRTSHeight = Mathf.Lerp(vData.curRTSHeight, vData.curRTSHeight + input.ScrollWheel, Time.deltaTime * vData.scrollWheelRatio);//滑轮控制当前高度

        vData.curRTSHeight = Mathf.Min(vData.curRTSHeight, vData.maxThirdHeight);//限制最高高度
        if (vData.curRTSHeight <= vData.minThirdHeight) {
            vData.curRTSHeight = vData.minThirdHeight - 0.01f;
        }

        float finalOffZ = -Mathf.Abs(vData.curRTSHeight / Mathf.Tan(Mathf.Deg2Rad * vData.offEngle.x));
        float finalOffY = vData.curRTSHeight;
        Vector3 finalOffVec = new Vector3(0, finalOffY, finalOffZ);//视角偏移 通过相机高度与相机偏移角度计算出 forward 方向的 z
        Vector3 cameraPos = preciousVec + finalOffVec;//相机位置
        cameraGo.position = Vector3.Lerp(cameraGo.position, cameraPos, Time.deltaTime * vData.thirdFollowRatio);

        Quaternion q = Quaternion.Lerp(cameraGo.rotation, Quaternion.Euler(vData.offEngle), Time.deltaTime * vData.changeRatio);
        cameraGo.rotation = q;
    }

    /// <summary>
    /// 相机边缘移动
    /// </summary>
    /// <param name="vData"></param>
    private void EdgeMove(ViewBehaviourData tempData) {
        if (isOpenEdgeMove) {//鼠标放到边界外相机移动
            float edgeSize = 10f;
            if (Input.mousePosition.x > Screen.width - edgeSize) {
                Vector3 moveVec = tempData.runtimData.previourVec;
                moveVec.x += tempData.edgeMoveRatio * Time.deltaTime;
                tempData.runtimData.previourVec = moveVec;
            } else if (Input.mousePosition.x < edgeSize) {
                Vector3 moveVec = tempData.runtimData.previourVec;
                moveVec.x -= tempData.edgeMoveRatio * Time.deltaTime;
                tempData.runtimData.previourVec = moveVec;
            } else if (Input.mousePosition.y > Screen.height - edgeSize) {
                Vector3 moveVec = tempData.runtimData.previourVec;
                moveVec.z += tempData.edgeMoveRatio * Time.deltaTime;
                tempData.runtimData.previourVec = moveVec;
            } else if (Input.mousePosition.y < edgeSize) {
                Vector3 moveVec = tempData.runtimData.previourVec;
                moveVec.z -= tempData.edgeMoveRatio * Time.deltaTime;
                tempData.runtimData.previourVec = moveVec;
            }
        }
    }

    #endregion
}

public class ViewBehaviourData {
    public int id;
    public GameObject go;
    public Transform cameraGo;
    public Transform viewTargetTran;//视角目标物体
    public Transform viewTPSTargetTran;//视角目标物体

    public Vector3 FPSRelationVec;
    public Vector3 TPSRelationVec;
    
    public ViewData viewData;//旧视角数据

    public float defaultCameraHeight;//默认相机高度
    public Vector3 defaultCameraAngle;//默认相机角度
    public float scrollWheelRatio;//滚轮速率
    public float edgeMoveRatio;//边缘移动速率

    public ViewBehaviourRuntimeData runtimData = new ViewBehaviourRuntimeData();//新视角运行数据
    
    
    public PlayerViewType viewType;
}

public class ViewBehaviourRuntimeData {
    public float curHeight;
    public Vector3 curAngle;
    public Vector3 previourVec;
}

/// <summary>
/// 相机视角数据
/// </summary>
public class ViewData {
    public float scrollWheelRatio; //滑轮系数
    public float defaultRelativeHeight;//默认相对位置

    //第一人称
    public float maxFPSHeight;
    public float minFPSHeight;
    public float FPSFollowRatio; //跟随系数

    public float curRelativeHeight; //当前相对高度
    public float changeRatio;//人称切换速率系数

    //第三人称
    public float maxTPSHeight;
    public float minTPSHeight;
    public float TPSFollowRatio;

    //即时战略视角
    public float curRTSHeight;//即时战略当前高度
    public bool isOpenEdgeMove;//是否开启边缘滑动
    public float edgeMoveRatio;//边缘滑动速率系数
    public Vector3 previourVec;//旧位置 可以通过该位置赋值到目标位置实现跟随目标的RTS跟随视角 所以需要一个事件使得改变当前的跟随目标 也许是屏幕的点击事件

    //即时战略锁定角色视角
    public Vector3 offEngle; //偏移角度
    public float maxThirdHeight; //最高高度 - 即时战略模式公用
    public float minThirdHeight; //最低高度
    public float thirdFollowRatio; //跟随系数
}