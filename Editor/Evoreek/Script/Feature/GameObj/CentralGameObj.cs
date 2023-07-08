public class CentralGameObj : GameObj {
    private CentralData centralData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        centralData = (CentralData)data;
        
        foreach (var f in centralData.FunctionTypes) {
            Logger.Print($"物体: {centralData.MyObjName} 注册功能 {f.ToString()}");
            Register(f, new B<int>() {
                t1 = centralData.InstanceID,
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