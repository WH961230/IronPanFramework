public class WhirlwindSkillGameObj : GameObj {
    private SkillWhirlwindData whirlwindskillData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        whirlwindskillData = (SkillWhirlwindData)data;
        Register();
    }
    
    private void Register() {
        foreach (var f in whirlwindskillData.FunctionTypes) {
            Logger.Print($"物体: {whirlwindskillData.MyObjName} 注册功能 {f.ToString()}");
            Register(f, new B<int>() {
                t1 = whirlwindskillData.InstanceID,
            });
        }
    }

    public override void Clear() {
        base.Clear();
    }

    public override void Update() {
        base.Update();
    }
}