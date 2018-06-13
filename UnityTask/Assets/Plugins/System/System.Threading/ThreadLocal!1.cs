namespace System.Threading.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// 线程变量
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThreadLocal<T> : IDisposable
    {
        #region 静态private members

        private static long g_lastId = -1;                                                                                  // 上一个id
        [ThreadStatic]
        private static IDictionary<long, T> t_threadLocalData;                                                              // 静态变量，每个线程维持一份副本
        private static IList<WeakReference> g_allDataDictionaries = new List<WeakReference>();                              // 弱引用集，整个进程维持一份副本。此处维护各个线程的t_threadLocalData

        #endregion

        #region 静态私有属性

        /// <summary>
        /// 获取弱引用集，每个线程一份
        /// </summary>
        private static IDictionary<long, T> ThreadLocalData
        {
            get
            {
                if (t_threadLocalData == null)
                {
                    t_threadLocalData = new Dictionary<long, T>();
                    lock (g_allDataDictionaries)
                    {
                        g_allDataDictionaries.Add(new WeakReference(t_threadLocalData));
                    }
                }
                return t_threadLocalData;
            }
        }

        #endregion

        #region private members

        private bool m_disposed = false;
        private readonly long m_id;
        private readonly Func<T> m_valueFactory;

        #endregion

        #region public properties

        /// <summary>
        /// 获取值
        /// </summary>
        public T Value
        {
            get
            {
                CheckDisposed();
                T result;
                if (ThreadLocalData.TryGetValue(m_id, out result))
                {
                    return result;
                }
                return ThreadLocalData[m_id] = m_valueFactory();
            }
            set
            {
                CheckDisposed();
                ThreadLocalData[m_id] = value;
            }
        }

        #endregion

        #region constructors destructor

        /// <summary>
        /// constructors
        /// </summary>
        public ThreadLocal()
          : this(() => default(T))
        {
        }

        /// <summary>
        /// constructors
        /// </summary>
        /// <param name="p_valueFactory"></param>
        public ThreadLocal(Func<T> p_valueFactory)
        {
            this.m_valueFactory = p_valueFactory;
            /*
             * 在大多数计算机上，增加变量操作不是一个原子操作，需要执行下列步骤：
             * 一：将实例变量中的值加载到寄存器中。
             * 二：增加或减少该值。
             * 三：在实例变量中存储该值。
             * 在多线程环境下，线程会在执行完前两个步骤后被抢先。然后由另一个线程执行所有三个步骤，当第一个线程重新开始执行时，它覆盖实例变量中的值，造成第二个线程执行增减操作的结果丢失。
             * Interlocked可以为多个线程共享的变量提供原子操作。
             */
            m_id = Interlocked.Increment(ref g_lastId);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~ThreadLocal()
        {
            if (!m_disposed)
            {
                Dispose();
            }
        }

        #endregion

        #region private functions

        private void CheckDisposed()
        {
            if (m_disposed)
            {
                throw new ObjectDisposedException("ThreadLocal has been disposed.");
            }
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            lock (g_allDataDictionaries)
            {
                for (int i = 0; i < g_allDataDictionaries.Count; i++)
                {
                    var data = g_allDataDictionaries[i].Target as IDictionary<object, T>;
                    if (data == null)
                    {
                        g_allDataDictionaries.RemoveAt(i);
                        i--;
                        continue;
                    }
                    data.Remove(m_id);
                }
            }
            m_disposed = true;
        }

        #endregion
    }
}

