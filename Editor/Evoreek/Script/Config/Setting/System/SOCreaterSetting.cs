using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(menuName = "SO/SOCreaterSetting")]
public class SOCreaterSetting : Setting {
    public List<CreaterMatInfo> CreaterMatInfoList;
    public bool testMode;
}

[Serializable]
public class CreaterMatInfo {
    public ObjectType type;
    public Material mat;
}