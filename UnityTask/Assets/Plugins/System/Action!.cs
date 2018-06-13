namespace System
{
    // net3.5已携带
    //public delegate void Action<TArg1>(TArg1 arg1);
    //public delegate void Action<TArg1, TArg2>(TArg1 arg1, TArg2 arg2);
    //public delegate void Action<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3);

    // .net4.5支持到7个参数
    //public delegate void Action<TArg1, TArg2, TArg3, TArg4>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
    public delegate void Action<TArg1, TArg2, TArg3, TArg4, TArg5>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);
    public delegate void Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6);
    public delegate void Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7);
}
