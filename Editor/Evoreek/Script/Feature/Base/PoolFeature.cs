
using UnityEngine;

public class PoolFeature : IFeature {
    private PoolManager poolManager;
    private Game game;
    public void Init(Game game) {
        this.game = game;
        poolManager = new PoolManager();
        poolManager.Init(game.gameSystem);
        GameData.PoolRoot = new GameObject("PoolRoot");
    }

    /// <summary>
    /// 获取资源
    /// </summary>
    /// <param name="sign"></param>
    /// <returns></returns>
    public GameObject Get(string sign) {
        return poolManager.Get(sign);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="sign"></param>
    /// <param name="go"></param>
    public void Release(string sign, GameObject go) {
        poolManager.Release(sign, go);
    }

    public void Update() {
    }

    public void FixedUpdate() {
    }

    public void LateUpdate() {
    }
    
    public void Clear() {
    }
}