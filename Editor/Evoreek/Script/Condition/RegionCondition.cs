using System.Collections.Generic;
using System.Reflection;
using Common;
using UnityEngine;

/// <summary>
/// 条件
/// </summary>
[CreateAssetMenu(menuName = "Evoreek/Condition/RegionCondition")]
public class RegionCondition : GameCondition {
    
    public GameObject RTSTargetPosGO;
    private RectTransform rectTran;
    public float clickDelay;
    
    private List<RegionConditionData> datas = new List<RegionConditionData>();
    private RegionConditionOutput output = new RegionConditionOutput();

    private InputConditionOutput input;
    
    public override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        RectTransform[] allRect = FindObjectsOfType<RectTransform>(true);
        foreach (var tempRect in allRect) {
            if (tempRect.name.Equals("Region")) {
                rectTran = tempRect;
            }
        }
    }

    public override void Register<T>(T arg1) {
        base.Register(arg1);
        B<int> info = arg1 as B<int>;
        int id = info.t1;
        LoadData(id);
    }
    
    protected override void LoadData(int id) {
        base.LoadData(id);
        
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        GameObject obj = go.GetObj();
        Data data = go.GetData();
        
        RegionConditionData tempData = new RegionConditionData() {
            id = id,
            go = obj,
        };

        RegionConditionOutputChild tempOutput = new RegionConditionOutputChild() {
            id = id,
        };
        
        if (BeRegisteredID.Contains(id)) {
            var reflectedType = MethodBase.GetCurrentMethod().ReflectedType;
            if (reflectedType != null) {
                Logger.PrintELog($"{reflectedType.Name} 重复注册 id: {id}");
            }
        } else {
            BeRegisteredID.Add(id);
            datas.Add(tempData);
            output.outputChild.Add(tempOutput);
        }
    }
    
    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
            GetInput(tempData);//获取输入 判断是否按下左键
            DrawSelect(tempData);//绘制框选
            SelectObject(tempData);//在长按且绘制的窗口的四个点计算被框选的物体
        }
    }

    public override void GetControl<T>(T t) {
        base.GetControl(t);
        if (t is InputConditionOutput) {
            input = t as InputConditionOutput;
        }
    }

    public override IOutput GetResult() {
        base.GetResult();
        return output;
    }

    private void GetInput(RegionConditionData tempData) {
        if (!tempData.runtimeData.isClick) {
            if (input.LeftClick) {
                tempData.runtimeData.isClick = true;
                tempData.runtimeData.firstClickPos = Input.mousePosition;
                tempData.runtimeData.firstClickTime = Time.time;
                ClearAllSelect(tempData);
            }
        }
        
        if (tempData.runtimeData.isClick) {
            if (input.LeftLongClick) {//如果比延时时间久，视为长按，否则是短按
                tempData.runtimeData.isLongClick = Time.realtimeSinceStartup >
                                                   tempData.runtimeData.firstClickTime +
                                                   clickDelay;
            }

            if (input.LeftUp) {
                tempData.runtimeData.isClick = false;
                tempData.runtimeData.isLongClick = false;
                tempData.runtimeData.firstClickTime = Time.realtimeSinceStartup;
            }
        }
    }

    private void DrawSelect(RegionConditionData tempData) {
        //设置UI的位置和大小
        if (tempData.runtimeData.isLongClick) {
            rectTran.gameObject.SetActive(true);
            Vector3 rectVec = (tempData.runtimeData.firstClickPos + Input.mousePosition) / 2;
            float x = Mathf.Abs(tempData.runtimeData.firstClickPos.x - Input.mousePosition.x);
            float y = Mathf.Abs(tempData.runtimeData.firstClickPos.y - Input.mousePosition.y);
            rectTran.position = rectVec;
            rectTran.sizeDelta = new Vector2(x, y);
        } else {
            rectTran.gameObject.SetActive(false);
        }
    }

    private void SelectObject(RegionConditionData tempData) {
        if (!tempData.runtimeData.isLongClick) {
            return;
        }
        //从四边形的四个点取到四个向量
        bool isPointA = GetTouchTerrainPos(tempData.runtimeData.firstClickPos, out tempData.runtimeData.pointA);// 选中物体判断在四个边的内部
        bool isPointB = GetTouchTerrainPos(new Vector3(Input.mousePosition.x, tempData.runtimeData.firstClickPos.y, tempData.runtimeData.firstClickPos.z), out tempData.runtimeData.pointB);
        bool isPointC = GetTouchTerrainPos(Input.mousePosition, out tempData.runtimeData.pointC);
        bool isPointD = GetTouchTerrainPos(new Vector3(tempData.runtimeData.firstClickPos.x, Input.mousePosition.y, tempData.runtimeData.firstClickPos.z), out tempData.runtimeData.pointD);
        
        if (tempData.runtimeData.pointC == tempData.runtimeData.pointA) {
            return;
        }
        
        Vector3 tempClockWiseVec = (tempData.runtimeData.pointC - tempData.runtimeData.pointA).normalized;
        bool isClockWise = tempClockWiseVec.x * tempClockWiseVec.z < 0; //框选的顺序是顺时针？逆时针？根据 p1p3 向量的 xz 乘积正负判断顺逆 顺负逆正
        
        Vector3 v1, v2, v3, v4; //叉积结果的Y判断在向量的左边还是右边，当四个向量都为的物体
        v1 = tempData.runtimeData.pointB - tempData.runtimeData.pointA;
        v2 = tempData.runtimeData.pointC - tempData.runtimeData.pointB;
        v3 = tempData.runtimeData.pointD - tempData.runtimeData.pointC;
        v4 = tempData.runtimeData.pointA - tempData.runtimeData.pointD;
        
        Dictionary<int, GameObj> goList = gameSystem.MyGameObjFeature.GetAllDic();
        foreach (var go in goList) {
            int id = go.Key;
            GameObject tempGo = go.Value.GetObj();
            Vector3 targetVec = tempGo.transform.position;
        
            Vector3 q1, q2, q3, q4;// 当前物体和点连接的向量
            q1 = targetVec - tempData.runtimeData.pointA;
            q2 = targetVec - tempData.runtimeData.pointB;
            q3 = targetVec - tempData.runtimeData.pointC;
            q4 = targetVec - tempData.runtimeData.pointD;
        
            float y1 = Vector3.Cross(v1, q1).y;// 判断 物体是否在多边形内 依次对四个向量做叉积 取Y值
            float y2 = Vector3.Cross(v2, q2).y;
            float y3 = Vector3.Cross(v3, q3).y;
            float y4 = Vector3.Cross(v4, q4).y;
        
            if (isClockWise) {//顺时针 玩家的向量在边框的向量的右边 逆时针相反
                if (y1 > 0 && y2 > 0 && y3 > 0 && y4 > 0) {
                    JoinSelect(tempData, id);
                } else {
                    ClearSelect(tempData, id);
                }
            } else {
                if (y1 < 0 && y2 < 0 && y3 < 0 && y4 < 0) {
                    JoinSelect(tempData, id);
                } else {
                    ClearSelect(tempData, id);
                }
            }
        }
    }

    private bool GetTouchTerrainPos(Vector3 point, out Vector3 retVec) {
        Ray ray = Camera.main.ScreenPointToRay(point);
        retVec = Vector3.zero;
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Terrain"))) {
            retVec = hit.point;
            return true;
        }
        
        return false;
    }

    private void ClearAllSelect(RegionConditionData tempData) {
        foreach (var tempOutput in output.outputChild) {
            if (tempOutput.id != tempData.id) {//处理当前ID的输出列表
                continue;
            }

            tempOutput.regionIds.Clear();
        }
    }
    
    private void ClearSelect(RegionConditionData tempData, int id) {
        foreach (var tempOutput in output.outputChild) {
            if (tempOutput.id != tempData.id) {
                continue;
            }

            if (tempOutput.regionIds.Contains(id)) {
                tempOutput.regionIds.Remove(id);
            }
        }
    }

    private void JoinSelect(RegionConditionData tempData, int id) {
        foreach (var tempOutput in output.outputChild) {
            if (tempOutput.id != tempData.id) {
                continue;
            }

            if (!tempOutput.regionIds.Contains(id)) {
                tempOutput.regionIds.Add(id);
            }
        }
    }
}

public class RegionConditionData {
    public int id;
    public GameObject go;
    public List<int> regionIds;//框选的单位
    public RegionConditionRuntimeData runtimeData = new RegionConditionRuntimeData();
}

public class RegionConditionRuntimeData {
    public float firstClickTime;
    public bool isClick;
    public bool isLongClick;
    public Vector3 firstClickPos;
    public Vector3 pointA, pointB, pointC, pointD;
}

public class RegionConditionOutput : IOutput {
    public List<RegionConditionOutputChild> outputChild = new List<RegionConditionOutputChild>();
}

public class RegionConditionOutputChild {
    public int id;//主动检测的角色
    public List<int> regionIds = new List<int>();//框选的角色ID列表
}