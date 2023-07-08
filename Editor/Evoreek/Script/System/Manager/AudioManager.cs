using System.Collections.Generic;
using UnityEngine;

public class AudioManager : IManager {
    
    private GameSystem gameSystem;
    private SOAudioSetting soAudioSetting;
    
    public void Init(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        this.soAudioSetting = gameSystem.SoData.SOGameSetting.GetSetting<SOAudioSetting>();
    }

    // /// <summary>
    // /// 添加音频
    // /// </summary>
    // public int AddAudioSound(AudioData data) {//创建音频示例
    //     //获取缓存的音频物体
    //     GameObject audioGO = gameSystem.MyPoolFeature.Get("Audio");
    //     audioGO.name = data.tempAudioName + audioGO.GetInstanceID();
    //
    //     //获取组件
    //     AudioSource audioSource = audioGO.GetComponent<AudioSource>();
    //     AudioMark audioMark = audioGO.GetComponent<AudioMark>();
    //     PoolMark poolMark = audioGO.GetComponent<PoolMark>();
    //
    //     //为空返回
    //     if (audioSource == null || audioMark == null || poolMark == null) {
    //         Logger.PrintELog("找不到音频组件或物体未挂载音频标识组件！");
    //         return 0;
    //     }
    //
    //     AudioClip clip = soAudioSetting.GetAudioClipInfo(data.tempAudioName);
    //
    //     if (clip == null) {
    //         Logger.PrintELog("找不到音频信息！");
    //         return 0;
    //     }
    //
    //     audioMark.audioClip = clip;
    //
    //     //组件赋值
    //     audioMark.pos = data.pos;
    //     audioMark.isFollow = data.isFollow;
    //     audioMark.is3D = data.is3D;
    //     audioMark.isLoop = data.isLoop;
    //     audioMark.instanceID = audioGO.GetInstanceID();
    //     audioMark.Play();
    //
    //     //创建块
    //     AudioBlock audioBlock = new AudioBlock(audioMark, data);
    //     string s = "";
    //     foreach (var VARIABLE in audioBlockDic) {
    //         s += $"[{VARIABLE.Key},{VARIABLE.Value}]";
    //     }
    //     Logger.Print($"添加音效字典：ID: {s}");
    //     Logger.Print($"添加音效：ID: {audioBlock.instanceID}");
    //     audioBlockDic.Add(audioBlock.instanceID, audioBlock);
    //     return audioBlock.instanceID;
    // }
    
    /// <summary>
    /// 添加音频
    /// </summary>
    public GameObject AddAudioSound(AudioData data) {//创建音频示例
        //获取缓存的音频物体
        GameObject audioGO = gameSystem.MyPoolFeature.Get("Audio");
        audioGO.name = data.tempAudioName + audioGO.GetInstanceID();

        //获取组件
        AudioSource audioSource = audioGO.GetComponent<AudioSource>();
        AudioMark audioMark = audioGO.GetComponent<AudioMark>();
        PoolMark poolMark = audioGO.GetComponent<PoolMark>();

        //为空返回
        if (audioSource == null || audioMark == null || poolMark == null) {
            Logger.PrintELog("找不到音频组件或物体未挂载音频标识组件！");
            return null;
        }

        AudioClip clip = soAudioSetting.GetAudioClipInfo(data.tempAudioName);

        if (clip == null) {
            Logger.PrintELog("找不到音频信息！");
            return null;
        }

        Transform tempPlayTran = gameSystem.MyGameObjFeature.Get(data.InstanceID).GetObj().transform;

        audioMark.audioClip = clip;

        //组件赋值
        audioMark.isFollow = data.isFollow;
        audioMark.targetTran = data.isFollow ? tempPlayTran : null;
        audioMark.transform.position = tempPlayTran.position;//设置初始位置

        audioMark.is3D = data.is3D;
        audioMark.isLoop = data.isLoop;
        audioMark.instanceID = audioGO.GetInstanceID();
        audioMark.Play();

        return audioGO;
    }

    /// <summary>
    /// 移除音效
    /// </summary>
    public void RemoveAudioSound(GameObject go) {
        gameSystem.MyPoolFeature.Release("Audio", go);
    }
}


public class AudioBlock {
    public int instanceID;
    public AudioData audioData;
    public AudioMark audioMark;

    public AudioBlock(AudioMark audioMark, AudioData audioData) {
        this.instanceID = audioMark.instanceID;
        this.audioMark = audioMark;
        this.audioData = audioData;
    }
}