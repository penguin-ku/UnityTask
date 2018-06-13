namespace UnityEngine.TaskExtension
{
    using System;
    using System.Collections;
    using System.Threading;
    using UnityEngine;

    /// <summary>
    /// Represents an object that handles the low-level work of queuing tasks onto threads.
    /// </summary>
    public class UnityTaskScheduler : MonoBehaviour
    {
        #region private members

        private static UnityTaskScheduler g_defaultContext;                                                      // 默认的上下文

        #endregion

        #region public functions

        /// <summary>
        /// Dispatches an asynchronous message to coroutine.
        /// </summary>
        /// <param name="p_action"></param>
        public Coroutine Post(IEnumerator p_action, Action<object> p_completeCallback)
        {
            return g_defaultContext.StartCoroutine(Excute(p_action, p_completeCallback));
        }

        public void Stop(Coroutine p_coroutine)
        {
            StopCoroutine(p_coroutine);
        }

        /// <summary>
        /// Creates a TaskScheduler associated with the current Dpjia.UnityTask.SynchronizationContext.
        /// </summary>
        /// <returns></returns>
        public static UnityTaskScheduler FromCurrentSynchronizationContext()
        {
            if (g_defaultContext == null)
            {
                GameObject go = new GameObject();
                go.name = "unity_task_scheduler";
                g_defaultContext = go.AddComponent<UnityTaskScheduler>();
                DontDestroyOnLoad(go);// 标记，不删除
            }
            return g_defaultContext;
        }

        #endregion

        #region private functions

        private static IEnumerator Excute(IEnumerator p_coroutine, Action<object> p_completeCallback)
        {
            while (p_coroutine.MoveNext())
            {
                yield return p_coroutine.Current;
            }
            if (p_completeCallback != null)
            {
                p_completeCallback(p_coroutine.Current);
            }
        }

        #endregion
    }
}

