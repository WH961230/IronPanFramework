public class CreaterEntity : Entity {
    private CreaterData createrData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.createrData = (CreaterData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}