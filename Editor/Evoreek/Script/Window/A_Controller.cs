using System;
using Kuroha.UI;
using UnityEngine;

public class A_Controller : UIPanelController {
    private A_View view;
    public A_Controller(in UIBaseManager manager, in UIPanelView view, in Type uiType) : base(in manager, in view, in uiType) {
        this.view = view as A_View;
    }

    public override void FirstOpen() {
    }

    public override void ReOpen() {
    }
}