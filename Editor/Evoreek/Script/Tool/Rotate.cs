using UnityEngine;

public class Rotate : MonoBehaviour {
    public Vector3 dir;
    public float speed;
    void Update() {
        transform.Rotate(dir * speed * Time.deltaTime);
    }
}