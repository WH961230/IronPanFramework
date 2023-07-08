using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor(typeof(EnemyQuickSetup))]
public class EnemyQuickSetupEditor : Editor{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("一键创建")) {
            EnemyQuickSetup tempScript = (EnemyQuickSetup) target;
            // 第一步添加 EnemyComponent
            EnemyComponent tempEnemyComp = tempScript.gameObject.AddComponent<EnemyComponent>();
            tempEnemyComp.WeaponDetects = new List<WeaponAttackDetect>();

            // 添加 NavMeshAgent
            NavMeshAgent tempAgent = tempScript.gameObject.AddComponent<NavMeshAgent>();
            // 添加 CapsuleCollider
            tempScript.gameObject.AddComponent<CapsuleCollider>();

            tempScript.gameObject.GetComponent<Animator>().applyRootMotion = false;
            
            tempEnemyComp.agent = tempAgent;
            tempEnemyComp.animator = tempScript.gameObject.GetComponent<Animator>();
                
            // 寻找 Hand_L Hand_R 设置攻击碰撞体 且执行增加碰撞体方法
            Transform tempHandL = FindTranInChild(tempScript.transform, "Hand_L");
            if (tempHandL != null) {
                WeaponAttackDetect tempWeapon = tempHandL.gameObject.AddComponent<WeaponAttackDetect>();

                Collider c = tempWeapon.gameObject.AddComponent<BoxCollider>();
                c.isTrigger = true;

                Rigidbody rb = tempWeapon.gameObject.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
                
                tempEnemyComp.WeaponDetects.Add(tempWeapon);
            } else {
                Logger.PrintE($"找不到 Hand_L");
            }
        
            Transform tempHandR = FindTranInChild(tempScript.transform, "Hand_R");
            if (tempHandR != null) {
                WeaponAttackDetect tempWeapon = tempHandR.gameObject.AddComponent<WeaponAttackDetect>();

                Collider c = tempWeapon.gameObject.AddComponent<BoxCollider>();
                c.isTrigger = true;

                Rigidbody rb = tempWeapon.gameObject.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
                
                tempEnemyComp.WeaponDetects.Add(tempWeapon);
            } else {
                Logger.PrintE($"找不到 Hand_R");
            }
        }
        
        // 添加头标
        // 脚本赋值
        // 弹出提示调整碰撞体大小
    }

    private Transform FindTranInChild(Transform parent, string name) {
        int count = parent.childCount;//S
        if (count == 0) {
            return null;
        }
        for (int i = 0; i < count; i++) {//S 的直属子集
            Transform tempTran = parent.GetChild(i);
            if (tempTran.name.Equals(name)) {//找到了返回
                return tempTran;
            } else {
                Transform tempTranChild = FindTranInChild(tempTran, name);//当前的遍历
                if (tempTranChild != null) {
                    return tempTranChild;
                }
            }
        }
        return null;
    }
}