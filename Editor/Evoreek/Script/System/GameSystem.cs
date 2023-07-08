using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 总系统
/// </summary>
public class GameSystem {
    private List<GameSys> systems = new List<GameSys>(); // 子系统
    private GameManager gameManager = new GameManager(); // 系统管理 存储数据
    public SOData SoData = new SOData();
    public MessageCenter messageCenter;
    // public NetworkCenter MyNetworkCenter;
    private Game game;
    public GameState MyGameState = GameState.None;

    public void Init(Game game) {
        this.game = game;
        messageCenter = new MessageCenter();
        InitFeature();
        InitSO();
    }

    public void Update() {
        foreach (var system in systems) {
            system.Update();
        }

        SoData.Update();
        gameManager.Update();
    }

    public void FixedUpdate() {
        foreach (var system in systems) {
            system.FixedUpdate();
        }

        SoData.FixedUpdate();
        gameManager.FixedUpdate();
    }

    public void LateUpdate() {
        foreach (var system in systems) {
            system.LateUpdate();
        }

        SoData.LateUpdate();
        gameManager.LateUpdate();
    }

    public void Clear() {
        messageCenter.Clear();
        messageCenter = null;
    }

    #region 配置

    private void InitSO() {
        SoData.Init(this);
    }

    #endregion

    #region 系统

    /// <summary>
    /// 初始化系统 - 注册系统
    /// </summary>
    public void AddSystem(GameSys system) {
        systems.Add(system);
    }

    /// <summary>
    /// 获取系统
    /// </summary>
    public T GetSystem<T>() where T : GameSys {
        foreach (var system in systems) {
            if (system.GetType() == typeof(T)) {
                return (T) system;
            }
        }

        return null;
    }

    #endregion

    #region 特征

    public EntityFeature MyEntityFeature => Get<EntityFeature>();
    public GameObjFeature MyGameObjFeature => Get<GameObjFeature>();
    public WindowFeature MyWindowFeature => Get<WindowFeature>();
    public PoolFeature MyPoolFeature => Get<PoolFeature>();
    public AnimationFeature MyAnimationFeature => Get<AnimationFeature>();

    private void InitFeature() {
        gameManager.Register<PoolFeature>(game);
        gameManager.Register<AnimationFeature>(game);
        
        gameManager.Register<WindowFeature>(game);
        gameManager.Register<GameObjFeature>(game);
        gameManager.Register<EntityFeature>(game);

    }

    private T Get<T>() where T : IFeature {
        return gameManager.Get<T>();
    }

    public int InstanceWindow<T1, T2, T3>(Data data) where T1 : Window, new()
        where T2 : GameObj, new()
        where T3 : Entity, new() {
        if (data == null) {
            return default;
        }

        MyEntityFeature.Register<T3>(data);
        GameObj tempInstanceGo = MyGameObjFeature.Register<T2>(data);
        MyWindowFeature.Register<T1>(tempInstanceGo);
        return data.InstanceID;
    }

    public int InstanceGameObj<T1, T2>(Data data) where T1 : GameObj, new() where T2 : Entity, new() {
        MyEntityFeature.Register<T2>(data);
        GameObj tempInstanceGo = MyGameObjFeature.Register<T1>(data);
        return data.InstanceID;
    }

    #endregion

    #region 公共实例创建

    public virtual T InstanceData<T>(Setting setting, GameObject go) where T : Data, new() {
        var data = new T();
        var prefab = go;
        if (prefab == null) {
            Logger.PrintE(" <b>实例化预制体失败!</b> ");
            return null;
        }

        data.InstanceID = prefab.GetInstanceID();
        data.MyObj = go;
        data.FunctionTypes = setting.FunctionTypes;
        return data;
    }

    public virtual T InstanceData<T>(Setting setting, string signName) where T : Data, new() {
        var data = new T();
        var prefab = InstanceGameObject(setting, signName);
        if (prefab == null) {
            Logger.PrintE(" <b>实例化预制体失败!</b> ");
            return null;
        }

        data.InstanceID = prefab.GetInstanceID();
        data.MyObj = data.MyObj == null ? prefab : data.MyObj;
        data.FunctionTypes = setting.FunctionTypes;
        return data;
    }

    private GameObject InstanceGameObject(Setting setting, string signName) {
        var go = AssetsLoad.GetAsset<GameObject>(setting.Prefix, signName, AssetsLoad.AssetType.Prefab);
        if (go == null) {
            return null;
        }

        go = Object.Instantiate(go);
        return go;
    }

    #endregion
}