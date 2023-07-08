using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "Evoreek/Behaviour/RealTimeStrategyControlBehaviour")]
public class RealTimeStrategyControlBehaviour : GameBehaviour {
    private List<RealTimeStrategyControlBehaviourData> datas = new List<RealTimeStrategyControlBehaviourData>();
    private InputConditionOutput input;
    private RegionConditionOutput region;
    
    public GameObject RTSTargetPosGO;
    private GameObject tempRTSTargetGO;
    public override void Register<T>(T arg1) {
        base.Register(arg1);
        B<int> info = arg1 as B<int>;
        LoadData(info.t1);
    }
    
    public override void UnRegister<T>(T arg1) {
        base.UnRegister(arg1);
        var info = arg1 as B<int>;
    }

    public override void LoadData(int id) {
        base.LoadData(id);
        GameObj go = gameSystem.MyGameObjFeature.Get(id);
        GameObject obj = go.GetObj();
        GameComp comp = go.GetComp();
        Data data = go.GetData();
        
        RealTimeStrategyControlBehaviourData tempData = new RealTimeStrategyControlBehaviourData() {
            id = id,
            go = obj,
            agent = comp.agent,
            animator = comp.animator,
            moveSpeed = data.MoveSpeed,
            destinationNearDistance = data.DestinationNearDistance,
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
        } else if (t is RegionConditionOutput) {
            region = t as RegionConditionOutput;
        }
    }

    public override void UpdateData() {
        base.UpdateData();
    }

    public override void Update() {
        base.Update();
        UpdateData();
        foreach (var tempData in datas) {
            RealTimeStrategy(tempData);
            MoveToDestination(tempData);
            SetAnimator(tempData);
        }
    }

