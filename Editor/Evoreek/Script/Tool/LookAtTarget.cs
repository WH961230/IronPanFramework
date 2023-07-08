using System;
using UnityEngine;

public class LookAtTarget : MonoBehaviour {
    public Vector3 offVec;
    public Transform target;

    public void SetTarget(Transform target) {
        this.target = target;
    }

    private void Update() {
        if (target == null) {
            return;
        }

        transform.position = target.position + offVec;
        this.transform.LookAt(target);
    }
}