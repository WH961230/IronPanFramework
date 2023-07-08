using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal class ToolStyle
    {
        public static GUIStyle TxtTitle { get; }
        public static GUIStyle TxtSubTitle { get; }

        static ToolStyle()
        {
            TxtTitle = new GUIStyle
            {
                fontSize = 24,
                alignment = TextAnchor.MiddleCenter,
                normal = new GUIStyleState
                {
                    textColor = EditorGUIUtility.isProSkin ? new Color(93 / 255f, 172 / 255f, 129 / 255f) : Color.black
                }
            };

            TxtSubTitle = new GUIStyle
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter,
                normal = new GUIStyleState
                {
                    textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black
                }
            };
        }
    }
}
