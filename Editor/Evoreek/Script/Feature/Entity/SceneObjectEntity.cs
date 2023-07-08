public class SceneObjectEntity : Entity {
    private SceneObjectData sceneobjectData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.sceneobjectData = (SceneObjectData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}