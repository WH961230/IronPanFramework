using UnityEngine;

namespace Kuroha.UI
{
    public abstract class UIBaseController
    {
        public CanvasGroup ViewCanvasGroup { get; private set; }

        protected UIBaseController(in UIWindowView view)
        {
            SetCanvasGroup(view);
        }

        protected UIBaseController(in UIPanelView view)
        {
            SetCanvasGroup(view);
        }

        private void SetCanvasGroup(in MonoBehaviour view)
        {
            if (!view.gameObject.TryGetComponent<CanvasGroup>(out var canvas))
            {
                canvas = view.gameObject.AddComponent<CanvasGroup>();
            }
            
            ViewCanvasGroup = canvas;
        }
        
        public virtual void Update() { }
        
        public virtual void FixedUpdate() { }
        
        public virtual void LateUpdate() { }
    }
}
