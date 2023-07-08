using UnityEngine;

public class AudioData : Data {
    public AudioComponent component;
    
    //临时音频参数
    public string tempAudioName;
    public bool is3D;//是否是 3D 音效
    public bool isFollow;//跟随
    public bool isLoop;
}