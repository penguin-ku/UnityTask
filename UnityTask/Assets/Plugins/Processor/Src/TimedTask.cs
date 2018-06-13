using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.Processor
{
    /// <summary>
    /// 任务单元
    /// </summary>
    internal class TimedTask
    {
        #region public properties

        /// <summary>
        /// 设置或获取需要执行的操作
        /// </summary>
        public Action Action { set; get; }

        /// <summary>
        /// 设置或获取等待间隔
        /// </summary>
        public int Interval { set; get; }

        /// <summary>
        /// 设置或获取需要循环执行次数
        /// </summary>
        public int LoopTimes { set; get; }

        /// <summary>
        /// 设置或获取延迟时间
        /// </summary>
        public int Delay { set; get; }

        /// <summary>
        /// 是否启用
        /// </summary>
        internal bool IsEnabled { set; get; }

        #endregion

        #region inner properties

        /// <summary>
        /// 设置或获取当前等待时间
        /// </summary>
        internal DateTime LastTime { set; get; }

        /// <summary>
        /// 设置或获取当前循环次数
        /// </summary>
        internal int CurrLoopTimes { set; get; }

        #endregion

        #region constructors

        /// <summary>
        /// constructors
        /// </summary>
        /// <param name="p_action"></param>
        /// <param name="p_interval"></param>
        /// <param name="p_loopTimes"></param>
        public TimedTask(Action p_action, int p_interval = 0, int p_loopTimes = 1, int p_delay = -1)
        {
            Action = p_action;
            Interval = p_interval;
            LoopTimes = p_loopTimes;
            LastTime = DateTime.Now;
            CurrLoopTimes = 0;
            Delay = p_delay == -1 ? p_interval : p_delay;
        }

        #endregion
    }
}
