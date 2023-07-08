using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Evoreek/Setting/SOAudioSetting")]
public class SOAudioSetting : Setting {
    [SerializeField] public List<AudioInfo> audioInfo;

    public AudioClip GetAudioClipInfo(string audioSign) {
        foreach (var tempInfo in audioInfo) {
            if (tempInfo.audioSign.Equals(audioSign)) {
                return tempInfo.audioClip[UnityEngine.Random.Range(0, tempInfo.audioClip.Count)];
            }
        }

        return null;
    }
}

[Serializable]
public class AudioInfo {
    public string audioSign;
    public List<AudioClip> audioClip;
}
    