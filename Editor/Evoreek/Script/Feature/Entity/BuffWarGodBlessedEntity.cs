public class BuffWarGodBlessedEntity : Entity {
    private BuffWarGodBlessedData buffwargodblessedData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.buffwargodblessedData = (BuffWarGodBlessedData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}