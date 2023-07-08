using System;
using System.IO;

namespace Kuroha.UtilitiesCollection
{
    internal static class ToolExtension
    {
        public static string ToAssetPath(this string fullPath)
        {
            string assetPath = null;

            if (!string.IsNullOrEmpty(fullPath))
            {
                var assetsIndex = fullPath.IndexOf("Assets", StringComparison.OrdinalIgnoreCase);
                if (assetsIndex > 0)
                {
                    assetPath = fullPath[assetsIndex..];
                }
            }

            if (string.IsNullOrEmpty(assetPath))
            {
                ToolDebug.LogError($"绝对路径非法! {fullPath}", ToolDebug.红);
            }

            return assetPath?.Replace('\\', '/');
        }

        public static string ToFullPath(this string assetPath)
        {
            string fullPath = null;

            if (!string.IsNullOrEmpty(assetPath) && assetPath.StartsWith("Assets"))
            {
                fullPath = Path.GetFullPath(assetPath);
            }

            if (string.IsNullOrEmpty(fullPath))
            {
                ToolDebug.LogError($"相对路径非法! {assetPath}", ToolDebug.红);
            }

            return fullPath?.Replace('\\', '/');
        }
    }
}
