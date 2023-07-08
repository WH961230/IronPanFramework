using UnityEditor;
using UnityEngine;

public class SystemPanel : EditorWindow {
    private string className;
    private GUIStyle settingTextStyle = new GUIStyle();

    private void OnGUI() {
        SystemTool();
    }
    
    public void SystemTool() {
        settingTextStyle.normal.textColor = Color.green;
        className = EditorGUILayout.TextField("起个名字 (大小严格):", className, new[] {
            GUILayout.Width(300)
        });
        
        if (GUILayout.Button("系统破壳诞生", GUILayout.Width(100))) {// 生成系统脚本
            LFPanel.GenerateSingleScript(className, PathData.SystemTemplatePath, PathData.SystemPath, "", "System");
        }
    
        EditorGUILayout.Separator();
        
        EditorGUILayout.LabelField("游戏已安装系统可视化");
        var fileName = LFPanel.FindAllScriptName(PathData.SystemPath, new string[] {
            "Base", "Manager", "IManager"
        });
        foreach (var name in fileName) {
            EditorGUILayout.BeginHorizontal();
            var content = EditorGUIUtility.IconContent("d_ShaderInclude Icon");// 图标1
            GUILayout.Label(content, GUILayout.Height(20), GUILayout.Width(20));
            EditorGUILayout.LabelField(name, settingTextStyle);
            EditorGUILayout.EndHorizontal();
        }
    }
}