namespace System.Threading.Tasks
{
    using Runtime.ExceptionServices;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;

    /// <summary>
    /// Task扩展方法
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Unwraps a nested task, producing a task that is complete when both the outer and inner tasks are complete. This is primarily useful for chaining asynchronous operations together.
        /// </summary>
        /// <param name="p_task">The task to unwrap.</param>
        /// <returns>A new task that is complete when both the outer and inner tasks are complete.</returns>
        public static Task Unwrap(this Task<Task> p_task)
        {
            var tcs = new TaskCompletionSource<int>();
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
                            tcs.TrySetResult(0);
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
        public static Task<T> Unwrap<T>(this Task<Task<T>> task)
        {
            var tcs = new TaskCompletionSource<T>();
            task.ContinueWith((Task<Task<T>> t) =>
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
                    t.Result.ContinueWith(delegate (Task<T> inner)
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
        /// 给定数据源，依次执行
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_source">数据源</param>
        /// <param name="p_executor">执行器</param>
        /// <returns></returns>
        public static Task<IEnumerable<TResult>> Chaining<TIn, TResult>(this IEnumerable<TIn> p_source, Func<TIn, Task<TResult>> p_executor)
        {
            List<TResult> rtn = new List<TResult>();
            IEnumerator<TIn> enumerator = p_source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return EndForNull<TResult>();
            }
            return Chaining(enumerator, p_executor, rtn);
        }

        /// <summary>
        /// 根据迭代器依次执行
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_enumerator"></param>
        /// <param name="p_executor"></param>
        /// <param name="p_rtn"></param>
        /// <returns></returns>
        private static Task<IEnumerable<TResult>> Chaining<TIn, TResult>(IEnumerator<TIn> p_enumerator, Func<TIn, Task<TResult>> p_executor, List<TResult> p_rtn)
        {
            if (p_enumerator.Current == null)
            {
                return EndForNull<TResult>();
            }
            TIn current = p_enumerator.Current;
            if (!p_enumerator.MoveNext())
            {
                return p_executor(current).ContinueWith(new Func<Task<TResult>, Task<IEnumerable<TResult>>>(t =>
                {
                    p_rtn.Add(t.Result);
                    System.Threading.Tasks.TaskCompletionSource<IEnumerable<TResult>> source = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<TResult>>();
                    source.SetResult(p_rtn);
                    return source.Task;
                })).Unwrap();
            }
            return p_executor(current).ContinueWith(new Func<Task<TResult>, Task<IEnumerable<TResult>>>(t =>
            {
                p_rtn.Add(t.Result);
                return Chaining<TIn, TResult>(p_enumerator, p_executor, p_rtn);
            })).Unwrap();
        }

        /// <summary>
        /// 给定数据源，依次执行，但每次执行之前有一个预备动作
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_source"></param>
        /// <param name="p_executor"></param>
        /// <param name="p_beforeNext"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TResult>> Chaining<TIn, TResult>(this IEnumerable<TIn> p_source, Func<TIn, Task<TResult>> p_executor, Func<TResult, TIn, TIn> p_beforeNext)
        {
            return p_source.Chaining(p_executor, p_beforeNext, default(TResult));
        }

        /// <summary>
        /// 给定数据源，依次执行，但每次执行之前有一个预备动作
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_source"></param>
        /// <param name="p_executor"></param>
        /// <param name="p_beforeNext"></param>
        /// <param name="p_firstSeed"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TResult>> Chaining<TIn, TResult>(this IEnumerable<TIn> p_source, Func<TIn, Task<TResult>> p_executor, Func<TResult, TIn, TIn> p_beforeNext, TResult p_firstSeed)
        {
            return p_source.Chaining(p_executor, p_beforeNext, null, p_firstSeed);
        }

        /// <summary>
        /// 给定数据源依次执行
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_source"></param>
        /// <param name="p_executor"></param>
        /// <param name="p_beforeNext"></param>
        /// <param name="p_afterLast"></param>
        /// <param name="p_firstSeed"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TResult>> Chaining<TIn, TResult>(this IEnumerable<TIn> p_source, Func<TIn, Task<TResult>> p_executor, Func<TResult, TIn, TIn> p_beforeNext, Action<TResult> p_afterLast, TResult p_firstSeed)
        {
            List<TResult> rtn = new List<TResult>();
            IEnumerator<TIn> enumerator = p_source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return EndForNull<TResult>();
            }
            return Chaining(enumerator, p_executor, rtn, p_beforeNext, p_afterLast, p_firstSeed);
        }

        /// <summary>
        /// 给定数据源依次执行
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_enumerator"></param>
        /// <param name="p_executor"></param>
        /// <param name="p_rtn"></param>
        /// <param name="p_beforeNext"> 预备</param>
        /// <param name="p_afterLast">善后</param>
        /// <param name="p_lastResult"></param>
        /// <returns></returns>
        public static Task<IEnumerable<TResult>> Chaining<TIn, TResult>(IEnumerator<TIn> p_enumerator, Func<TIn, Task<TResult>> p_executor, List<TResult> p_rtn, Func<TResult, TIn, TIn> p_beforeNext, Action<TResult> p_afterLast, TResult p_lastResult)
        {
            if (p_enumerator.Current == null)
            {
                TaskCompletionSource<IEnumerable<TResult>> source = new TaskCompletionSource<IEnumerable<TResult>>();
                source.SetResult(null);
                return source.Task;
            }
            TIn current = p_enumerator.Current;
            if (p_beforeNext != null)
            {
                current = p_beforeNext(p_lastResult, current);
            }
            if (!p_enumerator.MoveNext())
            {
                return p_executor(current).ContinueWith(new Func<Task<TResult>, Task<IEnumerable<TResult>>>(t =>
                {
                    if (p_afterLast != null)
                    {
                        p_afterLast(t.Result);
                    }
                    System.Threading.Tasks.TaskCompletionSource<IEnumerable<TResult>> source = new System.Threading.Tasks.TaskCompletionSource<IEnumerable<TResult>>();
                    source.SetResult(p_rtn);
                    return source.Task;
                })).Unwrap();
            }
            return p_executor(current).ContinueWith(new Func<Task<TResult>, Task<IEnumerable<TResult>>>(t =>
            {
                p_rtn.Add(t.Result);
                if (p_afterLast != null)
                {
                    p_afterLast(t.Result);
                }
                return Chaining<TIn, TResult>(p_enumerator, p_executor, p_rtn, p_beforeNext, p_afterLast, t.Result);
            })).Unwrap();
        }

        /// <summary>
        /// 空任务
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        private static System.Threading.Tasks.Task<IEnumerable<TResult>> EndForNull<TResult>()
        {
            TaskCompletionSource<IEnumerable<TResult>> source = new TaskCompletionSource<IEnumerable<TResult>>();
            source.SetResult(null);
            return source.Task;
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="p_predicate">第一件任务，其结果true表示执行第二件任务，false表示第二件任务停止</param>
        /// <param name="p_body">第二件任务</param>
        /// <returns></returns>
        private static Task WhileAsync(Func<Task<bool>> p_predicate, Func<Task> p_body)
        {
            return p_predicate().OnSuccess(t =>
            {
                if (!t.Result)
                {
                    return Task.FromResult(0);
                }
                return p_body().OnSuccess<Task>(_ => p_predicate()).Unwrap();
            }).Unwrap();
        }

        /// <summary>
        /// 异步拷贝
        /// </summary>
        /// <param name="p_stream"></param>
        /// <param name="p_destination"></param>
        /// <returns></returns>
        public static Task CopyToAsync(this Stream p_stream, Stream p_destination)
        {
            return p_stream.CopyToAsync(p_destination, 0x800, CancellationToken.None);
        }

        /// <summary>
        /// 异步拷贝
        /// </summary>
        /// <param name="p_stream"></param>
        /// <param name="p_destination"></param>
        /// <param name="p_bufferSize"></param>
        /// <param name="p_cancellationToken"></param>
        /// <returns></returns>
        public static Task CopyToAsync(this Stream p_stream, Stream p_destination, int p_bufferSize, CancellationToken p_cancellationToken)
        {
            byte[] numArray = new byte[p_bufferSize];
            int result = 0;
            return WhileAsync(() =>
            {
                return p_stream.ReadAsync(numArray, 0, p_bufferSize, p_cancellationToken).OnSuccess<int, bool>(delegate (System.Threading.Tasks.Task<int> readTask)
                {
                    result = readTask.Result;
                    return result > 0;
                });
            }, () =>
            {
                p_cancellationToken.ThrowIfCancellationRequested();
                return p_destination.WriteAsync(numArray, 0, result, p_cancellationToken).OnSuccess(_ =>
                {
                    p_cancellationToken.ThrowIfCancellationRequested();
                });
            });
        }

        /// <summary>
        /// 异步读
        /// </summary>
        /// <param name="p_stream"></param>
        /// <param name="p_buffer"></param>
        /// <param name="p_offset"></param>
        /// <param name="p_count"></param>
        /// <param name="p_cancellationToken"></param>
        /// <returns></returns>
        public static Task<int> ReadAsync(this Stream p_stream, byte[] p_buffer, int p_offset, int p_count, CancellationToken p_cancellationToken)
        {
            if (p_cancellationToken.IsCancellationRequested)
            {
                TaskCompletionSource<int> source = new TaskCompletionSource<int>();
                source.SetCanceled();
                return source.Task;
            }
            System.Threading.Tasks.TaskFactory factory = System.Threading.Tasks.Task.Factory;
            Stream stream2 = p_stream;
            Stream stream3 = p_stream;
            return factory.FromAsync(new System.Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(stream2.BeginRead), new Func<IAsyncResult, int>(stream3.EndRead), p_buffer, p_offset, p_count, null);
        }

        /// <summary>
        /// 异步读
        /// </summary>
        /// <param name="p_reader"></param>
        /// <returns></returns>
        public static Task<string> ReadToEndAsync(this StreamReader p_reader)
        {
            return Task.Run(() => p_reader.ReadToEnd());
        }

        /// <summary>
        /// 异步写
        /// </summary>
        /// <param name="p_stream"></param>
        /// <param name="p_buffer"></param>
        /// <param name="p_offset"></param>
        /// <param name="p_count"></param>
        /// <param name="p_cancellationToken"></param>
        /// <returns></returns>
        public static Task WriteAsync(this Stream p_stream, byte[] p_buffer, int p_offset, int p_count, CancellationToken p_cancellationToken)
        {
            if (p_cancellationToken.IsCancellationRequested)
            {
                TaskCompletionSource<object> source = new TaskCompletionSource<object>();
                source.SetCanceled();
                return source.Task;
            }
            TaskFactory factory = Task.Factory;
            Stream stream2 = p_stream;
            Stream stream3 = p_stream;
            return factory.FromAsync<byte[], int, int>(new System.Func<byte[], int, int, AsyncCallback, object, IAsyncResult>(stream2.BeginWrite), new Action<IAsyncResult>(stream3.EndWrite), p_buffer, p_offset, p_count, null);
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <param name="zipper"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Zip<T1, T2, TResult>(this IEnumerable<T1> list1, IEnumerable<T2> list2, Func<T1, T2, TResult> zipper)
        {
            var e1 = list1.GetEnumerator();
            var e2 = list2.GetEnumerator();
            while (e1.MoveNext() && e2.MoveNext())
            {
                yield return zipper(e1.Current, e2.Current);
            }
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task OnSuccess(this Task p_task, Action<Task> p_continuation)
        {
            return p_task.OnSuccess<object>(t =>
            {
                p_continuation(t);
                return null;
            });
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task<TResult> OnSuccess<TResult>(this Task p_task, Func<Task, TResult> p_continuation)
        {
            return p_task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    if (!t.IsCanceled)
                    {
                        return Task.FromResult(p_continuation(t));
                    }
                    TaskCompletionSource<TResult> source = new TaskCompletionSource<TResult>();
                    source.SetCanceled();
                    return source.Task;
                }
                AggregateException ex = t.Exception.Flatten();
                if (ex.InnerExceptions.Count != 1)
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
                else
                {
                    ExceptionDispatchInfo.Capture(ex.InnerExceptions[0]).Throw();
                }
                return Task.FromResult<TResult>(default(TResult));
            }).Unwrap();
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task OnSuccess<TIn>(this Task<TIn> p_task, Action<Task<TIn>> p_continuation)
        {
            return p_task.OnSuccess<TIn, object>(t =>
            {
                p_continuation(t);
                return null;
            });
        }

        /// <summary>
        /// OnSuccess
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="p_task"></param>
        /// <param name="p_continuation"></param>
        /// <returns></returns>
        public static Task<TResult> OnSuccess<TIn, TResult>(this Task<TIn> p_task, Func<Task<TIn>, TResult> p_continuation)
        {
            return p_task.OnSuccess((Task t) => p_continuation((Task<TIn>)t));
        }
    }
}

