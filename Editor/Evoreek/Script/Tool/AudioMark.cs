using UnityEngine;

public class AudioMark : MonoBehaviour {
    public int instanceID;
    public bool is3D;
    public bool isFollow;
    public Transform targetTran;
    public AudioClip audioClip;
    public AudioSource audioSource;
    public PoolMark poolMark;
    public bool isLoop;

    public void Play() {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;

        poolMark = GetComponent<PoolMark>();

        audioSource.spread = is3D ? 360 : 0;
        audioSource.spatialBlend = is3D ? 1 : 0;
        audioSource.rolloffMode = is3D ? AudioRolloffMode.Linear : AudioRolloffMode.Logarithmic;
        audioSource.loop = isLoop;
        
        if (isLoop) {
            poolMark.RecycleTime = 9999;
        } else {
            poolMark.RecycleTime = audioClip.length;
        }
        
        if (audioSource == null) {
            audioSource = GetComponent<AudioSource>();
        }

        audioSource.Play();
    }

    private void Update() {
        if (isFollow) {//跟随
            transform.position = targetTran.position;
        }
    }
}