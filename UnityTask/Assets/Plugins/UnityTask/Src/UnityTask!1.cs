namespace UnityEngine.TaskExtension
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using UnityEngine.Processor;

    /// <summary>
    /// Represents an asynchronous task that has a result.
    /// </summary>
    /// <typeparam name="T">The type of the task's result.</typeparam>
    public sealed class UnityTask<T> : UnityTask
    {
        public new static UnityTask<T> None
        {
            get
            {
                return UnityTask.FromResult<T>(default(T));
            }
        }

        #region private members

        private T result;

        #endregion

        #region constructors

        internal UnityTask()
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
                //Wait();
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
        public UnityTask<UnityTask<T2>> ContinueWith<T2>(Func<UnityTask<T>, UnityTask<T2>> p_continuation)
        {
            return base.ContinueWith(t => p_continuation((UnityTask<T>)t));
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <param name="continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument.</param>
        /// <returns>A new Task that is complete after both the task and the continuation are
        /// complete.</returns>
        public UnityTask<UnityTask> ContinueWith(Func<UnityTask<T>, UnityTask> p_continuation)
        {
            return base.ContinueWith(t => p_continuation((UnityTask<T>)t));
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <param name="continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument.</param>
        /// <returns>A new Task that is complete after both the task and the continuation are
        /// complete.</returns>
        public UnityTask ContinueWith(Action<UnityTask<T>> p_continuation)
        {
            return base.ContinueWith(t =>
            {
                //if (t.IsFaulted)
                //{
                //    return UnityTask.FromException<int>(t.Exception);
                //}
                //else
                //{
                    try
                    {
                        p_continuation((UnityTask<T>)t);
                        return UnityTask.FromResult(0);
                    }
                    catch (Exception ex)
                    {
                        return UnityTask.FromException<int>(ex);
                    }
                //}
            });
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <param name="continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument.</param>
        /// <returns>A new Task that is complete after both the task and the continuation are
        /// complete.</returns>
        public UnityTask<T2> ContinueWith<T2>(Func<UnityTask<T>, T2> p_continuation)
        {
            return base.ContinueWith(t =>
            {
                //if (t.IsFaulted)
                //{
                //    return UnityTask.FromException<T2>(t.Exception);
                //}
                //else
                //{
                    try
                    {
                        T2 result = p_continuation((UnityTask<T>)t);
                        return UnityTask.FromResult(result);
                    }
                    catch (Exception ex)
                    {
                        return UnityTask.FromException<T2>(ex);
                    }
                //}
            }).Unwrap();
        }


        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="TResult">The type returned by the continuation.</typeparam>
        /// <param name="continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public UnityTask<UnityTask<TResult>> ContinueWith<TResult>(Func<UnityTask<T>, IEnumerator> continuation)
        {
            return base.ContinueWith<TResult>(t => continuation((UnityTask<T>)t));
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="TResult">The type returned by the continuation.</typeparam>
        /// <param name="continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public UnityTask<UnityTask> ContinueWith(Func<UnityTask<T>, IEnumerator> continuation)
        {
            return base.ContinueWith(t => continuation((UnityTask<T>)t));
        }

        #endregion

        #region private functions

        /// <summary>
        /// run tasks in continuation list
        /// </summary>
        private void RunContinuations()
        {
            if (Thread.CurrentThread.IsBackground)
            {
                var continueActions = m_continuationActions.ToList();
                ForegroundInvoker.Invoke(() =>
                {
                    foreach (var item in continueActions)
                    {
                        item(this);
                    }
                });
                m_continuationActions.Clear();
            }
            else
            {
                foreach (var item in m_continuationActions)
                {
                    item(this);
                }
                m_continuationActions.Clear();
            }
        }

        #endregion

        #region internal functions

        /// <summary>
        /// try to set specified result
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        internal bool TrySetResult(T p_result)
        {
            if (m_isCompleted)
            {
                return false;
            }
            m_isCompleted = true;
            this.result = p_result;
            RunContinuations();
            return true;
        }

        /// <summary>
        /// try to cancel
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        internal bool TrySetCanceled()
        {
            if (m_isCompleted)
            {
                return false;
            }
            UnityTaskScheduler.FromCurrentSynchronizationContext().Stop(TaskCoroutine);
            m_isCompleted = true;
            this.m_isCanceled = true;
            RunContinuations();
            return true;
        }

        /// <summary>
        /// try to set specified exception
        /// </summary>
        /// <param name="p_result"></param>
        /// <returns></returns>
        internal bool TrySetException(System.Threading.Tasks.AggregateException p_exception)
        {
            if (m_isCompleted)
            {
                return false;
            }
            m_isCompleted = true;
            this.m_exception = p_exception;
            RunContinuations();
            return true;
        }

        #endregion
    }
}

