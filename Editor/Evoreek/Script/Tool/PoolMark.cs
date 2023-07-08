using System;
using UnityEngine;
using UnityEngine.Events;

public class PoolMark : MonoBehaviour {
    [Header("每次实例个数")]
    public int InstanceNum = 30;//资源池每次创建个数
    [Header("资源几秒后回收")]
    public float RecycleTime = 1;//实例回收时间

    private float instanceContinueTime;//实例化持续时间
    private string sign;
    public string Sign => sign;
    public bool isRecycle = false;

    public UnityAction poolReleaseAction;
    private GameSystem gameSystem;
    private void OnEnable() {
        instanceContinueTime = 0;
        isRecycle = false;
    }

    public void Init(GameSystem gameSystem, string sign) {
        instanceContinueTime = 0;
        this.gameSystem = gameSystem;
        this.sign = sign;
        gameObject.name = sign;
    }

    private void Update() {
        if (RecycleTime == 0 || gameSystem == null || isRecycle) {
            return;
        }
        instanceContinueTime += Time.deltaTime;
        if (instanceContinueTime >= RecycleTime) {
            instanceContinueTime = 0;
            poolReleaseAction?.Invoke();
            gameSystem.MyPoolFeature.Release(sign, gameObject);
            isRecycle = true;
        }
    }
}