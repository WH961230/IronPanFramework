using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/SOSkillWarTramplingSetting")]
public class SOSkillWarTramplingSetting : Setting {
    [Header("检测范围")] public float DetectedRange;
    [Header("检测层级")] public List<ObjectType> DetectedObjectTypeList;
}