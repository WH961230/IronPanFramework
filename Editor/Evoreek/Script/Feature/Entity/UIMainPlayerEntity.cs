public class UIMainPlayerEntity : Entity {
    private UIMainPlayerData uimainplayerData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.uimainplayerData = (UIMainPlayerData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}