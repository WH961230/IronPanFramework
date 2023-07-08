using System;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
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
                    ToolDebug.Log($"项目中存在 {guids.Length} 个配置文件!", ToolDebug.红);

                    foreach (var guid in guids)
                    {
                        var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        AssetDatabase.DeleteAsset(assetPath);
                        ToolDebug.Log($"已删除配置文件!\n{assetPath}", ToolDebug.红);
                    }

                    setting = CreateConfig<T>(settingFolderName, settingFileName);
                }
                else if (guids.Length == 1)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    setting = AssetDatabase.LoadAssetAtPath<T>(path);
                    if (setting == null)
                    {
                        ToolDebug.LogError($"配置文件版本已过期! 已删除!\n{settingFileName} ", ToolDebug.红);
                        setting = CreateConfig<T>(settingFolderName, settingFileName);
                    }
                    else
                    {
                        ToolDebug.Log($"成功读取到配置文件!\n{path}", ToolDebug.浅绿, setting);
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

            ToolDebug.Log($"成功新建配置文件!\n{settingsPath}", ToolDebug.浅绿, setting);

            return setting;
        }
    }
}
