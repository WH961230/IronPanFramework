using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 游戏行为
/// </summary>
public abstract class GameBehaviour : GameSetting, IBehaviour {
    /// <summary>
    /// 注册的所有 ID
    /// </summary>
    protected List<int> BeRegisteredID = new List<int>();

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
            Logger.PrintType(info.t1, callingMethod.DeclaringType.FullName, LoggerType.BEHAVIOURREGISTER);// 输出调用该方法的类名
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
            Logger.PrintType(info.t1, callingMethod.DeclaringType.FullName, LoggerType.BEHAVIOURUNREGISTER);// 输出调用该方法的类名
        }
    }

    /// <summary>
    /// 加载行为数据
    /// </summary>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    public virtual void LoadData(int id) { }

    /// <summary>
    /// 更新行为数据
    /// </summary>
    public virtual void UpdateData() {
    }

    /// <summary>
    /// 获取控制
    /// </summary>
    /// <param name="t"></param>
    /// <typeparam name="T"></typeparam>
    public virtual void GetControl<T>(T t) { }

    public override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        this.gameSystem = gameSystem;
    }

    public virtual void Update() {
    }

    public virtual void FixedUpdate() { }

    public virtual void LateUpdate() { }
}

public interface IB {
    
}

public class B<T1> : IB {
    public T1 t1;
}

public class B<T1, T2> : IB {
    public T1 t1;
    public T2 t2;
}

public class B<T1, T2, T3> : IB {
    public T1 t1;
    public T2 t2;
    public T3 t3;
}

public class B<T1, T2, T3, T4> : IB {
    public T1 t1;
    public T2 t2;
    public T3 t3;
    public T4 t4;
}

public class B<T1, T2, T3, T4, T5> : IB {
    public T1 t1;
    public T2 t2;
    public T3 t3;
    public T4 t4;
    public T5 t5;
}

public class B<T1, T2, T3, T4, T5, T6> : IB {
    public T1 t1;
    public T2 t2;
    public T3 t3;
    public T4 t4;
    public T5 t5;
    public T6 t6;
}

public class B<T1, T2, T3, T4, T5, T6, T7> : IB {
    public T1 t1;
    public T2 t2;
    public T3 t3;
    public T4 t4;
    public T5 t5;
    public T6 t6;
    public T7 t7;
}

public class B<T1, T2, T3, T4, T5, T6, T7, T8> : IB {
    public T1 t1;
    public T2 t2;
    public T3 t3;
    public T4 t4;
    public T5 t5;
    public T6 t6;
    public T7 t7;
    public T8 t8;
}

