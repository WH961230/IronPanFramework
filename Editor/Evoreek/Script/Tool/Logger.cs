using System.Diagnostics;
using Debug = UnityEngine.Debug;

/// <summary>
/// 对全局的日志监控，设置日志开关，方便调试
/// </summary>
public static class Logger {
    /// <summary>
    /// 打印报错日志加暂停
    /// </summary>
    public static void PrintE(string info) {
        // 如果已经暂停 不输出日志
        if (GameData.IsUnityPause) {
            return;
        }
        // 输出日志
        var trace = new StackTrace();
        var frame = trace.GetFrame(1);
        var method = frame.GetMethod();
        var className = method.ReflectedType.Name;
        var methodName = method.Name;
        Debug.LogError(string.Format("<color=#E56A5E>{0}</color> <color=#09F2EB>{1}</color>", $"{className}.{methodName}:{info}", "游戏暂停"));
        // 如果没有暂停 输入日志
        GameData.IsUnityPause = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = true;
#endif
    }

    /// <summary>
    /// 打印报错日志不暂停
    /// </summary>
    public static void PrintELog(string info) {
        // 如果已经暂停 不输出日志
        if (GameData.IsUnityPause) {
            return;
        }
        // 输出日志
        var trace = new StackTrace();
        var frame = trace.GetFrame(1);
        var method = frame.GetMethod();
        var className = method.ReflectedType.Name;
        var methodName = method.Name;
        Debug.LogError(string.Format("<color=#E56A5E>{0}</color>", $"{className}.{methodName}{info}"));
    }

    /// <summary>
    /// 打印普通日志
    /// </summary>
    public static void Print(string info) {
        if (Game.Instance.loggerType != 0) {
            return;
        }
        Debug.Log(string.Format("<color=#09F285>{0}</color>", $"Log 内容：{info}"));
    }

    private static void PrintType(string info) {
        Debug.Log(string.Format("<color=#09F285>{0}</color>", $"{info}"));
    }

    /// <summary>
    /// 打印不同类型日志
    /// </summary>
    /// <param name="info"></param>
    public static void PrintType(int id, string info, LoggerType type) {
        if (Game.Instance.loggerType == 0) {
            return;
        }
        
        bool IsAllInit = (Game.Instance.loggerType & LoggerType.ALLINIT) == LoggerType.ALLINIT;
        bool IsFunction = (Game.Instance.loggerType & LoggerType.FUNCTIONRUNTIME) == LoggerType.FUNCTIONRUNTIME;
        bool IsBehaviour = (Game.Instance.loggerType & LoggerType.BEHAVIOURRUNTIME) == LoggerType.BEHAVIOURRUNTIME;
        bool IsCondition = (Game.Instance.loggerType & LoggerType.CONDITIONRUNTIME) == LoggerType.CONDITIONRUNTIME;
        switch (type) {
            case LoggerType.SYSTEM:
                if ((Game.Instance.loggerType & LoggerType.SYSTEM) == LoggerType.SYSTEM || IsAllInit) {
                    PrintType($"系统初始化[{id}][{info}]");
                }
                break;
            case LoggerType.ENTITY:
                if ((Game.Instance.loggerType & LoggerType.ENTITY) == LoggerType.ENTITY || IsAllInit) {
                    PrintType($"实体生成[{id}][{info}]");
                }
                break;
            case LoggerType.GAMEOBJ:
                if ((Game.Instance.loggerType & LoggerType.GAMEOBJ) == LoggerType.GAMEOBJ || IsAllInit) {
                    PrintType($"物体生成[{id}][{info}]");
                }
                break;
            case LoggerType.WINDOW:
                if ((Game.Instance.loggerType & LoggerType.WINDOW) == LoggerType.WINDOW || IsAllInit) {
                    PrintType($"窗口生成[{id}][{info}]");
                }
                break;
            case LoggerType.FUNCTIONREGISTER:
                if ((Game.Instance.loggerType & LoggerType.FUNCTIONREGISTER) == LoggerType.FUNCTIONREGISTER || IsFunction) {
                    PrintType($"功能注册[{id}][{info}]");
                }
                break;
            case LoggerType.FUNCTIONUNREGISTER:
                if ((Game.Instance.loggerType & LoggerType.FUNCTIONUNREGISTER) == LoggerType.FUNCTIONUNREGISTER || IsFunction) {
                    PrintType($"功能反注册[{id}][{info}]");
                }
                break;
            case LoggerType.BEHAVIOURREGISTER:
                if ((Game.Instance.loggerType & LoggerType.BEHAVIOURREGISTER) == LoggerType.BEHAVIOURREGISTER || IsBehaviour) {
                    PrintType($"行为注册[{id}][{info}]");
                }
                break;
            case LoggerType.BEHAVIOURUNREGISTER:
                if ((Game.Instance.loggerType & LoggerType.BEHAVIOURUNREGISTER) == LoggerType.BEHAVIOURUNREGISTER || IsBehaviour) {
                    PrintType($"行为反注册[{id}][{info}]");
                }
                break;
            case LoggerType.CONDITIONREGISTER:
                if ((Game.Instance.loggerType & LoggerType.CONDITIONREGISTER) == LoggerType.CONDITIONREGISTER || IsCondition) {
                    PrintType($"条件注册[{id}][{info}]");
                }
                break;
            case LoggerType.CONDITIONUNREGISTER:
                if ((Game.Instance.loggerType & LoggerType.CONDITIONUNREGISTER) == LoggerType.CONDITIONUNREGISTER || IsCondition) {
                    PrintType($"条件反注册[{id}][{info}]");
                }
                break;
        }
    }
}

[System.Flags]
public enum LoggerType {//日志类型
    #region 初始化

    SYSTEM = 1 << 2,//仅输出系统类型
    ENTITY = 1 << 3,
    GAMEOBJ = 1 << 4,
    WINDOW = 1 << 6,
    ALLINIT = SYSTEM | ENTITY | GAMEOBJ | WINDOW,//以下所有类型

    #endregion

    FUNCTIONREGISTER = 1 << 10,//注册功能
    FUNCTIONUNREGISTER = 1 << 11,
    FUNCTIONRUNTIME = FUNCTIONREGISTER | FUNCTIONUNREGISTER,
    
    BEHAVIOURREGISTER = 1 << 12,//注册行为
    BEHAVIOURUNREGISTER = 1 << 13,
    BEHAVIOURRUNTIME = BEHAVIOURREGISTER | BEHAVIOURUNREGISTER,

    CONDITIONREGISTER = 1 << 14,//注册条件
    CONDITIONUNREGISTER = 1 << 15,
    CONDITIONRUNTIME = CONDITIONREGISTER | CONDITIONUNREGISTER,
}