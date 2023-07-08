using UnityEditor;

namespace Kuroha.UtilitiesCollection
{
    /// <summary>
    /// 可自动关闭的进度条
    /// </summary>
    internal class ProgressBar
    {
        /// <summary>
        /// 不可中途取消的进度条
        /// </summary>
        /// <param name="title">进度条窗口标题</param>
        /// <param name="info">进度条下方的描述信息</param>
        /// <param name="current">当前进度, 相等时结束</param>
        /// /// <param name="total">总进度, 相等时结束</param>
        public void Show(string title, string info, int current, int total)
        {
            if (current < 0)
            {
                ToolDebug.LogError("进度条的当前进度不允许为负!", ToolDebug.红);
                EditorUtility.ClearProgressBar();
            }

            if (total <= 0)
            {
                ToolDebug.LogError("进度条的总进度必须大于零!", ToolDebug.红);
                EditorUtility.ClearProgressBar();
            }
            else
            {
                if (current >= total)
                {
                    EditorUtility.ClearProgressBar();
                }
                else
                {
                    EditorUtility.DisplayProgressBar(title, info, (float) current / total);
                }
            }
        }

        /// <summary>
        /// 可中途取消的进度条
        /// </summary>
        /// <param name="title">进度条窗口标题</param>
        /// <param name="info">进度条下方的描述信息</param>
        /// <param name="current">当前进度</param>
        /// /// <param name="total">总进度</param>
        public bool ShowCancel(string title, string info, int current, int total)
        {
            var isCancel = false;

            if (current < 0)
            {
                ToolDebug.LogError("进度条的当前进度不允许为负!", ToolDebug.红);
                isCancel = true;
                EditorUtility.ClearProgressBar();
            }

            if (total <= 0)
            {
                ToolDebug.LogError("进度条的总进度必须大于零!", ToolDebug.红);
                isCancel = true;
                EditorUtility.ClearProgressBar();
            }
            else
            {
                if (current >= total)
                {
                    EditorUtility.ClearProgressBar();
                }
                else
                {
                    isCancel = EditorUtility.DisplayCancelableProgressBar(title, info, (float) current / total);

                    if (isCancel)
                    {
                        EditorUtility.ClearProgressBar();
                    }
                }
            }

            return isCancel;
        }
    }
}
