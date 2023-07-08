using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : IManager {
    private Dictionary<string, PoolBlock> poolBlockDic = new Dictionary<string, PoolBlock>();
    private GameSystem gameSystem;
    public void Init(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
    }

    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="sign"></param>
    /// <returns></returns>
    public GameObject Get(string sign) {
        if (sign == null) {
            Logger.PrintE("资源标识为空！");
            return null;
        }

        if (poolBlockDic.ContainsKey(sign)) {//存在标识
            return poolBlockDic[sign].Get();
        }

        //不存在标识 创建
        string prefix = "Evoreek_";
        GameObject poolGo = AssetsLoad.GetAsset<GameObject>(prefix, sign, AssetsLoad.AssetType.Prefab);
        
        if (poolGo == null) { Logger.PrintE($"资源标识获取物体为空！{prefix + sign}"); return null; }
        
        PoolMark poolMark = poolGo.GetComponent<PoolMark>();
        if (poolMark == null) { Logger.PrintE($"资源标识获取物体未挂载 PoolMark 组件！{prefix + sign}"); return null; }

        poolMark.Init(gameSystem, sign);
        PoolBlock poolBlockInstance = new PoolBlock(gameSystem, poolMark);

        poolBlockDic[sign] = poolBlockInstance;
        return poolBlockInstance.Get();
    }
    
    /// <summary>
    /// 回收
    /// </summary>
    /// <param name="sign"></param>
    public void Release(string sign, GameObject go) {
        if (sign == null) {
            Logger.PrintE("待回收资源标识为空！");
        }

        if (go == null) {
            Logger.PrintE("回收物体为空！");
        }

        if (poolBlockDic.ContainsKey(sign)) {//存在标识
            poolBlockDic[sign].Recycle(go);
        } else {//不存在标识 创建
            PoolMark poolMark = go.GetComponent<PoolMark>();
            if (poolMark == null) { Logger.PrintE($"待回收资源标识获取物体未挂载 PoolMark 组件！{go.name}"); }

            PoolBlock poolBlockInstance = new PoolBlock(gameSystem, poolMark);
            poolBlockDic[sign] = poolBlockInstance;
        }
    }
}

/// <summary>
/// 对象池块
/// </summary>
/// <typeparam name="T"></typeparam>
public class PoolBlock {
    private Queue queue = new Queue();
    private GameSystem gameSystem;
    private PoolMark poolMark;//池块存储的物体

    public PoolBlock(GameSystem gameSystem, PoolMark poolMark) {//mark: 资源物体 instanceNum: 创建数量
        this.gameSystem = gameSystem;

        if (this.poolMark == null) {
            this.poolMark = poolMark;
        } else {
            if (poolMark != this.poolMark) {
                Logger.PrintE("资源池录入错误");
            }
        }
        InstanceWithNum(poolMark.InstanceNum);
    }

    /// <summary>
    /// 获取
    /// </summary>
    /// <returns></returns>
    public GameObject Get() {
        if (queue.Count > 0) {
            GameObject go = queue.Dequeue() as GameObject;
            go.SetActive(true);
            return go;
        }
        
        InstanceWithNum(poolMark.InstanceNum);//生成指定个数的实例
        return Get();
    }

    /// <summary>
    /// 实例化
    /// </summary>
    /// <param name="go"></param>
    private void InstanceWithNum(int instanceNum) {
        while (instanceNum > 0) {
            string name = poolMark.gameObject.name;
            GameObject tempGo = Object.Instantiate(poolMark.gameObject, GameData.PoolRoot.transform);//实例化物体
            PoolMark tempMark = tempGo.GetComponent<PoolMark>();
            tempMark.Init(gameSystem, name);
            Recycle(tempGo);//回收
            instanceNum--;
        }
    }

    /// <summary>
    /// 回收
    /// </summary>
    /// <param name="go"></param>
    public void Recycle(GameObject go) {
        go.transform.parent = GameData.PoolRoot.transform;
        go.SetActive(false);
        queue.Enqueue(go);
    }
}