    private void RealTimeStrategy(RealTimeStrategyControlBehaviourData tempData) {
        if (input.RightClick) {//右键 先判断是否右键敌人，如果是敌人优先寻路+攻击敌人，否则寻路到位置
            GameData.AudioSystem.AddSoundPlay(new AudioData() {
                InstanceID = tempData.id,
                tempAudioName = "点击寻路",
                isLoop = false,
                is3D = false,
                isFollow = false,
            });
            //清理旧路径目标
            ClearPreciousDesitination(tempData);
            Vector3 targetHitVec = Vector3.zero;
            bool isFirstClick = GameData.GetInputRay("Monster", out RaycastHit hit1);//右键选中敌人
            if (isFirstClick) {
                tempData.runtimeData.nextDestinationVec = hit1.point;
                tempData.agent.speed = tempData.moveSpeed;
                tempData.agent.isStopped = false;
                tempData.runtimeData.isMoving = true;
                tempData.runtimeData.isChasing = true;

                tempData.runtimeData.beChasedId = hit1.collider.transform.root.gameObject.GetInstanceID();//赋值被追踪的敌人 当目标角色死亡后 停止寻路
                if (GameData.DetectedAttackTargetID.ContainsKey(tempData.id)) {
                    GameData.DetectedAttackTargetID[tempData.id] = tempData.runtimeData.beChasedId;
                } else {
                    GameData.DetectedAttackTargetID.Add(tempData.id, tempData.runtimeData.beChasedId);
                }
                
                targetHitVec = hit1.point;
                
                //取消所有物体的框选
                List<GameObj> gos = gameSystem.MyGameObjFeature.GetAllGameObj();
                foreach (var tempGO in gos) {
                    SpriteRenderer renderer = tempGO.GetComp().SpriteRenderer;
                    if (renderer != null) {
                        Color color = renderer.color;
                        color.a = 0f;
                        renderer.color = color;
                    }
                }
                
                //选择怪物的框选透明度
                GameObj tempObj = gameSystem.MyGameObjFeature.Get(tempData.runtimeData.beChasedId);
                if (tempObj != null) {
                    SpriteRenderer renderer = tempObj.GetComp().SpriteRenderer;
                    if (renderer != null) {
                        Color color = renderer.color;
                        color.a = 1f;
                        renderer.color = color;
                    }
                }
                
                if (tempData.runtimeData.voiceAttackContinue == 0) {
                    GameData.AudioSystem.AddSoundPlay(new AudioData() {
                        InstanceID = tempData.id,
                        tempAudioName = "本地玩家攻击语音",
                        isLoop = false,
                        is3D = true,
                        isFollow = true,
                    });
                    tempData.runtimeData.voiceAttackContinue = 2;
                }

                if (tempRTSTargetGO != null) {
                    DestroyImmediate(tempRTSTargetGO);
                    tempRTSTargetGO = null;
                }
            }

            if (!isFirstClick) {
                Vector3 mousePos = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                bool isClickTerrain = Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Terrain"));
                if (isClickTerrain) {
                    tempData.runtimeData.nextDestinationVec = hit.point;
                    tempData.agent.speed = tempData.moveSpeed;
                    tempData.agent.SetDestination(tempData.runtimeData.nextDestinationVec);
                    tempData.agent.isStopped = false;
                    tempData.runtimeData.isMoving = true;
                    targetHitVec = hit.point;
                    if (GameData.MainPlayerID != tempData.id) {
                        Logger.Print($"选中地面开始移动 到达后停止移动");
                    }
                    
                    if (GameData.DetectedAttackTargetID.ContainsKey(tempData.id)) {
                        GameData.DetectedAttackTargetID[tempData.id] = 0;
                    } else {
                        GameData.DetectedAttackTargetID.Add(tempData.id, 0);
                    }

                    if (tempData.runtimeData.voiceMoveContinue == 0) {
                        GameData.AudioSystem.AddSoundPlay(new AudioData() {
                            InstanceID = tempData.id,
                            tempAudioName = "本地玩家移动语音",
                            isLoop = false,
                            is3D = true,
                            isFollow = true,
                        });
                        tempData.runtimeData.voiceMoveContinue = 2;
                    }

                    if (tempRTSTargetGO != null) {
                        DestroyImmediate(tempRTSTargetGO);
                        tempRTSTargetGO = null;
                    }
                }
                
                if (targetHitVec != Vector3.zero) {
                    if (tempRTSTargetGO == null) {
                        var sphere = Instantiate(RTSTargetPosGO);
                        sphere.transform.position = targetHitVec;
                        sphere.transform.localScale /= 3;
                        tempRTSTargetGO = sphere;
                        Animator animator = tempRTSTargetGO.GetComponent<Animator>();
                        animator.Play("TargetAnim", 0, 0f);
                    }
                }
            }
        }

        if (tempData.runtimeData.voiceAttackContinue > 0) {
            tempData.runtimeData.voiceAttackContinue -= Time.deltaTime;
        } else {
            tempData.runtimeData.voiceAttackContinue = 0;
        }

        if (tempData.runtimeData.voiceMoveContinue > 0) {
            tempData.runtimeData.voiceMoveContinue -= Time.deltaTime;
        } else {
            tempData.runtimeData.voiceMoveContinue = 0;
        }

        Data tempEntityData = gameSystem.MyEntityFeature.Get(tempData.id).GetData();
        if (tempData.runtimeData.isMoving) {
            tempEntityData.baseRuntimeState.isMove = true;
            Vector3 moveDir = (tempData.runtimeData.nextDestinationVec - tempData.go.transform.position).normalized; 
            tempData.agent.Move(moveDir * tempData.moveSpeed * Time.deltaTime);
            if (tempData.runtimeData.FootStepGO == null) {
                tempData.runtimeData.FootStepGO = gameSystem.MyPoolFeature.Get("FootStep");
                tempData.runtimeData.FootStepGO.transform.parent = GameData.MainPlayer.GetObj().transform;
                tempData.runtimeData.FootStepGO.transform.localPosition = Vector3.zero;
            }
        } else {
            tempEntityData.baseRuntimeState.isMove = false;
            if (tempData.runtimeData.FootStepGO != null) {
                gameSystem.MyPoolFeature.Release("FootStep", tempData.runtimeData.FootStepGO);
                tempData.runtimeData.FootStepGO = null;
            }
        }
    }

