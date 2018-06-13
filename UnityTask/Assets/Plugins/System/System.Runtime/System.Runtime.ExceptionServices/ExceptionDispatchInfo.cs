namespace System.Runtime.ExceptionServices
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents an exception whose state is captured at a certain point in code.
    /// </summary>
    public class ExceptionDispatchInfo
    {
        #region public properties

        /// <summary>
        /// Gets the exception that is represented by the current instance.
        /// </summary>
        public Exception SourceException { get; private set; }

        #endregion

        #region public functions

        /// <summary>
        /// Throws the exception that is represented by the current ExceptionDispatchInfo object, after restoring the state that was saved when the exception was captured.
        /// </summary>
        public void Throw()
        {
            throw SourceException;
        }

        #endregion

        #region 静态public functions

        /// <summary>
        /// Creates an ExceptionDispatchInfo object that represents the specified exception at the current point in code.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static ExceptionDispatchInfo Capture(Exception ex)
        {
            return new ExceptionDispatchInfo() { SourceException = ex };
        }

        #endregion
    }
}

