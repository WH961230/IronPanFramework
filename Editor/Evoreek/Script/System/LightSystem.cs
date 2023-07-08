using System.Reflection;

public class LightSystem : GameSys {
    private SOLightSetting lightSetting;
    private GameSystem gameSystem;
    public LightSystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        Init(gameSystem);
        InstanceGameObject();
    }

    protected override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        lightSetting = gameSystem.SoData.SOGameSetting.GetSetting<SOLightSetting>();
        setting = lightSetting;
    }

    private int InstanceGameObject() {
        return Instance<LightGameObj, LightEntity, LightData>((data) => {
            data.MyObjName = "灯光";
            data.MyStartPointInfo = lightSetting.LightPointInfo;
        });
    }
}