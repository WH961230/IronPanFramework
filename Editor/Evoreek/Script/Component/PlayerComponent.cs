using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerComponent : GameComp {
    public Transform HeadTran;//头部
    public Transform BodyTran;//身体
    public Transform HandParentTran;//手总结点
    public Transform RightHandTran;//右手
    public Transform LeftHandTran;//左手
    public Transform AK;//武器
    public Image bloodFillImage;

    public Transform QuaTran;//脚下选中框
    public Sprite ownerQua;//控制着白色圈圈
    public Sprite friendQua;//友军绿色圈圈
    public Sprite otherQua;//其他红色圈圈

    public List<WeaponAttackDetect> WeaponDetects;

    public Transform SwordEffectRootTran;//剑气特效父物体

    private void Start() {
        Game.Instance.gameSystem.GetSystem<PlayerSystem>().InstanceAction(gameObject, GetComponent<NetworkIdentity>().isOwned);
    }

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

    private GameObject swordEffect;
    public void OpenSwordEffectAction() {
        if (swordEffect== null) {
            swordEffect = Game.Instance.gameSystem.MyPoolFeature.Get("SwordEffect");
            swordEffect.transform.parent = SwordEffectRootTran.transform;
            swordEffect.transform.localPosition = Vector3.zero;
            swordEffect.transform.localRotation = Quaternion.identity;
        }
        
        ParticleSystem ps = swordEffect.GetComponentInChildren<ParticleSystem>();
        ps.Stop(); // 停止粒子系统
        ps.Clear(); // 清除所有的粒子
        ps.Play(); // 重新开始播放粒子

        GameData.AudioSystem.AddSoundPlay(new AudioData() {
            InstanceID = InstanceID,
            tempAudioName = "本地玩家近战攻击敌人",
            isLoop = false,
            is3D = true,
            isFollow = true,
        });
    }

    public void CloseSwordEffectAction() {
        if (swordEffect != null) {
            Game.Instance.gameSystem.MyPoolFeature.Release("SwordEffect", swordEffect);
            swordEffect = null;
        }
    }
}