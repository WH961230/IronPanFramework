using UnityEngine;

public class BaseNode : ScriptableObject {
    public Rect windowRect;
    public string windowTitle;
    public virtual void DrawWindow(){

    }

    public virtual void DrawCurve(){

    }
}