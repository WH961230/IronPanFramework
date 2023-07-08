namespace Kuroha.UI
{
    public class UIDebug
    {
        public const string 浅绿 = "#58D2BC";

        public const string 红 = "#CB1B45";

        public const string 山吹 = "#FFB11B";

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