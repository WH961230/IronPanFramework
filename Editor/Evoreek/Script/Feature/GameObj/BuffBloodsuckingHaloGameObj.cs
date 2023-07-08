public class BuffBloodsuckingHaloGameObj : GameObj {
    private BuffBloodsuckingHaloData buffbloodsuckinghaloData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        buffbloodsuckinghaloData = (BuffBloodsuckingHaloData)data;
        Register();
    }
    
    private void Register() {
        foreach (var f in buffbloodsuckinghaloData.FunctionTypes) {
            Logger.Print($"物体: {buffbloodsuckinghaloData.MyObjName} 注册功能 {f.ToString()}");
            Register(f, new B<int>() {
                t1 = buffbloodsuckinghaloData.InstanceID,
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