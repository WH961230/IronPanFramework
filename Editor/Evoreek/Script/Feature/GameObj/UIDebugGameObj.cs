public class UIDebugGameObj : GameObj {
    private UIDebugData uidebugData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        uidebugData = (UIDebugData)data;
        Register(FunctionType.DebugFunction, new B<int> {
            t1 = uidebugData.InstanceID
        });
    }

    public override void Clear() {
        base.Clear();
    }

    public override void Update() {
        base.Update();
    }
}