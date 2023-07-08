public class BuffAttackRangeHaloEntity : Entity {
    private BuffAttackRangeHaloData buffattackrangehaloData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.buffattackrangehaloData = (BuffAttackRangeHaloData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}