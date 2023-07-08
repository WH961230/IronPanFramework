using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 框选行为
/// </summary>
[CreateAssetMenu(menuName = "Evoreek/Behaviour/RegionBehaviour")]
public class RegionBehaviour : GameBehaviour {
    public List<RegionBehaviourData> datas = new List<RegionBehaviourData>();
    private RegionConditionOutput region;

    public override void Register<T>(T arg1) {
        base.Register(arg1);
        var tempInfo = arg1 as B<int>;
        LoadData(tempInfo.t1);
    }

    public override void UnRegister<T>(T arg1) {
        base.UnRegister(arg1);
    }

    public override void Clear() {
        base.Clear();
    }

    public override void LoadData(int id) {
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        GameObject obj = go.GetObj();
        GameComp comp = go.GetComp();
        Data data = go.GetData();
        
        RegionBehaviourData tempData = new RegionBehaviourData() {
            id = id, 
            go = obj,
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
        if (t is RegionConditionOutput) {
            region = t as RegionConditionOutput;
        }
    }

    public override void Update() {
        base.Update();
        if (region == null) {
            return;
        }
        foreach (var tempData in datas) {
            GetSelectIds(tempData);
            if (tempData.regionIds == null || tempData.regionIds.Count == 0) {
                continue;
            }
            UnSelectEffect(tempData);
            SelectEffect(tempData);
        }
    }

    private void GetSelectIds(RegionBehaviourData tempData) {
        foreach (var tempOutput in region.outputChild) {
            if (tempOutput.id == tempData.id) {
                tempData.regionIds = tempOutput.regionIds;
            }
        }
    }
    
    private void UnSelectEffect(RegionBehaviourData tempData) {
        List<GameObj> gos = gameSystem.MyGameObjFeature.GetAllGameObj();
        foreach (var tempGO in gos) {
            SpriteRenderer renderer = tempGO.GetComp().SpriteRenderer;
            if (renderer != null) {
                Color color = renderer.color;
                color.a = 0f;
                renderer.color = color;
            }
        }
    }

    private void SelectEffect(RegionBehaviourData tempData) {
        foreach (var tempId in tempData.regionIds) {
            GameObj tempObj = gameSystem.MyGameObjFeature.Get(tempId);
            if (tempObj != null) {
                SpriteRenderer renderer = tempObj.GetComp().SpriteRenderer;
                if (renderer != null) {
                    Color color = renderer.color;
                    color.a = 1f;
                    renderer.color = color;
                }
            }
        }

        tempData.regionIds = null;
    }

    // private GameObject s, s5;
    // private void GetInput() {
    //     if (input.LeftClick) {
    //         info.btnDownTime = Time.realtimeSinceStartup;//按下时间
    //         info.leftClickPos = Input.mousePosition;//点击起始位置
    //         CancelRegion();
    //         //检测单击物体是否可被框选
    //         s = PrintLogObject(info.leftClickPos, false, out Vector3 p, s);// 选中物体判断在四个边的内部
    //         if (SelectSingleObject(p, out int id, out GameObject go)) {
    //             JoinRegion(id, go);
    //         }
    //     }
    //
    //     if (input.LeftLongClick) {
    //         if (Time.realtimeSinceStartup > info.btnDownTime + info.delay) {// 延迟时间
    //             info.isLongClicked = true;
    //         }
    //     }
    //
    //     if (input.LeftUp) {//松开之后如果选中物体 发送列表更新消息 如果没有选中任何物体不发送消息
    //         info.selectionScreenCube.gameObject.SetActive(false);
    //         info.btnDownTime = 0;
    //         info.isLongClicked = false;
    //     }
    //
    //     if (input.RightClick) {//按下右键
    //         info.rightClickPos = Input.mousePosition;
    //         rightClickObj = PrintLogObject(info.rightClickPos, true, out Vector3 pot, rightClickObj);
    //         foreach (var tempId in info.regionIds) {
    //             PlayerGameObj playerGameObj = gameSystem.MyGameObjFeature.Get<PlayerGameObj>(tempId);
    //             PlayerComponent playerComponent = playerGameObj.GetComp<PlayerComponent>();
    //             NavMeshAgent agent = playerComponent.agent;
    //             agent.destination = pot;
    //         }
    //     }
    // }
    //
    // private void DrawSelect() {
    //     if (info.isLongClicked) {
    //         DrawSelect();
    //         SelectObject();
    //     }
    //
    //     if (!info.selectionScreenCube.gameObject.activeInHierarchy) {
    //         info.selectionScreenCube.gameObject.SetActive(true);
    //     }
    //
    //     var nowPos = Input.mousePosition;
    //     var sizeX = Mathf.Abs(info.leftClickPos.x - nowPos.x);
    //     var sizeY = Mathf.Abs(info.leftClickPos.y - nowPos.y);
    //     info.selectionScreenCube.position = (info.leftClickPos + nowPos) / 2f;
    //     info.selectionScreenCube.sizeDelta = new Vector2(sizeX, sizeY);
    // }
    //
    // private bool SelectSingleObject(Vector3 pot, out int id, out GameObject go) {
    //     Dictionary<int, GameObj> goList = gameSystem.MyGameObjFeature.GetAllDic();
    //     foreach (var val in goList) {
    //         if (Vector3.Distance(pot, val.Value.GetObj().transform.position) <= info.checkObjRange) {
    //             id = val.Key;
    //             go = val.Value.GetObj();
    //             return true;
    //         }
    //     }
    //
    //     id = default;
    //     go = default;
    //     return false;
    // }
    //
    // private void SelectObject() {
    //     // 调试选择框的四个物体
    //     s1 = PrintLogObject(info.leftClickPos, false, out Vector3 p1, s1);// 选中物体判断在四个边的内部
    //     s2 = PrintLogObject(new Vector3(Input.mousePosition.x, info.leftClickPos.y, info.leftClickPos.z), false, out Vector3 p2, s2);
    //     s3 = PrintLogObject(Input.mousePosition, false, out Vector3 p3, s3);
    //     s4 = PrintLogObject(new Vector3(info.leftClickPos.x, Input.mousePosition.y, info.leftClickPos.z), false, out Vector3 p4, s4);
    //
    //     if (p3 == p1) {
    //         return;
    //     }
    //
    //     Vector3 tempClockWiseVec = (p3 - p1).normalized;
    //     bool isClockWise = tempClockWiseVec.x * tempClockWiseVec.z < 0; //框选的顺序是顺时针？逆时针？根据 p1p3 向量的 xz 乘积正负判断顺逆 顺负逆正
    //
    //     Vector3 v1, v2, v3, v4; //叉积结果的Y判断在向量的左边还是右边，当四个向量都为的物体
    //     v1 = p2 - p1;
    //     v2 = p3 - p2;
    //     v3 = p4 - p3;
    //     v4 = p1 - p4;
    //
    //     Dictionary<int, GameObj> goList = gameSystem.MyGameObjFeature.GetAllDic();
    //     foreach (var go in goList) {
    //         // if (go.Value.GetData().regionType == RegionType.NONE) {
    //         //     continue;
    //         // }
    //         int id = go.Key;
    //         GameObject tempGo = go.Value.GetObj();
    //         Vector3 targetVec = tempGo.transform.position;
    //
    //         Vector3 q1, q2, q3, q4;// 当前物体和点连接的向量
    //         q1 = targetVec - p1;
    //         q2 = targetVec - p2;
    //         q3 = targetVec - p3;
    //         q4 = targetVec - p4;
    //
    //         float y1 = Vector3.Cross(v1, q1).y;// 判断 物体是否在多边形内 依次对四个向量做叉积 取Y值
    //         float y2 = Vector3.Cross(v2, q2).y;
    //         float y3 = Vector3.Cross(v3, q3).y;
    //         float y4 = Vector3.Cross(v4, q4).y;
    //
    //         if (isClockWise) {//顺时针 玩家的向量在边框的向量的右边 逆时针相反
    //             if (y1 > 0 && y2 > 0 && y3 > 0 && y4 > 0) {
    //                 JoinRegion(id, tempGo);
    //             }
    //         } else {
    //             if (y1 < 0 && y2 < 0 && y3 < 0 && y4 < 0) {
    //                 JoinRegion(id, tempGo);
    //             }
    //         }
    //     }
    // }
    //
    // private void JoinRegion(int id, GameObject go) {
    //     if (info.regionIds.Contains(id)) {
    //         return;
    //     }
    //
    //     info.regionIds.Add(id);
    //     gameSystem.messageCenter.Dispatcher(MessageConstant.ConsoleLogMsg, $"框选物体 -> [{id}] + {go.transform.name}");
    //
    //     gameSystem.messageCenter.Dispatcher(MessageConstant.RegionSingleMsg, id, true);
    // }
    //
    // private void CancelRegion() {
    //     info.regionIds.Clear();//清空缓存的框选物体Id
    //     Dictionary<int, GameObj> goList = gameSystem.MyGameObjFeature.GetAllDic();
    //     foreach (var go in goList) {
    //         gameSystem.messageCenter.Dispatcher(MessageConstant.RegionSingleMsg, go.Key, false);
    //     }
    // }
    //
    // private void JoinAllRegion() {
    //     Dictionary<int, GameObj> goList = gameSystem.MyGameObjFeature.GetAllDic();
    //     foreach (var go in goList) {
    //         // if (!info.regionIds.Contains(go.Key) && go.Value.GetData().regionType != RegionType.NONE) {
    //         //     info.regionIds.Add(go.Key);
    //         //     gameSystem.messageCenter.Dispatcher(MessageConstant.RegionSingleMsg, go.Key, true);
    //         // }
    //     }
    // }
    //
    // private GameObject s1, s2, s3, s4;
    // private GameObject rightClickObj;
    // private GameObject PrintLogObject(Vector3 screenPot, bool isRightClick, out Vector3 clickPoint, GameObject s) {
    //     Ray ray = Camera.main.ScreenPointToRay(screenPot);
    //     clickPoint = Vector3.zero;
    //     if (Physics.Raycast(ray, out var hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Terrain"))) {
    //         if (isRightClick && isCreateDebugSphere) {
    //             if (info.regionIds.Count > 0) {
    //                 if (s == null) {
    //                     var sphere = Instantiate(RTSTargetPosGO);
    //                     sphere.transform.position = hit.point;
    //                     sphere.transform.localScale /= 3;
    //                     s = sphere;
    //                 } else {
    //                     s.transform.position = hit.point;
    //                 }
    //             }
    //         }
    //
    //         clickPoint = hit.point;
    //     }
    //
    //     return s;
    // }
}

public class RegionBehaviourData {
    public int id;
    public GameObject go;
    public List<int> regionIds = new List<int>();
}