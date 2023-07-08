public static class MessageConstant {
    #region 游戏流程

    public static int StartGameMsg = 0; //游戏开始
    public static int BackToMainMsg = 17; //返回大厅
    public static int GameOverMsg = 23;//游戏结束
    public static int EnemyWaveTipMsg = 24;//波数提示

    #endregion

    #region 加载相关

    public static int TerrainLoadFinished = 11; //地形加载完成
    public static int MainPlayerLoadFinished = 10; //本地玩家加载完成
    public static int MainPlayerWindowLoadFinished = 26;//本地玩家界面显示完成
    #endregion

    #region 控制台

    public static int ConsoleLogMsg = 7; //控制台日志

    #endregion

    #region 功能

    public static int StartFlightMsg = 3; //开始起飞
    public static int StartParachuteMsg = 5; //开始跳伞
    public static int PickItemMsg = 1; //拾取物品
    public static int DropItemMsg = 2; //丢弃物品
    public static int ViewTypeMsg = 12; //切换视角类型
    public static int EdgeMoveMsg = 14; //相机边缘移动
    public static int RegionSingleMsg = 13; //框选消息 id:框选中的物体ID bool:框选/取消

    public static int ShakeCamera = 18; //相机震动
    public static int BaseValueFlyOut = 19; //数值飞出

    public static int ObjectDead = 27;//物体死亡

    #endregion

    #region 数值

    public static int ValueSingleChangeMsg = 16;//单个单位数值修改消息
    public static int BarInfoMsg = 25;//角色血条信息消息

    #endregion

    #region 技能

    public static int SkillAcquireSucMsg = 20;//成功装载技能
    public static int ShowSkillInfo = 28;//技能信息 参数：技能
    public static int CloseSkillInfo = 29;//关闭技能信息

    #endregion
    
    #region 增益

    public static int BuffDisplayMsg = 21;//增益展示
    public static int BuffAcquireSucMsg = 22;//成功装载增益

    #endregion
}