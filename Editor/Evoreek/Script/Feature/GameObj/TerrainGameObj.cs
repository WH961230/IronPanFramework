public class TerrainGameObj : GameObj {
    private TerrainData terrainData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        terrainData = (TerrainData)data;
    }
}