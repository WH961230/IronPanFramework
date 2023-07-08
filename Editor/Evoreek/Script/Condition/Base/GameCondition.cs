using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

/// <summary>
/// 游戏条件 - 入口是条件获取 出口是输出结果
/// </summary>
public abstract class GameCondition : GameSetting, ICondition {
    /// <summary>
    /// 注册的所有 ID
    /// </summary>
    protected readonly List<int> BeRegisteredID = new List<int>();
    public List<GameCondition> conditions;

    public override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        foreach (var tempCond in conditions) {
            tempCond.Init(gameSystem);
        }
    }

    public override void Update() {
        base.Update();
        foreach (var tempCond in conditions) {
            tempCond.Update();
            GetControl(tempCond.GetResult());
        }
    }

    /// <summary>
    /// 注册
    /// </summary>
    public virtual void Register<T>(T arg1) where T : IB {
        //日志使用切勿删除
        StackTrace stackTrace = new StackTrace();
        StackFrame[] stackFrames = stackTrace.GetFrames();
        if (stackFrames.Length > 1) {
            MethodBase callingMethod = stackFrames[1].GetMethod();// 获取调用该方法的方法信息
            B<int> info = arg1 as B<int>;
            Logger.PrintType(info.t1, callingMethod.DeclaringType.FullName, LoggerType.CONDITIONREGISTER);// 输出调用该方法的类名
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
            Logger.PrintType(info.t1, callingMethod.DeclaringType.FullName, LoggerType.CONDITIONUNREGISTER);// 输出调用该方法的类名
        }
    }

    /// <summary>
    /// 加载条件数据
    /// </summary>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    protected virtual void LoadData(int id) { }

    /// <summary>
    /// 更新条件数据
    /// </summary>
    protected virtual void UpdateData() {
    }

    /// <summary>
    /// 获取控制
    /// </summary>
    /// <param name="t"></param>
    /// <typeparam name="T"></typeparam>
    public virtual void GetControl<T>(T t) {
        
    }

    public virtual IOutput GetResult() {
        return null;
    }
}