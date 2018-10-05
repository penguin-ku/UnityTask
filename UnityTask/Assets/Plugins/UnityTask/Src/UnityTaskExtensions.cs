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
        public static UnityTask<IEnumerable<TResult>> EndForNull<TResult>()
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
        public static UnityTask OnSuccess(this UnityTask p_task, Action<UnityTask> p_continuation)
        {
            UnityTaskCompletionSource<object> utcs = new UnityTaskCompletionSource<object>();

            p_task.ContinueWith(t =>
            {
                if (!t.IsFaulted && !t.IsCanceled)
                {
                    p_continuation(t);
                    utcs.TrySetResult(null);
                }
                else
                {
                    utcs.TrySetException(t.Exception);
                }
            });
            return utcs.Task;
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask<TResult> OnSuccess<TResult>(this UnityTask p_task, Func<UnityTask, TResult> p_continuation)
        {
            UnityTaskCompletionSource<TResult> utcs = new UnityTaskCompletionSource<TResult>();
            p_task.ContinueWith(t =>
            {
                if (!t.IsFaulted && !t.IsCanceled)
                {
                    TResult result = p_continuation(t);
                    utcs.TrySetResult(result);
                }
                else
                {
                    utcs.TrySetException(t.Exception);
                }
            });

            return utcs.Task;
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask OnSuccess<TIn>(this UnityTask<TIn> p_task, Action<UnityTask<TIn>> p_continuation)
        {
            return p_task.OnSuccess((UnityTask t) => p_continuation((UnityTask<TIn>)t));
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask<TResult> OnSuccess<TIn, TResult>(this UnityTask<TIn> p_task, Func<UnityTask<TIn>, TResult> p_continuation)
        {
            return p_task.OnSuccess((UnityTask t) => p_continuation((UnityTask<TIn>)t));
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
        public static Task ContinueToBackground<T>(this UnityTask<T> p_task, Action<UnityTask<T>> p_continuation)
        {
            return ContinueToBackground(p_task, p_continuation, CancellationToken.None);
        }

        /// <summary>
        /// switch to backgroud processor, thread pool
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task ContinueToBackground<T>(this UnityTask<T> p_task, Action<UnityTask<T>> p_continuation, CancellationToken p_cancellationToken)
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
        public static Task<TResult> ContinueToBackground<T, TResult>(this UnityTask<T> p_task, Func<UnityTask<T>, TResult> p_continuation)
        {
            return ContinueToBackground(p_task, p_continuation, CancellationToken.None);
        }

        /// <summary>
        /// switch to backgroud processor, thread pool
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task<TResult> ContinueToBackground<T, TResult>(this UnityTask<T> p_task, Func<UnityTask<T>, TResult> p_continuation, CancellationToken p_cancellationToken)
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
        /// 作为后台线程
        /// </summary>
        /// <param name="p_task"></param>
        /// <returns></returns>
        public static Task AsBackground(this UnityTask p_task)
        {
            return p_task.ContinueToBackground(t =>
            {
                if (t.IsFaulted)
                {
                    return Task.FromException(t.Exception);
                }
                else
                {
                    return Task.FromResult(0);
                }
            }).Unwrap();
        }

        /// <summary>
        /// 作为后台线程
        /// </summary>
        /// <param name="p_task"></param>
        /// <returns></returns>
        public static Task<T> AsBackground<T>(this UnityTask<T> p_task)
        {
            return p_task.ContinueToBackground(t =>
            {
                if (t.IsFaulted)
                {
                    return Task.FromException<T>(t.Exception);
                }
                else
                {
                    return Task.FromResult(t.Result);
                }
            }).Unwrap();
        }

        /// <summary>
        ///  switch to foreground processor, UnityTask
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask ContinueToForeground(this Task p_task, Action<Task> p_continuation)
        {
            return ContinueToForeground(p_task, p_continuation, CancellationToken.None);
        }

        /// <summary>
        ///  switch to foreground processor, UnityTask
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask ContinueToForeground(this Task p_task, Action<Task> p_continuation, CancellationToken p_cancellationToken)
        {
            UnityTaskCompletionSource<int> utcs = new UnityTaskCompletionSource<int>();
            var cancellation = p_cancellationToken.Register(() => utcs.TrySetCanceled());
            p_task.ContinueWith(t =>
            {
                UnityEngine.Processor.ForegroundInvoker.Invoke(() =>
                {
                    try
                    {
                        p_continuation(t);
                        utcs.TrySetResult(0);
                        cancellation.Dispose();
                    }
                    catch (Exception ex)
                    {
                        utcs.TrySetException(ex);
                        cancellation.Dispose();
                    }

                });
            });
            return utcs.Task;
        }

        /// <summary>
        /// switch to foreground processor, UnityTask
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask<TResult> ContinueToForeground<TResult>(this Task p_task, Func<Task, TResult> p_continuation)
        {
            return ContinueToForeground(p_task, p_continuation, CancellationToken.None);
        }

        /// <summary>
        /// switch to foreground processor, UnityTask
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask<TResult> ContinueToForeground<TResult>(this Task p_task, Func<Task, TResult> p_continuation, CancellationToken p_cancellationToken)
        {
            UnityTaskCompletionSource<TResult> utcs = new UnityTaskCompletionSource<TResult>();
            var cancellation = p_cancellationToken.Register(() => utcs.TrySetCanceled());
            p_task.ContinueWith(t =>
            {
                UnityEngine.Processor.ForegroundInvoker.Invoke(() =>
                {
                    try
                    {
                        var result = p_continuation(t);
                        utcs.TrySetResult(result);
                        cancellation.Dispose();
                    }
                    catch (Exception ex)
                    {
                        utcs.TrySetException(ex);
                        cancellation.Dispose();
                    }
                });
            });
            return utcs.Task;
        }

        /// <summary>
        ///  switch to foreground processor, UnityTask
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask ContinueToForeground<T>(this Task<T> p_task, Action<Task<T>> p_continuation)
        {
            return ContinueToForeground(p_task, p_continuation, CancellationToken.None);
        }

        /// <summary>
        ///  switch to foreground processor, UnityTask
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask ContinueToForeground<T>(this Task<T> p_task, Action<Task<T>> p_continuation, CancellationToken p_cancellationToken)
        {
            UnityTaskCompletionSource<int> utcs = new UnityTaskCompletionSource<int>();
            var cancellation = p_cancellationToken.Register(() => utcs.TrySetCanceled());
            p_task.ContinueWith(t =>
            {
                UnityEngine.Processor.ForegroundInvoker.Invoke(() =>
                {
                    try
                    {
                        p_continuation(t);
                        utcs.TrySetResult(0);
                        cancellation.Dispose();
                    }
                    catch (Exception ex)
                    {
                        utcs.TrySetException(ex);
                        cancellation.Dispose();
                    }
                });
            });
            return utcs.Task;
        }

        /// <summary>
        /// switch to foreground processor, UnityTask
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask<TResult> ContinueToForeground<T, TResult>(this Task<T> p_task, Func<Task<T>, TResult> p_continuation)
        {
            return ContinueToForeground(p_task, p_continuation, CancellationToken.None);
        }

        /// <summary>
        /// switch to foreground processor, UnityTask
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static UnityTask<TResult> ContinueToForeground<T, TResult>(this Task<T> p_task, Func<Task<T>, TResult> p_continuation, CancellationToken p_cancellationToken)
        {
            UnityTaskCompletionSource<TResult> utcs = new UnityTaskCompletionSource<TResult>();
            var cancellation = p_cancellationToken.Register(() => utcs.TrySetCanceled());
            p_task.ContinueWith(t =>
            {
                UnityEngine.Processor.ForegroundInvoker.Invoke(() =>
                {
                    try
                    {
                        var result = p_continuation(t);
                        utcs.TrySetResult(result);
                        cancellation.Dispose();
                    }
                    catch (Exception ex)
                    {
                        utcs.TrySetException(ex);
                        cancellation.Dispose();
                    }
                });
            });
            return utcs.Task;
        }

        /// <summary>
        /// 作为后台线程
        /// </summary>
        /// <param name="p_task"></param>
        /// <returns></returns>
        public static UnityTask AsForeground(this Task p_task)
        {
            return p_task.ContinueToForeground(t =>
            {
                if (t.IsFaulted)
                {
                    return UnityTask.FromException<int>(t.Exception);
                }
                else
                {
                    return UnityTask.FromResult(0);
                }
            }).Unwrap();
        }

        /// <summary>
        /// 作为后台线程
        /// </summary>
        /// <param name="p_task"></param>
        /// <returns></returns>
        public static UnityTask<T> AsForeground<T>(this Task<T> p_task)
        {
            return p_task.ContinueToForeground(t =>
            {
                if (t.IsFaulted)
                {
                    return UnityTask.FromException<T>(t.Exception);
                }
                else
                {
                    return UnityTask.FromResult(t.Result);
                }
            }).Unwrap();
        }

    }
}