using UnityEngine;

public class SOData {
    public SOGameSetting SOGameSetting => Resources.Load<SOGameSetting>(PathData.SOGameSetting);
    private GameSystem gameSystem;

    public void Init(GameSystem gameSystem) {
        SOGameSetting.Init(gameSystem);
    }

    public void Update() {
        SOGameSetting.Update();
    }

    public void FixedUpdate() {
        SOGameSetting.FixedUpdate();
    }

    public void LateUpdate() {
        SOGameSetting.LateUpdate();
    }
}