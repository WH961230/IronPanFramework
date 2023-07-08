using System;

namespace Kuroha.UI
{
    /// <summary>
    /// 抽象类, 所有 Controller 层的父类
    /// </summary>
    public abstract class UIPanelController : UIBaseController
    {
        /// <summary>
        /// Manager 层
        /// </summary>
        private UIPanelManager Global { get; }

        /// <summary>
        /// UI 名 (同时作为 UI 的唯一标识)
        /// </summary>
        internal Type UIType { get; }

        /// <summary>
        /// 构造器
        /// </summary>
        protected UIPanelController(in UIBaseManager manager, in UIPanelView view, in Type uiType) : base(in view)
        {
            Global = manager as UIPanelManager;
            UIType = uiType;
        }

        /// <summary>
        /// 打开 UIPanel
        /// </summary>
        protected T OpenPanel<T>() where T : UIPanelController
        {
            return Global.Open<T>();
        }

        /// <summary>
        /// 打开 UIWindow
        /// </summary>
        protected T1 OpenWindow<T1, T2>(T2 displayContent) where T1 : UIWindowController
        {
            return Global.UI.Window.Open<T1, T2>(displayContent);
        }

        /// <summary>
        /// 关闭 UI
        /// </summary>
        protected void Close()
        {
            Global.Close();
        }

        /// <summary>
        /// 初始化 (第一次打开)
        /// </summary>
        public abstract void FirstOpen();

        /// <summary>
        /// 重置 (后续打开)
        /// </summary>
        public abstract void ReOpen();
    }
}
