using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class LFPanel : EditorWindow {
    private bool isInit;
    private bool isOpenEntityTool;
    private bool isOpenSettingTool;
    private bool isOpenSystemTool;
    private bool isOpenFunctionTool;
    private bool isOpenPrefabEntityTool;

    private static int currentProccess;
    private static int totalCount;

    private float btnHeight = 50;

    public Dictionary<Type, EditorWindow> editorWindows = new Dictionary<Type, EditorWindow>();

    private int index;
    private string[] values = {
        "实体工具 Entity", "配置 Setting", "系统 System", "功能 Function", "预制体 Prefab", "关卡 Level"
    };


    [MenuItem("点这里/创建/自动生成模板类 _F1")]
    public static void GenerateScriptWindow() {
        LFPanel window = (LFPanel) GetWindow(typeof(LFPanel), false, "LazyFramework", true);
        window.Show();
    }

    private void OnGUI() {
        index = GUILayout.Toolbar(index, values, GUILayout.Height(30));
        switch (index) {
            case 0:
                if (editorWindows.TryGetValue(typeof(EntityPanel), out EditorWindow ret)) {
                    if (ret is EntityPanel ep) {
                        ep.EntityPanelGUI();
                    }
                } else {
                    editorWindows.Add(typeof(EntityPanel), CreateInstance<EntityPanel>());
                }
                break;
            case 1:
                if (editorWindows.TryGetValue(typeof(SettingPanel), out EditorWindow ret1)) {
                    if (ret1 is SettingPanel sp) {
                        sp.SettingTool();
                    }
                } else {
                    editorWindows.Add(typeof(SettingPanel), CreateInstance<SettingPanel>());
                }
                break;
            case 2:
                if (editorWindows.TryGetValue(typeof(SystemPanel), out EditorWindow ret2)) {
                    if (ret2 is SystemPanel sp) {
                        sp.SystemTool();
                    }
                } else {
                    editorWindows.Add(typeof(SystemPanel), CreateInstance<SystemPanel>());
                }
                break;
            case 3:
                if (editorWindows.TryGetValue(typeof(FunctionPanel), out EditorWindow ret3)) {
                    if (ret3 is FunctionPanel sp) {
                        sp.FunctionTool();
                    }
                } else {
                    editorWindows.Add(typeof(FunctionPanel), CreateInstance<FunctionPanel>());
                }
                break;
            case 4:
                if (editorWindows.TryGetValue(typeof(PrefabPanel), out EditorWindow ret4)) {
                    if (ret4 is PrefabPanel sp) {
                        sp.PrefabTool();
                    }
                } else {
                    editorWindows.Add(typeof(PrefabPanel), CreateInstance<PrefabPanel>());
                }
                break;
            case 5:
                if (editorWindows.TryGetValue(typeof(LevelPanel), out EditorWindow ret5)) {
                    if (ret5 is LevelPanel lp) {
                        lp.LevelTool();
                    }
                } else {
                    editorWindows.Add(typeof(LevelPanel), CreateInstance<LevelPanel>());
                }
                break;
        }
    }

    private void EntityToolGUI() {
        if (GUILayout.Button("实体工具", new GUILayoutOption[] {GUILayout.Height(btnHeight)})) {
            EntityPanel window = (EntityPanel) GetWindow(typeof(EntityPanel), false, "EntityPanel", true);
            window.Show();
        }
    }

    private void SettingToolGUI() {
        if (GUILayout.Button("配置工具", new GUILayoutOption[] {GUILayout.Height(btnHeight)})) {
            SettingPanel window = (SettingPanel) GetWindow(typeof(SettingPanel), false, "SettingPanel", true);
            window.Show();
        }
    }

    private void SystemToolGUI() {
        if (GUILayout.Button("系统工具", new GUILayoutOption[] {GUILayout.Height(btnHeight)})) {
            SystemPanel window = (SystemPanel) GetWindow(typeof(SystemPanel), false, "SystemPanel", true);
            window.Show();
        }
    }

    private void PrefabToolGUI() {
        if (GUILayout.Button("预制体工具", new GUILayoutOption[] {GUILayout.Height(btnHeight)})) {
            PrefabPanel window = (PrefabPanel) GetWindow(typeof(PrefabPanel), false, "PrefabPanel", true);
            window.Show();
        }
    }

    private void FunctionToolGUI() {
        if (GUILayout.Button("行为工具", new GUILayoutOption[] {GUILayout.Height(btnHeight)})) {
            FunctionPanel window = (FunctionPanel) GetWindow(typeof(FunctionPanel), false, "FunctionPanel", true);
            window.Show();
        }
    }

    /// <summary>
    /// 创建预制体
    /// </summary>
    /// <param name="className"></param>
    /// <param name="prefabName"></param>
    public static void CreatePrefabFile(string className, string prefabName) {
        AssetDatabase.CreateFolder("Assets/Assets/Bundles/Prefabs", className);
        var createGO = (GameObject) System.Activator.CreateInstance(typeof(GameObject));
        PrefabUtility.SaveAsPrefabAsset(createGO, PathData.PrefabPath + className + "/" + prefabName + ".prefab");
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 找到路径下的所有文件名
    /// </summary>
    /// <param name="path"></param>
    /// <param name="filterFile"></param>
    /// <returns></returns>
    public static string[] FindAllScriptName(string path, string[] filterFile) {
        var assetsId = AssetDatabase.FindAssets("t:script", new[] {
            path
        });
        List<string> retStrArray = new List<string>();
        for (int i = 0; i < assetsId.Length; i++) {
            var id = assetsId[i];
            var assetPath = AssetDatabase.GUIDToAssetPath(id);
            var cutAssetPath = assetPath.Split('/');
            string entityName = "";
            for (int j = 0; j < cutAssetPath.Length; j++) {
                if (j == cutAssetPath.Length - 1) {
                    if (!filterFile.Contains(cutAssetPath[j - 1])) {
                        retStrArray.Add(cutAssetPath[j]);
                    }
                }
            }
        }
    
        return retStrArray.ToArray();
    }
    
    /// <summary>
    /// 找到所有配置
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static GameSetting[] FindAllSetting(string path) {
        // 遍历配置文件夹 获取所有的系统设置
        var assetsId = AssetDatabase.FindAssets("t:scriptableobject", new[] {
            path
        });
        GameSetting[] retSetting = new GameSetting[assetsId.Length];
        for (int i = 0; i < assetsId.Length; i++) {
            var id = assetsId[i];
            GameSetting setting = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(id)) as GameSetting;
            retSetting[i] = setting;
        }

        return retSetting;
    }
    
    /// <summary>
    /// 找到所有配置
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static GameFunc[] FindAllFuncSetting(string path) {
        // 遍历配置文件夹 获取所有的系统设置
        var assetsId = AssetDatabase.FindAssets("t:scriptableobject", new[] {
            path
        });
        GameFunc[] retSetting = new GameFunc[assetsId.Length];
        for (int i = 0; i < assetsId.Length; i++) {
            var id = assetsId[i];
            retSetting[i] = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(id)) as GameFunc;
        }

        return retSetting;
    }

    /// <summary>
    /// 找到所有配置名
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[] FindAllSettingName(string path) {
        GameSetting[] settings = FindAllSetting(path);
        string[] settingNameArray = new string[settings.Length];
        for (var i = 0; i < settings.Length; i++) {
            GameSetting setting = settings[i];
            settingNameArray[i] = setting.name;
        }
    
        return settingNameArray;
    }

    /// <summary>
    /// 生成脚本
    /// </summary>
    /// <param name="className"></param>
    /// <param name="haveData"></param>
    /// <param name="haveEntity"></param>
    /// <param name="haveGameObj"></param>
    /// <param name="haveWindow"></param>
    /// <param name="haveComponent"></param>
    /// <param name="haveSOSetting"></param>
    public static void GenerateScript(string className, bool haveData, bool haveEntity, bool haveGameObj, bool haveWindow, bool haveComponent, bool haveSOSetting) {
        if (string.IsNullOrEmpty(className)) {
            Debug.LogError("class Name is Null");
            return;
        }

        totalCount += haveData ? 1 : 0;
        totalCount += haveEntity ? 1 : 0;
        totalCount += haveGameObj ? 1 : 0;
        totalCount += haveWindow ? 1 : 0;
        totalCount += haveComponent ? 1 : 0;
        totalCount += haveSOSetting ? 1 : 0;
        if (haveData) {
            LFPanel.CreateStript(PathData.DataTemplatePath, PathData.DataPath, className, "", "Data");
        }

        if (haveEntity) {
            LFPanel.CreateStript(PathData.EntityTemplatePath, PathData.EntityPath, className, "", "Entity");
        }

        if (haveGameObj) {
            LFPanel.CreateStript(PathData.GameObjTemplatePath, PathData.GameObjPath, className, "", "GameObj");
        }

        if (haveWindow) {
            LFPanel.CreateStript(PathData.WindowTemplatePath, PathData.WindowPath, className, "", "Window");
        }

        if (haveComponent) {
            LFPanel.CreateStript(PathData.ComponentTemplatePath, PathData.ComponentPath, className, "", "Component");
        }

        if (haveSOSetting) {
            LFPanel.CreateStript(PathData.SOSettingTemplatePath, PathData.SettingPath, className, "SO", "Setting");
        }

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("创建成功", $"创建标志：【{className}】", "确定");
    }

    /// <summary>
    /// 生成系统脚本
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool GenerateSingleScript(string name, string inputPath, string outputPath, string front, string end) {
        if (string.IsNullOrEmpty(name)) {
            Debug.LogError("Name is Null");
            return false;
        }
    
        var ret = CreateStript(inputPath, outputPath, name, front, end);
        if (ret) {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("创建成功", $"创建标志：【{name}】", "确定");
        }
    
        return ret;
    }

    /// <summary>
    /// 创建脚本
    /// </summary>
    /// <param name="inputPath"></param>
    /// <param name="outputPath"></param>
    /// <param name="className"></param>
    /// <param name="front"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    private static bool CreateStript(string inputPath, string outputPath, string className, string front, string end) {
        if (inputPath.EndsWith(".txt")) {
            var streamReader = new StreamReader(inputPath);
            var log = streamReader.ReadToEnd();
            streamReader.Close();
            log = Regex.Replace(log, "#ClassName#", className);
            log = Regex.Replace(log, "#ClassParamName#", className.ToLower());
            var createPath = $"{outputPath}{front}{className}{end}.cs";
            var streamWriter = new StreamWriter(createPath, false, new UTF8Encoding(true, false));
            streamWriter.Write(log);
            streamWriter.Close();
            AssetDatabase.ImportAsset(createPath);
            ++currentProccess;
            EditorUtility.DisplayProgressBar("创建中 ...", "", (float) currentProccess / totalCount);
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// 创建配置
    /// </summary>
    /// <param name="outputPath"></param>
    /// <param name="className"></param>
    /// <param name="front"></param>
    /// <param name="end"></param>
    public static void CreateSetting(string outputPath, string className, string front, string end) {
        var fullName = $"{front}{className}{end}";
        var type = typeof(Game).Assembly.GetType(fullName);
        if (type == null) {
            Logger.PrintE($"没有找到配置文件 {fullName}");
            return;
        }

        var soFile = (Object) Activator.CreateInstance(type);
        AssetDatabase.CreateAsset(soFile, outputPath + fullName + ".asset");
    }
    
    /// <summary>
    /// 创建行为配置
    /// </summary>
    /// <param name="outputPath"></param>
    /// <param name="className"></param>
    /// <param name="front"></param>
    /// <param name="end"></param>
    public static void CreateFuncSetting(string outputPath, string assetName, string className) {
        var type = typeof(Game).Assembly.GetType(className);
        if (type == null) {
            Logger.PrintE($"没有找到脚本 {className}");
            return;
        }

        var soFile = (Object) Activator.CreateInstance(type);
        AssetDatabase.CreateAsset(soFile, outputPath + assetName + ".asset");
    }
    
    /// <summary>
    /// 配置是否包含 GameSetting
    /// </summary>
    /// <param name="setting"></param>
    /// <param name="settingInstance"></param>
    /// <returns></returns>
    public static bool HasContainSetting(GameSetting setting, GameSetting[] settingInstance) {
        foreach (var curSet in settingInstance) {
            if (setting.Equals(curSet)) {
                return true;
            }
        }

        return false;
    }
    
    /// <summary>
    /// 配置是否包含 GameFunc
    /// </summary>
    /// <param name="setting"></param>
    /// <param name="settingInstance"></param>
    /// <returns></returns>
    public static bool HasContainFuncSetting(GameFunc setting, GameFunc[] settingInstance) {
        foreach (var curSet in settingInstance) {
            if (setting.Equals(curSet)) {
                return true;
            }
        }

        return false;
    }
}