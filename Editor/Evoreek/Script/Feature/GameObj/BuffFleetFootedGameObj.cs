public class BuffFleetFootedGameObj : GameObj {
    private BuffFleetFootedData bufffleetfootedData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        bufffleetfootedData = (BuffFleetFootedData)data;
        Register();
    }
    
    private void Register() {
        foreach (var f in bufffleetfootedData.FunctionTypes) {
            Logger.Print($"物体: {bufffleetfootedData.MyObjName} 注册功能 {f.ToString()}");
            Register(f, new B<int>() {
                t1 = bufffleetfootedData.InstanceID,
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