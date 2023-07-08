public class BuffAttackRangeHaloGameObj : GameObj {
    private BuffAttackRangeHaloData buffattackrangehaloData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        buffattackrangehaloData = (BuffAttackRangeHaloData)data;
        Register();
    }
    
    private void Register() {
        foreach (var f in buffattackrangehaloData.FunctionTypes) {
            Logger.Print($"物体: {buffattackrangehaloData.MyObjName} 注册功能 {f.ToString()}");
            Register(f, new B<int>() {
                t1 = buffattackrangehaloData.InstanceID,
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