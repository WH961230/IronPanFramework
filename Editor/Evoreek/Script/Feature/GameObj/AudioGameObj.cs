using System;
using UnityEngine;

public class AudioGameObj : GameObj {
    private AudioData audioData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        audioData = (AudioData)data;
        audioData.component = data.MyObj.GetComponent<AudioComponent>();
    }

    public override void Clear() {
        base.Clear();
    }

    public override void Update() {
        base.Update();
        if (GameData.MainCamera != null) {
            audioData.MyObj.transform.position = GameData.MainCamera.GetData().camera.transform.position;
        }
    }
}