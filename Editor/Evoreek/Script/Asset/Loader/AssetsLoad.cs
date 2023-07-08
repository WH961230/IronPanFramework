using Common;
using UnityEditor;
using UnityEngine;

public partial class AssetsLoad {
    public enum AssetType {
        Prefab,
        So,
        Texture2D,
    }

    private static string GetSuffixByAssetType(AssetType type) {
        switch (type) {
            case AssetType.Prefab:
                return ".prefab";
            case AssetType.So:
                return ".asset";
            case AssetType.Texture2D:
                return ".png";
        }

        return default;
    }

    /// <summary>
    /// 获取资源 - 提供路径，提供标识，提供类型
    /// </summary>
    public static T GetAsset<T>(string path, string sign, AssetType type) where T : Object {
        if (string.IsNullOrEmpty(sign)) {
            Logger.PrintE($" <b>物体的标识为空 咋帮你干事呢？ 路径: {path}</b> ");
            return default;
        }

        T go = null;
        string address = path + sign;
        go = Global.Asset.Load<T>(address);
        if (go == null) {
            Logger.PrintE($" <b>路径：</b>{path} <b>未找到物体：</b>{sign} <b>类型：</b>{type.ToString()} ");
            return default;
        }

        return go;
    }
}