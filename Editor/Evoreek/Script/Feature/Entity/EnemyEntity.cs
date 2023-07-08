public class EnemyEntity : Entity {
    private EnemyData enemyData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.enemyData = (EnemyData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}