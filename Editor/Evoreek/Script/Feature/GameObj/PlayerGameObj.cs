using Common;
using UnityEngine;

public class PlayerGameObj : GameObj {
    private PlayerData playerData;
    private PlayerComponent playerComponent;
    private bool isAttacking;

    public override void Init(Game game, Data data) {
        base.Init(game, data);
        playerData = (PlayerData) data;
        playerComponent = GetComp<PlayerComponent>();

        playerComponent.SpriteRenderer.transform.gameObject.SetActive(true);
        if (playerData.isOwner && playerData.ObjectType == ObjectType.PLAYER) {
            playerComponent.bloodFillImage.color = Color.green;
            playerComponent.SpriteRenderer.sprite = playerComponent.ownerQua;
            playerData.viewTPSTargetGo = playerComponent.CameraTPSViewTarget;
        } else if (playerData.ObjectType == ObjectType.MONSTER) {
            playerComponent.bloodFillImage.color = Color.red;
            playerComponent.SpriteRenderer.sprite = playerComponent.otherQua;
        } else if (playerData.ObjectType == ObjectType.CENTRAL || playerData.ObjectType == ObjectType.PLAYER) {
            playerComponent.bloodFillImage.color = Color.green;
            playerComponent.SpriteRenderer.sprite = playerComponent.friendQua;
        }

        gameSystem.messageCenter.Reg(MessageConstant.BackToMainMsg, UnRegister);
        gameSystem.messageCenter.Reg<int, bool>(MessageConstant.RegionSingleMsg, RegionObj);

        Register();
    }

    public override void Update() {
        base.Update();
    }

    public override void Clear() {
        gameSystem.messageCenter.UnReg(MessageConstant.BackToMainMsg, UnRegister);
        gameSystem.messageCenter.UnReg<int, bool>(MessageConstant.RegionSingleMsg, RegionObj);

        base.Clear();
    }

    /// <summary>
    /// 注册
    /// </summary>
    private void Register() {
        if (playerData.isOwner) {
            foreach (var f in playerData.FunctionTypes) {
                Register(f, new B<int>() {
                    t1 = playerData.InstanceID,
                });
            }
            gameSystem.messageCenter.Dispatcher(MessageConstant.ViewTypeMsg, true, PlayerViewType.TPS);//类似 Debug 工具直接设置人称
        }
    }

    /// <summary>
    /// 反注册
    /// </summary>
    private void UnRegister() {
        if (playerData.isOwner) {
            foreach (var f in playerData.FunctionTypes) {
                UnRegister(f, new B<int>() {
                    t1 = playerData.InstanceID,
                });
            }
            gameSystem.messageCenter.Dispatcher(MessageConstant.ViewTypeMsg, true, PlayerViewType.NONE);//类似 Debug 工具直接设置人称
        }
    }

    /// <summary>
    /// 选中物体
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isRegion"></param>
    private void RegionObj(int id, bool isRegion) {
        if (playerData.InstanceID == id) {
            //playerComponent.QuaTran.gameObject.SetActive(isRegion);
        }
    }
}