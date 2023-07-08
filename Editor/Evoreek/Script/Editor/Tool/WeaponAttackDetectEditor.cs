using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponAttackDetect))]
public class WeaponAttackDetectEditor : Editor{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        WeaponAttackDetect tempScript = (WeaponAttackDetect) target;
        if (GUILayout.Button("创建碰撞体")) {
            Collider c = tempScript.gameObject.AddComponent<BoxCollider>();
            c.isTrigger = true;

            Rigidbody rb = tempScript.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }
}