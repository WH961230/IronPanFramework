using System.Reflection;
using UnityEngine;

public class SceneObjectSystem : GameSys {
    private SOSceneObjectSetting sceneObjectSetting;
    private GameSystem gameSystem;
    public SceneObjectSystem(GameSystem gameSystem) {
        this.gameSystem = gameSystem;
        Init(gameSystem);
    }

    protected override void Init(GameSystem gameSystem) {
        base.Init(gameSystem);
        sceneObjectSetting = gameSystem.SoData.SOGameSetting.GetSetting<SOSceneObjectSetting>();
        setting = sceneObjectSetting;
        gameSystem.messageCenter.Reg(MessageConstant.StartGameMsg, InstanceAllSceneObject);
    }

    protected override void Clear() {
        gameSystem.messageCenter.UnReg(MessageConstant.StartGameMsg, InstanceAllSceneObject);
        base.Clear();
    }

    private void InstanceAllSceneObject() {
        int sceneObjectId = InstanceGameObject();
        GameObject sceneObjectParent = gameSystem.MyGameObjFeature.Get<SceneObjectGameObj>(sceneObjectId).GetObj();
        //根据场景物体设置的物体列表创建所有场景物品
        foreach (var info in sceneObjectSetting.SceneObjectInfoList) {
            var go = Object.Instantiate(info.obj, sceneObjectParent.transform, true);
            go.transform.position = info.pos;
            go.transform.rotation = Quaternion.Euler(info.engle);
        }
    }
    
    #region 不同实例创建接口

    private int InstanceGameObject() {
        return Instance<SceneObjectGameObj, SceneObjectEntity, SceneObjectData>((data) => {
            data.MyObjName = "场景物体";
        });
    }
    
    #endregion
}