using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Evoreek/Setting/SOGameSetting")]
public class SOGameSetting : GameSetting {
    [Header("游戏功能")]
    public List<GameFunc> funcList = new List<GameFunc>();
    [Header("游戏设置")]
    public List<GameSetting> settingList = new List<GameSetting>();
    [Header("技能设置")]
    public List<GameSetting> skillSettingList = new List<GameSetting>();
    [Header("游戏开始飞机起飞")]
    public bool isStartFlight;
    [Header("是否开启主窗口")]
    public bool openMainWindow;

    public override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        foreach (var fl in funcList) {
            fl.Init(gameSystem);
        }

        // 获取指定文件夹下的系统设置
        foreach (var sl in settingList) {
            sl.Init(gameSystem);
        }

        foreach (var sl in skillSettingList) {
            sl.Init(gameSystem);
        }
    }

    public void Update() {
        foreach (var fl in funcList) {
            fl.Update();
        }
    }

    public void FixedUpdate() {
        foreach (var fl in funcList) {
            fl.FixedUpdate();
        }
    }

    public void LateUpdate() {
        foreach (var fl in funcList) {
            fl.LateUpdate();
        }
    }

    /// <summary>
    /// 获取游戏设置的功能模块
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public GameFunc GetFunc(FunctionType t) {
        foreach (var fl in funcList) {
            if (t.Equals(fl.functionType)) {
                return fl;
            }
        }

        return null;
    }

    public T GetSetting<T>() where T : GameSetting {
        foreach (var sl in settingList) {
            if (sl.GetType() == typeof(T)) {
                return (T)sl;
            }
        }

        return null;
    }
}