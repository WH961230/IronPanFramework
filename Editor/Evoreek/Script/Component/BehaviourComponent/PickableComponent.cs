using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class PickableComponent : MonoBehaviour {
    private GameSystem gameSystem;
    private float pickRange;
    private int itemId;

    public void Init(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
    }

    public void Register<T>(T arg) {
        var info = (B<int, GameObject, AudioClip, float>) Convert.ChangeType(arg, typeof(B<int, GameObject, AudioClip, float>));
        this.itemId = info.t1;
        this.pickRange = info.t4;

        var collider = transform.GetComponent<Collider>();
        if (!collider) {
            transform.gameObject.AddComponent<Collider>();
        }
    }

    public void Update() {
        PickUpEvent();
    }

    private void PickUpEvent() {
        foreach (var t in gameSystem.GetSystem<PlayerSystem>().playerManager.AllPlayer) {
            var dis = Vector3.Distance(t.Value.transform.position, transform.position);
            if (dis < pickRange) {
                gameSystem.messageCenter.Dispatcher(MessageConstant.ConsoleLogMsg, $"id :{t.Key} 拾取 ：{gameObject.name} itemId: {itemId}");
                Logger.Print("拾取 ：" + gameObject.name);
                gameSystem.messageCenter.Dispatcher<int>(MessageConstant.PickItemMsg, itemId);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(transform.position, pickRange);
    }
}