using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源池 - 存储游戏物体
/// </summary>
public static class GameObjectPool {
    private static List<GameObject> PoolsList = new List<GameObject>();
    private static Dictionary<string, List<GameObject>> PoolsDic = new Dictionary<string, List<GameObject>>();

    /// <summary>
    /// 注册游戏物体
    /// </summary>
    public static void Set(string sign, GameObject go) {
        if (go == null) {
            Logger.PrintE(" 传入资源池 GO 为空！ ");
        }

        go.SetActive(true);
        // 存在注册物体
        if (PoolsDic.TryGetValue(sign, out var ret)) {
            if (ret.Count > 0) {
                ret.Add(go);
            } else {
                var list = new List<GameObject>();
                list.Add(go);
                PoolsDic.Add(sign, list);
            }
        } else {
            var list = new List<GameObject>();
            list.Add(go);
            PoolsDic.Add(sign, list);
        }
        
        Logger.Print($" 资源池子新增物体{go.name}");
    }

    /// <summary>
    /// 获取游戏物体
    /// </summary>
    public static GameObject Get(string sign) {
        if (PoolsDic.TryGetValue(sign, out var ret)) {
            if (ret.Count > 0) {
                var go = ret[0]; 
                go.SetActive(true);
                return go;
            }
        }

        return null;
    }
}