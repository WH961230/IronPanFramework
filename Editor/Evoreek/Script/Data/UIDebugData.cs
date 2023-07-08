using System.Collections.Generic;
using UnityEngine;

public class UIDebugData : Data {
    //指令 Command
    public string previousContent;//先前控制台日志内容
    public List<DebugCommand> debugCommands;
    public bool isEditorPoint = false;//编辑器
    public bool isEditorEnemyPoint = false;//
    public EditorPointType type;//编辑器调试类型
    public bool isEditorPointDisplay = false;//显示编辑器所有点位信息 在结束保存后重置
    public int isEditorClickTime = 0;
    public GameObject currentEditorGo;
    public GameObject currentEnemyEditorGo;
    public SOSceneObjectSetting sceneObjectSetting;
    public SOCentralSetting centralSetting;
    public SOEnemySetting enemySetting;
}

public enum EditorPointType {
    SCENEOBJECT,//场景物体
    CENTRAL,//守卫塔
    ENEMY,//怪物
}