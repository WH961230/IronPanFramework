public class BuffAttackSpeedHaloEntity : Entity {
    private BuffAttackSpeedHaloData buffattackspeedhaloData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.buffattackspeedhaloData = (BuffAttackSpeedHaloData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}