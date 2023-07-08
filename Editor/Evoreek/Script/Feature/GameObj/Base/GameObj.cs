using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 游戏物体
/// </summary>
public class GameObj : IGameObj {
    protected GameObject MyObj;
    protected GameComp MyComp;
    private Data MyData;
    protected Game game;
    protected GameSystem gameSystem;
    private SOGameSetting gameSetting;
    // private List<GameFunc> MyObjRegistedFunc = new List<GameFunc>();

    public virtual void Init(Game game, Data data) {
        this.game = game;
        gameSystem = game.gameSystem;
        gameSetting = gameSystem.SoData.SOGameSetting;

        MyData = data;
        MyObj = data.MyObj;
        data.MyObjName = string.Concat(data.MyObjName, "_", data.InstanceID);
        MyObj.name = data.MyObjName;
        MyObj.transform.position = data.MyStartPointInfo.vec;
        MyObj.transform.rotation = Quaternion.Euler(data.MyStartPointInfo.engle);

        MyData.BaseRoleInfo = new BaseRoleInfo() {
            Name = MyData.MyObjName,
            ExpLevel = MyData.baseValueData.EXPLEVEL.ToString(),
        };

        if (MyObj == null) {
            Logger.PrintE("obj is null !");
            return;
        }

        MyComp = MyObj.GetComponent<GameComp>();
        MyComp.InstanceID = data.InstanceID;
        
        gameSystem.MyAnimationFeature.Register(MyData.InstanceID, MyComp.animator);//注册动画状态机

        gameSystem.messageCenter.Reg<PlayerValue, int, int, int>(MessageConstant.ValueSingleChangeMsg, (valueType, id, value, valueMax) => {
            if (id != MyData.InstanceID) {
                return;
            }

            if (valueType == PlayerValue.HP) {
                if (MyComp.HPSlider != null) {
                    MyComp.HPSlider.value = (float) value / valueMax;
                }
            } else if (valueType == PlayerValue.PHYSICALPOWER) {
                if (MyComp.PhysicalPowerSlider != null) {
                    MyComp.PhysicalPowerSlider.value = (float) value / valueMax;
                }
            }
        });

        gameSystem.messageCenter.Reg<int, BaseRoleInfo>(MessageConstant.BarInfoMsg, (id, info) => {
            if (MyData.InstanceID == id) {
                if (MyComp.PlayerName != null) {
                    MyComp.PlayerName.text = info.Name;
                }

                if (MyComp.PlayerExpLevel != null) {
                    MyComp.PlayerExpLevel.text = info.ExpLevel;
                }
            }
        });
        
        gameSystem.messageCenter.Dispatcher(MessageConstant.BarInfoMsg, MyData.InstanceID, new BaseRoleInfo() {
            Name = MyData.MyObjName,
            ExpLevel = MyData.baseValueData.EXPLEVEL.ToString(),
        });

        gameSystem.messageCenter.Reg(MessageConstant.MainPlayerWindowLoadFinished, () => {
            if (MyData.InstanceID != GameData.MainPlayerID) {
                return;
            }
            //只有玩家需要重新发送一次供窗口信息的消息
            gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.HP, MyData.InstanceID,MyData.runtimeBaseValueData.HP, MyData.baseValueData.HPMAX);
            gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.PHYSICALPOWER, MyData.InstanceID,MyData.runtimeBaseValueData.PHYSICALPOWER, MyData.baseValueData.PHYSICALPOWERMAX);
            gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.ATK, MyData.InstanceID, MyData.runtimeBaseValueData.ATK, 1);
            gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.DEF, MyData.InstanceID, MyData.runtimeBaseValueData.DEF, 1);
            gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.MOVESPEED, MyData.InstanceID, MyData.runtimeBaseValueData.MOVESPEED, 1);
            gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.EXPVAL, MyData.InstanceID,MyData.runtimeBaseValueData.EXPVAL, MyData.baseValueData.EXPVALMAX);
            gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.EXPLEVEL, MyData.InstanceID,MyData.runtimeBaseValueData.EXPLEVEL, MyData.baseValueData.EXPLEVELMAX);

            gameSystem.messageCenter.Dispatcher(MessageConstant.BarInfoMsg, MyData.InstanceID, MyData.BaseRoleInfo);//玩家名字消息发送
        });

        gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.HP, MyData.InstanceID,MyData.runtimeBaseValueData.HP, MyData.baseValueData.HPMAX);
        gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.PHYSICALPOWER, MyData.InstanceID,MyData.runtimeBaseValueData.PHYSICALPOWER, MyData.baseValueData.PHYSICALPOWERMAX);
        gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.EXPVAL, MyData.InstanceID,MyData.runtimeBaseValueData.EXPVAL, MyData.baseValueData.EXPVALMAX);
        gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.EXPLEVEL, MyData.InstanceID,MyData.runtimeBaseValueData.EXPLEVEL, MyData.baseValueData.EXPLEVELMAX);

        if (MyComp.bloodTran != null) {
            MyComp.bloodTran.gameObject.SetActive(true);
        }
        
        if (MyComp.PhysicalPowerTran != null) {
            MyComp.PhysicalPowerTran.gameObject.SetActive(true);
        }

        if (MyComp.RegionTran != null) {
            MyComp.RegionTran.gameObject.SetActive(true);
            Color color = MyComp.SpriteRenderer.color;
            color.a = 0f;
            MyComp.SpriteRenderer.color = color;
        }

        StackTrace stackTrace = new StackTrace();
        StackFrame[] stackFrames = stackTrace.GetFrames();
        
        if (stackFrames.Length > 1)
        {
            // 获取调用该方法的方法信息
            MethodBase callingMethod = stackFrames[1].GetMethod();
            
            // 输出调用该方法的类名
            Logger.PrintType(MyData.InstanceID, callingMethod.DeclaringType.FullName, LoggerType.GAMEOBJ);
        }
    }

    public virtual void Clear() {
        gameSystem.messageCenter.UnReg<int, BaseRoleInfo>(MessageConstant.BarInfoMsg, (id, info) => {
            if (MyData.InstanceID == id) {
                if (MyComp.PlayerName != null) {
                    MyComp.PlayerName.text = info.Name;
                }

                if (MyComp.PlayerExpLevel != null) {
                    MyComp.PlayerExpLevel.text = info.ExpLevel;
                }
            }
        });
        
        gameSystem.messageCenter.UnReg(MessageConstant.MainPlayerWindowLoadFinished, () => {
            gameSystem.messageCenter.Dispatcher(MessageConstant.ValueSingleChangeMsg, PlayerValue.HP, MyData.InstanceID,MyData.runtimeBaseValueData.HP, MyData.baseValueData.HPMAX);
        });

        gameSystem.MyAnimationFeature.UnRegister(MyData.InstanceID);//反注册动画状态机

        gameSystem.messageCenter.UnReg<PlayerValue, int, int, int>(MessageConstant.ValueSingleChangeMsg, (valueType, id, value, valueMax) => {
            if (id == MyData.InstanceID) {
                if (valueType == PlayerValue.HP) {
                    MyComp.HPSlider.value = (float) value / valueMax;
                }
            }
        });
        gameSystem.MyAnimationFeature.UnRegister(MyData.InstanceID);//反注册动画状态机
    }

    public virtual void Update() {
    }

    public virtual void FixedUpdate() {
    }

    public virtual void LateUpdate() {
    }

    /// <summary>
    /// 注册功能
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <typeparam name="T"></typeparam>
    public virtual void Register<T>(FunctionType arg0, T arg1) where T : IB {
        var func = gameSetting.GetFunc(arg0);
        if (func == null) {
            Logger.PrintE($"functionType : {arg0} no found in sogamesetting !");
        }

        func.Register(arg1);
    }

    /// <summary>
    /// 反注册功能
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <typeparam name="T"></typeparam>
    public virtual void UnRegister<T>(FunctionType arg0, T arg1) where T : IB {
        var func = gameSetting.GetFunc(arg0);
        if (func == null) {
            Logger.PrintE($"functionType : {arg0} no found in sogamesetting !");
        }

        func.UnRegister(arg1);
    }

    public GameObject GetObj() {
        return MyObj;
    }

    public GameComp GetComp() {
        return MyComp;
    }

    public Data GetData() {
        return MyData;
    }

    public T GetComp<T>() where T : GameComp {
        if (MyComp == null) {
            MyComp = MyObj.GetComponent<T>();
        }

        return (T)MyComp;
    }

    public T GetData<T>() where T : Data {//给 Window 用的 Data 数据 不然 Window 难以拿到 Data
        return (T) MyData;
    }
}