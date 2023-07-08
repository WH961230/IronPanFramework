using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BehaviorEditor : EditorWindow {
    #region Variables

    static List<BaseNode> windows = new List<BaseNode>(); // Node 窗口
    Vector3 mousePosition; // 鼠标位置
    bool makeTransition; // 过渡中
    bool clickedOnAwindow; // 点击
    BaseNode selectedNode;

    public enum UserActions {
        addStateNode, // 添加状态结点
        addTransitionNode, // 添加过渡结点
        deleteNode, // 删除结点
        commentNode, // 评论结点
    }

    #endregion

    #region Init

    [MenuItem("Behavior/Editor")]
    public static void ShowEditor() {
        BehaviorEditor editor = GetWindow<BehaviorEditor>();
        editor.minSize = new Vector2(800, 600);
    }

    #endregion

    #region GUI Methods

    public void OnGUI() {
        Event e = Event.current;
        mousePosition = e.mousePosition;
        UserInput(e); // 角色输入
        DrawWindows(); // 绘制窗口
    }

    private void OnEnable() {
        windows.Clear();
    }

    void DrawWindows() { // 绘制所有窗口
        BeginWindows();
        foreach (BaseNode n in windows) { // 遍历窗口
            n.DrawCurve();
        }

        for (int i = 0; i < windows.Count; i++) { // 遍历窗口
            BaseNode window = windows[i];
            window.windowRect = GUI.Window(i, window.windowRect, DrawNodeWindow, window.windowTitle);
        }

        EndWindows();
    }

    void DrawNodeWindow(int id) {
        windows[id].DrawWindow(); // 绘制窗口
        GUI.DragWindow();
    }

    void UserInput(Event e) {
        if (e.button == 1 && !makeTransition) {
            if (e.type == EventType.MouseDown) {
                RightClick(e); // 右击
            }
        }

        if (e.button == 0 && !makeTransition) {
            if (e.type == EventType.MouseDown) {
                // RightClick(e);
            }
        }
    }

    void RightClick(Event e) {
        selectedNode = null;
        // 判断鼠标正在选择哪个结点？
        for (int i = 0; i < windows.Count; i++) {
            // 窗口包含鼠标位置
            if (windows[i].windowRect.Contains(e.mousePosition)) {
                // 按下按钮在窗口
                clickedOnAwindow = true;
                // 选择结点
                selectedNode = windows[i];
                break;
            }
        }

        // 如果按下窗口修改窗口结点
        if (clickedOnAwindow) {
            ModifyNode(e);
        } else {
            // 未按下窗口添加新结点
            AddNewNode(e);
        }
    }

    void AddNewNode(Event e) {
        // 窗口 menu
        GenericMenu menu = new GenericMenu(); 
        menu.AddItem(new GUIContent("Add state"), false, ContextCallback, UserActions.addStateNode);
        menu.AddItem(new GUIContent("Add comment"), false, ContextCallback, UserActions.commentNode);
        menu.ShowAsContext();
        e.Use();
    }

    void ModifyNode(Event e) {
        GenericMenu menu = new GenericMenu();
        // 修改状态结点
        if (selectedNode is StateNode stateNode) {
            // 当前状态不为空
            if (stateNode.currentState != null) {
                // 添加选项增加过渡结点
                menu.AddItem(new GUIContent("Add transition"), false, ContextCallback, UserActions.addTransitionNode);
            } else {
                // 隐藏过渡结点
                menu.AddDisabledItem(new GUIContent("Add Transition"));
            }
            // 删除
            menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.deleteNode);
        }

        if (selectedNode is CommentNode commentNode) {
            // 删除
            menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.deleteNode);
        }

        if (selectedNode is TransitionNode transitionNode) {
            if (transitionNode.targetTransition != null) {
                // 添加选项增加过渡结点
                menu.AddItem(new GUIContent("Add transition"), false, ContextCallback, UserActions.addTransitionNode);
            } else {
                // 隐藏过渡结点
                menu.AddDisabledItem(new GUIContent("Add Transition"));
            }
            // 删除
            menu.AddItem(new GUIContent("Delete"), false, ContextCallback, UserActions.deleteNode);
        }

        menu.ShowAsContext();
        e.Use();
    }

    void ContextCallback(object o) {
        UserActions a = (UserActions)o;
        switch (a) {
            case UserActions.addStateNode:
                StateNode stateNode = CreateInstance<StateNode>();
                stateNode.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 300);
                stateNode.windowTitle = "State";
                windows.Add(stateNode);
                clickedOnAwindow = false;
                break;
            case UserActions.addTransitionNode:
                if (selectedNode is StateNode from) {
                    from.dependences.Add(AddTransitionNode(from.currentState.transitions.Count, from.AddTransition(), from));
                }
                clickedOnAwindow = false;
                break;
            case UserActions.commentNode:
                CommentNode commentNode = CreateInstance<CommentNode>();
                commentNode.windowRect = new Rect(mousePosition.x, mousePosition.y, 200, 100);
                commentNode.windowTitle = "Comment";
                windows.Add(commentNode);
                clickedOnAwindow = false;
                break;
            case UserActions.deleteNode:
                if (selectedNode is StateNode s) {
                    s.ClearTransition();
                    s.ClearReferences();
                    windows.Remove(s);
                }

                if (selectedNode is CommentNode c) {
                    windows.Remove(c);
                }

                if (selectedNode is TransitionNode t) {
                    t.enterState.ClearTransition(t);
                    windows.Remove(t);
                }
                clickedOnAwindow = false;
                break;
            default:
                break;
        }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// 添加过渡结点
    /// </summary>
    public static TransitionNode AddTransitionNode(int index, Transition transition, StateNode from) {
        Rect fromRect = from.windowRect;

        fromRect.x += 50; // 右边 50
        fromRect.y -= 100;
        if (from.currentState != null) {
            fromRect.y += (index * 100);
        }

        TransitionNode transitionNode = CreateInstance<TransitionNode>();
        transitionNode.Init(from, transition);
        // 创建条件结点
        transitionNode.windowRect = new Rect(fromRect.x + 300, fromRect.y + 100, 200, 80);
        transitionNode.windowTitle = "Condition Check";
        windows.Add(transitionNode);

        return transitionNode;
    }
    
    /// <summary>
    /// 绘制节点曲线
    /// </summary>
    public static void DrawNodeCurve(Rect start, Rect end, bool left, Color curveColor) {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + (start.height * .5f), 0);
        Vector3 endPos = new Vector3(end.x, end.y, 0);
        Vector3 startTran = startPos + Vector3.right * 50;
        Vector3 endTran = endPos + Vector3.left * 50;
        Handles.DrawBezier(startPos, endPos, startTran, endTran, curveColor, null, 1);
    }

    public static void ClearWindowsFromList(List<BaseNode> l) {
        for (int i = 0; i < l.Count; i++) {
            var ll = l[i];
            if (windows.Contains(ll)) {
                windows.Remove(ll);
            }
        }
    }

    #endregion
}