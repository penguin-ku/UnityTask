namespace UnityEngine.TaskExtension
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides support for creating and scheduling Task objects.
    /// </summary>
    public class UnityTaskFactory
    {

        #region constructors

        /// <summary>
        /// Initializes a TaskFactory instance with the specified configuration.
        /// </summary>
        /// <param name="p_cancellationToken"></param>
        public UnityTaskFactory()
        {
        }

        #endregion

        #region public functions

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <param name="p_func"></param>
        /// <returns></returns>
        public UnityTask<TResult> StartNew<TResult>(Func<System.Collections.IEnumerator> p_func)
        {
            var tcs = new UnityTaskCompletionSource<TResult>();
            tcs.Task.TaskGenerator = () => p_func();
            tcs.Task.ReturnResult = result =>
            {
                try
                {
                    tcs.SetResult((TResult)result);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            };
            tcs.Task.TaskCoroutine = UnityTaskScheduler.FromCurrentSynchronizationContext().Post(tcs.Task.TaskGenerator(), tcs.Task.ReturnResult);
            return tcs.Task;
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public UnityTask<TResult> StartNew<TResult>(Func<object, System.Collections.IEnumerator> p_func, object state)
        {
            return StartNew<TResult>(() => p_func(state));
        }

        /// <summary>
        /// Creates and starts a Task<TResult>
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_func"></param>
        /// <param name="p_arg1"></param>
        /// <returns></returns>
        public UnityTask<TResult> StartNew<TArg1, TResult>(Func<TArg1, System.Collections.IEnumerator> p_func, TArg1 p_arg1)
        {
            return StartNew<TResult>(() => p_func(p_arg1));
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
        public UnityTask<TResult> StartNew<TArg1, TArg2, TResult>(Func<TArg1, TArg2, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2)
        {
            return StartNew<TResult>(() => p_func(p_arg1, p_arg2));
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
        public UnityTask<TResult> StartNew<TArg1, TArg2, TArg3, TResult>(Func<TArg1, TArg2, TArg3, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3)
        {
            return StartNew<TResult>(() => p_func(p_arg1, p_arg2, p_arg3));
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
        public UnityTask<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TResult>(Func<TArg1, TArg2, TArg3, TArg4, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4)
        {
            return StartNew<TResult>(() => p_func(p_arg1, p_arg2, p_arg3, p_arg4));
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
        public UnityTask<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5)
        {
            return StartNew<TResult>(() => p_func(p_arg1, p_arg2, p_arg3, p_arg4, p_arg5));
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
        public UnityTask<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6)
        {
            return StartNew<TResult>(() => p_func(p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6));
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
        public UnityTask<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6, TArg7 p_arg7)
        {
            return StartNew<TResult>(() => p_func(p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6, p_arg7));
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
        public UnityTask<TResult> StartNew<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, System.Collections.IEnumerator> p_func, TArg1 p_arg1, TArg2 p_arg2, TArg3 p_arg3, TArg4 p_arg4, TArg5 p_arg5, TArg6 p_arg6, TArg7 p_arg7, TArg8 p_arg8)
        {
            return StartNew<TResult>(() => p_func(p_arg1, p_arg2, p_arg3, p_arg4, p_arg5, p_arg6, p_arg7, p_arg8));
        }

        /// <summary>
        /// Creates a continuation task that starts when a set of specified tasks has completed.
        /// </summary>
        /// <param name="p_tasks"></param>
        /// <param name="p_continuationAction"></param>
        /// <returns></returns>
        public UnityTask<T> ContinueWhenAll<T>(UnityTask[] p_tasks, Func<UnityTask[], System.Collections.IEnumerator> p_continuationAction)
        {
            int remaining = p_tasks.Length;
            var tcs = new UnityTaskCompletionSource<UnityTask[]>();
            if (remaining == 0)
            {
                tcs.TrySetResult(p_tasks);
            }
            foreach (var task in p_tasks)
            {
                task.ContinueWith(_ =>
                {
                    remaining--;
                    if (remaining == 0)
                    {
                        tcs.TrySetResult(p_tasks);
                    }
                });
            }
            return tcs.Task.ContinueWith<T>(t => p_continuationAction(t.Result));
        }

        /// <summary>
        /// Creates a continuation task that starts when a set of specified tasks has completed.
        /// </summary>
        /// <param name="p_tasks"></param>
        /// <param name="p_continuationAction"></param>
        /// <returns></returns>
        public UnityTask ContinueWhenAll(UnityTask[] p_tasks, Func<UnityTask[], System.Collections.IEnumerator> p_continuationAction)
        {
            int remaining = p_tasks.Length;
            var tcs = new UnityTaskCompletionSource<UnityTask[]>();
            if (remaining == 0)
            {
                tcs.TrySetResult(p_tasks);
            }
            foreach (var task in p_tasks)
            {
                task.ContinueWith(_ =>
                {
                    remaining--;
                    if (remaining == 0)
                    {
                        tcs.TrySetResult(p_tasks);
                    }
                });
            }
            return tcs.Task.ContinueWith<UnityTask[]>(t => p_continuationAction(t.Result));
        }

        /// <summary>
        /// Creates a continuation task that starts when a set of specified tasks has completed.
        /// </summary>
        /// <param name="p_tasks"></param>
        /// <param name="p_continuationAction"></param>
        /// <returns></returns>
        public void ContinueWhenAll(UnityTask[] p_tasks, Action<UnityTask[]> p_continuationAction)
        {
            int remaining = p_tasks.Length;
            if (remaining == 0)
            {
                p_continuationAction(p_tasks);
            }
            foreach (var task in p_tasks)
            {
                task.ContinueWith(_ =>
                {
                    remaining--;
                    if (remaining == 0)
                    {
                        p_continuationAction(p_tasks);
                    }
                });
            }
        }

        #endregion
    }

}

