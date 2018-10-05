namespace System.Threading.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Represents an asynchronous task.
    /// </summary>
    public abstract class Task
    {
        public static Task None
        {
            get
            {
                return Task.FromResult(default(int));
            }
        }

        private static readonly ThreadLocal<int> t_executionDepth = new ThreadLocal<int>(() => 0);
        private static readonly Action<Action> g_immediateExecutor = a =>
        {
            // TODO (hallucinogen): remove this after Unity resolves the ThreadPool problem.
            bool IsCompiledByIL2CPP = System.AppDomain.CurrentDomain.FriendlyName.Equals("IL2CPP Root Domain");
            int maxDepth = 10;
            if (IsCompiledByIL2CPP)
            {
                maxDepth = 200;
            }
            t_executionDepth.Value++;
            try
            {
                if (t_executionDepth.Value <= maxDepth)
                {
                    a();
                }
                else
                {
                    Factory.Scheduler.Post(a);
                }
            }
            finally
            {
                t_executionDepth.Value--;
            }
        };

        #region protected members

        protected readonly object m_mutex = new object();
        protected IList<Action<Task>> m_continuations = new List<Action<Task>>();
        protected AggregateException m_exception;
        protected bool m_isCompleted;
        protected bool m_isCanceled;

        #endregion

        #region constructors

        internal Task()
        {
        }

        #endregion

        #region public properties

        /// <summary>
        /// 获取工厂
        /// </summary>
        internal static TaskFactory Factory
        {
            get
            {
                return new TaskFactory();
            }
        }

        /// <summary>
        /// Gets the exceptions for the task, if there are any. <c>null</c> otherwise.
        /// </summary>
        public AggregateException Exception
        {
            get
            {
                lock (m_mutex)
                {
                    return m_exception;
                }
            }
        }

        /// <summary>
        /// Gets whether the task was cancelled.
        /// </summary>
        public bool IsCanceled
        {
            get
            {
                lock (m_mutex)
                {
                    return m_isCanceled;
                }
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
                lock (m_mutex)
                {
                    return m_isCompleted;
                }
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
        /// Blocks until the task is complete.
        /// </summary>
        public void Wait()
        {
            lock (m_mutex)
            {
                if (!IsCompleted)
                {
                    Monitor.Wait(m_mutex);//Releases the lock on an object and blocks the current thread until it reacquires the lock.
                }
                if (IsFaulted)
                {
                    throw Exception;
                }
            }
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <typeparam name="T">The type returned by the continuation.</typeparam>
        /// <param name="p_continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument and can return a value.</param>
        /// <returns>A new Task that returns the value returned by the continuation after both
        /// the task and the continuation are complete.</returns>
        public Task<T> ContinueWith<T>(Func<Task, T> p_continuation)
        {
            return ContinueWith(p_continuation, CancellationToken.None);
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
        public Task<T> ContinueWith<T>(Func<Task, T> p_continuation, CancellationToken p_cancellationToken)
        {
            bool completed = false;
            var tcs = new TaskCompletionSource<T>();
            var cancellation = p_cancellationToken.Register(() => tcs.TrySetCanceled());
            Action<Task> completeTask = t =>
            {
                g_immediateExecutor(() =>
                {
                    try
                    {
                        tcs.TrySetResult(p_continuation(t));
                        cancellation.Dispose();
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                        cancellation.Dispose();
                    }
                });
            };

            // 此处防止判断为false之后正好执行完毕
            lock (m_mutex)
            {
                completed = IsCompleted;
                if (!completed)
                {
                    m_continuations.Add(completeTask);
                }
            }
            if (completed)
            {
                if (Thread.CurrentThread.IsBackground)
                {
                    completeTask(this);
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(p => 
                    {
                        completeTask(this);
                    }));
                }
            }

            return tcs.Task;
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <param name="continuation">The continuation to run after the task completes.
        /// The function takes the completed task as an argument.</param>
        /// <returns>A new Task that is complete after both the task and the continuation are
        /// complete.</returns>
        public Task ContinueWith(Action<Task> continuation)
        {
            return ContinueWith(continuation, CancellationToken.None);
        }

        /// <summary>
        /// Registers a continuation for the task that will run when the task is complete.
        /// </summary>
        /// <param name="p_continuation">The continuation to run after the task completes.</param>
        /// <param name="p_cancellationToken">The cancellation token.</param>
        /// <returns>A new Task that is complete after both the task and the continuation are
        /// complete.</returns>
        public Task ContinueWith(Action<Task> p_continuation, CancellationToken p_cancellationToken)
        {
            return ContinueWith<int>(t =>
            {
                p_continuation(t);
                return 0;
            }, p_cancellationToken);
        }

        #endregion

        #region static public functions

        /// <summary>
        /// Creates a task that is complete when all of the provided tasks are complete.
        /// If any of the tasks has an exception, all exceptions raised in the tasks will
        /// be aggregated into the returned task. Otherwise, if any of the tasks is cancelled,
        /// the returned task will be cancelled.
        /// </summary>
        /// <param name="tasks">The tasks to aggregate.</param>
        /// <returns>A task that is complete when all of the provided tasks are complete.</returns>
        public static Task WhenAll(params Task[] tasks)
        {
            return WhenAll((IEnumerable<Task>)tasks);
        }

        /// <summary>
        /// Creates a task that is complete when all of the provided tasks are complete.
        /// If any of the tasks has an exception, all exceptions raised in the tasks will
        /// be aggregated into the returned task. Otherwise, if any of the tasks is cancelled,
        /// the returned task will be cancelled.
        /// </summary>
        /// <param name="p_tasks">The tasks to aggregate.</param>
        /// <returns>A task that is complete when all of the provided tasks are complete.</returns>
        public static Task WhenAll(IEnumerable<Task> p_tasks)
        {
            var taskArr = p_tasks.ToArray();
            if (taskArr.Length == 0)
            {
                return Task.FromResult(0);
            }
            var tcs = new TaskCompletionSource<int>();
            Task.Factory.ContinueWhenAll(taskArr, _ =>
            {
                var exceptions = taskArr.Where(p => p.IsFaulted).Select(p => p.Exception).ToArray();
                if (exceptions.Length > 0)
                {
                    tcs.SetException(new AggregateException(exceptions));
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
        internal static Task<Task> WhenAny(params Task[] p_tasks)
        {
            return WhenAny((IEnumerable<Task>)p_tasks);
        }

        /// <summary>
        /// Waits for any of the provided Task objects to complete execution.
        /// </summary>
        /// <param name="p_tasks"></param>
        /// <returns></returns>
        internal static Task<Task> WhenAny(IEnumerable<Task> p_tasks)
        {
            var tcs = new TaskCompletionSource<Task>();
            foreach (var task in p_tasks)
            {
                task.ContinueWith(t => tcs.TrySetResult(t));
            }
            return tcs.Task;
        }

        /// <summary>
        /// Creates a task that is complete when all of the provided tasks are complete.
        /// If any of the tasks has an exception, all exceptions raised in the tasks will
        /// be aggregated into the returned task. Otherwise, if any of the tasks is cancelled,
        /// the returned task will be cancelled. If all of the tasks succeed, the result of the
        /// returned task will be an array containing the results of all of the input tasks.
        /// </summary>
        /// <typeparam name="T">The result type of the tasks.</typeparam>
        /// <param name="p_tasks">The tasks to aggregate.</param>
        /// <returns>A task that is complete when all of the provided tasks are complete.</returns>
        public static Task<T[]> WhenAll<T>(IEnumerable<Task<T>> p_tasks)
        {
            return WhenAll(p_tasks.Cast<Task>()).OnSuccess(_ => p_tasks.Select(t => t.Result).ToArray());
        }

        /// <summary>
        /// Creates a new, completed task for the given result.
        /// </summary>
        /// <typeparam name="T">The result type of the task.</typeparam>
        /// <param name="p_result"></param>
        /// <returns>A completed task.</returns>
        public static Task<T> FromResult<T>(T p_result)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(p_result);
            return tcs.Task;
        }

        /// <summary>
        /// Executes a function asynchronously, returning a task that represents the operation.
        /// </summary>
        /// <typeparam name="T">The return type of the task.</typeparam>
        /// <param name="p_toRun">The function to run.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task<T> Run<T>(Func<T> p_toRun)
        {
            return Task.Factory.StartNew(p_toRun);
        }

        /// <summary>
        /// Executes an action asynchronously, returning a task that represents the operation.
        /// </summary>
        /// <param name="p_toRun">The action to run.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task Run(Action p_toRun)
        {
            return Task.Factory.StartNew(() =>
            {
                p_toRun();
                return 0;
            });
        }

        /// <summary>
        /// Creates a task that will complete successfully after the given timespan.
        /// </summary>
        /// <param name="p_timespan">The amount of time to wait.</param>
        /// <returns>A new task.</returns>
        public static Task Delay(TimeSpan p_timespan)
        {
            var tcs = new TaskCompletionSource<int>();
            var timer = new Timer(_ =>
            {
                tcs.TrySetResult(0);
            });
            timer.Change(p_timespan, TimeSpan.FromMilliseconds(-1));
            return tcs.Task;
        }


        ///// <summary>
        ///// Creates a task that will complete successfully after the given timespan.
        ///// </summary>
        ///// <param name="p_timespan">The amount of time to wait.</param>
        ///// <returns>A new task.</returns>
        //public static Task Delay(TimeSpan p_timespan)
        //{
        //    var tcs = new TaskCompletionSource<int>();
        //    var timer = new Timer(_ =>
        //    {
        //        tcs.TrySetResult(0);
        //    });
        //    timer.Change(p_timespan, TimeSpan.FromMilliseconds(-1));
        //    return tcs.Task;
        //}

        /// <summary>Creates a <see cref="Task{TResult}"/> that's completed exceptionally with the specified exception.</summary>
        /// <typeparam name="TResult">The type of the result returned by the task.</typeparam>
        /// <param name="exception">The exception with which to complete the task.</param>
        /// <returns>The faulted task.</returns>
        public static Task<TResult> FromException<TResult>(Exception exception)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            tcs.TrySetException(exception);
            return tcs.Task;
        }


        /// <summary>Creates a <see cref="Task{TResult}"/> that's completed exceptionally with the specified exception.</summary>
        /// <typeparam name="TResult">The type of the result returned by the task.</typeparam>
        /// <param name="exception">The exception with which to complete the task.</param>
        /// <returns>The faulted task.</returns>
        public static Task FromException(Exception exception)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            tcs.TrySetException(exception);
            return tcs.Task;
        }

        #endregion
    }
}

