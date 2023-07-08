public class CentralEntity : Entity {
    private CentralData centralData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.centralData = (CentralData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}