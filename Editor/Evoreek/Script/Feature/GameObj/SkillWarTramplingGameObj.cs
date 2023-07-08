public class SkillWarTramplingGameObj : GameObj {
    private SkillWarTramplingData skillwartramplingData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        skillwartramplingData = (SkillWarTramplingData)data;
        Register();

        GameData.MainPlayer.GetData().detectedRange = skillwartramplingData.detectedRange;
        GameData.MainPlayer.GetData().detectedObjectType = skillwartramplingData.detectedObjectType;
    }
    
    private void Register() {
        foreach (var f in skillwartramplingData.FunctionTypes) {
            Logger.Print($"物体: {skillwartramplingData.MyObjName} 注册功能 {f.ToString()}");
            Register(f, new B<int>() {
                t1 = skillwartramplingData.InstanceID,
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