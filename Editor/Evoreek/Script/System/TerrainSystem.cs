using System.Reflection;

public class TerrainSystem : GameSys {
    private SOTerrainSetting terrainSetting;
    private GameSystem gameSystem;
    public TerrainSystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        Init(gameSystem);
        gameSystem.messageCenter.Reg(MessageConstant.StartGameMsg, MsgInstance);
    }

    protected override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        terrainSetting = gameSystem.SoData.SOGameSetting.GetSetting<SOTerrainSetting>();
        setting = terrainSetting;
    }

    protected override void Clear() {
        gameSystem.messageCenter.UnReg(MessageConstant.StartGameMsg, MsgInstance);
        base.Clear();
    }

    private void MsgInstance() {
        Instance<TerrainGameObj, TerrainEntity, TerrainData>((data) => {
            data.MyObjName = "地形";
        });
        gameSystem.messageCenter.Dispatcher(MessageConstant.TerrainLoadFinished);
    }
}