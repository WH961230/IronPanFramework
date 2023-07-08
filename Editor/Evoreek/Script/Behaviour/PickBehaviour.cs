using System;
using UnityEngine;
[CreateAssetMenu(menuName = "Evoreek/Behaviour/PickBehaviour")]
public class PickBehaviour : GameBehaviour {
    public override void Register<T>(T arg) {
        base.Register(arg);
        var info = (B<int, GameObject, AudioClip, float>) Convert.ChangeType(arg, typeof(B<int, GameObject, AudioClip, float>));
        var component = info.t2.AddComponent<PickableComponent>();
        component.Init(gameSystem);
        component.Register(arg);
    }
}

public class PickSettingInfo {
    public GameObject go;
    public AudioClip audioClip;
    public float pickRange;
}