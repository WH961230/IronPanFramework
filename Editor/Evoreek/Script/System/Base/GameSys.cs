using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

public abstract class GameSys : ISystem {
    protected GameSystem gameSystem;
    protected Setting setting;

    protected virtual void Init(GameSystem gameSystem) {
        this.gameSystem = gameSystem;

        StackTrace stackTrace = new StackTrace();
        StackFrame[] stackFrames = stackTrace.GetFrames();
        if (stackFrames.Length > 1) {
            MethodBase callingMethod = stackFrames[1].GetMethod();// 获取调用该方法的方法信息
            Logger.PrintType(0, callingMethod.DeclaringType.FullName, LoggerType.SYSTEM);// 输出调用该方法的类名
        }
    }

    public virtual void Update() {
        
    }

    public virtual void FixedUpdate() {
        
    }

    public virtual void LateUpdate() {
        
    }

    protected virtual void Clear() {
        
    }

    /// <summary>
    /// 自定义配置实例化
    /// </summary>
    /// <param name="tempSetting"></param>
    /// <param name="initFunc"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <returns></returns>
    protected virtual int Instance<T1, T2, T3, T4>(T1 tempSetting, Action<T4> initFunc)
        where T1 : Setting, new()
        where T2 : GameObj, new()
        where T3 : Entity, new()
        where T4 : Data, new() {
        if (tempSetting == null) {
            Logger.PrintE("技能无法实例 技能配置为空");
            return 0;
        }
        var data = gameSystem.InstanceData<T4>(tempSetting, tempSetting.PrefabSign);
        initFunc?.Invoke(data);
        return gameSystem.InstanceGameObj<T2, T3>(data);
    }

    /// <summary>
    /// 自定义物体实例化
    /// </summary>
    /// <param name="go"></param>
    /// <param name="initFunc"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <returns></returns>
    protected virtual int Instance<T1, T2, T3>(GameObject go, Action<T3> initFunc) 
        where T1 : GameObj, new()
        where T2 : Entity, new()
        where T3 : Data, new() {
        var data = gameSystem.InstanceData<T3>(setting, go);
        initFunc?.Invoke(data);
        return gameSystem.InstanceGameObj<T1, T2>(data);
    }    

    /// <summary>
    /// 普普通通实例化
    /// </summary>
    /// <param name="initFunc"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <returns></returns>
    protected virtual int Instance<T1, T2, T3>(Action<T3> initFunc) 
        where T1 : GameObj, new()
        where T2 : Entity, new()
        where T3 : Data, new() {
        var data = gameSystem.InstanceData<T3>(setting, setting.PrefabSign);
        initFunc?.Invoke(data);
        return gameSystem.InstanceGameObj<T1, T2>(data);
    }

    /// <summary>
    /// 带窗口实例化
    /// </summary>
    /// <param name="initFunc"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <returns></returns>
    public int Instance<T1, T2, T3, T4>(Action<T4> initFunc)
        where T1 : Window, new()
        where T2 : GameObj, new()
        where T3 : Entity, new()
        where T4 : Data, new() {
        var data = gameSystem.InstanceData<T4>(setting, typeof(T1).Name);
        initFunc?.Invoke(data);
        return gameSystem.InstanceWindow<T1, T2, T3>(data);
    }

    public T GetSetting<T>() where T : Setting {
        return (T)setting;
    }
}