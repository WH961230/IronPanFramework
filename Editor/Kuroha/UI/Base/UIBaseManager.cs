using UnityEngine;

namespace Kuroha.UI
{
    public abstract class UIBaseManager
    {
        public UISystem UI { get; }
        protected Transform UIParent { get; }
        protected string AssetPrefix { get; }

        protected UIBaseManager(UISystem uiSystem, Transform position, string assetPrefix)
        {
            UI = uiSystem;
            UIParent = position;
            AssetPrefix = assetPrefix;
        }

        protected void Show(UIBaseController controller)
        {
            controller.ViewCanvasGroup.alpha = 1;
            controller.ViewCanvasGroup.interactable = true;
            controller.ViewCanvasGroup.blocksRaycasts = true;
        }

        protected void Hide(UIBaseController controller)
        {
            controller.ViewCanvasGroup.alpha = 0;
            controller.ViewCanvasGroup.interactable = false;
            controller.ViewCanvasGroup.blocksRaycasts = false;
        }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }

        public virtual void LateUpdate() { }
    }
}
