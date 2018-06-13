using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine.Processor.Timers
{
    /// <summary>
    /// foregroud timer
    /// </summary>
    public sealed class ForegroudTimer : IDisposable
    {
        #region private members

        private string m_timedTaskKey;

        #endregion

        #region public events

        public event ElapsedEventHandler ElapsedEvent;

        #endregion

        /// <summary>
        /// Initializes a new instance of the Timer class, and sets the Interval property to the specified number of milliseconds.
        /// </summary>
        public ForegroudTimer(int p_interval)
        {
            m_timedTaskKey = ForegroundProcessor.AppendAction(() =>
            {
                if (ElapsedEvent != null)
                {
                    ElapsedEvent(this, new ElapsedEventArgs());
                }
            }, p_interval, -1, 0);
            ForegroundProcessor.GetTimedTask(m_timedTaskKey).IsEnabled = false;
        }

        /// <summary>
        /// Starts raising the Elapsed event by setting Enabled to true.
        /// </summary>
        public void Start()
        {
            ForegroundProcessor.GetTimedTask(m_timedTaskKey).IsEnabled = true;
        }

        /// <summary>
        /// Stops raising the Elapsed event by setting Enabled to false.
        /// </summary>
        public void Stop()
        {
            ForegroundProcessor.GetTimedTask(m_timedTaskKey).IsEnabled = false;
        }

        #region IDisposable members

        /// <summary>
        /// Occurs when the component is disposed by a call to the Dispose method.
        /// </summary>
        public void Dispose()
        {
            ForegroundProcessor.CancelAction(m_timedTaskKey);
        }

        #endregion
    }
}
