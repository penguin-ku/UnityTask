namespace System.Threading.Tasks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides support for creating and scheduling Task objects.
    /// </summary>
    public class TaskFactory
    {
        #region private members

        private readonly TaskScheduler m_scheduler;
        private readonly CancellationToken m_cancellationToken;

        #endregion

        #region public properties

        /// <summary>
        /// Gets the default task scheduler for this task factory.
        /// </summary>
        public TaskScheduler Scheduler
        {
            get
            {
                return m_scheduler;
            }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a TaskFactory instance with the specified configuration.
        /// </summary>
        /// <param name="p_scheduler"></param>
        /// <param name="p_cancellationToken"></param>
        public TaskFactory(TaskScheduler p_scheduler, CancellationToken p_cancellationToken)
        {
            this.m_scheduler = p_scheduler;
            this.m_cancellationToken = p_cancellationToken;
        }

        /// <summary>
        /// Initializes a TaskFactory instance with the specified configuration.
        /// </summary>
        /// <param name="p_scheduler"></param>
        public TaskFactory(TaskScheduler p_scheduler)
          : this(p_scheduler, CancellationToken.None)
        {
        }

        /// <summary>
        /// Initializes a TaskFactory instance with the specified configuration.
        /// </summary>
        /// <param name="p_cancellationToken"></param>
        public TaskFactory(CancellationToken p_cancellationToken)
          : this(TaskScheduler.FromCurrentSynchronizationContext(), p_cancellationToken)
        {
        }

        /// <summary>
        /// Initializes a TaskFactory instance with the default configuration.
        /// </summary>
        public TaskFactory()
          : this(TaskScheduler.FromCurrentSynchronizationContext(), CancellationToken.None)
        {
        }

        /// <summary>
        /// Initializes a TaskFactory instance with the default configuration.
        /// </summary>
        /// <param name="p_cancellationToken"></param>
        /// <param name="p_creationOptions"></param>
        /// <param name="p_continuationOptions"></param>
        /// <param name="p_scheduler"></param>
        public TaskFactory(CancellationToken p_cancellationToken, TaskCreationOptions p_creationOptions, TaskContinuationOptions p_continuationOptions, TaskScheduler p_scheduler)
          : this(p_scheduler, p_cancellationToken)
        {
            // Just ignore the other arguments -- we don't use them.
        }

        #endregion

        #region public functions

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <param name="p_func"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TResult>(Func<TResult> p_func)
        {
            var tcs = new TaskCompletionSource<TResult>();
            m_scheduler.Post(() =>
            {
                try
                {
                    tcs.SetResult(p_func());
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TResult>(Func<object, TResult> p_func, object state)
        {
            return StartNew(() => p_func(state));
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TResult>(Func<TArg1, TResult> p_func, TArg1 p_arg1)
        {
            return StartNew(() => p_func(p_arg1));
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2, TResult>(Func<TArg1, TArg2, TResult> p_func, TArg1 p_arg1, TArg2 p_arg2)
        {
            return StartNew(() => p_func(p_arg1, p_arg2));
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2, TArg3, TResult>(Func<TArg1, TArg2, TArg3, TResult> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3)
        {
            return StartNew(() => p_func(p_arg1, p_arg2, p_arg3));
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TResult> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4)
        {
            return StartNew(() => p_func(p_arg1, p_arg2, p_arg3, p_arg4));
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TArg5"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5)
        {
            return StartNew(() => p_func(p_arg1, p_arg2, p_arg3, p_arg4, p_arg5));
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TArg5"></typeparam>
        /// <typeparam name="TArg6"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <param name="p_arg6"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6)
        {
            return StartNew(() => p_func(p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6));
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TArg5"></typeparam>
        /// <typeparam name="TArg6"></typeparam>
        /// <typeparam name="TArg7"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <param name="p_arg6"></param>
        /// <param name="p_arg7"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6, TArg7 p_arg7)
        {
            return StartNew(() => p_func(p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6, p_arg7));
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TArg5"></typeparam>
        /// <typeparam name="TArg6"></typeparam>
        /// <typeparam name="TArg7"></typeparam>
        /// <typeparam name="TArg8"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <param name="p_arg6"></param>
        /// <param name="p_arg7"></param>
        /// <param name="p_arg8"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6, TArg7 p_arg7, TArg8 p_arg8)
        {
            return StartNew(() => p_func(p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6, p_arg7, p_arg8));
        }

        /// <summary>
        /// Creates a Task that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task FromAsync(Func<AsyncCallback, object, IAsyncResult> p_beginMethod, Action<IAsyncResult> p_endMethod, object p_state)
        {
            return FromAsync(p_beginMethod, result =>
            {
                p_endMethod(result);
                return 0;
            }, p_state);
        }

        /// <summary>
        /// Creates a Task that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task FromAsync<TArg1>(Func<TArg1, AsyncCallback, object, IAsyncResult> p_beginMethod, Action<IAsyncResult> p_endMethod, TArg1 p_arg1, object p_state)
        {
            return FromAsync((callback, _) => p_beginMethod(p_arg1, callback, p_state), p_endMethod, p_state);
        }

        /// <summary>
        /// Creates a Task that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task FromAsync<TArg1, TArg2>(Func<TArg1, TArg2, AsyncCallback, object, IAsyncResult> p_beginMethod, Action<IAsyncResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, object p_state)
        {
            return FromAsync((callback, _) => p_beginMethod(p_arg1, p_arg2, callback, p_state), p_endMethod, p_state);
        }

        /// <summary>
        /// Creates a Task that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task FromAsync<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, AsyncCallback, object, IAsyncResult> p_beginMethod, Action<IAsyncResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, object p_state)
        {
            return FromAsync((callback, _) => p_beginMethod(p_arg1, p_arg2, p_arg3, callback, p_state), p_endMethod, p_state);
        }

        /// <summary>
        /// Creates a Task that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task FromAsync<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, AsyncCallback, object, IAsyncResult> p_beginMethod, Action<IAsyncResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, object p_state)
        {
            return FromAsync((callback, _) => p_beginMethod(p_arg1, p_arg2, p_arg3, p_arg4, callback, p_state), p_endMethod, p_state);
        }

        /// <summary>
        /// Creates a Task that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TArg5"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task FromAsync<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, AsyncCallback, object, IAsyncResult> p_beginMethod, Action<IAsyncResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, object p_state)
        {
            return FromAsync((callback, _) => p_beginMethod(p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, callback, p_state), p_endMethod, p_state);
        }

        /// <summary>
        /// Creates a Task that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TArg5"></typeparam>
        /// <typeparam name="TArg6"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <param name="p_arg6"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task FromAsync<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, AsyncCallback, object, IAsyncResult> p_beginMethod, Action<IAsyncResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6, object p_state)
        {
            return FromAsync((callback, _) => p_beginMethod(p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6, callback, p_state), p_endMethod, p_state);
        }

        /// <summary>
        /// Creates a Task<TResult> that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Task<TResult> FromAsync<TResult>(Func<AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, object state)
        {
            var tcs = new TaskCompletionSource<TResult>();
            var cancellation = m_cancellationToken.Register(() => tcs.TrySetCanceled());
            if (m_cancellationToken.IsCancellationRequested)
            {
                tcs.TrySetCanceled();
                cancellation.Dispose();
                return tcs.Task;
            }
            try
            {
                p_beginMethod(result =>
                {
                    try
                    {
                        var value = p_endMethod(result);
                        tcs.TrySetResult(value);
                        cancellation.Dispose();
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                        cancellation.Dispose();
                    }
                }, state);
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
                cancellation.Dispose();
            }
            return tcs.Task;
        }

        /// <summary>
        /// Creates a Task<TResult> that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="arg1"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Task<TResult> FromAsync<TArg1, TResult>(Func<TArg1, AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, TArg1 arg1, object state)
        {
            return FromAsync((callback, _) => p_beginMethod(arg1, callback, state), p_endMethod, state);
        }

        /// <summary>
        /// Creates a Task<TResult> that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task<TResult> FromAsync<TArg1, TArg2, TResult>(Func<TArg1, TArg2, AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, object p_state)
        {
            return FromAsync((callback, _) => p_beginMethod(p_arg1, p_arg2, callback, p_state), p_endMethod, p_state);
        }

        /// <summary>
        /// Creates a Task that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult>(Func<TArg1, TArg2, TArg3, AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, object p_state)
        {
            return FromAsync((callback, _) => p_beginMethod(p_arg1, p_arg2, p_arg3, callback, p_state), p_endMethod, p_state);
        }

        /// <summary>
        /// Creates a Task that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TArg4, TResult>(Func<TArg1, TArg2, TArg3, TArg4, AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, object p_state)
        {
            return FromAsync((callback, _) => p_beginMethod(p_arg1, p_arg2, p_arg3, p_arg4, callback, p_state), p_endMethod, p_state);
        }

        /// <summary>
        /// Creates a Task that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TArg5"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, object p_state)
        {
            return FromAsync((callback, _) => p_beginMethod(p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, callback, p_state), p_endMethod, p_state);
        }

        /// <summary>
        /// Creates a Task that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TArg5"></typeparam>
        /// <typeparam name="TArg6"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <param name="p_arg6"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6, object p_state)
        {
            return FromAsync((callback, _) => p_beginMethod(p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6, callback, p_state), p_endMethod, p_state);
        }

        /// <summary>
        /// Creates a continuation task that starts when a set of specified tasks has completed.
        /// </summary>
        /// <param name="p_tasks"></param>
        /// <param name="p_continuationAction"></param>
        /// <returns></returns>
        public Task ContinueWhenAll(Task[] p_tasks, Action<Task[]> p_continuationAction)
        {
            int remaining = p_tasks.Length;
            var tcs = new TaskCompletionSource<Task[]>();
            if (remaining == 0)
            {
                tcs.TrySetResult(p_tasks);
            }
            foreach (var task in p_tasks)
            {
                task.ContinueWith(_ =>
                {
                    if (Interlocked.Decrement(ref remaining) == 0)
                    {
                        tcs.TrySetResult(p_tasks);
                    }
                });
            }
            return tcs.Task.ContinueWith(t =>
            {
                p_continuationAction(t.Result);
            });
        }

        #endregion
    }

}

