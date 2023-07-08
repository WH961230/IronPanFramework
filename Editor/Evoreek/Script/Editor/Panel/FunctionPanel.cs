using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FunctionPanel : EditorWindow {
    private float blockHeight = 700;
    private float blockWidth = 100;
    private string className; // 类名
    private string functionClassName;
    private GUIStyle settingTextStyle = new GUIStyle();

    public void FunctionTool() {
        className = EditorGUILayout.TextField("起个名字 (大小严格):", className, new[] {
            GUILayout.Width(300)
        });

        GUILayout.BeginHorizontal();
        //===================== 条件 =====================
        if (GUILayout.Button("条件脚本创建", GUILayout.Width(100))) {
            LFPanel.GenerateSingleScript(className, PathData.ConditionTemplatePath, PathData.ConditionPath, "", "Condition");//创建脚本
        }
        
        if (GUILayout.Button("条件脚本销毁", GUILayout.Width(100))) {
            AssetDatabase.DeleteAsset(PathData.ConditionPath + className + "Condition.cs");//解除 Behaviour
        }

        if (GUILayout.Button("条件破壳诞生", GUILayout.Width(100))) {
            LFPanel.CreateSetting(PathData.ConditionSettingPath, className, "", "Condition");//创建条件配置
        }

        if (GUILayout.Button("条件配置卸载", GUILayout.Width(100))) {
            AssetDatabase.DeleteAsset(PathData.ConditionSettingPath + "" + className + "Condition.asset");
        }
        
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        //===================== 行为 =====================
        if (GUILayout.Button("行为脚本创建", GUILayout.Width(100))) {
            LFPanel.GenerateSingleScript(className, PathData.BehaviourTemplatePath, PathData.BehaviourPath, "", "Behaviour"); //创建脚本
        }
        
        if (GUILayout.Button("行为脚本销毁", GUILayout.Width(100))) {
            AssetDatabase.DeleteAsset(PathData.BehaviourPath + className + "Behaviour.cs");//解除 Behaviour
        }

        if (GUILayout.Button("行为配置创建", GUILayout.Width(100))) {
            LFPanel.CreateSetting(PathData.BehaviourSettingPath, className, "", "Behaviour");//创建行为配置
        }
        
        if (GUILayout.Button("行为配置卸载", GUILayout.Width(100))) {
            AssetDatabase.DeleteAsset(PathData.BehaviourSettingPath + "" + className + "Behaviour.asset");
        }
        
        GUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();

        GUILayout.BeginVertical("box", GUILayout.Width(blockWidth), GUILayout.Height(blockHeight));

        #region 条件
        
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("所有条件脚本");
        var fileName = LFPanel.FindAllScriptName(PathData.ConditionPath, new string[] {
            "Base"
        });
        
        settingTextStyle.normal.textColor = Color.green;
        
        foreach (var name in fileName) {
            EditorGUILayout.BeginHorizontal();
            var content = EditorGUIUtility.IconContent("d_ShaderInclude Icon");// 图标1
            GUILayout.Label(content, GUILayout.Height(20), GUILayout.Width(20));
            EditorGUILayout.LabelField(name, settingTextStyle);
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("所有条件配置");
        var ConditionSettingNameArray = LFPanel.FindAllSettingName(PathData.ConditionSettingPath);
        foreach (var tempSetting in ConditionSettingNameArray) {
            EditorGUILayout.BeginHorizontal();
            var content = EditorGUIUtility.IconContent("d_ScriptableObject Icon");// 图标1
            GUILayout.Label(content, GUILayout.Height(20), GUILayout.Width(20));
            EditorGUILayout.LabelField(tempSetting, settingTextStyle);
            EditorGUILayout.EndHorizontal();
        }

        #endregion
        
        GUILayout.EndVertical();


        #region 行为

        GUILayout.BeginVertical("box", GUILayout.Width(blockWidth), GUILayout.Height(blockHeight));
        
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("所有行为脚本");
        var fileNameBehaviour = LFPanel.FindAllScriptName(PathData.BehaviourPath, new string[] {
            "Base"
        });
        
        foreach (var name in fileNameBehaviour) {
            EditorGUILayout.BeginHorizontal();
            var content = EditorGUIUtility.IconContent("d_ShaderInclude Icon");// 图标1
            GUILayout.Label(content, GUILayout.Height(20), GUILayout.Width(20));
            EditorGUILayout.LabelField(name, settingTextStyle);
            EditorGUILayout.EndHorizontal();
        }
        
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box", GUILayout.Width(blockWidth), GUILayout.Height(blockHeight));

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("所有行为配置");
        var BehaviourSettingNameArray = LFPanel.FindAllSettingName(PathData.BehaviourSettingPath);
        foreach (var tempSetting in BehaviourSettingNameArray) {
            EditorGUILayout.BeginHorizontal();
            var content = EditorGUIUtility.IconContent("d_ScriptableObject Icon");// 图标1
            GUILayout.Label(content, GUILayout.Height(20), GUILayout.Width(20));
            EditorGUILayout.LabelField(tempSetting, settingTextStyle);
            EditorGUILayout.EndHorizontal();
        }
        
        GUILayout.EndVertical();

        #endregion
        
        
        GUILayout.BeginVertical("box", GUILayout.Width(blockWidth), GUILayout.Height(blockHeight));

        #region 功能

        EditorGUILayout.Separator();

        functionClassName = EditorGUILayout.TextField("起个名字 (大小严格):", functionClassName, new[] {
            GUILayout.Width(300)
        });

        if (GUILayout.Button("一键创建功能配置", GUILayout.Width(100))) {
            LFPanel.CreateFuncSetting(PathData.FunctionSettingPath, functionClassName, "Function");
        }

        EditorGUILayout.Separator();
        
        // 游戏已安装配置可视化
        var gameSetting = LFPanel.FindAllSetting(PathData.SOGameSettingPath)[0] as SOGameSetting;
        var settingAlreadySetList = gameSetting.funcList.ToArray();
        string[] settingAlreadySetStr = new string[settingAlreadySetList.Length];
        for (var i = 0; i < settingAlreadySetList.Length; i++) {
            settingAlreadySetStr[i] = settingAlreadySetList[i].name;
        }

        // 游戏文件夹下所有的配置
        GameFunc[] settingInFile = LFPanel.FindAllFuncSetting(PathData.FunctionSettingPath);
        string[] settingInFileStr = LFPanel.FindAllSettingName(PathData.FunctionSettingPath);
        List<GameFunc> waitToInGameSetting = new List<GameFunc>();
        EditorGUILayout.LabelField("游戏已安装配置可视化");
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < settingInFileStr.Length; i++) {
            EditorGUILayout.BeginHorizontal();
            var setStr = settingInFileStr[i];
            var set = settingInFile[i];
            var isInGame = LFPanel.HasContainFuncSetting(set, settingAlreadySetList);
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
        if (GUILayout.Button("把行为配置塞进游戏里", GUILayout.Width(100))) {
            if (waitToInGameSetting.Count > 0) {
                for (int i = 0; i < waitToInGameSetting.Count; i++) {
                    var set = waitToInGameSetting[i];
                    gameSetting.funcList.Add(set);
                }
            }
    
            EditorUtility.SetDirty(gameSetting);
            AssetDatabase.SaveAssets();
        }

        #endregion

        GUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }
}