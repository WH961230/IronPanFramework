using System.Collections.Generic;
using UnityEngine;

public class GameObjFeature : IFeature {
    private GameObjManager gameObjManager = new GameObjManager();
    private Game game;

    public void Init(Game game) {
        this.game = game;
    }

    public void Update() {
        gameObjManager.Update();
    }

    public void FixedUpdate() {
        gameObjManager.FixedUpdate();
    }

    public virtual void LateUpdate() {
        gameObjManager.LateUpdate();
    }

    public void Clear() {
        gameObjManager.RemoveAll();
    }

    public T Register<T>(Data data) where T : GameObj, new() {
        return gameObjManager.Register<T>(game, data);
    }

    public void Remove(int id) {
        gameObjManager.Remove(id);
    }

    public T Get<T>(int id) where T : GameObj, new() {
        return (T) gameObjManager.Get(id);
    }

    public GameObj Get(int id) {
        return gameObjManager.Get(id);
    }

    public List<GameObj> GetAllGameObj() {
        return gameObjManager.GetAll();
    }

    public Dictionary<int, GameObj> GetAllDic() {
        return gameObjManager.GetAllDic();
    }
    
    /// <summary>
    /// 获取所有指定类型的物体
    /// </summary>
    /// <returns></returns>
    public List<GameObj> GetObjByType(ObjectType type) {
        List<GameObj> allGo = game.gameSystem.MyGameObjFeature.GetAllGameObj();
        List<GameObj> retGo = new List<GameObj>();
        foreach (var tempGo in allGo) {
            if (tempGo.GetData().ObjectType == type) {
                retGo.Add(tempGo);
            }
        }

        return retGo;
    }
}