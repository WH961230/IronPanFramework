using System;
using UnityEditor;

namespace Kuroha.QHierarchy.Editor
{
    /// <summary>
    /// 编辑器计时器 (毫秒)
    /// </summary>
    internal class EditorTimer
    {
        private static EditorTimer currentTimer;
        
        private int Millis { get; set; }

        internal bool AutoRestart { get; set; }

        private Action TimedAction { get; set; }

        private bool isStart;
        private long lastTicks;
        private long currentTicks;
        private long currentMillis;

        /// <summary>
        /// 计时器
        /// </summary>
        /// <param name="millis">毫秒数</param>
        /// <param name="timedAction">计时结束后回调</param>
        /// <param name="autoRestart">自动进行循环定时</param>
        internal EditorTimer(int millis, Action timedAction, bool autoRestart = false)
        {
            Millis      = millis;
            TimedAction = timedAction;
            AutoRestart = autoRestart;
            isStart = false;
        }

        /// <summary>
        /// 开始计时
        /// </summary>
        internal void Start()
        {
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;

            lastTicks = DateTime.Now.Ticks;
            isStart   = true;
        }

        /// <summary>
        /// 停止计时
        /// </summary>
        internal void Stop()
        {
            isStart = false;

            EditorApplication.update -= EditorUpdate;
        }

        /// <summary>
        /// 编辑器更新时计算计时
        /// </summary>
        private void EditorUpdate()
        {
            if (isStart == false)
            {
                return;
            }

            currentTicks  = DateTime.Now.Ticks;
            currentMillis = (currentTicks - lastTicks) / 10000;

            if (currentMillis >= Millis)
            {
                currentTimer = this;
                TimedAction?.Invoke();

                if (AutoRestart)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// 停止正在执行 TimedAction 的计时器
        /// </summary>
        internal static void StopSelf()
        {
            currentTimer.AutoRestart = false;
            currentTimer.Stop();
        }
    }
}
