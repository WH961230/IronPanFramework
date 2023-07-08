namespace Kuroha.QHierarchy.Editor
{
    /// <summary>
    /// 布局计算状态
    /// </summary>
    internal enum EM_QHierarchyLayoutStatus
    {
        /// <summary>
        /// 布局成功
        /// </summary>
        Success,

        /// <summary>
        /// 部分布局成功
        /// 有的功能需要显示多个图标, 这个标志着只能显示预期全部图标的一部分图标
        /// </summary>
        Partly,

        /// <summary>
        /// 布局失败
        /// </summary>
        Failed
    }
}
