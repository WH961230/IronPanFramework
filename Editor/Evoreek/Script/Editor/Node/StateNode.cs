using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StateNode : BaseNode {
    public bool collapse;
    public State currentState;
    public State previoursState;
    public float collapseHeight = 100;
    public float defaultHeight = 300;
    public List<BaseNode> dependences = new List<BaseNode>();
    public override void DrawWindow(){
        if (currentState == null) {
            EditorGUILayout.LabelField("Add state to modify");
        } else {
            windowRect.height = collapse ? defaultHeight : collapseHeight;
        }

        collapse = EditorGUILayout.Toggle(" ", collapse);
        currentState = (State)EditorGUILayout.ObjectField(currentState, typeof(State), false);
        if (previoursState != currentState) {
            previoursState = currentState;
            if (currentState != null) {
                ClearReferences();
                var t = currentState.transitions;
                for (int i = 0; i < t.Count; i++) {
                    var tt = t[i];
                    dependences.Add(BehaviorEditor.AddTransitionNode(i, tt, this));
                }
            }
        }
    }

    public override void DrawCurve(){
        
    }

    public Transition AddTransition() {
        return currentState.AddTransition();
    }

    // 清除过渡
    public void ClearTransition() {
        if (currentState != null) {
            currentState.ClearTransition();
        }
    }

    // 清除指定的参考
    public void ClearTransition(TransitionNode t) {
        if (dependences.Contains(t)) {
            currentState.ClearTransition(t.targetTransition);
            dependences.Remove(t);
        }
    }

    // 清除参考
    public void ClearReferences() {
        // 清除窗口
        BehaviorEditor.ClearWindowsFromList(dependences);
        dependences.Clear();
    }
}