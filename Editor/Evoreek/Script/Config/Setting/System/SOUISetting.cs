using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Evoreek/Setting/SOUISetting")]
public class SOUISetting : Setting {
    [SerializeField] public List<DebugCommand> debugCommand;
}

[Serializable]
public struct DebugCommand {
    public string commandContent;//指令内容
    public string commandSpecification;//指令说明
}