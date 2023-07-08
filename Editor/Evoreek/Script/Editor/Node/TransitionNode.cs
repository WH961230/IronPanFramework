using System;
using UnityEditor;
using UnityEngine;

public class TransitionNode : BaseNode {
    public Transition targetTransition;
    public StateNode enterState;
    public StateNode targetState;
    public float defalutHeight;

    public void Init(StateNode enterState, Transition transition) {
        targetTransition = transition;
        this.enterState = enterState;
    }

    public override void DrawWindow() {
        if (targetTransition == null) {
            return;
        }
        
        EditorGUILayout.LabelField("");
        targetTransition.condition = (Condition)EditorGUILayout.ObjectField(targetTransition.condition, typeof(Condition), false);
        if (targetTransition.condition == null) {
            EditorGUILayout.LabelField("No Condition");
        } else {
            // targetTransition.disable = EditorGUILayout.Toggle("Disable", targetTransition.disable);
        }
    }

    public override void DrawCurve() {
        if (enterState) {
            Rect rect = windowRect;
            rect.y += windowRect.height * .5f;
            BehaviorEditor.DrawNodeCurve(enterState.windowRect, rect, false, Color.black);
        }
    }
}