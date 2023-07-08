using System;
using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(menuName = "SO/SOCentralSetting")]
public class SOCentralSetting : Setting {
    [Header("出生点")] public PointInfo CentralStartPoint;
    [Header("调试物体")] public Object CentralDebugObj;
}