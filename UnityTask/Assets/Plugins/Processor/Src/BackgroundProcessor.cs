using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace UnityEngine.Processor
{ 
    /// <summary>
    /// 后台处理器
    /// </summary>
    public static class BackgroundProcessor
    {
        #region  private const members

        private const int WORK_INTERVAL = 10;

        #endregion

        #region private members

        private static ManualResetEvent g_waiter;
        private static RegisteredWaitHandle g_procHandle;
        private static object m_oneLoopQueueLock = new object();
        private static Dictionary<string, TimedTask> m_oneLoopQueues = new Dictionary<string, TimedTask>();

        #endregion

        #region public functions

        /// <summary>
        /// Initialize
        /// </summary>
        public static void Initialize()
        {
            if (g_waiter != null)
            {
                return;
            }
            g_waiter = new ManualResetEvent(false);
            g_procHandle = ThreadPool.RegisterWaitForSingleObject(g_waiter, WorkProc, null, WORK_INTERVAL, false);
        }

        /// <summary>
        /// exit
        /// </summary>
        public static void Exit()
        {
            g_procHandle.Unregister(g_waiter);
        }

        /// <summary>
        /// append action
        /// </summary>
        /// <param name="p_action"></param>
        /// <param name="p_interval"></param>
        /// <param name="p_loopTimes"></param>
        public static string AppendAction(Action p_action, int p_interval = 0, int p_loopTimes = 1, int p_delay = -1)
        {
            if (g_waiter == null)//如果未初始化，则此处需要进行初始化
            {
                throw new Exception("background processor未初始化");
            }
            TimedTask cell = new TimedTask(p_action, p_interval, p_loopTimes, p_delay);
            lock (m_oneLoopQueueLock)
            {
                string actionKey = System.Guid.NewGuid().ToString();
                m_oneLoopQueues.Add(actionKey, cell);
                return actionKey;
            }
        }

        /// <summary>
        /// cancel action
        /// </summary>
        /// <param name="p_actionKey"></param>

        public static void CancelAction(string p_actionKey)
        {
            lock (m_oneLoopQueueLock)
            {
                m_oneLoopQueues.Remove(p_actionKey);
            }
        }

        #endregion

        #region private functions

        /// <summary>
        /// life cycle
        /// </summary>
        private static void WorkProc(object p_state, bool p_isTimeOut)
        {
            lock (m_oneLoopQueueLock)
            {
                DateTime now = DateTime.Now;
                var keys = m_oneLoopQueues.Keys.ToList();
                foreach (var itemKey in keys)
                {
                    try
                    {
                        var item = m_oneLoopQueues[itemKey];
                        if (item.LoopTimes != -1 && item.CurrLoopTimes >= item.LoopTimes)                           // 前置，防止出现0次循环要求的操作
                        {
                            m_oneLoopQueues.Remove(itemKey);
                            continue;
                        }
                        if ((item.CurrLoopTimes == 0 && item.LastTime.AddMilliseconds(item.Delay) < now) ||         // 首次执行，判断延迟时间
                            (item.CurrLoopTimes != 0 && item.LastTime.AddMilliseconds(item.Interval) < now))        // 非首次执行，判断间隔
                        {
                            item.Action();
                            item.LastTime = now;
                            item.CurrLoopTimes++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("后台任务失败" + ex.ToString());
                        m_oneLoopQueues.Remove(itemKey);// 失败的任务从队列移除，即出错的不会做多次尝试
                        continue;
                    }
                }
            }
        }

        #endregion
    }
}
