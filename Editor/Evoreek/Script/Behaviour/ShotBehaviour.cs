using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Evoreek/Behaviour/ShotBehaviour")]
public class ShotBehaviour : GameBehaviour {
    private List<ShotSettingInfo> infoList = new List<ShotSettingInfo>();
    private InputConditionOutput input;

    public override void Register<T>(T arg1) {
        base.Register(arg1);
        var info = (B<int, Animator, Transform, Transform, AudioClip>) Convert.ChangeType(arg1,
            typeof(B<int, Animator, Transform, Transform, AudioClip>));
        infoList.Add(new ShotSettingInfo() {
            id = info.t1,
            animator = info.t2,
            weaponCameraTran = info.t3,
            weaponTran = info.t4,
            shotSoundClip = info.t5,
        });
    }

    public override void GetControl<T>(T t) {
        this.input = (InputConditionOutput)Convert.ChangeType(t, typeof(InputConditionOutput));
    }

    public override void LateUpdate() {
        base.LateUpdate();
        ShotEvent();//射击
    }

    private void ShotEvent() {
        for (int i = 0; i < infoList.Count; i++) {
            ShotSettingInfo info = infoList[i];
            if (input.LeftClick) {
                gameSystem.messageCenter.Dispatcher(MessageConstant.ConsoleLogMsg, "射击");
                if (info.animator != null) {
                    info.animator.Play("Shot");
                }

                // var bulletGO = gameSystem.GetSystem<BulletSystem>().LoadBullet();
                var bulletGO = new GameObject();
                // if (!GameData.AudioSource.isPlaying) {
                //     GameData.AudioSource.clip = info.shotSoundClip;
                //     GameData.AudioSource.Play();
                // }

                BulletEvent(bulletGO, info.weaponTran, info.weaponCameraTran);
            }
        }
    }

    private void BulletEvent(GameObject bulletComp, Transform weaponTran, Transform cameraTran) {
        var startPoint = weaponTran.position + weaponTran.forward * 2;
        var endPoint = cameraTran.position + cameraTran.forward * 30;
        bulletComp.transform.position = startPoint;
        bulletComp.transform.LookAt(endPoint);
        bulletComp.GetComponent<Rigidbody>().velocity = (endPoint - startPoint).normalized * 100;
        gameSystem.messageCenter.Dispatcher(MessageConstant.ConsoleLogMsg, $"子弹发射 {bulletComp.name}");
    }
}

public class ShotSettingInfo {
    public int id;
    public Transform weaponTran;
    public Transform weaponCameraTran;
    public Animator animator;
    public AudioClip shotSoundClip;
}