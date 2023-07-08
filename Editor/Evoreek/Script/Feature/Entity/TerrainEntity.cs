public class TerrainEntity : Entity {
    private TerrainData terrainData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.terrainData = (TerrainData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}