public class SkillWhirlwindEntity : Entity {
    private SkillWhirlwindData whirlwindskillData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.whirlwindskillData = (SkillWhirlwindData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}