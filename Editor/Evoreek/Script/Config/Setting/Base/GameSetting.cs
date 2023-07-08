using System;
using UnityEngine;

public abstract class GameSetting : ScriptableObject, ISetting {
    [Header("配置说明")]
    public string SettingInfo;
    [Header("资源前缀")]
    public string Prefix;
    [Header("系统标识")]
    public string SystemSign;
    protected GameSystem gameSystem;

    public virtual void Init(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
    }

    public virtual void Clear() { }

    public virtual void Update() {
    }
}