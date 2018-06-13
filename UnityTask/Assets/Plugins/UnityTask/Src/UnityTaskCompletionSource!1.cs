namespace UnityEngine.TaskExtension
{
    using System;

    /// <summary>
    /// Represents the producer side of a Task<TResult> unbound to a delegate, providing access to the consumer side through the Task property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnityTaskCompletionSource<T>
    {
        #region public properties

        /// <summary>
        /// Gets the Task<TResult> created by this TaskCompletionSource<TResult>.
        /// </summary>
        public UnityTask<T> Task { get; private set; }

        #endregion

        #region public functions

        /// <summary>
        /// Creates a TaskCompletionSource<TResult>.
        /// </summary>
        public UnityTaskCompletionSource()
        {
            this.Task = new UnityTask<T>();
        }

        /// <summary>
        /// Transitions the underlying Task<TResult> into the Canceled state.
        /// </summary>
        public void SetCanceled()
        {
            if (!this.TrySetCanceled())
            {
                throw new InvalidOperationException("Cannot cancel a completed task.");
            }
        }

        /// <summary>
        /// Transitions the underlying Task<TResult> into the Faulted state and binds it to a specified exception.
        /// </summary>
        /// <param name="p_exception"></param>
        public void SetException(System.Threading.Tasks.AggregateException p_exception)
        {
            if (!this.TrySetException(p_exception))
            {
                throw new InvalidOperationException("Cannot set the exception of a completed task.");
            }
        }

        /// <summary>
        /// Transitions the underlying Task<TResult> into the Faulted state and binds it to a specified exception.
        /// </summary>
        /// <param name="p_exception"></param>
        public void SetException(Exception p_exception)
        {
            if (!this.TrySetException(p_exception))
            {
                throw new InvalidOperationException("Cannot set the exception of a completed task.");
            }
        }

        /// <summary>
        /// Transitions the underlying Task<TResult> into the RanToCompletion state.
        /// </summary>
        /// <param name="p_result"></param>
        public void SetResult(T p_result)
        {
            if (!this.TrySetResult(p_result))
            {
                throw new InvalidOperationException("Cannot set the result of a completed task.");
            }
        }

        /// <summary>
        /// Attempts to transition the underlying Task<TResult> into the Canceled state.
        /// </summary>
        /// <returns></returns>
        public bool TrySetCanceled()
        {
            return this.Task.TrySetCanceled();
        }

        /// <summary>
        /// Attempts to transition the underlying Task<TResult> into the Faulted state and binds it to a specified exception.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool TrySetException(System.Threading.Tasks.AggregateException exception)
        {
            return this.Task.TrySetException(exception);
        }

        /// <summary>
        /// Attempts to transition the underlying Task<TResult> into the Faulted state and binds it to a specified exception.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public bool TrySetException(Exception exception)
        {
            System.Threading.Tasks.AggregateException exception2 = exception as System.Threading.Tasks.AggregateException;
            if (exception2 != null)
            {
                return this.Task.TrySetException(exception2);
            }
            UnityTask<T> task = this.Task;
            Exception[] innerExceptions = new Exception[] { exception };
            return task.TrySetException(new System.Threading.Tasks.AggregateException(innerExceptions).Flatten());
        }

        /// <summary>
        /// Attempts to transition the underlying Task<TResult> into the RanToCompletion state.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TrySetResult(T result)
        {
            return this.Task.TrySetResult(result);
        }

        #endregion
    }
}

