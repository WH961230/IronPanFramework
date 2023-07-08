using UnityEngine;
using TMPro;

public class Fps : MonoBehaviour {
    float updateInterval = 0.5f;
    private float accum = 0.0f;
    private float frames = 0;
    private float timeleft;// Use this for initialization
    public TextMeshProUGUI guiText;
    private int defaultRate;
    public int frameRate;
    void Start() {
        if (!guiText) {
            enabled = false;
            return;
        }

        timeleft = updateInterval;
        Application.targetFrameRate = defaultRate;
        frameRate = defaultRate;
    }

    void Update(){
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        if (timeleft <= 0.0f) {
            guiText.text = "" + (accum / frames).ToString("f2");
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }

        if (frameRate != Application.targetFrameRate) {
            Application.targetFrameRate = frameRate;
        }
    }
}

