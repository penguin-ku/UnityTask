namespace System
{
    // net3.5已携带
    //public delegate TResult Func<TResult>();
    //public delegate TResult Func<TArg1, TResult>(TArg1 arg1);
    //public delegate TResult Func<TArg1, TArg2, TResult>(TArg1 arg1, TArg2 arg2);
    //public delegate TResult Func<TArg1, TArg2, TArg3, TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3);
    //public delegate TResult Func<TArg1, TArg2, TArg3, TArg4, TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);

    // .net4.5支持到7个参数
    public delegate TResult Func<TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);
    public delegate TResult Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6);
    public delegate TResult Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7);
    public delegate TResult Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8);
}
