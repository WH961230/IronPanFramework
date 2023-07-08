using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BigHeadCameraComponent : GameComp {
    public List<MonsterDisplayInfo> monsterDisplayInfo;
    public TextMeshProUGUI displayName;
    public Animator displayAnimator;
}

[Serializable]
public class MonsterDisplayInfo {
    public int waveNum;
    public GameObject monsterGO;
    public TextMeshProUGUI monsterText;
}