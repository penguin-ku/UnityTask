namespace System.Threading.Tasks
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// .net4.5中用于协助超时任务取消
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CancellationToken
    {
        #region private members

        private CancellationTokenSource m_source;

        #endregion

        #region public members

        /// <summary>
        /// Gets whether cancellation has been requested for this token.
        /// </summary>
        public bool IsCancellationRequested
        {
            get
            {
                if (this.m_source == null)
                {
                    return false;
                }
                return this.m_source.IsCancellationRequested;
            }
        }

        /// <summary>
        /// Returns an empty CancellationToken value.
        /// </summary>
        public static CancellationToken None
        {
            get
            {
                return new CancellationToken();
            }
        }

        #endregion

        #region constructors

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="p_source"></param>
        public CancellationToken(CancellationTokenSource p_source)
        {
            this.m_source = p_source;
        }

        #endregion

        #region public functions

        /// <summary>
        /// Registers a delegate that will be called when this CancellationToken is canceled.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public CancellationTokenRegistration Register(Action callback)
        {
            if (this.m_source == null)
            {
                return new CancellationTokenRegistration();
            }
            return this.m_source.Register(callback);
        }

        /// <summary>
        /// Throws a OperationCanceledException if this token has had cancellation requested.
        /// </summary>
        public void ThrowIfCancellationRequested()
        {
            if (this.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
        }

        #endregion
    }
}

