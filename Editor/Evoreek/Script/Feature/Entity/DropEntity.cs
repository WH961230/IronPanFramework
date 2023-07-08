public class DropEntity : Entity {
    private DropData dropData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.dropData = (DropData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}