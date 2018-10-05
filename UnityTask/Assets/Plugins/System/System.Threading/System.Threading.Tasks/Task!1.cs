namespace System.Threading.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Represents an asynchronous task that has a result.
    /// </summary>
    /// <typeparam name="T">The type of the task's result.</typeparam>
    public sealed class Task<T> : Task
    {
        public new static Task<T> None
        {
            get
            {
                return Task.FromResult<T>(default(T));
            }
        }

        #region private members

        private T result;

        #endregion

        #region constructors

        internal Task()
        {
        }

        #endregion

        #region public functions

        /// <summary>
        /// Gets the result of the task. If the task is not complete, this property blocks
        /// until the task is complete. If the task has an Exception or was cancelled, this
        /// property will rethrow the exception.
        /// </summary>
        public T Result
        {
            get
            {
                Wait();
                return result;
            }
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <param name="continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument.</param>
        /// <returns>A new Task that is complete after both the task and the continuation are
        /// complete.</returns>
        public Task ContinueWith(Action<Task<T>> continuation)
        {
            return base.ContinueWith(t => continuation((Task<T>)t));
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="TResult">The type returned by the continuation.</typeparam>
        /// <param name="continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public Task<TResult> ContinueWith<TResult>(Func<Task<T>, TResult> continuation)
        {
            return base.ContinueWith(t => continuation((Task<T>)t));
        }

        #endregion

        #region private functions

        /// <summary>
        /// run tasks in continuation list
        /// </summary>
        private void RunContinuations()
        {
            if (!Thread.CurrentThread.IsBackground)// 如果在前台线程，则强制移至后台
            {
                List<Action<Task>> continuations = null;
                lock (m_mutex)
                {
                    continuations = m_continuations.ToList();
                    m_continuations.Clear();
                }
                // 扔到线程池
                ThreadPool.QueueUserWorkItem(new WaitCallback(p =>
                {
                    foreach (var continuation in continuations)
                    {
                        continuation(this);
                    }
                }));
            }
            else
            {
                lock (m_mutex)
                {
                    foreach (var continuation in m_continuations)
                    {
                        continuation(this);
                    }
                    m_continuations.Clear();
                }
            }
        }

        #endregion

        #region private functions

        /// <summary>
        /// try to set specified result
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        internal bool TrySetResult(T p_result)
        {
            lock (m_mutex)
            {
                if (m_isCompleted)
                {
                    return false;
                }
                m_isCompleted = true;
                this.result = p_result;
                Monitor.PulseAll(m_mutex);
                RunContinuations();
                return true;
            }
        }

        /// <summary>
        /// try to cancel
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        internal bool TrySetCanceled()
        {
            lock (m_mutex)
            {
                if (m_isCompleted)
                {
                    return false;
                }
                m_isCompleted = true;
                this.m_isCanceled = true;
                Monitor.PulseAll(m_mutex);
                RunContinuations();
                return true;
            }
        }

        /// <summary>
        /// try to set specified exception
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        internal bool TrySetException(AggregateException p_exception)
        {
            lock (m_mutex)
            {
                if (m_isCompleted)
                {
                    return false;
                }
                m_isCompleted = true;
                this.m_exception = p_exception;
                Monitor.PulseAll(m_mutex);
                RunContinuations();
                return true;
            }
        }

        #endregion
    }
}

