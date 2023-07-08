public class SceneObjectGameObj : GameObj {
    private SceneObjectData sceneobjectData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        sceneobjectData = (SceneObjectData)data;
    }

    public override void Clear() {
        base.Clear();
    }

    public override void Update() {
        base.Update();
    }
}