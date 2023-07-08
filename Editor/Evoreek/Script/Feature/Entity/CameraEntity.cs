using UnityEngine;

public class CameraEntity : Entity {
    private CameraData cameraData;
    public override void Init(Game game, Data data) {
        base.Init(game, data);
        this.cameraData = (CameraData)data;
        gameSystem.messageCenter.Reg<bool, PlayerViewType>(MessageConstant.ViewTypeMsg, (isDebug, type) => {
            cameraData.viewType = type;
        });
    }

    public override void Clear() {
        gameSystem.messageCenter.UnReg<bool, PlayerViewType>(MessageConstant.ViewTypeMsg, (isDebug, type) => {
            cameraData.viewType = type;
        });
        base.Clear();
    }
}