using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneEditorManager))]
public class SceneEditorManagerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        SceneEditorManager tool = (SceneEditorManager) target;

        GUILayout.Label("地形相关");
        if (GUILayout.Button("加载所有地形")) {
            tool.LoadTerrain();
        }

        if (GUILayout.Button("销毁地形")) {
            tool.DestroyTerrain();
        }

        GUILayout.Label("场景相关");
        if (GUILayout.Button("加载所有的场景物体")) {
            tool.LoadAllSceneObject();
        }

        if (GUILayout.Button("销毁所有的场景物体")) {
            tool.DestroyAllSceneObject();
        }

        GUILayout.Label("场景调试点位物体相关");
        if (GUILayout.Button("加载所有的场景调试点位物体")) {
            tool.LoadAllSceneObjectDebug();
        }
        
        if (GUILayout.Button("销毁所有的场景调试物体")) {
            tool.DestroyAllSceneObjectDebug();
        }

        if (GUILayout.Button("保存所有的场景调试点位信息到配置")) {
            tool.SaveAllSceneObjDebug();
        }
    }
}