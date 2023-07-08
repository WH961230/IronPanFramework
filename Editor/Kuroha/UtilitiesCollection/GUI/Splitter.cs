using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    public enum SplitMode
    {
        Horizontal,
        Vertical
    }

    internal class Splitter
    {
        // 固定
        private readonly SplitMode splitMode;
        private readonly MouseCursor mouseCursor;
        private readonly bool[] freezeList;
        private readonly Action[] onGUIList;
        private readonly List<float> rectSizeList;

        // 实时更新
        private bool[] isDragList;

        // 源
        private List<float> lineList;

        // 推导
        private bool initialized;
        private Rect[] cursorList;

        // 公共
        public Rect[] Rect { get; private set; }

        public Splitter(SplitMode splitMode, bool[] freezeList, List<float> rectSizeList, Action[] onGUIList)
        {
            this.splitMode = splitMode;
            this.freezeList = freezeList;
            this.rectSizeList = rectSizeList;
            this.onGUIList = onGUIList;
            mouseCursor = splitMode == SplitMode.Vertical ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical;
        }

        public void OnGUI(Rect showRect)
        {
            if (!initialized)
            {
                Init();
            }

            if (initialized)
            {
                UpdateLine(showRect);
                DrawLine(showRect);

                UpdateCursor(showRect);
                DrawCursor(showRect);

                UpdateRect(showRect);
                DrawRect();

                CheckDragBegin();
                CheckDragEnd();
            }
        }

        private void Init()
        {
            if (rectSizeList.Count != freezeList.Length + 1)
            {
                ToolDebug.LogError($"sizeList.Count != freezeList.Count + 1 : {rectSizeList.Count} != {freezeList.Length + 1}", ToolDebug.红);
                initialized = false;
            }

            if (rectSizeList.Count != onGUIList.Length)
            {
                ToolDebug.LogError($"sizeList.Count != onGUIList.Count : {rectSizeList.Count} != {onGUIList.Length}", ToolDebug.红);
                initialized = false;
            }

            Rect = new Rect[rectSizeList.Count];
            cursorList = new Rect[freezeList.Length];
            isDragList = new bool[freezeList.Length];
            lineList = new List<float> {rectSizeList[0]};
            for (var index = 1; index < rectSizeList.Count - 1; index++)
            {
                lineList.Add(rectSizeList[index - 1] + rectSizeList[index]);
            }

            initialized = true;
        }

        private void UpdateRect(Rect showRect)
        {
            lineList.Sort();

            Rect[0] = new Rect
            {
                x = showRect.x + 3,
                y = showRect.y + 3,
                width = splitMode == SplitMode.Vertical ? lineList[0] - 6 : showRect.width - 6,
                height = splitMode == SplitMode.Vertical ? showRect.height - 6 : lineList[0] - 6
            };

            for (var index = 1; index < Rect.Length; index++)
            {
                if (index < Rect.Length - 1)
                {
                    Rect[index] = new Rect
                    {
                        x = splitMode == SplitMode.Vertical ? showRect.x + lineList[index - 1] + 3 : showRect.x + 3,
                        y = splitMode == SplitMode.Vertical ? showRect.y + 3 : showRect.y + lineList[index - 1] + 3,
                        width = splitMode == SplitMode.Vertical ? lineList[index] - lineList[index - 1] - 6 : showRect.width - 6,
                        height = splitMode == SplitMode.Vertical ? showRect.height - 6 : lineList[index] - lineList[index - 1] - 6
                    };
                }
                else if (index == Rect.Length - 1)
                {
                    Rect[index] = new Rect
                    {
                        x = splitMode == SplitMode.Vertical ? showRect.x + lineList[index - 1] + 3 : showRect.x + 3,
                        y = splitMode == SplitMode.Vertical ? showRect.y + 3 : showRect.y + lineList[index - 1] + 3,
                        width = splitMode == SplitMode.Vertical ? showRect.width - lineList[index - 1] - 6 : showRect.width - 6,
                        height = splitMode == SplitMode.Vertical ? showRect.height - 6 : showRect.height - lineList[index - 1] - 6
                    };
                }
            }
        }

        private void DrawRect()
        {
            for (var index = 0; index < Rect.Length; index++)
            {
                GUILayout.BeginArea(Rect[index]);
                onGUIList[index]();
                GUILayout.EndArea();
            }
        }

        private void UpdateLine(Rect showRect)
        {
            if (Event.current.type == EventType.MouseDrag)
            {
                for (var index = 0; index < isDragList.Length; index++)
                {
                    if (isDragList[index] && !freezeList[index])
                    {
                        var showRectMax = splitMode == SplitMode.Vertical ? showRect.width : showRect.height;
                        var mouse = splitMode == SplitMode.Vertical ? Event.current.mousePosition.x : Event.current.mousePosition.y;
                        var offset = splitMode == SplitMode.Vertical ? showRect.x : showRect.y;
                        lineList[index] = Mathf.Clamp(mouse - offset, 10, showRectMax - 10);

                        if (EditorWindow.focusedWindow != null)
                        {
                            EditorWindow.focusedWindow.Repaint();
                        }
                    }
                }
            }
        }

        private void DrawLine(Rect showRect)
        {
            foreach (var line in lineList)
            {
                var rect = new Rect(showRect.x + line, showRect.y, 1, showRect.height);
                if (splitMode == SplitMode.Horizontal)
                {
                    rect = new Rect(showRect.x, showRect.y + line, showRect.width, 1);
                }

                EditorGUI.DrawRect(rect, Color.yellow);
            }
        }

        private void UpdateCursor(Rect showRect)
        {
            for (var index = 0; index < cursorList.Length; index++)
            {
                cursorList[index].x = splitMode == SplitMode.Vertical ? showRect.x + lineList[index] - 7 : showRect.x;
                cursorList[index].y = splitMode == SplitMode.Vertical ? showRect.y : showRect.y + lineList[index] - 7;
                cursorList[index].width = splitMode == SplitMode.Vertical ? 15 : showRect.width;
                cursorList[index].height = splitMode == SplitMode.Vertical ? showRect.height : 15;
            }
        }

        private void DrawCursor(Rect showRect)
        {
            foreach (var line in lineList)
            {
                var rect = splitMode == SplitMode.Vertical
                    ? new Rect(showRect.x + line - 7, showRect.y, 15, showRect.height)
                    : new Rect(showRect.x, showRect.y + line - 7, showRect.width, 15);
                EditorGUIUtility.AddCursorRect(rect, mouseCursor);
            }
        }

        private void CheckDragBegin()
        {
            if (Event.current.type == EventType.MouseDown)
            {
                for (var index = 0; index < cursorList.Length; index++)
                {
                    if (cursorList[index].Contains(Event.current.mousePosition))
                    {
                        isDragList[index] = true;
                        break;
                    }
                }
            }
        }

        private void CheckDragEnd()
        {
            if (Event.current.type == EventType.MouseUp || Event.current.type == EventType.Ignore)
            {
                for (var index = 0; index < isDragList.Length; index++)
                {
                    isDragList[index] = false;
                }
            }
        }
    }
}
