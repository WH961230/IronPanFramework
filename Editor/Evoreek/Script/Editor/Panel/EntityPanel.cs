using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EntityPanel : EditorWindow {
    //规格
    private float blockHeight = 700;
    private float blockWidth = 300;
    
    private string className; // 类名
    private bool haveData; // 是否有数据
    private bool haveEntity = true; // 是否有实体
    private bool haveGameObj; // 是否有游戏物体
    private bool haveWindow; // 是否有窗口
    private bool haveComponent; // 是否有组件
    private bool haveSOSetting; // 是否有配置
    private GUIStyle settingTextStyle = new GUIStyle();
    
    private bool foldOut = true;

    public void EntityPanelGUI() {
        className = EditorGUILayout.TextField("起个名字 (大小严格):", className, new[] {
            GUILayout.Width(300)
        });
        
        // 自动创建包含部分
        haveWindow = EditorGUILayout.Toggle("生成实体窗口", haveWindow);
        haveSOSetting = EditorGUILayout.Toggle("生成实体配置", haveSOSetting);
        haveData = EditorGUILayout.Toggle("生成实体数据", haveData);
        haveComponent = EditorGUILayout.Toggle("生成实体组件", haveComponent);
        haveGameObj = EditorGUILayout.Toggle("生成实体物体", haveGameObj);
        haveEntity = EditorGUILayout.Toggle("生成实体 (默认生成)", haveEntity);
        
        if (GUILayout.Button("实体破壳诞生", GUILayout.Width(100))) {// 创建
            LFPanel.GenerateScript(className, haveData, haveEntity, haveGameObj, haveWindow, haveComponent, haveSOSetting);
        }
        
        RemoveEntity();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("游戏已安装实体可视化");
        
        settingTextStyle.fontSize = 12;// 设定字体样式
        settingTextStyle.normal.textColor = Color.green;

        var fileName = LFPanel.FindAllScriptName(PathData.EntityPath, new string[] {
            "Base"
        });

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical("box", GUILayout.Width(blockWidth), GUILayout.Height(blockHeight));

        foldOut = EditorGUILayout.Foldout(foldOut, "查看 Entity", true);
        if (foldOut) {
            foreach (var name in fileName) {
                EditorGUILayout.BeginHorizontal();
                var content = EditorGUIUtility.IconContent("d_ShaderInclude Icon");// 图标1
                GUILayout.Label(content, GUILayout.Height(20), GUILayout.Width(20));
                EditorGUILayout.LabelField(name, settingTextStyle);
                EditorGUILayout.EndHorizontal();
            }
        }

        fileName = LFPanel.FindAllScriptName(PathData.GameObjPath, new string[] {
            "Base"
        });
        
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box", GUILayout.Width(blockWidth), GUILayout.Height(blockHeight));

        foldOut = EditorGUILayout.Foldout(foldOut, "查看 GameObj", true);
        if (foldOut) {
            foreach (var name in fileName) {
                EditorGUILayout.BeginHorizontal();
                var content = EditorGUIUtility.IconContent("d_ShaderInclude Icon");// 图标1
                GUILayout.Label(content, GUILayout.Height(20), GUILayout.Width(20));
                EditorGUILayout.LabelField(name, settingTextStyle);
                EditorGUILayout.EndHorizontal();
            }
        }
        
        EditorGUILayout.EndVertical();


        fileName = LFPanel.FindAllScriptName(PathData.WindowPath, new string[] {
            "Base"
        });
        
        EditorGUILayout.BeginVertical("box", GUILayout.Width(blockWidth), GUILayout.Height(blockHeight));


        foldOut = EditorGUILayout.Foldout(foldOut, "查看 Window", true);
        if (foldOut) {
            foreach (var name in fileName) {
                EditorGUILayout.BeginHorizontal();
                var content = EditorGUIUtility.IconContent("d_ShaderInclude Icon");// 图标1
                GUILayout.Label(content, GUILayout.Height(20), GUILayout.Width(20));
                EditorGUILayout.LabelField(name, settingTextStyle);
                EditorGUILayout.EndHorizontal();
            }
        }
        
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void RemoveEntity() {
        if (GUILayout.Button("实体解除", GUILayout.Width(100))) {
            // 解除 Entity
            AssetDatabase.DeleteAsset(PathData.EntityPath + className + "Entity.cs");
            // 解除 GameObj（如果有）
            AssetDatabase.DeleteAsset(PathData.GameObjPath + className + "GameObj.cs");
            // 解除 Window（如果有）
            AssetDatabase.DeleteAsset(PathData.WindowPath + className + "Window.cs");
            // 解除 Data
            AssetDatabase.DeleteAsset(PathData.DataPath + className + "Data.cs");
            // 解除 Component
            AssetDatabase.DeleteAsset(PathData.ComponentPath + className + "Component.cs");
            // 解除 系统
            AssetDatabase.DeleteAsset(PathData.SystemPath + className + "System.cs");
            // 解除 配置
            AssetDatabase.DeleteAsset(PathData.SettingPath + "SO" + className + "Setting.cs");
            // 删除预制文件
            Directory.Delete(PathData.PrefabPath + className, true);
            // 刷新配置实体 恢复 GameSetting Missing
            AssetDatabase.DeleteAsset(PathData.SOSystemSettingPath + "SO" + className + "Setting.asset");
            var gameSetting = LFPanel.FindAllSetting(PathData.SOGameSettingPath)[0] as SOGameSetting;
            for (int i = 0; i < gameSetting.settingList.Count; i++) {
                if (gameSetting.settingList[i] == null) {
                    gameSetting.settingList.RemoveAt(i);
                }
            }
        }
    }
}