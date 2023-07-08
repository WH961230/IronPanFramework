using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : GameComp {
    public List<WeaponAttackDetect> WeaponDetects;
    public void OpenAttackAction() {
        foreach (var tempDetect in WeaponDetects) {
            tempDetect.OpenDetect(true);
        }
    }

    public void CloseAttackAction() {
        foreach (var tempDetect in WeaponDetects) {
            tempDetect.CloseDetect();
        }
    }

    public void LookTargetAction() {
        if (GameData.DetectedAttackTargetID.ContainsKey(InstanceID)) {
            int detectedAttackTargetID = GameData.DetectedAttackTargetID[InstanceID];
            if (detectedAttackTargetID != 0) {
                GameObject go = GameData.gameSystem.MyGameObjFeature.Get(detectedAttackTargetID).GetObj();
                transform.forward = (go.transform.position - transform.position).normalized;
            }
        }
    }

    private void OnMouseOver() {
        GameData.SetCursor("攻击光标");
    }

    private void OnMouseExit() {
        GameData.SetCursor("游戏内光标");
    }
}