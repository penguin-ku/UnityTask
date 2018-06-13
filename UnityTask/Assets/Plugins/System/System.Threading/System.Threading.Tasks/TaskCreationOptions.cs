namespace System.Threading.Tasks
{
    using System;

    [Serializable, Flags]
    public enum TaskCreationOptions
    {
        AttachedToParent = 4,
        DenyChildAttach = 8,
        HideScheduler = 0x10,
        LongRunning = 2,
        None = 0,
        PreferFairness = 1,
        RunContinuationsAsynchronously = 0x40
    }
}
