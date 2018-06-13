namespace System.Threading.Tasks
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Signals to a CancellationToken that it should be canceled.
    /// </summary>
    public sealed class CancellationTokenSource
    {
        #region private members

        private Action m_actions;
        private bool m_isCancellationRequested;
        private object m_mutex = new object();

        #endregion

        #region public members

        /// <summary>
        /// Get a new CancellationToken associated with this CancellationTokenSource
        /// </summary>
        public CancellationToken Token
        {
            get
            {
                return new CancellationToken(this);
            }
        }

        #endregion

        #region public functions

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        public void Cancel()
        {
            this.Cancel(false);
        }

        /// <summary>
        /// Communicates a request for cancellation, and specifies whether remaining callbacks and cancelable operations should be processed.
        /// </summary>
        /// <param name="p_throwOnFirstException">whether throw exception on first exception</param>
        public void Cancel(bool p_throwOnFirstException)
        {
            lock (m_mutex)
            {
                this.m_isCancellationRequested = true;
                if (this.m_actions != null)
                {
                    try
                    {
                        if (!p_throwOnFirstException)
                        {
                            List<Exception> innerExceptions = new List<Exception>();
                            foreach (Delegate delegateItem in this.m_actions.GetInvocationList())
                            {
                                try
                                {
                                    ((Action)delegateItem)();
                                }
                                catch (Exception exception)
                                {
                                    innerExceptions.Add(exception);
                                }
                            }
                            if (innerExceptions.Count > 0)
                            {
                                throw new AggregateException(innerExceptions);
                            }
                        }
                        else
                        {
                            this.m_actions();
                        }
                    }
                    finally
                    {
                        this.m_actions = null;
                    }
                }
            }
        }

        #endregion

        #region internal functions

        /// <summary>
        /// regist action on cancel
        /// </summary>
        /// <param name="p_action"></param>
        /// <returns></returns>
        internal CancellationTokenRegistration Register(Action p_action)
        {
            lock (m_mutex)
            {
                this.m_actions = Delegate.Combine(this.m_actions, p_action) as Action;
                return new CancellationTokenRegistration(this, p_action);
            }
        }

        /// <summary>
        /// unregist action from cancel
        /// </summary>
        /// <param name="p_action"></param>
        internal void Unregister(Action p_action)
        {
            lock (m_mutex)
            {
                this.m_actions = (Action)Delegate.Remove(this.m_actions, p_action);
            }
        }

        /// <summary>
        /// Gets whether cancellation has been requested for this source.
        /// </summary>
        internal bool IsCancellationRequested
        {
            get
            {
                lock (m_mutex)
                {
                    return this.m_isCancellationRequested;
                }
            }
        }

        #endregion
    }
}

