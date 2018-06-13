namespace System.Threading.Tasks
{
    using System;
    using System.Threading;

    /// <summary>
    /// Represents an object that handles the low-level work of queuing tasks onto threads.
    /// </summary>
    public class TaskScheduler
    {
        #region private members

        private static SynchronizationContext g_defaultContext = new SynchronizationContext();                                                      // 默认的上下文
        private SynchronizationContext m_context;                                                                                                   // 同步上下文

        #endregion

        #region constructors

        /// <summary>
        /// constructors
        /// </summary>
        /// <param name="p_context">同步上下文</param>
        public TaskScheduler(SynchronizationContext p_context)
        {
            this.m_context = p_context ?? g_defaultContext;
        }

        #endregion

        #region public functions

        /// <summary>
        /// Dispatches an asynchronous message to a synchronization context.
        /// </summary>
        /// <param name="p_action"></param>
        public void Post(Action p_action)
        {
            m_context.Post(o => p_action(), null);
        }

        /// <summary>
        /// Creates a TaskScheduler associated with the current System.Threading.SynchronizationContext.
        /// </summary>
        /// <returns></returns>
        public static TaskScheduler FromCurrentSynchronizationContext()
        {
            return new TaskScheduler(SynchronizationContext.Current);
        }

        #endregion
    }
}

