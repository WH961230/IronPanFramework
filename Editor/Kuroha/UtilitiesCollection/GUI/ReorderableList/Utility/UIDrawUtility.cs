using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    internal static class UIDrawUtility
    {
        public static readonly Func<Rect> visibleRect;
        public static readonly Action<string> focusTextInControl;
        private static readonly Color separatorColor;
        private static readonly GUIStyle separatorStyle;
        private static readonly GUIStyle tempStyle = new GUIStyle();
        private static readonly GUIContent tempIconContent = new GUIContent();
        private static readonly int iconButtonHint = "_ReorderableIconButton_".GetHashCode();

        static UIDrawUtility()
        {
            var tyGUIClip = Type.GetType("UnityEngine.GUIClip,UnityEngine");
            if (tyGUIClip != null)
            {
                var piVisibleRect = tyGUIClip.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (piVisibleRect != null)
                {
                    var getMethod = piVisibleRect.GetGetMethod(true) ?? piVisibleRect.GetGetMethod(false);
                    visibleRect = (Func<Rect>) Delegate.CreateDelegate(typeof(Func<Rect>), getMethod);
                }
            }

            var miFocusTextInControl = typeof(EditorGUI).GetMethod("FocusTextInControl", BindingFlags.Static | BindingFlags.Public);

            if (miFocusTextInControl == null)
            {
                miFocusTextInControl = typeof(UnityEngine.GUI).GetMethod("FocusControl", BindingFlags.Static | BindingFlags.Public);
            }

            if (miFocusTextInControl != null)
            {
                focusTextInControl = (Action<string>) Delegate.CreateDelegate(typeof(Action<string>), miFocusTextInControl);
            }

            separatorColor = EditorGUIUtility.isProSkin ? new Color(0.11f, 0.11f, 0.11f) : new Color(0.5f, 0.5f, 0.5f);

            separatorStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.whiteTexture
                },
                stretchWidth = true
            };
        }

        public static void DrawTexture(Rect position, Texture2D texture)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            tempStyle.normal.background = texture;
            tempStyle.Draw(position, GUIContent.none, false, false, false, false);
        }

        public static bool IconButton(Rect position, bool visible, Texture2D iconNormal, Texture2D iconActive, GUIStyle style)
        {
            var controlID = GUIUtility.GetControlID(iconButtonHint, FocusType.Passive);
            var result = false;

            position.height += 1;

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    // Do not allow button to be pressed using right mouse button since context menu should be shown instead!
                    if (UnityEngine.GUI.enabled && Event.current.button != 1 && position.Contains(Event.current.mousePosition))
                    {
                        GUIUtility.hotControl = controlID;
                        GUIUtility.keyboardControl = 0;
                        Event.current.Use();
                    }

                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        Event.current.Use();
                    }

                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        result = position.Contains(Event.current.mousePosition);
                        Event.current.Use();
                    }

                    break;

                case EventType.Repaint:
                    if (visible)
                    {
                        var isActive = GUIUtility.hotControl == controlID && position.Contains(Event.current.mousePosition);
                        tempIconContent.image = isActive ? iconActive : iconNormal;
                        position.height -= 1;
                        style.Draw(position, tempIconContent, isActive, isActive, false, false);
                    }

                    break;
            }

            return result;
        }

        public static void Separator(Rect position, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var restoreColor = UnityEngine.GUI.color;
                UnityEngine.GUI.color = color;
                separatorStyle.Draw(position, false, false, false, false);
                UnityEngine.GUI.color = restoreColor;
            }
        }

        public static void Separator(Rect position)
        {
            Separator(position, separatorColor);
        }
    }
}
