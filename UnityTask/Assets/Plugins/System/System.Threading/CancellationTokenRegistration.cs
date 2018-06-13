namespace System.Threading.Tasks
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a callback delegate that has been registered with a CancellationToken.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CancellationTokenRegistration : IDisposable
    {
        #region private members

        private Action m_action;
        private CancellationTokenSource m_source;

        #endregion

        #region constructors

        /// <summary>
        /// constructors
        /// </summary>
        /// <param name="p_source"></param>
        /// <param name="p_action"></param>
        internal CancellationTokenRegistration(CancellationTokenSource p_source, Action p_action)
        {
            this.m_source = p_source;
            this.m_action = p_action;
        }

        #endregion

        # region IDisposable members

        public void Dispose()
        {
            if ((this.m_source != null) && (this.m_action != null))
            {
                this.m_source.Unregister(this.m_action);
                this.m_action = null;
                this.m_source = null;
            }
        }

        #endregion
    }
}

