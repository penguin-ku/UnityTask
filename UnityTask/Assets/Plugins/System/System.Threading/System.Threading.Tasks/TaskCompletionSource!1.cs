namespace System.Threading.Tasks
{
    using System;

    /// <summary>
    /// Represents the producer side of a Task<TResult> unbound to a delegate, providing access to the consumer side through the Task property.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskCompletionSource<T>
    {
        #region public properties

        /// <summary>
        /// Gets the Task<TResult> created by this TaskCompletionSource<TResult>.
        /// </summary>
        public Task<T> Task { get; private set; }

        #endregion

        #region public functions

        /// <summary>
        /// Creates a TaskCompletionSource<TResult>.
        /// </summary>
        public TaskCompletionSource()
        {
            this.Task = new System.Threading.Tasks.Task<T>();
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
        public void SetException(AggregateException p_exception)
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
        public bool TrySetException(AggregateException exception)
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
            AggregateException exception2 = exception as AggregateException;
            if (exception2 != null)
            {
                return this.Task.TrySetException(exception2);
            }
            System.Threading.Tasks.Task<T> task = this.Task;
            Exception[] innerExceptions = new Exception[] { exception };
            return task.TrySetException(new AggregateException(innerExceptions).Flatten());
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

