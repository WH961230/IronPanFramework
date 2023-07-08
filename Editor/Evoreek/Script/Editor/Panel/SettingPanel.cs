using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class SettingPanel : EditorWindow {
    private string className;
    private string prefabName;
    private GUIStyle settingTextStyle = new GUIStyle();
    private void OnGUI() {
        SettingTool();
    }

    public void SettingTool() {
        className = EditorGUILayout.TextField("起个名字 (大小严格):", className, new[] {
            GUILayout.Width(300)
        });

        prefabName = EditorGUILayout.TextField("预制体名字:", prefabName, new[] {
            GUILayout.Width(300)
        });
        // 导航：创建配置
        // 生成配置
        if (GUILayout.Button("配置破壳诞生", GUILayout.Width(100))) {
            LFPanel.CreatePrefabFile(className, prefabName);
            LFPanel.CreateSetting(PathData.SOSystemSettingPath, className, "SO", "Setting");
            var allSetting = LFPanel.FindAllSetting(PathData.SOSystemSettingPath);
            for (int i = 0; i < allSetting.Length; i++) {
                var set = allSetting[i];
                if (set.name.Contains(className) && set is Setting tempSet) {
                    tempSet.SystemSign = className + "System";
                    tempSet.Prefix = "Evoreek_";
                    tempSet.PrefabSign = prefabName;
                    EditorUtility.SetDirty(tempSet);
                    AssetDatabase.SaveAssets();
                }
            }
            AssetDatabase.Refresh();
        }
    
        EditorGUILayout.Separator();
    
        // 游戏已安装配置可视化
        var gameSetting = LFPanel.FindAllSetting(PathData.SOGameSettingPath)[0] as SOGameSetting;
        var settingAlreadySetList = gameSetting.settingList.ToArray();
        string[] settingAlreadySetStr = new string[settingAlreadySetList.Length];
        for (var i = 0; i < settingAlreadySetList.Length; i++) {
            settingAlreadySetStr[i] = settingAlreadySetList[i].name;
        }
    
        // 游戏文件夹下所有的配置
        var settingInFile = LFPanel.FindAllSetting(PathData.SOSystemSettingPath);
        var settingInFileStr = LFPanel.FindAllSettingName(PathData.SOSystemSettingPath);
        List<GameSetting> waitToInGameSetting = new List<GameSetting>();
        EditorGUILayout.LabelField("游戏已安装配置可视化");
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < settingInFileStr.Length; i++) {
            EditorGUILayout.BeginHorizontal();
            var setStr = settingInFileStr[i];
            var set = settingInFile[i];
            var isInGame = LFPanel.HasContainSetting(set, settingAlreadySetList);
            if (!isInGame) {
                waitToInGameSetting.Add(set);
            }
    
            // 文字样式
            settingTextStyle.fontSize = 12;
            settingTextStyle.normal.textColor = isInGame ? Color.green : Color.red;
    
            // 图标
            var content = EditorGUIUtility.IconContent("d_ScriptableObject Icon");
            GUILayout.Label(content, GUILayout.Height(20), GUILayout.Width(20));
    
            // 名称
            EditorGUILayout.LabelField(setStr, settingTextStyle);
            EditorGUILayout.EndHorizontal();
        }
    
        EditorGUILayout.EndVertical();
        if (GUILayout.Button("把配置塞进游戏里", GUILayout.Width(100))) {
            if (waitToInGameSetting.Count > 0) {
                for (int i = 0; i < waitToInGameSetting.Count; i++) {
                    var set = waitToInGameSetting[i];
                    gameSetting.settingList.Add(set);
                }
            }
    
            EditorUtility.SetDirty(gameSetting);
            AssetDatabase.SaveAssets();
        }
    }
}