using UnityEngine;

public class SliderSlow : MonoBehaviour {
    public float SlowSpeed;
    public RectTransform Rect;
    public RectTransform AlienRect;//对齐的

    public void Update() {
        if (AlienRect != null && AlienRect.gameObject != null && AlienRect.gameObject.activeSelf) {
            Rect.sizeDelta = new Vector2(Mathf.Lerp(Rect.rect.width, AlienRect.rect.width, Time.deltaTime * SlowSpeed), AlienRect.rect.height);
        }
    }
}