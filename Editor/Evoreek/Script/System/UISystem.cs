using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// UI 系统 处理 UI 之间的关系 不负责 UI 的具体功能
/// </summary>
public class UISystem : GameSys {
    private SOUISetting souiSetting;
    private UIManager uiManager;

    public UISystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        Init(gameSystem);
    }

    protected override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        souiSetting = gameSystem.SoData.SOGameSetting.GetSetting<SOUISetting>();
        setting = souiSetting;
        bool openMainWindow = gameSystem.SoData.SOGameSetting.openMainWindow;
        if (openMainWindow) {
            Instance<UIMainWindow, UIMainGameObj, UIMainEntity, UIMainData>((data) => {
                    data.MyObjName = "主界面";
                }
            );
        }

        gameSystem.messageCenter.Reg(MessageConstant.TerrainLoadFinished, () => {
            Instance<UIDebugWindow, UIDebugGameObj, UIDebugEntity, UIDebugData>((data) => {
                data.MyObjName = "调试界面";
                data.debugCommands = souiSetting.debugCommand;
                data.sceneObjectSetting = gameSystem.GetSystem<SceneObjectSystem>().GetSetting<SOSceneObjectSetting>();
                data.centralSetting = gameSystem.GetSystem<CentralSystem>().GetSetting<SOCentralSetting>();
                data.enemySetting = gameSystem.GetSystem<EnemySystem>().GetSetting<SOEnemySetting>();
            });
        });
    }

    protected override void Clear() {
        gameSystem.messageCenter.UnReg(MessageConstant.TerrainLoadFinished, () => {
            Instance<UIDebugWindow, UIDebugGameObj, UIDebugEntity, UIDebugData>((data) => {
                data.MyObjName = "调试界面";
                data.debugCommands = souiSetting.debugCommand;
                data.sceneObjectSetting = gameSystem.GetSystem<SceneObjectSystem>().GetSetting<SOSceneObjectSetting>();
                data.centralSetting = gameSystem.GetSystem<CentralSystem>().GetSetting<SOCentralSetting>();
                data.enemySetting = gameSystem.GetSystem<EnemySystem>().GetSetting<SOEnemySetting>();
            });
        });
        base.Clear();
    }
}