using System;
using Kuroha.QHierarchy.RunTime;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal static class ConfigUtil
    {
        internal static void CheckConfig<T>(ref T setting, string settingTypeName, string settingFolderName, string settingFileName) where T : ScriptableObject
        {
            if (setting == null)
            {
                var guids = AssetDatabase.FindAssets($"t:{settingTypeName}");

                if (guids.Length > 1)
                {
                    QHierarchyDebugger.Log($"项目中存在 {guids.Length} 个配置文件!", QHierarchyDebugger.红);

                    foreach (var guid in guids)
                    {
                        var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        AssetDatabase.DeleteAsset(assetPath);
                        QHierarchyDebugger.Log($"已删除配置文件!\n{assetPath}", QHierarchyDebugger.红);
                    }

                    setting = CreateConfig<T>(settingFolderName, settingFileName);
                }
                else if (guids.Length == 1)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    setting = AssetDatabase.LoadAssetAtPath<T>(path);
                    if (setting == null)
                    {
                        QHierarchyDebugger.LogError($"配置文件版本已过期! 已删除!\n{settingFileName} ", QHierarchyDebugger.红);
                        setting = CreateConfig<T>(settingFolderName, settingFileName);
                    }
                    else
                    {
                        QHierarchyDebugger.Log($"成功读取到配置文件!\n{path}", QHierarchyDebugger.浅绿, setting);
                    }
                }
                else
                {
                    setting = CreateConfig<T>(settingFolderName, settingFileName);
                }
            }
        }

        private static T CreateConfig<T>(string settingFolderName, string settingFileName) where T : ScriptableObject
        {
            var setting = ScriptableObject.CreateInstance<T>();

            var path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(setting));
            var lastFolderIndex = path.LastIndexOf("/", StringComparison.Ordinal);
            var folder = $"{path[..lastFolderIndex]}/{settingFolderName}";

            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }

            var settingsPath = $"{folder}/{settingFileName}.asset";
            AssetDatabase.CreateAsset(setting, settingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            QHierarchyDebugger.Log($"成功新建配置文件!\n{settingsPath}", QHierarchyDebugger.浅绿, setting);

            return setting;
        }
    }
}
