using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Evoreek/Behaviour/DebugBehaviour")]
public class DebugBehaviour : GameBehaviour {
#if UNITY_EDITOR
    private List<DebugBehaviourData> datas = new List<DebugBehaviourData>();
    private InputConditionOutput input;

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
        DebugBehaviourData tempBehaviourData = new DebugBehaviourData();
        tempBehaviourData.id = id;
        
        if (BeRegisteredID.Contains(id)) {
            var reflectedType = MethodBase.GetCurrentMethod().ReflectedType;
            if (reflectedType != null) {
                Logger.PrintELog($"{reflectedType.Name} 重复注册 id: {id}");
            }
        } else {
            BeRegisteredID.Add(id);
            datas.Add(tempBehaviourData);
        }
    }

    public override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        gameSystem.messageCenter.Reg<string>(MessageConstant.ConsoleLogMsg, SetDebugContent);
    }

    public override void Clear() {
        gameSystem.messageCenter.UnReg<string>(MessageConstant.ConsoleLogMsg, SetDebugContent);
        base.Clear();
    }

    public override void GetControl<T>(T t) { // 控制
        base.GetControl(t);
        if (t is InputConditionOutput) {
            input = t as InputConditionOutput;
        }
    }

    public override void Update() {
        base.Update();
        if (input == null) {
            return;
        }

        RefreshContent();
        DebugCommand();//调试指令
        EditorPoint();//编辑点
        EditorEnemy();
    }

    private void SetDebugContent(string content) {
        foreach (var d in datas) {
            UIDebugEntity tempEntity = gameSystem.MyEntityFeature.Get<UIDebugEntity>(d.id);
            UIDebugData data = tempEntity.GetData<UIDebugData>();

            if (string.IsNullOrEmpty(content)) {
                return;
            }

            data.previousContent += "\n" + content;
        }
    }
    
    private void RefreshContent() {
        foreach (var d in datas) {
            UIDebugGameObj tempGo = gameSystem.MyGameObjFeature.Get<UIDebugGameObj>(d.id);
            UIDebugEntity tempEntity = gameSystem.MyEntityFeature.Get<UIDebugEntity>(d.id);
            UIDebugComponent comp = tempGo.GetComp<UIDebugComponent>();
            UIDebugData data = tempEntity.GetData<UIDebugData>();
            comp.DebugConsoleContent.text = data.previousContent;//缓存不清除文本
        }
    }

    private void DebugCommand() {
        foreach (var data in datas) {
            if (input.Enter) { //按下回车 如果控制台未开启 开启控制台 否则向控制台传入指令
                UIDebugComponent comp = gameSystem.MyGameObjFeature.Get<UIDebugGameObj>(data.id).GetComp<UIDebugComponent>();
                if (comp.DebugConsoleArea.activeSelf) { //如果调试面板激活则发送数据
                    if (comp.DebugCommandTxt.text.Trim() == "") { //如果指令为空则激活指令输入
                        comp.DebugCommandTxt.ActivateInputField();
                    } else {
                        string commandStr = comp.DebugCommandTxt.text;
                        ReciveDebugCommand(data.id, commandStr);
                    }
                } else {//如果调试面板未激活则激活
                    comp.DebugConsoleArea.gameObject.SetActive(true);
                }

                comp.DebugCommandTxt.text = default;
                comp.DebugCommandTxt.ActivateInputField();
            }
        }
    }
    
    private void ReciveDebugCommand(int id, string commandStr) {
        UIDebugGameObj tempGo = gameSystem.MyGameObjFeature.Get<UIDebugGameObj>(id);
        UIDebugEntity tempEntity = gameSystem.MyEntityFeature.Get<UIDebugEntity>(id);
        UIDebugComponent comp = tempGo.GetComp<UIDebugComponent>();
        UIDebugData data = tempEntity.GetData<UIDebugData>();

        Logger.Print($"接受数据 {commandStr}");
        switch (commandStr) {
            case "help"://help菜单
                HelpCommandAction(data);
                break;
            case "clear"://清除
                data.previousContent = null;
                break;
            case "close"://关闭Debug窗口
                comp.DebugConsoleArea.SetActive(false);
                break;
            case "sceneobj"://场景物体
                data.type = EditorPointType.SCENEOBJECT;
                data.isEditorPoint = true;
                break;
            case "central"://中心建筑
                data.type = EditorPointType.CENTRAL;
                data.isEditorPoint = true;
                break;
            case "save"://保存点位信息
                if (data.type == EditorPointType.SCENEOBJECT) {
                    SceneObjectPointSave(data);
                } else if(data.type == EditorPointType.CENTRAL) {
                    CentralPointSave(data);
                }
                break;
            case "enemy":
                data.isEditorEnemyPoint = !data.isEditorEnemyPoint;
                data.type = EditorPointType.ENEMY;
                break;
            case "rts"://切换 rts 视角
                gameSystem.messageCenter.Dispatcher(MessageConstant.ViewTypeMsg, true, PlayerViewType.RTS);
                break;
            case "fps"://切换 fps 视角
                gameSystem.messageCenter.Dispatcher(MessageConstant.ViewTypeMsg, true, PlayerViewType.FPS);
                break;
            case "tps"://切换 tps 视角
                gameSystem.messageCenter.Dispatcher(MessageConstant.ViewTypeMsg, true, PlayerViewType.TPS);
                break;
            case "edge"://相机边缘触碰移动
                gameSystem.messageCenter.Dispatcher(MessageConstant.EdgeMoveMsg, true);
                break;
            case "edgeclose"://取消相机边缘触碰移动
                gameSystem.messageCenter.Dispatcher(MessageConstant.EdgeMoveMsg, false);
                break;
            case "god"://上帝模式
                if (GameData.MainPlayer != null) {
                    GameData.MainPlayer.GetData().PlayerMod = PlayerMode.GODMODE;
                }
                break;
            case "normal":
                if (GameData.MainPlayer != null) {
                    GameData.MainPlayer.GetData().PlayerMod = PlayerMode.NORMALMODE;
                }
                break;
        }
    }
    
    private void HelpCommandAction(UIDebugData data) {
        string commands = null;
        foreach (var com in data.debugCommands) {
            commands += $"{com.commandContent}:{com.commandSpecification}\n";
        }

        gameSystem.messageCenter.Dispatcher(MessageConstant.ConsoleLogMsg, commands);
    }

    private void SceneObjectPointSave(UIDebugData data) {
        GameObject go = GameObject.Find("【场景物体位置信息】");
        data.sceneObjectSetting.SceneObjectInfoList = null;
        data.sceneObjectSetting.SceneObjectInfoList = new List<InstancePointInfo>();
        for (int i = 0; i < go.transform.childCount; i++) {//保存点位信息到配置列表
            var child = go.transform.GetChild(i).gameObject;
            data.sceneObjectSetting.SceneObjectInfoList.Add(new InstancePointInfo() {
                obj = data.sceneObjectSetting.SceneObjectGoList[Random.Range(0, data.sceneObjectSetting.SceneObjectGoList.Count)],
                pos = child.transform.position,
                engle = child.transform.rotation.eulerAngles,
            });
            Logger.Print($"保存点位信息{child.name}");
        }

        EditorUtility.SetDirty(data.sceneObjectSetting);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Destroy(go);

        data.isEditorPointDisplay = false;
        data.isEditorPoint = false;
    }

    private void CentralPointSave(UIDebugData data) {
        GameObject go = GameObject.Find("【中心建筑位置信息】");
        data.centralSetting.CentralStartPoint = new PointInfo() {//直接将调试物体的位置和旋转赋值
            engle = go.transform.rotation.eulerAngles,
            vec = go.transform.position
        };

        EditorUtility.SetDirty(data.centralSetting);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Destroy(go);

        data.isEditorPointDisplay = false;
        data.isEditorPoint = false;
    }

    /// <summary>
    /// 地图点位设置
    /// </summary>
    private void EditorPoint() {
        foreach (var d in datas) {
            UIDebugEntity tempEntity = gameSystem.MyEntityFeature.Get<UIDebugEntity>(d.id);
            UIDebugData data = tempEntity.GetData<UIDebugData>();
            if (!data.isEditorPoint) {
                continue;
            }

            GameObject parentGO = null;
            if (data.type == EditorPointType.SCENEOBJECT) {
                parentGO = SceneObjectEditorPoint(data);
            } else if (data.type == EditorPointType.CENTRAL) {
                parentGO = CentralEditorPoint(data);
            } else {
                Logger.PrintE("Error parentGo is null!");
            }

            if (input.LeftClick) {
                if (data.isEditorClickTime == 0) {//首次按下
                    data.isEditorClickTime = 1;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    bool hitSomething = Physics.Raycast(ray, out var hit, Mathf.Infinity);
                    if (!hitSomething) {
                        continue;
                    }

                    if (data.type == EditorPointType.SCENEOBJECT) {
                        var tempGo = Instantiate(data.sceneObjectSetting.SceneObjectDebugObj, parentGO.transform);
                        tempGo.transform.position = hit.point;
                        data.currentEditorGo = tempGo;
                    } else if (data.type == EditorPointType.CENTRAL) {//生成调试物体 如果当前已有调试
                        data.currentEditorGo = parentGO;
                        data.currentEditorGo.transform.position = hit.point;
                    }
                } else if (data.isEditorClickTime == 1) {//二次按下
                    data.isEditorClickTime = 0;
                    data.currentEditorGo = null;
                }
            }

            if (input.RightClick) {
                if (data.isEditorClickTime == 1) {//右键按下销毁当前调试物体
                    data.isEditorClickTime = 0;
                    Destroy(data.currentEditorGo);
                    data.currentEditorGo = null;
                }
            }

            if (data.currentEditorGo != null) {//朝向鼠标方向
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool hitSomething = Physics.Raycast(ray, out var hit, Mathf.Infinity);
                if (hitSomething) {
                    Vector3 lookAtTarget = new Vector3(0, data.currentEditorGo.transform.position.y, 0);
                    lookAtTarget.x = hit.point.x;
                    lookAtTarget.z = hit.point.z;
                    data.currentEditorGo.transform.LookAt(lookAtTarget);
                }
            }
        }
    }

    private void EditorEnemy() {
        foreach (var tempData in datas) {
            UIDebugEntity tempEntity = gameSystem.MyEntityFeature.Get<UIDebugEntity>(tempData.id);
            UIDebugData data = tempEntity.GetData<UIDebugData>();
            
            if (!data.isEditorEnemyPoint) {
                if (data.currentEnemyEditorGo != null) {
                    DestroyImmediate(data.currentEnemyEditorGo);
                }
                continue;
            }

            //触发指令后创建怪物
            if (data.currentEnemyEditorGo == null) {
                //获取怪物的游戏物体，且将其暂且挂载在检测到的射线的位置点，二次点击创建。
                GameObject enemyGO = AssetsLoad.GetAsset<GameObject>(data.enemySetting.Prefix, data.enemySetting.PrefabSign, AssetsLoad.AssetType.Prefab);
                data.currentEnemyEditorGo = Instantiate(enemyGO);
            }

            //鼠标检测到物体跟随
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hitSomething = Physics.Raycast(ray, out var hit, Mathf.Infinity);
            if (hitSomething) {
                if (data.currentEnemyEditorGo != null) {
                    data.currentEnemyEditorGo.transform.position = hit.point;
                }
            }

            // 点击物品创建，销毁调试物体
            if (input.LeftClick) {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                hitSomething = Physics.Raycast(ray, out hit, Mathf.Infinity);
                if (hitSomething) {
                    DestroyImmediate(data.currentEnemyEditorGo);
                    data.currentEnemyEditorGo = null;
                    int enemyID = gameSystem.GetSystem<EnemySystem>().Instance();
                    GameObject instanceEnemyGO = gameSystem.MyGameObjFeature.Get(enemyID).GetObj();
                    instanceEnemyGO.transform.position = hit.point;
                    data.currentEnemyEditorGo = null;
                }
            }
        }
    }

    private GameObject SceneObjectEditorPoint(UIDebugData data) {
        GameObject parentGO = GameObject.Find("【场景物体位置信息】");
        if (parentGO == null) {
            parentGO = new GameObject("【场景物体位置信息】");
        }

        if (!data.isEditorPointDisplay) {
            InstanceDebugGO(data, parentGO);
            data.isEditorPointDisplay = true;
        }

        return parentGO;
    }

    private GameObject CentralEditorPoint(UIDebugData data) {
        GameObject parentGO = GameObject.Find("【中心建筑位置信息】");
        if (parentGO == null) {
            parentGO = Instantiate(data.centralSetting.CentralDebugObj) as GameObject;
            parentGO.name = "【中心建筑位置信息】";
        }

        if (!data.isEditorPointDisplay) {
            InstanceDebugGO(data, parentGO);
            data.isEditorPointDisplay = true;
        }

        return parentGO;
    }

    private static void InstanceDebugGO(UIDebugData data, GameObject parentGo) {
        if (data.type == EditorPointType.SCENEOBJECT) {
            foreach (var o in data.sceneObjectSetting.SceneObjectInfoList) {
                GameObject tempGO = Instantiate(data.sceneObjectSetting.SceneObjectDebugObj);
                tempGO.transform.SetParent(parentGo.transform);
                tempGO.transform.position = o.pos;
                tempGO.transform.rotation = Quaternion.Euler(o.engle);
            }
        } else if (data.type == EditorPointType.CENTRAL) {
            parentGo.transform.position = data.centralSetting.CentralStartPoint.vec;
            parentGo.transform.rotation = Quaternion.Euler(data.centralSetting.CentralStartPoint.engle);
        }
    }
#endif
}

public class DebugBehaviourData {
    public int id;
}