public class BuffFleetFootedEntity : Entity {
    private BuffFleetFootedData bufffleetfootedData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.bufffleetfootedData = (BuffFleetFootedData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}