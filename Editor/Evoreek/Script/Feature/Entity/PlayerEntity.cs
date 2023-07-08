using Common;
using UnityEngine;

public class PlayerEntity : Entity {
    private PlayerData playerData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.playerData = (PlayerData)data;

        if (playerData.isOwner) {
            Global.UI.Panel.Open<A_Controller>();
        }

        gameSystem.messageCenter.Reg<bool, PlayerViewType>(MessageConstant.ViewTypeMsg, (isDebug, type) => {
            playerData.viewType = type;
        });
    }

    public override void Clear() {
        gameSystem.messageCenter.UnReg<bool, PlayerViewType>(MessageConstant.ViewTypeMsg, (isDebug, type) => {
            playerData.viewType = type;
        });
        base.Clear();
    }

    public override void Update() {
        base.Update();
    }
}