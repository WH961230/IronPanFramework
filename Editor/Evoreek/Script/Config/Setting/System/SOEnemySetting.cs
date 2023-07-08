using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/SOEnemySetting")]
public class SOEnemySetting : Setting {
    public PointInfo StartPointInfo;
    public List<Setting> BossSettings;
}