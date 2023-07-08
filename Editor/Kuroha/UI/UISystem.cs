using UnityEngine;

namespace Kuroha.UI
{
    public class UISystem
    {
        public UIPanelManager Panel { get; private set; }
        public UIWindowManager Window { get; private set; }

        private Transform PanelUIParent { get; set; }
        private Transform WindowUIParent { get; set; }

        private const string PANEL = "UI_Panel_";
        private const string WINDOW = "UI_Window_";

        public void OnEnable(Transform panelUIParent, Transform windowUIParent)
        {
            PanelUIParent = panelUIParent;
            WindowUIParent = windowUIParent;
        }

        public void Start()
        {
            Panel = new UIPanelManager(this, PanelUIParent, PANEL);
            Window = new UIWindowManager(this, WindowUIParent, WINDOW);
        }

        public void Update()
        {
            Panel?.Update();
            Window?.Update();
        }

        public void FixedUpdate()
        {
            Panel?.FixedUpdate();
            Window?.FixedUpdate();
        }

        public void LateUpdate()
        {
            Panel?.LateUpdate();
            Window?.LateUpdate();
        }
    }
}
