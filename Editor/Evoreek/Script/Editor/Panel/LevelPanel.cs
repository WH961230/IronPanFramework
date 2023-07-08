using System;
using UnityEditor;

public class LevelPanel : EditorWindow {
    private void OnEnable() {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable() {
        
    }

    private void OnSceneGUI(SceneView obj) {
        
    }

    public void LevelTool() {
        //展示所有的关卡可选物体
        
    }
}
