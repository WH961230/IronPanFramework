using UnityEngine;

public class LightGameObj : GameObj {
    private LightData lightData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        lightData = (LightData)data;
        MyObj.transform.position = data.MyStartPointInfo.vec;
        MyObj.transform.rotation = Quaternion.Euler(data.MyStartPointInfo.engle);
    }

    public override void Clear() {
        base.Clear();
    }

    public override void Update() {
        base.Update();
    }
}