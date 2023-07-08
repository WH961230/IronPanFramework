public class BuffAttackSpeedHaloGameObj : GameObj {
    private BuffAttackSpeedHaloData buffattackspeedhaloData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        buffattackspeedhaloData = (BuffAttackSpeedHaloData)data;
        Register();
    }
    
    private void Register() {
        foreach (var f in buffattackspeedhaloData.FunctionTypes) {
            Logger.Print($"物体: {buffattackspeedhaloData.MyObjName} 注册功能 {f.ToString()}");
            Register(f, new B<int>() {
                t1 = buffattackspeedhaloData.InstanceID,
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