using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻击检测
/// </summary>
public class WeaponAttackDetect : MonoBehaviour {
    public List<int> detectedId = new List<int>();//攻击检测到的ID
    private GameSystem gameSystem;
    private Data rootData;
    public float roundSpeed;//旋转速度
    public bool IsOpenDetect;//是否开启检测
    public bool IsOpenSingleDetect;//是否开启单次攻击检测
    private void Start() {
        GameComp comp = transform.root.GetComponent<GameComp>();
        if (comp == null) {
            Logger.PrintE($"物体 : {gameObject.name} 没找到 GameComp 在 Root 节点");
        }

        Entity tempEntity = GameData.gameSystem.MyEntityFeature.Get(comp.InstanceID);
        if (tempEntity == null) {
            Logger.PrintE($"物体 : {comp.InstanceID} 未找到 Entity !");
        }
        rootData = tempEntity.GetData();
    }

    /// <summary>
    /// 开启检测
    /// </summary>
    /// <param name="IsOpenSingleDetect">是否开启单次攻击检测</param>
    public void OpenDetect(bool IsOpenSingleDetect) {
        detectedId.Clear();
        IsOpenDetect = true;
        this.IsOpenSingleDetect = IsOpenSingleDetect;
    }

    public void CloseDetect() {
        detectedId.Clear();
        IsOpenDetect = false;
        this.IsOpenSingleDetect = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (!IsOpenDetect) {
            return;
        }
        
        if (rootData == null) {
            return;
        }

        foreach (var tempLayer in rootData.EnemyObjectLayerMasks) {//遍历检测的敌人层级
            if (other.gameObject.layer == LayerMask.NameToLayer(tempLayer)) {
                GameComp tempComp = other.gameObject.transform.root.GetComponent<GameComp>();

                if (IsOpenSingleDetect) {
                    if (detectedId.Contains(tempComp.InstanceID)) {
                        continue;
                    }
                    detectedId.Add(tempComp.InstanceID);
                }

                if (tempComp) {
                    int enemyId = tempComp.InstanceID;
                    Entity entity = GameData.gameSystem.MyEntityFeature.Get(enemyId);
                    entity.HPChange(rootData.InstanceID, -rootData.runtimeBaseValueData.ATK);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (!IsOpenDetect) {
            return;
        }
        
        if (rootData == null) {
            return;
        }

        foreach (var tempLayer in rootData.EnemyObjectLayerMasks) {//遍历检测的敌人层级
            if (other.gameObject.layer == LayerMask.NameToLayer(tempLayer)) {
                GameComp tempComp = other.gameObject.transform.root.GetComponent<GameComp>();
                if (detectedId.Contains(tempComp.InstanceID)) {
                    continue;
                }
                detectedId.Add(tempComp.InstanceID);
                if (tempComp) {
                    int enemyId = tempComp.InstanceID;
                    Entity entity = GameData.gameSystem.MyEntityFeature.Get(enemyId);
                    int damageValue = UnityEngine.Random.Range(rootData.runtimeBaseValueData.ATK / 2, rootData.runtimeBaseValueData.ATK);
                    entity.HPChange(rootData.InstanceID, -damageValue);
                }
            }
        }
    }
}