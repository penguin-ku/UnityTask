namespace UnityEngine.TaskExtension
{
    using System.Runtime.ExceptionServices;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Collections;
    using System.Threading.Tasks;

    /// <summary>
    /// Task扩展方法
    /// </summary>
    public static class UnityTaskExtensions
    {
        /// <summary>
        /// Unwraps a nested task, producing a task that is complete when both the outer and inner tasks are complete. This is primarily useful for chaining asynchronous operations together.
        /// </summary>
        /// <param name="p_task">The task to unwrap.</param>
        /// <returns>A new task that is complete when both the outer and inner tasks are complete.</returns>
        public static UnityTask Unwrap(this UnityTask<UnityTask> p_task)
        {
            var tcs = new UnityTaskCompletionSource<object>();
            p_task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    p_task.Result.ContinueWith(inner =>
                    {
                        if (inner.IsFaulted)
                        {
                            tcs.TrySetException(inner.Exception);
                        }
                        else if (inner.IsCanceled)
                        {
                            tcs.TrySetCanceled();
                        }
                        else
                        {
                            tcs.TrySetResult(null);
                        }
                    });
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// Unwraps a nested task, producing a task that is complete when both the outer
        /// and inner tasks are complete and that has the inner task's result.
        /// This is primarily useful for chaining asynchronous operations together.
        /// </summary>
        /// <param name="task">The task to unwrap.</param>
        /// <returns>A new task that is complete when both the outer and inner tasks
        /// are complete and that has the inner task's result.</returns>
        public static UnityTask<T> Unwrap<T>(this UnityTask<UnityTask<T>> task)
        {
            var tcs = new UnityTaskCompletionSource<T>();
            task.ContinueWith((UnityTask<UnityTask<T>> t) =>
            {
                if (t.IsFaulted)
                {
                    tcs.TrySetException(t.Exception);
                }
                else if (t.IsCanceled)
                {
                    tcs.TrySetCanceled();
                }
                else
                {
                    t.Result.ContinueWith(delegate (UnityTask<T> inner)
                    {
                        if (inner.IsFaulted)
                        {
                            tcs.TrySetException(inner.Exception);
                        }
                        else if (inner.IsCanceled)
                        {
                            tcs.TrySetCanceled();
                        }
                        else
                        {
                            tcs.TrySetResult(inner.Result);
                        }
                    });
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// 空任务
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        private static UnityTask<IEnumerable<TResult>> EndForNull<TResult>()
        {
            UnityTaskCompletionSource<IEnumerable<TResult>> source = new UnityTaskCompletionSource<IEnumerable<TResult>>();
            source.SetResult(null);
            return source.Task;
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        internal static void OnSuccess(this UnityTask p_task, Action<UnityTask> p_continuation)
        {
            p_task.ContinueWith(t => 
            {
                if (!t.IsFaulted && !t.IsCanceled)
                {
                    p_continuation(t);
                }
            });
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        internal static UnityTask<TResult> OnSuccess<TResult>(this UnityTask p_task, Func<UnityTask, IEnumerator> p_continuation, CancellationToken p_cancel)
        {
            return p_task.ContinueWith<TResult>(t =>
            {
                if (!t.IsFaulted && !t.IsCanceled)
                {
                    return p_continuation(t);
                }
                else
                {
                    return FromResultCoroutine(default(TResult));
                }
            }, p_cancel);
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        internal static UnityTask<TResult> OnSuccess<TResult>(this UnityTask p_task, Func<UnityTask, IEnumerator> p_continuation)
        {
            return p_task.OnSuccess<TResult>(p_continuation, CancellationToken.None);
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        internal static UnityTask OnSuccess(this UnityTask p_task, Func<UnityTask, IEnumerator> p_continuation, CancellationToken p_cancel)
        {
            return p_task.ContinueWith(t =>
            {
                if (!t.IsFaulted && !t.IsCanceled)
                {
                    return p_continuation(t);
                }
                else
                {
                    return FromResultCoroutine();
                }
            }, p_cancel);
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        internal static UnityTask OnSuccess(this UnityTask p_task, Func<UnityTask, IEnumerator> p_continuation)
        {
            return p_task.OnSuccess(p_continuation, CancellationToken.None);
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        internal static UnityTask<TResult> OnSuccess<TResult>(this UnityTask p_task, Func<IEnumerator> p_continuation, CancellationToken p_cancel)
        {
            return p_task.ContinueWith<TResult>(t =>
            {
                if (!t.IsFaulted && !t.IsCanceled)
                {
                    return p_continuation();
                }
                else
                {
                    return FromResultCoroutine(default(TResult));
                }
            }, p_cancel);
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        internal static UnityTask<TResult> OnSuccess<TResult>(this UnityTask p_task, Func<IEnumerator> p_continuation)
        {
            return p_task.OnSuccess<TResult>(p_continuation, CancellationToken.None);
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        internal static UnityTask OnSuccess(this UnityTask p_task, Func<IEnumerator> p_continuation, CancellationToken p_cancel)
        {
            return p_task.ContinueWith(t =>
            {
                if (!t.IsFaulted && !t.IsCanceled)
                {
                    return p_continuation();
                }
                else
                {
                    return FromResultCoroutine();
                }
            }, p_cancel);
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        internal static UnityTask OnSuccess(this UnityTask p_task, Func<IEnumerator> p_continuation)
        {
            return p_task.OnSuccess(p_continuation, CancellationToken.None);
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        internal static void OnSuccess<TIn>(this UnityTask<TIn> p_task, Action<UnityTask<TIn>> p_continuation)
        {
            p_task.OnSuccess((UnityTask t) => p_continuation((UnityTask<TIn>)t));
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        internal static UnityTask<TResult> OnSuccess<TIn, TResult>(this UnityTask<TIn> p_task, Func<UnityTask<TIn>, IEnumerator> p_continuation)
        {
            return p_task.OnSuccess<TResult>((UnityTask t) => p_continuation((UnityTask<TIn>)t));
        }


        private static IEnumerator FromResultCoroutine<T>(T p_result)
        {
            yield return p_result;
        }

        private static IEnumerator FromResultCoroutine()
        {
            yield return null;
        }

        /// <summary>
        /// switch to backgroud processor, thread pool
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task<TResult> ContinueToBackground<TResult>(this UnityTask p_task, Func<TResult> p_continuation)
        {
            return p_task.ContinueToBackground(p_continuation, CancellationToken.None);
        }

        /// <summary>
        /// switch to backgroud processor, thread pool
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task<TResult> ContinueToBackground<TResult>(this UnityTask p_task, Func<TResult> p_continuation, CancellationToken p_cancellationToken)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            var cancellation = p_cancellationToken.Register(() => tcs.TrySetCanceled());
            p_task.ContinueWith(t =>
            {
                TaskScheduler.FromCurrentSynchronizationContext().Post(() =>
                {
                    try
                    {
                        tcs.SetResult(p_continuation());
                        cancellation.Dispose();
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                        cancellation.Dispose();
                    }
                });
            });
            return tcs.Task;
        }

        /// <summary>
        /// switch to backgroud processor, thread pool
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task<TResult> ContinueToBackground<TResult>(this UnityTask p_task, Func<UnityTask, TResult> p_continuation)
        {
            return p_task.ContinueToBackground(p_continuation, CancellationToken.None);
        }

        /// <summary>
        /// switch to backgroud processor, thread pool
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task<TResult> ContinueToBackground<TResult>(this UnityTask p_task, Func<UnityTask, TResult> p_continuation, CancellationToken p_cancellationToken)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            var cancellation = p_cancellationToken.Register(() => tcs.TrySetCanceled());
            p_task.ContinueWith(t =>
            {
                TaskScheduler.FromCurrentSynchronizationContext().Post(() =>
                {
                    try
                    {
                        tcs.SetResult(p_continuation(t));
                        cancellation.Dispose();
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                        cancellation.Dispose();
                    }
                });
            });
            return tcs.Task;
        }

        /// <summary>
        /// switch to backgroud processor, thread pool
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task ContinueToBackground(this UnityTask p_task, Action p_continuation, CancellationToken p_cancellationToken)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            var cancellation = p_cancellationToken.Register(() => tcs.TrySetCanceled());
            p_task.ContinueWith(t =>
            {
                TaskScheduler.FromCurrentSynchronizationContext().Post(() =>
                {
                    try
                    {
                        p_continuation();
                        tcs.SetResult(null);
                        cancellation.Dispose();
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                        cancellation.Dispose();
                    }
                });
            });
            return tcs.Task;
        }

        /// <summary>
        /// switch to backgroud processor, thread pool
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task ContinueToBackground(this UnityTask p_task, Action p_continuation)
        {
            return p_task.ContinueToBackground(p_continuation, CancellationToken.None);
        }

        /// <summary>
        /// switch to backgroud processor, thread pool
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task ContinueToBackground(this UnityTask p_task, Action<UnityTask> p_continuation, CancellationToken p_cancellationToken)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            var cancellation = p_cancellationToken.Register(() => tcs.TrySetCanceled());
            p_task.ContinueWith(t =>
            {
                TaskScheduler.FromCurrentSynchronizationContext().Post(() =>
                {
                    try
                    {
                        p_continuation(t);
                        tcs.SetResult(null);
                        cancellation.Dispose();
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                        cancellation.Dispose();
                    }
                });
            });
            return tcs.Task;
        }

        /// <summary>
        /// switch to backgroud processor, thread pool
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task ContinueToBackground(this UnityTask p_task, Action<UnityTask> p_continuation)
        {
            return p_task.ContinueToBackground(p_continuation, CancellationToken.None);
        }

        /// <summary>
        /// switch to backgroud processor, ForegroundInvoker
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static void ContinueToForeground(this Task p_task, Action p_continuation)
        {
            p_task.ContinueWith(t =>
            {
                UnityEngine.Processor.ForegroundInvoker.Invoke(() =>
                {
                    p_continuation();
                });
            });
        }

        /// <summary>
        /// switch to backgroud processor, ForegroundInvoker
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static void ContinueToForeground(this Task p_task, Action<Task> p_continuation)
        {
            p_task.ContinueWith(t =>
            {
                UnityEngine.Processor.ForegroundInvoker.Invoke(() =>
                {
                    p_continuation(t);
                });
            });
        }

        /// <summary>
        /// switch to backgroud processor, ForegroundInvoker
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask<TResult> ContinueToForeground<TResult>(this Task p_task, Func<IEnumerator> p_continuation, CancellationToken p_cancellationToken)
        {
            UnityTaskCompletionSource<TResult> tcs = new UnityTaskCompletionSource<TResult>();
            var cancellation = p_cancellationToken.Register(() => tcs.TrySetCanceled());
            tcs.Task.TaskGenerator = p_continuation;
            tcs.Task.ReturnResult = p =>
            {
                try
                {
                    tcs.SetResult((TResult)p);
                    cancellation.Dispose();
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                    cancellation.Dispose();
                }
            };
            p_task.ContinueWith(t =>
            {
                UnityEngine.Processor.ForegroundInvoker.Invoke(() =>
                {
                    UnityTaskScheduler.FromCurrentSynchronizationContext().Post(tcs.Task.TaskGenerator(), tcs.Task.ReturnResult);
                });
            });
            return tcs.Task;
        }

        /// <summary>
        /// switch to backgroud processor, thread pool
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask<TResult> ContinueToForeground<TResult>(this Task p_task, Func<IEnumerator> p_continuation)
        {
            return p_task.ContinueToForeground<TResult>(p_continuation, CancellationToken.None);
        }

        /// <summary>
        /// switch to backgroud processor, ForegroundInvoker
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask<TResult> ContinueToForeground<TResult>(this Task p_task, Func<Task, IEnumerator> p_continuation, CancellationToken p_cancellationToken)
        {
            UnityTaskCompletionSource<TResult> tcs = new UnityTaskCompletionSource<TResult>();
            var cancellation = p_cancellationToken.Register(() => tcs.TrySetCanceled());
            tcs.Task.TaskGenerator = () => p_continuation(p_task);
            tcs.Task.ReturnResult = p =>
            {
                try
                {
                    tcs.SetResult((TResult)p);
                    cancellation.Dispose();
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                    cancellation.Dispose();
                }
            };

            p_task.ContinueWith(t =>
            {
                UnityEngine.Processor.ForegroundInvoker.Invoke(() =>
                {
                    UnityTaskScheduler.FromCurrentSynchronizationContext().Post(tcs.Task.TaskGenerator(), tcs.Task.ReturnResult);
                });

            });
            return tcs.Task;
        }

        /// <summary>
        /// switch to backgroud processor, ForegroundInvoker
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask<TResult> ContinueToForeground<TResult>(this Task p_task, Func<Task, IEnumerator> p_continuation)
        {
            return p_task.ContinueToForeground<TResult>(p_continuation, CancellationToken.None);
        }
    }
}