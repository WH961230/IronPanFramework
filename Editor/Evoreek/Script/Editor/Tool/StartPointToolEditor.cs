using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StartPointTool))]
public class StartPointToolEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        StartPointTool tool = (StartPointTool) target;
        if (GUILayout.Button($"清空 {tool.settingType.ToString()} 起始点")) {
            foreach (Transform t in tool.transform) {
                Destroy(t.gameObject);
            }
        }

        if (GUILayout.Button($"读取 {tool.settingType.ToString()} 起始点")) {
            List<PointInfo> points = new List<PointInfo>();
            switch (tool.settingType) {
                case SettingType.Player:
                    SOPlayerSetting psetting = GetSetting<SOPlayerSetting>();
                    points = psetting.StartPoint;
                    break;
                case SettingType.Enemy:
                    SOEnemySetting asetting = GetSetting<SOEnemySetting>();
                    break;
            }

            foreach (PointInfo p in points) {
                tool.CreateGameObjectInVec3(p.vec, Quaternion.Euler(p.engle));
            }
        }

        if (GUILayout.Button($"添加 {tool.settingType.ToString()} 起始点配置数据")) {
            switch (tool.settingType) {
                case SettingType.Player:
                    SOPlayerSetting psetting = GetSetting<SOPlayerSetting>();
                    if (psetting == null) {
                        Logger.PrintE(" 未获取到玩家配置 ");
                    }
                    psetting.StartPoint.Clear();

                    var ppoints = tool.GetAllPointInfo();
                    foreach (PointInfo p in ppoints) {
                        psetting.StartPoint.Add(p);
                    }

                    EditorUtility.SetDirty(psetting);
                    AssetDatabase.SaveAssets();
                    break;
                case SettingType.Enemy:
                    break;
            }
        }
    }

    private T GetSetting<T>() where T : GameSetting {
        string[] assetsId = AssetDatabase.FindAssets("t:scriptableobject", new[] {
            PathData.SOSystemSettingPath
        });
        for (int i = 0; i < assetsId.Length; i++) {
            var id = assetsId[i];
            GameSetting setting = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(id)) as GameSetting;
            if (setting.GetType() == typeof(T)) {
                return (T) setting;
            }
        }

        return null;
    }
}