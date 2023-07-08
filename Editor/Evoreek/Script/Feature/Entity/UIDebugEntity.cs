using System.Buffers.Text;

public class UIDebugEntity : Entity {
    private UIDebugData uidebugData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.uidebugData = (UIDebugData)data;
    }

    public override void Clear() {
        base.Clear();
    }

    public override void Update() {
        base.Update();
    }
}