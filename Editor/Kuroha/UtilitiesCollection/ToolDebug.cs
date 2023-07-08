namespace Kuroha.UtilitiesCollection
{
    internal static class ToolDebug
    {
        public const string 淡蓝 = "#58B2DC";
        public const string 浅绿 = "#58D2BC";
        public const string 红 = "#CB1B45";
        public const string 橙 = "#FFB11B";

        private static object SetColor(string color, string log)
        {
            return string.IsNullOrEmpty(color) ? log : $"<color={color}>{log}</color>";
        }

        public static void Log(string log, string color = null, UnityEngine.Object target = null)
        {
            UnityEngine.Debug.Log(SetColor(color, log), target);
        }

        public static void LogWarning(string log, string color = null, UnityEngine.Object target = null)
        {
            UnityEngine.Debug.LogWarning(SetColor(color, log), target);
        }

        public static void LogError(string log, string color = null, UnityEngine.Object target = null)
        {
            UnityEngine.Debug.LogError(SetColor(color, log), target);
        }
    }
}
