using Kuroha.QHierarchy.RunTime;
using UnityEditor;
using UnityEngine;

namespace Kuroha.QHierarchy.Editor
{
    internal class QHierarchyComponentStatic : QHierarchyBaseComponent
    {
        private Color activeColor;
        private Color inactiveColor;

        private GameObject[] gameObjects;

        private Texture2D staticButton;
        private Color32[] staticButtonColors;
        private StaticEditorFlags staticFlags;

        private const int ICON_WIDTH = 11;
        private const int ICON_HEIGHT = 10;

        /// <summary>
        /// 构造方法
        /// </summary>
        internal QHierarchyComponentStatic()
        {
            rect.width = ICON_WIDTH;
            rect.height = ICON_HEIGHT;

            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.StaticShow, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.StaticShowDuringPlayMode, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.AdditionalActiveColor, SettingsChanged);
            QHierarchySettings.Instance().AddEventListener(EM_QHierarchySettings.AdditionalInactiveColor, SettingsChanged);

            SettingsChanged();
        }

        /// <summary>
        /// 更改设置
        /// </summary>
        private void SettingsChanged()
        {
            enabled = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.StaticShow);
            showComponentDuringPlayMode = QHierarchySettings.Instance().Get<bool>(EM_QHierarchySettings.StaticShowDuringPlayMode);
            activeColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.AdditionalActiveColor);
            inactiveColor = QHierarchySettings.Instance().GetColor(EM_QHierarchySettings.AdditionalInactiveColor);
        }

        /// <summary>
        /// 计算布局
        /// </summary>
        internal override EM_QHierarchyLayoutStatus Layout(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Rect selectionRect, ref Rect curRect, float maxWidth)
        {
            if (maxWidth < ICON_WIDTH + COMPONENT_SPACE)
            {
                return EM_QHierarchyLayoutStatus.Failed;
            }

            curRect.x -= ICON_WIDTH + COMPONENT_SPACE;

            rect.x = curRect.x;
            rect.y = curRect.y + (GAME_OBJECT_HEIGHT - ICON_HEIGHT) / 2f;

            return EM_QHierarchyLayoutStatus.Success;
        }

        /// <summary>
        /// 绘制 GUI
        /// </summary>
        internal override void Draw(GameObject gameObjectToDraw, QHierarchyObjectList hierarchyObjectList, Rect selectionRect)
        {
            if (staticButton == null)
            {
                staticButton = new Texture2D(11, 10, TextureFormat.ARGB32, false, true);
                staticButtonColors = new Color32[11 * 10];
            }

            var selfFlag = GameObjectUtility.GetStaticEditorFlags(gameObjectToDraw);

            var state = StaticEditorFlags.ContributeGI;
            DrawQuad(37, 3, 4, (selfFlag & state) > 0);

            state = StaticEditorFlags.OccluderStatic;
            DrawQuad(6, 5, 2, (selfFlag & state) > 0);

            state = StaticEditorFlags.BatchingStatic;
            DrawQuad(33, 3, 4, (selfFlag & state) > 0);

            state = StaticEditorFlags.NavigationStatic;
            DrawQuad(88, 5, 2, (selfFlag & state) > 0);

            state = StaticEditorFlags.OccludeeStatic;
            DrawQuad(0, 5, 2, (selfFlag & state) > 0);

            state = StaticEditorFlags.OffMeshLinkGeneration;
            DrawQuad(94, 5, 2, (selfFlag & state) > 0);

            state = StaticEditorFlags.ReflectionProbeStatic;
            DrawQuad(41, 3, 4, (selfFlag & state) > 0);

            staticButton.SetPixels32(staticButtonColors);
            staticButton.Apply();

            UnityEngine.GUI.DrawTexture(rect, staticButton);
        }

        /// <summary>
        /// 左键单击事件
        /// </summary>
        internal override void EventHandler(GameObject gameObject, QHierarchyObjectList hierarchyObjectList, Event currentEvent)
        {
            if (currentEvent.isMouse && currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && rect.Contains(currentEvent.mousePosition))
            {
                currentEvent.Use();

                staticFlags = GameObjectUtility.GetStaticEditorFlags(gameObject);
                gameObjects = Selection.Contains(gameObject) ? Selection.gameObjects : new[] {gameObject};

                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Nothing"), staticFlags == 0, StaticChangeHandler, 0);
                menu.AddItem(new GUIContent("Everything"), (int) staticFlags == -1, StaticChangeHandler, -1);

                var state = StaticEditorFlags.ContributeGI;
                menu.AddItem(new GUIContent(state.ToString()), (staticFlags & state) > 0, StaticChangeHandler, state);

                state = StaticEditorFlags.OccluderStatic;
                menu.AddItem(new GUIContent(state.ToString()), (staticFlags & state) > 0, StaticChangeHandler, state);

                state = StaticEditorFlags.BatchingStatic;
                menu.AddItem(new GUIContent(state.ToString()), (staticFlags & state) > 0, StaticChangeHandler, state);

                state = StaticEditorFlags.NavigationStatic;
                menu.AddItem(new GUIContent(state.ToString()), (staticFlags & state) > 0, StaticChangeHandler, state);

                state = StaticEditorFlags.OccludeeStatic;
                menu.AddItem(new GUIContent(state.ToString()), (staticFlags & state) > 0, StaticChangeHandler, state);

                state = StaticEditorFlags.OffMeshLinkGeneration;
                menu.AddItem(new GUIContent(state.ToString()), (staticFlags & state) > 0, StaticChangeHandler, state);

                state = StaticEditorFlags.ReflectionProbeStatic;
                menu.AddItem(new GUIContent(state.ToString()), (staticFlags & state) > 0, StaticChangeHandler, state);

                menu.ShowAsContext();
            }
        }

        /// <summary>
        /// 静态标志更改
        /// </summary>
        private void StaticChangeHandler(object result)
        {
            var intResult = (int) result;
            var flagResult = (StaticEditorFlags) result;

            if (intResult != 0 && intResult != -1)
            {
                flagResult = staticFlags ^ flagResult;
            }

            foreach (var gameObject in gameObjects)
            {
                Undo.RecordObject(gameObject, "Change Static Flags");
                GameObjectUtility.SetStaticEditorFlags(gameObject, flagResult);
                EditorUtility.SetDirty(gameObject);
            }
        }

        /// <summary>
        /// 绘制方块
        /// </summary>
        private void DrawQuad(int startPosition, int width, int height, bool isActiveColor)
        {
            var color = (Color32) (isActiveColor ? activeColor : inactiveColor);

            for (var heightCounter = 0; heightCounter < height; heightCounter++)
            {
                for (var widthCounter = 0; widthCounter < width; widthCounter++)
                {
                    var pos = startPosition + widthCounter + heightCounter * 11;

                    staticButtonColors[pos].r = color.r;
                    staticButtonColors[pos].g = color.g;
                    staticButtonColors[pos].b = color.b;
                    staticButtonColors[pos].a = color.a;
                }
            }
        }
    }
}
