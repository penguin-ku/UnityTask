namespace UnityEngine.TaskExtension
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Processor;

    /// <summary>
    /// Represents an asynchronous task.
    /// </summary>
    public abstract class UnityTask
    {
        public static UnityTask None
        {
            get
            {
                return UnityTask.FromResult(default(int));
            }
        }

        #region protected members

        protected List<Action<UnityTask>> m_continuationActions = new List<Action<UnityTask>>();

        protected System.Threading.Tasks.AggregateException m_exception;

        protected bool m_isCompleted;
        protected bool m_isCanceled;

        #endregion

        #region internal properties

        internal Func<IEnumerator> TaskGenerator { set; get; }
        internal Action<object> ReturnResult { set; get; }
        internal Coroutine TaskCoroutine { set; get; }

        #endregion

        #region constructors

        internal UnityTask()
        {
        }

        #endregion

        #region public properties

        /// <summary>
        /// 获取工厂
        /// </summary>
        internal static UnityTaskFactory Factory
        {
            get
            {
                return new UnityTaskFactory();
            }
        }

        /// <summary>
        /// Gets the exceptions for the task, if there are any. <c>null</c> otherwise.
        /// </summary>
        public System.Threading.Tasks.AggregateException Exception
        {
            get
            {
                return m_exception;
            }
        }

        /// <summary>
        /// Gets whether the task was cancelled.
        /// </summary>
        public bool IsCanceled
        {
            get
            {
                return m_isCanceled;
            }
        }

        /// <summary>
        /// Gets whether the task has been completed with either an exception,
        /// cancellation, or a result.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return m_isCompleted;
            }
        }

        /// <summary>
        /// Gets whether the task failed.
        /// </summary>
        public bool IsFaulted
        {
            get
            {
                return Exception != null;
            }
        }

        #endregion

        #region public functions

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="T">The type returned by the continuation.</typeparam>
        /// <param name="p_continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public UnityTask ContinueWith(Action<UnityTask> p_continuation)
        {
            return ContinueWith(p_continuation, System.Threading.Tasks.CancellationToken.None);
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="T">The type returned by the continuation.</typeparam>
        /// <param name="p_continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public UnityTask ContinueWith(Action<UnityTask> p_continuation, System.Threading.Tasks.CancellationToken p_cancellationToken)
        {
            bool completed = false;

            UnityTaskCompletionSource<int> utcs = new UnityTaskCompletionSource<int>();
            CancellationTokenRegistration cancelToken = p_cancellationToken.Register(() => utcs.TrySetCanceled());
            // 此处防止判断为false之后正好执行完毕
            completed = IsCompleted;
            if (!completed)
            {
                m_continuationActions.Add(t =>
                {
                    try
                    {
                        p_continuation(this);
                        utcs.TrySetResult(0);
                        cancelToken.Dispose();
                    }
                    catch (Exception ex)
                    {
                        utcs.TrySetException(ex);
                        cancelToken.Dispose();
                    }
                });
            }
            else
            {
                ForegroundInvoker.Invoke(() =>// 如果当前不在前端线程，则切换
                {
                    try
                    {
                        p_continuation(this);
                        utcs.TrySetResult(0);
                        cancelToken.Dispose();
                    }
                    catch (Exception ex)
                    {
                        utcs.TrySetException(ex);
                        cancelToken.Dispose();
                    }
                });
            }

            return utcs.Task;
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="TResult">The type returned by the continuation.</typeparam>
        /// <param name="p_continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public UnityTask<UnityTask<TResult>> ContinueWith<TResult>(Func<UnityTask, UnityTask<TResult>> p_continuation)
        {
            return ContinueWith(p_continuation, System.Threading.Tasks.CancellationToken.None);
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="TResult">The type returned by the continuation.</typeparam>
        /// <param name="p_continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <param name="p_cancellationToken">The cancellation token.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public UnityTask<UnityTask<TResult>> ContinueWith<TResult>(Func<UnityTask, UnityTask<TResult>> p_continuation, System.Threading.Tasks.CancellationToken p_cancellationToken)
        {
            bool completed = false;

            UnityTaskCompletionSource<UnityTask<TResult>> utcs = new UnityTaskCompletionSource<UnityTask<TResult>>();
            CancellationTokenRegistration cancelToken = p_cancellationToken.Register(() => utcs.TrySetCanceled());
            // 此处防止判断为false之后正好执行完毕
            completed = IsCompleted;
            if (!completed)
            {
                m_continuationActions.Add(t =>
                {
                    //if (t.IsFaulted)
                    //{
                    //    utcs.TrySetException(t.Exception);
                    //    cancelToken.Dispose();
                    //}
                    //else
                    //{
                    try
                    {
                        UnityTask<TResult> result = p_continuation(t);
                        utcs.TrySetResult(result);
                        cancelToken.Dispose();
                    }
                    catch (Exception ex)
                    {
                        utcs.TrySetException(ex);
                        cancelToken.Dispose();
                    }
                    //}
                });
            }
            else
            {
                ForegroundInvoker.Invoke(() =>// 如果当前不在前端线程，则切换
                {
                    //if (this.IsFaulted)
                    //{
                    //    utcs.TrySetException(this.Exception);
                    //    cancelToken.Dispose();
                    //}
                    //else
                    //{
                    try
                    {
                        UnityTask<TResult> result = p_continuation(this);
                        utcs.TrySetResult(result);
                        cancelToken.Dispose();
                    }
                    catch (Exception ex)
                    {
                        utcs.TrySetException(ex);
                        cancelToken.Dispose();
                    }
                    //}
                });
            }

            return utcs.Task;
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="T">The type returned by the continuation.</typeparam>
        /// <param name="p_continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <param name="p_cancellationToken">The cancellation token.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public UnityTask<UnityTask> ContinueWith(Func<UnityTask, UnityTask> p_continuation)
        {
            return ContinueWith(p_continuation, System.Threading.Tasks.CancellationToken.None);
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="T">The type returned by the continuation.</typeparam>
        /// <param name="p_continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <param name="p_cancellationToken">The cancellation token.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public UnityTask<UnityTask> ContinueWith(Func<UnityTask, UnityTask> p_continuation, System.Threading.Tasks.CancellationToken p_cancellationToken)
        {
            bool completed = false;

            UnityTaskCompletionSource<UnityTask> utcs = new UnityTaskCompletionSource<UnityTask>();
            CancellationTokenRegistration cancelToken = p_cancellationToken.Register(() => utcs.TrySetCanceled());
            // 此处防止判断为false之后正好执行完毕
            completed = IsCompleted;
            if (!completed)
            {
                m_continuationActions.Add(t =>
                {
                    //if (t.IsFaulted)
                    //{
                    //    utcs.TrySetException(t.Exception);
                    //    cancelToken.Dispose();
                    //}
                    //else
                    //{
                        try
                        {
                            UnityTask result = p_continuation(t);
                            utcs.TrySetResult(result);
                            cancelToken.Dispose();
                        }
                        catch (Exception ex)
                        {
                            utcs.TrySetException(ex);
                            cancelToken.Dispose();
                        }
                    //}
                });
            }
            else
            {
                ForegroundInvoker.Invoke(() =>
                {
                    //if (this.IsFaulted)
                    //{
                    //    utcs.TrySetException(this.Exception);
                    //    cancelToken.Dispose();
                    //}
                    //else
                    //{
                        try
                        {
                            UnityTask result = p_continuation(this);
                            utcs.TrySetResult(result);
                            cancelToken.Dispose();
                        }
                        catch (Exception ex)
                        {
                            utcs.TrySetException(ex);
                            cancelToken.Dispose();
                        }
                    //}
                });
            }

            return utcs.Task;
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="T">The type returned by the continuation.</typeparam>
        /// <param name="p_continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public UnityTask<UnityTask<TResult>> ContinueWith<TResult>(Func<UnityTask, IEnumerator> p_continuation)
        {
            return ContinueWith<TResult>(p_continuation, System.Threading.Tasks.CancellationToken.None);
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="T">The type returned by the continuation.</typeparam>
        /// <param name="p_continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <param name="p_cancellationToken">The cancellation token.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public UnityTask<UnityTask<TResult>> ContinueWith<TResult>(Func<UnityTask, IEnumerator> p_continuation, System.Threading.Tasks.CancellationToken p_cancellationToken)
        {
            Func<UnityTask, UnityTask<TResult>> continuation = t =>
            {
                return UnityTask.Run<TResult>(() => p_continuation(t));
            };

            return ContinueWith(continuation, p_cancellationToken);
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="T">The type returned by the continuation.</typeparam>
        /// <param name="p_continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public UnityTask<UnityTask> ContinueWith(Func<UnityTask, IEnumerator> p_continuation)
        {
            return ContinueWith(p_continuation, System.Threading.Tasks.CancellationToken.None);

        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="T">The type returned by the continuation.</typeparam>
        /// <param name="p_continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <param name="p_cancellationToken">The cancellation token.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public UnityTask<UnityTask> ContinueWith(Func<UnityTask, IEnumerator> p_continuation, System.Threading.Tasks.CancellationToken p_cancellationToken)
        {
            Func<UnityTask, UnityTask> continuation = t =>
            {
                return UnityTask.Run(() => p_continuation(t));
            };

            return ContinueWith(continuation, p_cancellationToken);
        }

        #endregion

        #region static public functions

        /// <summary>
        /// Executes a function asynchronously, returning a task that represents the operation.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="p_toRun">The function to run.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static UnityTask Run(Action p_action)
        {
            return UnityTask.FromResult(0).ContinueWith(t => p_action());
        }

        /// <summary>
        /// Executes a function asynchronously, returning a task that represents the operation.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="p_toRun">The function to run.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static UnityTask<T> Run<T>(Func<T> p_action)
        {
            return UnityTask.FromResult(0).ContinueWith(t => p_action());
        }

        /// <summary>
        /// Executes a function asynchronously, returning a task that represents the operation.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="p_toRun">The function to run.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static UnityTask<T> Run<T>(Func<IEnumerator> p_toRun)
        {
            return UnityTask.Factory.StartNew<T>(p_toRun);
        }

        /// <summary>
        /// Executes an action asynchronously, returning a task that represents the operation.
        /// </summary>
        /// <param name="p_toRun">The action to run.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static UnityTask Run(Func<IEnumerator> p_toRun)
        {
            return UnityTask.Factory.StartNew<object>(p_toRun);
        }

        /// <summary>
        /// Creates a task that is complete when all of the provided tasks are complete.
        /// If any of the tasks has an exception, all exceptions raised in the tasks will
        /// be aggregated into the returned task. Otherwise, if any of the tasks is cancelled,
        /// the returned task will be cancelled.
        /// </summary>
        /// <param name="tasks">The tasks to aggregate.</param>
        /// <returns>A task that is complete when all of the provided tasks are complete.</returns>
        public static UnityTask WhenAll(params UnityTask[] tasks)
        {
            return WhenAll(tasks.ToList());
        }

        /// <summary>
        /// Creates a task that is complete when all of the provided tasks are complete.
        /// If any of the tasks has an exception, all exceptions raised in the tasks will
        /// be aggregated into the returned task. Otherwise, if any of the tasks is cancelled,
        /// the returned task will be cancelled.
        /// </summary>
        /// <param name="p_tasks">The tasks to aggregate.</param>
        /// <returns>A task that is complete when all of the provided tasks are complete.</returns>
        public static UnityTask WhenAll(IEnumerable<UnityTask> p_tasks)
        {
            var taskArr = p_tasks.ToArray();
            if (taskArr.Length == 0)
            {
                return UnityTask.FromResult(0);
            }
            var tcs = new UnityTaskCompletionSource<object>();
            UnityTask.Factory.ContinueWhenAll(taskArr, _ =>
            {
                var exceptions = taskArr.Where(p => p.IsFaulted).Select(p => p.Exception).ToArray();
                if (exceptions.Length > 0)
                {
                    tcs.SetException(new System.Threading.Tasks.AggregateException(exceptions));
                }
                else if (taskArr.Any(t => t.IsCanceled))
                {
                    tcs.SetCanceled();
                }
                else
                {
                    tcs.SetResult(0);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// Waits for any of the provided Task objects to complete execution.
        /// </summary>
        /// <param name="p_tasks"></param>
        /// <returns></returns>
        internal static UnityTask<UnityTask> WhenAny(params UnityTask[] p_tasks)
        {
            return WhenAny((IEnumerable<UnityTask>)p_tasks);
        }

        /// <summary>
        /// Waits for any of the provided Task objects to complete execution.
        /// </summary>
        /// <param name="p_tasks"></param>
        /// <returns></returns>
        internal static UnityTask<UnityTask> WhenAny(IEnumerable<UnityTask> p_tasks)
        {
            var tcs = new UnityTaskCompletionSource<UnityTask>();
            foreach (var task in p_tasks)
            {
                task.ContinueWith(t => tcs.TrySetResult(t));
            }
            return tcs.Task;
        }

        /// <summary>
        /// Creates a new, completed task for the given result.
        /// </summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="p_result"></param>
        /// <returns>A completed task.</returns>
        public static UnityTask<T> FromResult<T>(T p_result)
        {
            var tcs = new UnityTaskCompletionSource<T>();
            tcs.SetResult(p_result);
            return tcs.Task;
        }

        /// <summary>
        /// Creates a new, completed task for the given result.
        /// </summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="p_result"></param>
        /// <returns>A completed task.</returns>
        public static UnityTask<T> FromException<T>(Exception p_exception)
        {
            var tcs = new UnityTaskCompletionSource<T>();
            tcs.SetException(p_exception);
            return tcs.Task;
        }

        /// <summary>
        /// Creates a new, completed task for the given result.
        /// </summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="p_result"></param>
        /// <returns>A completed task.</returns>
        public static UnityTask FromException(Exception p_exception)
        {
            var tcs = new UnityTaskCompletionSource<int>();
            tcs.SetException(p_exception);
            return tcs.Task;
        }

        /// <summary>
        /// Creates a task that will complete successfully after the given timespan.
        /// </summary>
        /// <param name="p_timespan">The amount of time to wait.</param>
        /// <returns>A new task.</returns>
        public static UnityTask RunDelay(float p_seconds, CancellationToken p_cancellationToken)
        {
            var tcs = new UnityTaskCompletionSource<int>();
            var cancellation = p_cancellationToken.Register(() => tcs.TrySetCanceled());
            tcs.Task.TaskGenerator = () => DelayCoroutine(p_seconds);
            tcs.Task.ReturnResult = _ =>
            {
                try
                {
                    tcs.TrySetResult(0);
                    cancellation.Dispose();
                }
                catch (Exception e)
                {
                    tcs.TrySetException(e);
                    cancellation.Dispose();
                }
            };
            if (Thread.CurrentThread.IsBackground)// 如果当前不在前端线程，则切换
            {
                ForegroundInvoker.Invoke(() =>
                {
                    tcs.Task.TaskCoroutine = UnityTaskScheduler.FromCurrentSynchronizationContext().Post(tcs.Task.TaskGenerator(), tcs.Task.ReturnResult);
                });
            }
            else
            {
                tcs.Task.TaskCoroutine = UnityTaskScheduler.FromCurrentSynchronizationContext().Post(tcs.Task.TaskGenerator(), tcs.Task.ReturnResult);
            }
            return tcs.Task;
        }

        /// <summary>
        /// Creates a task that will complete successfully after the given timespan.
        /// </summary>
        /// <param name="p_timespan">The amount of time to wait.</param>
        /// <returns>A new task.</returns>
        public static UnityTask RunDelay(float p_seconds)
        {
            return RunDelay(p_seconds, CancellationToken.None);
        }

        #endregion

        #region private static functions

        private static IEnumerator DelayCoroutine(float p_seconds)
        {
            yield return new WaitForSeconds(p_seconds);
        }

        #endregion
    }
}

