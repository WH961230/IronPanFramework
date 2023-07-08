public class BigHeadCameraEntity : Entity {
    private BigHeadCameraData bigheadcameraData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.bigheadcameraData = (BigHeadCameraData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}