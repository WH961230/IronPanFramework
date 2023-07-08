using UnityEngine;

public class LookCamera : MonoBehaviour {
    public bool IsRevert;
    private void Update() {
        if (Camera.main != null) {
            if (IsRevert) {
                transform.forward = -Camera.main.transform.forward;
            } else {
                transform.forward = Camera.main.transform.forward;
            }
        }
    }
}