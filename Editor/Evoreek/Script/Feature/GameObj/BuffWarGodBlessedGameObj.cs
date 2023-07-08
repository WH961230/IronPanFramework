public class BuffWarGodBlessedGameObj : GameObj {
    private BuffWarGodBlessedData buffwargodblessedData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        buffwargodblessedData = (BuffWarGodBlessedData)data;
        Register();
    }
    
    private void Register() {
        foreach (var f in buffwargodblessedData.FunctionTypes) {
            Logger.Print($"物体: {buffwargodblessedData.MyObjName} 注册功能 {f.ToString()}");
            Register(f, new B<int>() {
                t1 = buffwargodblessedData.InstanceID,
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