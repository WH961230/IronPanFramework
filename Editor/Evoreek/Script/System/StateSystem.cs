/// <summary>
/// 状态系统
///  - 注册状态 Register [int GameCond]
///  - 获取状态 Get [int GameCond ret : GameCond]
/// </summary>
public class StateSystem : GameSys {
    private GameSystem gameSystem;
    private StateManager stateManager;
    public StateSystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        Init(gameSystem);
    }

    protected override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        
    }

    public bool Get(int id, GameCondition gameCondition) {
        stateManager.Get(id, gameCondition);
        return true;
    }
}