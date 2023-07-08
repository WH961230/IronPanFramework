using System.Reflection;
using UnityEngine;

public class AudioSystem : GameSys {
    private AudioManager audioManager = new AudioManager();
    private SOAudioSetting audioSetting;
    private int localAudioId;

    public AudioSystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        Init(gameSystem);
        localAudioId = Instance<AudioGameObj, AudioEntity, AudioData>((data) => {
            data.MyObjName = "音频";
        });
        var comp = gameSystem.MyGameObjFeature.Get<AudioGameObj>(localAudioId).GetComp<AudioComponent>();
        GameData.AudioListener = comp.listener;
    }

    protected override void Init(GameSystem gameSystem) { 
        base.Init(gameSystem);
        GameData.AudioSystem = this;
        audioSetting = gameSystem.SoData.SOGameSetting.GetSetting<SOAudioSetting>();
        setting = audioSetting;
        audioManager.Init(gameSystem);
    }

    public AudioEntity GetAudioEntity() {
        return gameSystem.MyEntityFeature.Get<AudioEntity>(localAudioId);
    }

    public AudioData GetAudioData() {
        return GetAudioEntity().GetData<AudioData>();
    }
    
    public AudioComponent GetAudioComp() {
        return GetAudioData().component;
    }

    public GameObject AddSoundPlay(AudioData data) {
        return audioManager.AddAudioSound(data);
    }

    public void RemoveSoundPlay(GameObject audioGO) {
        audioManager.RemoveAudioSound(audioGO);
    }
}