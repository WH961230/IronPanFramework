public class SkillWarTramplingEntity : Entity {
    private SkillWarTramplingData skillwartramplingData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.skillwartramplingData = (SkillWarTramplingData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}