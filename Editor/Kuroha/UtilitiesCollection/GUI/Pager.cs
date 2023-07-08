﻿using UnityEngine;

namespace Kuroha.UtilitiesCollection
{
    /// <summary>
    /// 分页管理器
    /// </summary>
    internal class Pager
    {
        /// <summary>
        /// 留白
        /// </summary>
        private const int UI_SPACE = 10;

        /// <summary>
        /// 页数跳转
        /// </summary>
        private int gotoPage = 1;

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="dataCount">数据总数量</param>
        /// <param name="countPerPage">每页的数据数量</param>
        /// <param name="currentPage">当前为第几页</param>
        /// <param name="beginIndex">返回: 当前页开始下标</param>
        /// <param name="endIndex">返回: 当前页结束下标</param>
        public void DoLayout(int dataCount, int countPerPage, ref int currentPage, out int beginIndex, out int endIndex)
        {
            #region 分页数据计算

            if (dataCount <= 0 || countPerPage <= 0 || currentPage <= 0)
            {
                beginIndex = 0;
                endIndex = 0;
                return;
            }

            var pageCount = dataCount / countPerPage;
            if (dataCount % countPerPage != 0)
            {
                pageCount++;
            }

            if (currentPage > pageCount)
            {
                currentPage = pageCount;
            }

            beginIndex = (currentPage - 1) * countPerPage;

            if (currentPage < pageCount)
            {
                endIndex = beginIndex + countPerPage - 1;
            }
            else
            {
                var remainder = dataCount % countPerPage;

                endIndex = remainder == 0 ? beginIndex + countPerPage - 1 : beginIndex + remainder - 1;
            }

            #endregion

            var fontSize = UnityEngine.GUI.skin.button.fontSize;
            UnityEngine.GUI.skin.button.fontSize = 13;

            GUILayout.BeginHorizontal();

            Top(ref currentPage);
            Previous(ref currentPage);
            Next(ref currentPage, pageCount);
            Last(ref currentPage, pageCount);

            GUILayout.Space(UI_SPACE);

            Info(currentPage, pageCount);
            Goto(ref currentPage, pageCount);

            GUILayout.EndHorizontal();

            UnityEngine.GUI.skin.button.fontSize = fontSize;
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="currentPage"></param>
        private void Top(ref int currentPage)
        {
            UnityEngine.GUI.enabled = currentPage > 1;
            if (GUILayout.Button("首页", GUILayout.Width(100), GUILayout.Height(24)))
            {
                currentPage = 1;
            }

            UnityEngine.GUI.enabled = true;
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="currentPage"></param>
        private void Previous(ref int currentPage)
        {
            UnityEngine.GUI.enabled = currentPage > 1;
            if (GUILayout.Button("上一页", GUILayout.Width(100), GUILayout.Height(24)))
            {
                if (currentPage > 1)
                {
                    currentPage--;
                }
            }

            UnityEngine.GUI.enabled = true;
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageCount"></param>
        private void Next(ref int currentPage, int pageCount)
        {
            UnityEngine.GUI.enabled = currentPage < pageCount;
            if (GUILayout.Button("下一页", GUILayout.Width(100), GUILayout.Height(24)))
            {
                currentPage++;
                if (currentPage > pageCount)
                {
                    currentPage = pageCount;
                }
            }

            UnityEngine.GUI.enabled = true;
        }

        /// <summary>
        /// 末页
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageCount"></param>
        private void Last(ref int currentPage, int pageCount)
        {
            UnityEngine.GUI.enabled = currentPage < pageCount;
            if (GUILayout.Button("末页", GUILayout.Width(100), GUILayout.Height(24)))
            {
                currentPage = pageCount;
            }

            UnityEngine.GUI.enabled = true;
        }

        /// <summary>
        /// 页数信息
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageCount"></param>
        private void Info(int currentPage, int pageCount)
        {
            var fontSize = UnityEngine.GUI.skin.label.fontSize;
            UnityEngine.GUI.skin.label.fontSize = 14;

            if (pageCount > 0)
            {
                GUILayout.Label($"第 {currentPage} 页 / 共 {pageCount} 页", GUILayout.Width(130), GUILayout.Height(24));
            }
            else
            {
                GUILayout.Label("无数据", GUILayout.Width(50), GUILayout.Height(24));
            }

            UnityEngine.GUI.skin.label.fontSize = fontSize;
        }

        /// <summary>
        /// 跳转
        /// </summary>
        private void Goto(ref int currentPage, int pageCount)
        {
            GotoButton(ref currentPage, pageCount);
            GotoText();
        }

        private void GotoButton(ref int currentPage, int pageCount)
        {
            UnityEngine.GUI.enabled = pageCount > 0 && gotoPage >= 1 && gotoPage <= pageCount;

            if (GUILayout.Button("跳转", GUILayout.Width(100), GUILayout.Height(24)))
            {
                if (gotoPage >= 1 && gotoPage <= pageCount)
                {
                    currentPage = gotoPage;
                }
            }

            UnityEngine.GUI.enabled = true;
        }

        private void GotoText()
        {
            var textRect = GUILayoutUtility.GetRect(40, 20, GUILayout.Width(75), GUILayout.Height(26));
            textRect.y += 3;
            textRect.height -= 3;

            var oldAlignment = UnityEngine.GUI.skin.textField.alignment;
            var fontSize = UnityEngine.GUI.skin.textField.fontSize;

            UnityEngine.GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
            UnityEngine.GUI.skin.textField.fontSize = 17;

            if (!int.TryParse(UnityEngine.GUI.TextField(textRect, gotoPage.ToString()), out gotoPage))
            {
                ToolDebug.LogError("只能输入数字!", ToolDebug.红);
            }

            UnityEngine.GUI.skin.textField.alignment = oldAlignment;
            UnityEngine.GUI.skin.textField.fontSize = fontSize;
        }
    }
}
