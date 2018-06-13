namespace System
{
    /// <summary>
    /// DateTime扩展方法
    /// </summary>
    public static class DateTimeExtensions
    {
        public static long UnixTimeStamp(this DateTime date)
        {
            return (long)date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}
