using UnityEngine;

public class DropComponent : GameComp {
    private GameObject skillInfo;
    private void OnMouseEnter() {
        Logger.Print("OnMouseEnter");
        Data data = GameData.gameSystem.MyEntityFeature.Get(InstanceID).MyData;//获取掉落物技能的信息
        // skillInfo = GameData.gameSystem.MyPoolFeature.Get("SkillInfo");//创建UI
        // if (skillInfo == null) {
        //     Logger.PrintE("skillInfo == null");
        //     return;
        // }
        // DisplaySkillInfo info = skillInfo.GetComponent<DisplaySkillInfo>();
        // Setting setting = GameData.gameSystem.GetSystem<SkillSystem>().GetSettingBySkillType(data.SkillType);
        // if (setting == null) {
        //     Logger.PrintE("setting == null");
        //     return;
        // }
        //
        // info.SkillDescription.text = setting.SkillInfo.description;
        //
        // Color tempColor = info.SkillIcon.color;
        // tempColor.a = 1;
        // info.SkillIcon.color = tempColor;
        //
        // info.SkillIcon.sprite = setting.SkillInfo.skillSprite;
        //
        // int UIMainPlayerID = GameData.MainPlayer.GetData().UIMainPlayerId;
        // Transform UImainParent = GameData.gameSystem.MyGameObjFeature.Get(UIMainPlayerID).GetObj().transform;//窗口
        // skillInfo.transform.parent = UImainParent;
        // skillInfo.transform.localRotation = Quaternion.identity;
        //
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);//3D 转 2D
        pos.z = 0;

        GameData.gameSystem.messageCenter.Dispatcher(MessageConstant.ShowSkillInfo, data.SkillType, pos);
        ClockUtil.Instance.AlarmAfter(5, OnMouseExit);
    }

    private void OnMouseExit() {
        // if (skillInfo == null) {
        //     return;
        // }
        // Logger.Print("OnMouseExit");
        // GameData.gameSystem.MyPoolFeature.Release("SkillInfo", skillInfo);
        // skillInfo = null;
        //
        GameData.gameSystem.messageCenter.Dispatcher(MessageConstant.CloseSkillInfo);
    }
}