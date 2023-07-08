namespace Kuroha.UI
{
    /// <summary>
    /// 抽象类, 所有 Controller 层的父类
    /// </summary>
    public abstract class UIWindowController : UIBaseController
    {
        /// <summary>
        /// Manager 层
        /// </summary>
        private UIWindowManager Global { get; }

        /// <summary>
        /// 构造器
        /// </summary>
        protected UIWindowController(in UIBaseManager manager, in UIWindowView view) : base(in view)
        {
            Global = manager as UIWindowManager;
        }

        /// <summary>
        /// 打开 UIWindow
        /// </summary>
        protected T1 OpenWindow<T1, T2>(T2 displayContent) where T1 : UIWindowController
        {
            return Global.Open<T1, T2>(displayContent);
        }

        /// <summary>
        /// 打开 UIPanel
        /// </summary>
        protected T OpenPanel<T>() where T : UIPanelController
        {
            return Global.UI.Panel.Open<T>();
        }

        /// <summary>
        /// 关闭 UI
        /// </summary>
        protected void Close<T>() where T : UIWindowController
        {
            Global.Close<T>();
        }

        /// <summary>
        /// 初始化窗口 (第一次打开)
        /// </summary>
        public abstract void FirstOpen<T>(T displayContent);

        /// <summary>
        /// 重置 (后续打开)
        /// </summary>
        public abstract void ReOpen<T>(T displayContent);
    }
}
