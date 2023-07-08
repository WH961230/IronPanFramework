using UnityEngine;

public class UIMainPlayerGameObj : GameObj {
    private UIMainPlayerData uimainplayerData;
    private UIMainPlayerComponent comp;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        uimainplayerData = (UIMainPlayerData)data;
        comp = uimainplayerData.MyObj.GetComponent<UIMainPlayerComponent>();

        GameData.AudioSystem.AddSoundPlay(new AudioData() {//播放背景音效
            InstanceID = uimainplayerData.InstanceID,
            tempAudioName = "游戏背景",
            isLoop = true,
            is3D = false,
            isFollow = false,
        });

        gameSystem.messageCenter.Reg<Vector3, int>(MessageConstant.BaseValueFlyOut, (vec, value) => {
            Vector3 pos = Camera.main.WorldToScreenPoint(vec);
            pos.z = 0;
            GameObject go = gameSystem.MyPoolFeature.Get("血量飞溅文字物体");
            ValueFlyOutSet fly = go.GetComponent<ValueFlyOutSet>();
            fly.baseRootTran.position = pos;
            fly.SetText(Mathf.Abs(value).ToString(), value > 0 ? Color.green : Color.white);
        });
    }

    public override void Clear() {
        gameSystem.messageCenter.UnReg<Vector3, int>(MessageConstant.BaseValueFlyOut, (vec, value) => {
            Vector3 pos = Camera.main.WorldToScreenPoint(vec);
            pos.z = 0;
            GameObject go = gameSystem.MyPoolFeature.Get("血量飞溅文字物体");
            ValueFlyOutSet fly = go.GetComponent<ValueFlyOutSet>();
            fly.baseRootTran.position = pos;
            fly.SetText(Mathf.Abs(value).ToString(), value > 0 ? Color.green : Color.white);
        });
        base.Clear();
    }

    public override void Update() {
        base.Update();
    }

    public override void LateUpdate() {
        base.LateUpdate();
    }
}