using TMPro;
using UnityEngine;

public class ValueFlyOutSet : MonoBehaviour {
    public Transform baseRootTran;//偏移物体
    public TextMeshProUGUI text;

    public void SetText(string content, Color color) {
        text.text = content;
        text.color = color;
    }
}