    private void ClearPreciousDesitination(RealTimeStrategyControlBehaviourData tempData) {
        StopMove(tempData);
    }

    private void MoveToDestination(RealTimeStrategyControlBehaviourData tempData) {
        if (tempData.runtimeData.nextDestinationVec == Vector3.zero) {
            return;
        }

        if (tempData.runtimeData.isChasing) {//如果在追逐中
            int beChasedId = tempData.runtimeData.beChasedId;
            bool isAlive = gameSystem.MyEntityFeature.Get(beChasedId).GetData().runtimeBaseValueData.HP > 0;
            if (isAlive) {//追逐的目标未死亡
                tempData.runtimeData.nextDestinationVec = gameSystem.MyGameObjFeature.Get(beChasedId).GetObj().transform.position;
                tempData.agent.SetDestination(tempData.runtimeData.nextDestinationVec);
                Debug.DrawLine(tempData.go.transform.position, gameSystem.MyGameObjFeature.Get(beChasedId).GetObj().transform.position, Color.blue);
                
                float distance = Vector3.Distance(tempData.go.transform.position, tempData.runtimeData.nextDestinationVec);
                Data data = gameSystem.MyEntityFeature.Get(tempData.id).MyData;
                if (distance < data.baseValueData.ATKRANGE) {
                    StopMove(tempData);
                } else {
                    if (!tempData.runtimeData.isMoving && GameData.MainPlayerID != tempData.id) {
                        Logger.Print($"向选中的活物目标移动中 : {gameSystem.MyEntityFeature.Get(beChasedId).GetData().MyObjName}"); 
                    }
                }
            } else {
                StopMove(tempData);
            }
        } else {
            if (tempData.runtimeData.isMoving) {
                if (!tempData.runtimeData.isMoving && GameData.MainPlayerID != tempData.id) {
                    Logger.Print($"向地面目标移动中 : {tempData.runtimeData.nextDestinationVec}"); 
                }
                float distance = Vector3.Distance(tempData.go.transform.position, tempData.runtimeData.nextDestinationVec);
                Debug.DrawLine(tempData.go.transform.position, tempData.runtimeData.nextDestinationVec, Color.yellow);
                if (distance <= tempData.destinationNearDistance) {
                    StopMove(tempData);
                }
            } else {
                StopMove(tempData);
            }
        }
    }

    private void StopMove(RealTimeStrategyControlBehaviourData tempData) {
        if (GameData.MainPlayerID != tempData.id) {
            if (tempData.runtimeData.isMoving && tempData.runtimeData.isChasing) {
                Logger.Print($"选中的活物目标到达停止移动"); 
            } else if(tempData.runtimeData.isMoving && !tempData.runtimeData.isChasing){
                Logger.Print($"选中的地板到达停止移动");
            }
        }

        tempData.agent.SetDestination(tempData.go.transform.position);
        tempData.agent.isStopped = true;
        tempData.runtimeData.isMoving = false;
        tempData.runtimeData.isChasing = false;
        tempData.runtimeData.nextDestinationVec = Vector3.zero;
    }

    private void SetAnimator(RealTimeStrategyControlBehaviourData tempData) {
        if (tempData.runtimeData.isMoving) {
            tempData.animator.SetFloat("Vertical", 2);
        } else {
            tempData.animator.SetFloat("Vertical", 0);
        }
    }
}

public class RealTimeStrategyControlBehaviourData {
    public int id;
    public GameObject go;
    public NavMeshAgent agent;
    public Animator animator;
    public float destinationNearDistance;
    public float moveSpeed;
    public RealTimeStrategyControlBehaviourRuntimeData runtimeData = new RealTimeStrategyControlBehaviourRuntimeData();
}

public class RealTimeStrategyControlBehaviourRuntimeData {
    public bool isMoving;//移动中
    public bool isChasing;//追踪中
    public int beChasedId;//被追踪的敌人 ID
    public float voiceMoveContinue;
    public float voiceAttackContinue;
    public Vector3 nextDestinationVec;//目的地位置
    public GameObject FootStepGO;
}