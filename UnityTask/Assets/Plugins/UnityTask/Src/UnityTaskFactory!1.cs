namespace UnityEngine.TaskExtension
{
    using System;
    using System.Collections;

    /// <summary>
    /// Provides support for creating and scheduling Task<TResult> objects.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class UnityTaskFactory<TResult>
    {
        #region private members

        private readonly UnityTaskFactory m_factory;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a TaskFactory<TResult> instance with the specified configuration.
        /// </summary>
        /// <param name="p_scheduler"></param>
        /// <param name="p_cancellationToken"></param>
        internal UnityTaskFactory()
        {
            this.m_factory = new UnityTaskFactory();
        }

        #endregion

        #region public functions

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <param name="p_func"></param>
        /// <returns></returns>
        public UnityTask<TResult> StartNew(Func<System.Collections.IEnumerator> p_func)
        {
            return m_factory.StartNew<TResult>(p_func);
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <param name="func"></param>
        /// <param name="p_state"></param>
        /// <returns></returns>
        public UnityTask<TResult> StartNew(Func<object, System.Collections.IEnumerator> p_func, object p_state)
        {
            return m_factory.StartNew<TResult>(p_func, p_state);
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <param name="func"></param>
        /// <param name="p_arg1"></param>
        /// <returns></returns>
        public UnityTask<TResult> StartNew<TArg1>(Func<TArg1, System.Collections.IEnumerator> p_func, TArg1 p_arg1)
        {
            return m_factory.StartNew<TArg1, TResult>(p_func, p_arg1);
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <returns></returns>
        public UnityTask<TResult> StartNew<TArg1, TArg2>(Func<TArg1, TArg2, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2)
        {
            return m_factory.StartNew<TArg1, TArg2, TResult>(p_func, p_arg1, p_arg2);
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <returns></returns>
        public UnityTask<TResult> StartNew<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3)
        {
            return m_factory.StartNew<TArg1, TArg2, TArg3, TResult>(p_func, p_arg1, p_arg2, p_arg3);
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <returns></returns>
        public UnityTask<TResult> StartNew<TArg1, TArg2, TArg3, TArg4>(Func<TArg1, TArg2, TArg3, TArg4, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4)
        {
            return m_factory.StartNew<TArg1, TArg2, TArg3, TArg4, TResult>(p_func, p_arg1, p_arg2, p_arg3, p_arg4);
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <typeparam name="TArg3"></typeparam>
        /// <typeparam name="TArg4"></typeparam>
        /// <typeparam name="TArg5"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <returns></returns>
        public UnityTask<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5)
        {
            return m_factory.StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(p_func, p_arg1, p_arg2, p_arg3, p_arg4, p_arg5);
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
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <param name="p_arg6"></param>
        /// <returns></returns>
        public UnityTask<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6)
        {
            return m_factory.StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(p_func, p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6);
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
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <param name="p_arg2"></param>
        /// <param name="p_arg3"></param>
        /// <param name="p_arg4"></param>
        /// <param name="p_arg5"></param>
        /// <param name="p_arg6"></param>
        /// <param name="p_arg7"></param>
        /// <returns></returns>
        public UnityTask<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6, TArg7 p_arg7)
        {
            return m_factory.StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(p_func, p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6, p_arg7);
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
        public UnityTask<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6, TArg7 p_arg7, TArg8 p_arg8)
        {
            return m_factory.StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(p_func, p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6, p_arg7, p_arg8);
        }

        /// <summary>
        /// Creates a continuation task that starts when a set of specified tasks has completed.
        /// </summary>
        /// <param name="p_tasks"></param>
        /// <param name="p_continuationAction"></param>
        /// <returns></returns>
        public void ContinueWhenAll(UnityTask[] p_tasks, Action<UnityTask[]> p_continuationAction)
        {
            m_factory.ContinueWhenAll(p_tasks, p_continuationAction);
        }

        /// <summary>
        /// Creates a continuation task that starts when a set of specified tasks has completed.
        /// </summary>
        /// <param name="p_tasks"></param>
        /// <param name="p_continuationAction"></param>
        /// <returns></returns>
        public UnityTask<UnityTask<T>> ContinueWhenAll<T>(UnityTask[] p_tasks, Func<UnityTask[], System.Collections.IEnumerator> p_continuationAction)
        {
            return m_factory.ContinueWhenAll<T>(p_tasks, p_continuationAction);
        }

        /// <summary>
        /// Creates a continuation task that starts when a set of specified tasks has completed.
        /// </summary>
        /// <param name="p_tasks"></param>
        /// <param name="p_continuationAction"></param>
        /// <returns></returns>
        public UnityTask ContinueWhenAll(UnityTask[] p_tasks, Func<UnityTask[], System.Collections.IEnumerator> p_continuationAction)
        {
            return m_factory.ContinueWhenAll(p_tasks, p_continuationAction);
        }

        #endregion
    }
}

