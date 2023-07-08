using System.Collections.Generic;
using UnityEngine;

public class UIMainPlayerData : Data {
    #region 框选信息
    
    #region 框选中的单个玩家信息

    public BaseValueData BaseData;

    #endregion

    #region 框选中的多个玩家信息 -> 目前只支持玩家 后期拓展所有可框选单位

    public List<BaseValueData> playerBaseDatas;

    #endregion

    #endregion

    public GameObject SkillInfoGO;
}