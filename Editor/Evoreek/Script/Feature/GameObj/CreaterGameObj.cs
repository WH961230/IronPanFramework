using UnityEngine;

public class CreaterGameObj : GameObj {
    private CreaterData createrData;
    private CreaterComponent createrComponent;
    private Stainer stainer;

    public override void Init(Game game, Data data) {
        base.Init(game, data);
        createrData = (CreaterData) data;
        stainer = MyObj.GetComponent<Stainer>();
        Material mat = null;
        foreach (var tempInfo in createrData.CreaterMatInfoList) {
            if (tempInfo.type == createrData.createrType) {
                mat = tempInfo.mat;
                break;
            }
        }

        stainer.SetMat(mat);
    }

    public override void Clear() {
        base.Clear();
    }

    public override void Update() {
        base.Update();
    }
}