using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityEngine.Processor
{
    /// <summary>
    /// 
    /// </summary>
    public class ForegroundInvoker : MonoBehaviour
    {
        #region private Singleton

        private static ForegroundInvoker g_instance;

        #endregion

        #region private members

        private Queue<Action> m_actionQueue = new Queue<Action>();

        #endregion

        #region internal static functions

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        internal static GameObject Initialize()
        {
            if (g_instance != null)
            {
                return g_instance.gameObject;
            }
            GameObject gObject = new GameObject();
            gObject.name = "ForegroundInvoker";
            g_instance = gObject.AddComponent<ForegroundInvoker>();
            return gObject;
        }

        /// <summary>
        /// 退出
        /// </summary>
        internal static void Exit()
        {
            if (g_instance != null)
            {
                Destroy(g_instance.gameObject);
            }
        }

        #endregion

        #region public static functions

        /// <summary>
        /// rise action to foreground
        /// </summary>
        /// <param name="p_action"></param>
        public static void Invoke(Action p_action)
        {
            if (g_instance == null)
            {
                //throw new Exception("foreground invoker未初始化");
                return;
            }
            if (!System.Threading.Thread.CurrentThread.IsBackground)// 如果当前是前台线程。则直接处理
            {
                p_action();
            }
            else
            {
                g_instance.m_actionQueue.Enqueue(p_action);
            }
        }

        #endregion

        #region life cycle

        private void Start()
        {
            StartCoroutine(RiseActionHandle());
        }

        private void OnApplicationQuit()
        {
            Destroy(gameObject);
        }

        #endregion

        #region private functions

        IEnumerator RiseActionHandle()
        {
            while (true)
            {
                while (m_actionQueue.Count > 0)
                {
                    try
                    {
                        Action action = m_actionQueue.Dequeue();
                        action();
                    }
                    catch (Exception ex)
                    {
                        log4net.Log4NetWrapper.GetLog().Error("前台invoke失败", ex);
                    }
                    yield return null;
                }
                yield return null;
            }
        }

        #endregion

    }
}
