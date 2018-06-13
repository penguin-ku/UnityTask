using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.Processor.Threading
{
    /// <summary>
    /// foregroud timer
    /// </summary>
    public sealed class ForegroudTimer : IDisposable
    {
        #region private members

        private TimerCallback m_callback;
        private object m_state;
        private string m_timedTaskKey;

        #endregion

        #region constructor

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="p_callback">A TimerCallback delegate representing a method to be executed.</param>
        /// <param name="p_state">An object containing information to be used by the callback method, or null.</param>
        /// <param name="p_delay">The amount of time to delay before callback is invoked, in milliseconds.</param>
        /// <param name="p_period">The time interval between invocations of callback, in milliseconds.</param>
        public ForegroudTimer(TimerCallback p_callback, object p_state, int p_delay, int p_period)
        {
            m_callback = p_callback;
            m_state = p_state;
            m_timedTaskKey = ForegroundProcessor.AppendAction(() => m_callback(m_state), p_period, -1, p_delay);
        }

        #endregion

        #region public functions

        /// <summary>
        /// Changes the start time and the interval between method invocations for a timer, using 32-bit signed integers to measure time intervals.
        /// </summary>
        /// <param name="p_delay">The amount of time to delay before callback is invoked, in milliseconds.</param>
        /// <param name="p_period">The time interval between invocations of callback, in milliseconds.</param>
        public void Change(int p_delay, int p_period)
        {
            TimedTask task = ForegroundProcessor.GetTimedTask(m_timedTaskKey);
            task.Delay = p_delay;
            task.Interval = p_period;
            task.LoopTimes = 0;
        }

        #endregion

        #region IDisposable members

        /// <summary>
        /// Releases all resources used by the current instance of Timer and signals when the timer has been disposed of.
        /// </summary>
        public void Dispose()
        {
            ForegroundProcessor.CancelAction(m_timedTaskKey);
        }

        #endregion
    }
}
