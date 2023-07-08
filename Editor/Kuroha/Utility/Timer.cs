using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kuroha.Utility
{
    /// <summary>
    /// 定时器
    /// </summary>
    [Serializable]
    public class TimerInfo
    {
        /// <summary>
        /// 定时器标签
        /// </summary>
        public int Tag { get; internal set; }

        /// <summary>
        /// 已计时时间
        /// </summary>
        public float Timed { get; internal set; }

        /// <summary>
        /// 定时器总时长
        /// </summary>
        public float Life { get; internal set; }

        /// <summary>
        /// 全部计时次数
        /// </summary>
        public int TotalCount { get; internal set; }

        /// <summary>
        /// 剩余计时次数
        /// </summary>
        public int Count { get; internal set; }

        /// <summary>
        /// 定时触发的委托
        /// </summary>
        public Action<int> OnEnd { get; internal set; }
    }
    
    /// <summary>
    /// 定时器管理类
    /// </summary>
    public class Timer
    {
        private readonly Dictionary<int, TimerInfo> clockDictionary = new Dictionary<int, TimerInfo>();
        private readonly Stack<TimerInfo> clockCache = new Stack<TimerInfo>();
        private readonly List<int> toRemoveClock = new List<int>();

        /// <summary>
        /// 定时器标签计数器
        /// </summary>
        private int clockTagCounter;

        /// <summary>
        /// 定时主函数
        /// 更新定时器
        /// </summary>
        public void FixedUpdate()
        {
            if (clockDictionary.Count == 0)
            {
                return;
            }

            if (toRemoveClock.Count > 0)
            {
                toRemoveClock.Clear();
            }

            foreach (var (tag, timerInfo) in clockDictionary)
            {
                // 移除 "空定时器" 和 "剩余无剩余次数定时器"
                if (timerInfo == null || timerInfo.Count == 0)
                {
                    toRemoveClock.Add(tag);
                    continue;
                }

                // 累计定时
                timerInfo.Timed += Time.deltaTime;

                // 定时结束
                if (timerInfo.Timed >= timerInfo.Life)
                {
                    timerInfo.OnEnd(timerInfo.TotalCount - timerInfo.Count + 1);
                    timerInfo.Timed -= timerInfo.Life;

                    // 减少 1 次定时次数
                    if (timerInfo.Count > 0)
                    {
                        --timerInfo.Count;
                    }
                }
            }

            foreach (var tag in toRemoveClock)
            {
                clockCache.Push(clockDictionary[tag]);
                clockDictionary.Remove(tag);
            }
        }

        /// <summary>
        /// 注册定时器
        /// </summary>
        /// <param name="time">定时时长 (单位秒)</param>
        /// <param name="count">循环定时次数, 负数为永久循环</param>
        /// <param name="onEnd">定时结束回调</param>
        /// <returns>定时器的标签值</returns>
        public int Register(float time, int count, Action<int> onEnd)
        {
            var clock = clockCache.Count == 0 ? new TimerInfo() : clockCache.Pop();

            clock.Timed      = 0;
            clock.Life       = time;
            clock.OnEnd      = onEnd;
            clock.Count      = count;
            clock.TotalCount = count;
            clock.Tag        = ++clockTagCounter;

            // 注册
            clockDictionary.Add(clock.Tag, clock);

            // 返回标签
            return clockTagCounter;
        }

        /// <summary>
        /// 移除定时器
        /// </summary>
        /// <param name="tag">定时器标签</param>
        public bool UnRegister(int tag)
        {
            return clockDictionary.Remove(tag);
        }

        /// <summary>
        /// 获取定时器
        /// </summary>
        /// <param name="tag">标签值</param>
        public TimerInfo Get(int tag)
        {
            return clockDictionary.TryGetValue(tag, out var clock) ? clock : null;
        }
    }
}
