using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

public abstract class GameFunc : GameSetting, IFunc {
    public FunctionType functionType;
    public List<FuncBlock> funcBlocks;

    public override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        if (funcBlocks == null || funcBlocks.Count <= 0) {
            Logger.PrintELog($"fileName:{this.name} -> FuncBlocks is empty");
            return;
        }

        foreach (var f in funcBlocks) {
            if (f.condition != null) {//条件初始化
                foreach (var cond in f.condition) {
                    cond.Init(gameSystem);
                }
            }

            f.gameBehaviour.Init(gameSystem);//行为初始化
            if (f.condition != null) {
                foreach (var cond in f.condition) {
                    f.gameBehaviour.GetControl(cond.GetResult());
                }
            }
        }
    }

    public virtual void Update() {
        if (funcBlocks != null && funcBlocks.Count > 0) {
            foreach (var f in funcBlocks) {
                f.gameBehaviour.Update();
                if (f.condition != null) {
                    foreach (var cond in f.condition) {
                        cond.Update();
                        f.gameBehaviour.GetControl(cond.GetResult());
                    }
                }
            }
        }
    }

    public virtual void FixedUpdate() {
        if (funcBlocks != null && funcBlocks.Count > 0) {
            foreach (var f in funcBlocks) {
                f.gameBehaviour.FixedUpdate();
            }
        }
    }

    public virtual void LateUpdate() {
        if (funcBlocks != null && funcBlocks.Count > 0) {
            foreach (var f in funcBlocks) {
                f.gameBehaviour.LateUpdate();
            }
        }
    }

    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="arg1"></param>
    /// <typeparam name="T"></typeparam>
    public virtual void Register<T>(T arg1) where T : IB {
        //日志使用切勿删除
        StackTrace stackTrace = new StackTrace();
        StackFrame[] stackFrames = stackTrace.GetFrames();
        if (stackFrames.Length > 1) {
            MethodBase callingMethod = stackFrames[1].GetMethod();// 获取调用该方法的方法信息
            B<int> info = arg1 as B<int>;
            Logger.PrintType(info.t1, callingMethod.DeclaringType.FullName, LoggerType.FUNCTIONREGISTER);// 输出调用该方法的类名
        }
    }

    /// <summary>
    /// 反注册
    /// </summary>
    /// <param name="arg1"></param>
    /// <typeparam name="T"></typeparam>
    public virtual void UnRegister<T>(T arg1) where T : IB {
        //日志使用切勿删除
        StackTrace stackTrace = new StackTrace();
        StackFrame[] stackFrames = stackTrace.GetFrames();
        if (stackFrames.Length > 1) {
            MethodBase callingMethod = stackFrames[1].GetMethod();// 获取调用该方法的方法信息
            B<int> info = arg1 as B<int>;
            Logger.PrintType(info.t1, callingMethod.DeclaringType.FullName, LoggerType.FUNCTIONUNREGISTER);// 输出调用该方法的类名
        }
    }
}

[Serializable]
public class FuncBlock {
    public List<GameCondition> condition; // 条件
    public GameBehaviour gameBehaviour; // 行为
}