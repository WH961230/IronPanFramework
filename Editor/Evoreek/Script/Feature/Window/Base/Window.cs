using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 游戏窗口
/// </summary>
public class Window : IWindow {
    protected Game MyGame;
    protected GameSystem gameSystem;

    public virtual void Init(Game game, GameObj gameObj) {
        MyGame = game;
        gameSystem = game.gameSystem;
        
        StackTrace stackTrace = new StackTrace();
        StackFrame[] stackFrames = stackTrace.GetFrames();
        
        if (stackFrames.Length > 1)
        {
            // 获取调用该方法的方法信息
            MethodBase callingMethod = stackFrames[1].GetMethod();
            
            // 输出调用该方法的类名
            Logger.PrintType(gameObj.GetData().InstanceID, callingMethod.DeclaringType.FullName, LoggerType.WINDOW);
        }
    }

    public virtual void Update() {
    }

    public virtual void FixedUpdate() {
    }

    public virtual void LateUpdate() {
    }

    public virtual void Clear() {
    }
}