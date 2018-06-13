namespace System.Threading.Tasks
{
    using System;

    /// <summary>
    /// Provides support for creating and scheduling Task<TResult> objects.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class TaskFactory<TResult>
    {
        #region private members

        private readonly System.Threading.Tasks.TaskFactory m_factory;

        #endregion

        #region public properties

        /// <summary>
        /// Gets the default task scheduler for this task factory.
        /// </summary>
        public System.Threading.Tasks.TaskScheduler Scheduler
        {
            get
            {
                return this.m_factory.Scheduler;
            }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a TaskFactory<TResult> instance with the default configuration.
        /// </summary>
        public TaskFactory() : this(TaskScheduler.FromCurrentSynchronizationContext(), CancellationToken.None)
        {
        }

        /// <summary>
        /// Initializes a TaskFactory<TResult> instance with the default configuration.
        /// </summary>
        /// <param name="p_cancellationToken"></param>
        public TaskFactory(CancellationToken p_cancellationToken) : this(TaskScheduler.FromCurrentSynchronizationContext(), p_cancellationToken)
        {
        }

        /// <summary>
        /// Initializes a TaskFactory<TResult> instance with the specified configuration.
        /// </summary>
        /// <param name="p_scheduler"></param>
        public TaskFactory(TaskScheduler p_scheduler) : this(p_scheduler, CancellationToken.None)
        {
        }

        /// <summary>
        /// Initializes a TaskFactory<TResult> instance with the specified configuration.
        /// </summary>
        /// <param name="p_scheduler"></param>
        /// <param name="p_cancellationToken"></param>
        internal TaskFactory(TaskScheduler p_scheduler, CancellationToken p_cancellationToken)
        {
            this.m_factory = new TaskFactory(p_scheduler, p_cancellationToken);
        }

        #endregion

        #region public functions

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Task<TResult> StartNew(Func<TResult> func)
        {
            return m_factory.StartNew<TResult>(func);
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <param name="func"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Task<TResult> StartNew(Func<object, TResult> func, object state)
        {
            return m_factory.StartNew<TResult>(func, state);
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <param name="func"></param>
        /// <param name="p_arg1"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1>(Func<TArg1, TResult> func, TArg1 p_arg1)
        {
            return m_factory.StartNew<TArg1, TResult>(func, p_arg1);
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2>(Func<TArg1, TArg2, TResult> func, TArg1 p_arg1, TArg2 p_arg2)
        {
            return m_factory.StartNew<TArg1, TArg2, TResult>(func, p_arg1, p_arg2);
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <param name="func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, TResult> func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3)
        {
            return m_factory.StartNew<TArg1, TArg2, TArg3, TResult>(func, p_arg1, p_arg2, p_arg3);
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <param name="func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, TResult> func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4)
        {
            return m_factory.StartNew<TArg1, TArg2, TArg3, TArg4, TResult>(func, p_arg1, p_arg2, p_arg3, p_arg4);
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TArg5"></typeparam>
        /// <param name="func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult> func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5)
        {
            return m_factory.StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(func, p_arg1, p_arg2, p_arg3, p_arg4, p_arg5);
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
        /// <param name="func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <param name="p_arg6"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6)
        {
            return m_factory.StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(func, p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6);
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
        /// <param name="func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <param name="p_arg6"></param>
        /// <param name="p_arg7"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6, TArg7 p_arg7)
        {
            return m_factory.StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(func, p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6, p_arg7);
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
        /// <param name="func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <param name="p_arg6"></param>
        /// <param name="p_arg7"></param>
        /// <param name="p_arg8"></param>
        /// <returns></returns>
        public Task<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6, TArg7 p_arg7, TArg8 p_arg8)
        {
            return m_factory.StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(func, p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6, p_arg7, p_arg8);
        }

        /// <summary>
        /// Creates a Task<TResult> that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task<TResult> FromAsync(Func<AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, object p_state)
        {
            return m_factory.FromAsync<TResult>(p_beginMethod, p_endMethod, p_state);
        }

        /// <summary>
        /// Creates a Task<TResult> that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task<TResult> FromAsync<TArg1>(Func<TArg1, AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, TArg1 p_arg1, object p_state)
        {
            return m_factory.FromAsync<TArg1, TResult>(p_beginMethod, p_endMethod, p_arg1, p_state);
        }

        /// <summary>
        /// Creates a Task<TResult> that represents a pair of begin and end methods that conform to the Asynchronous Programming Model pattern.
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="p_beginMethod"></param>
        /// <param name="p_endMethod"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public Task<TResult> FromAsync<TArg1, TArg2>(Func<TArg1, TArg2, AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, object p_state)
        {
            return m_factory.FromAsync<TArg1, TArg2, TResult>(p_beginMethod, p_endMethod, p_arg1, p_arg2, p_state);
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
        public Task<TResult> FromAsync<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, object p_state)
        {
            return m_factory.FromAsync<TArg1, TArg2, TArg3, TResult>(p_beginMethod, p_endMethod, p_arg1, p_arg2, p_arg3, p_state);
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
        public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, object p_state)
        {
            return m_factory.FromAsync<TArg1, TArg2, TArg3, TArg4, TResult>(p_beginMethod, p_endMethod, p_arg1, p_arg2, p_arg3, p_arg4, p_state);
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
        public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, object p_state)
        {
            return m_factory.FromAsync<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(p_beginMethod, p_endMethod, p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_state);
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
        public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, AsyncCallback, object, IAsyncResult> p_beginMethod, Func<IAsyncResult, TResult> p_endMethod, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6, object p_state)
        {
            return m_factory.FromAsync<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(p_beginMethod, p_endMethod, p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6, p_state);
        }

        /// <summary>
        /// Creates a continuation task that starts when a set of specified tasks has completed.
        /// </summary>
        /// <param name="p_tasks"></param>
        /// <param name="p_continuationAction"></param>
        /// <returns></returns>
        public Task ContinueWhenAll(Task[] p_tasks, Action<Task[]> p_continuationAction)
        {
            return m_factory.ContinueWhenAll(p_tasks, p_continuationAction);
        }

        #endregion
    }
}

