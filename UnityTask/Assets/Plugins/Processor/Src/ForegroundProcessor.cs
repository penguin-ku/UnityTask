using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityEngine.Processor
{
    /// <summary>
    /// 前端处理器
    /// </summary>
    public class ForegroundProcessor : MonoBehaviour
    {
        #region private members

        private static ForegroundProcessor g_instance;
        private static Dictionary<string, TimedTask> m_oneLoopQueues = new Dictionary<string, TimedTask>();

        #endregion

        #region internal static functions

        /// <summary>
        /// 初始化
        /// </summary>
        internal static GameObject Initialize()
        {
            if (g_instance != null)
            {
                return g_instance.gameObject;
            }
            GameObject go = new GameObject();
            go.name = "ForegroundProcessor";
            g_instance = go.AddComponent<ForegroundProcessor>();
            return go;
        }

        /// <summary>
        /// 退出
        /// </summary>
        internal static void Exit()
        {
            Destroy(g_instance.gameObject);
        }

        #endregion

        #region public static functions

        /// <summary>
        /// 追加任务
        /// </summary>
        /// <param name="p_action"></param>
        /// <param name="p_interval"></param>
        /// <param name="p_loopTimes"></param>
        public static string AppendAction(Action p_action, int p_interval = 0, int p_loopTimes = 1, int p_delay = -1)
        {
            TimedTask cell = new TimedTask(p_action, p_interval, p_loopTimes, p_delay);
            cell.IsEnabled = true;

            string actionKey = System.Guid.NewGuid().ToString();
            m_oneLoopQueues.Add(actionKey, cell);
            return actionKey;
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="p_timedTaskKey"></param>

        public static void CancelAction(string p_timedTaskKey)
        {
            m_oneLoopQueues.Remove(p_timedTaskKey);
        }

        /// <summary>
        /// 获取定时任务
        /// </summary>
        /// <param name="p_timedTaskKey"></param>
        /// <returns></returns>
        internal static TimedTask GetTimedTask(string p_timedTaskKey)
        {
            return m_oneLoopQueues.ContainsKey(p_timedTaskKey) ? m_oneLoopQueues[p_timedTaskKey] : null;
        }

        #endregion

        #region private functions

        /// <summary>
        /// 周期函数
        /// </summary>
        private static void WorkProc()
        {
            DateTime now = DateTime.Now;
            var keys = m_oneLoopQueues.Keys.ToList();
            foreach (var itemKey in keys)
            {
                try
                {
                    var item = m_oneLoopQueues[itemKey];
                    if (!item.IsEnabled)
                    {
                        continue;
                    }
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
                    m_oneLoopQueues.Remove(itemKey);// 防止无限循环
                    continue;
                }
            }
        }

        #endregion

        #region life cycle

        void Update()
        {
            WorkProc();
        }

        #endregion
    }
}
