public class AudioEntity : Entity {
    private AudioData audioData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.audioData = (AudioData)data;
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        base.Clear();
    }
}