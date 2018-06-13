using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace log4net
{
    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 关闭
        /// </summary>
        Off,
        /// <summary>
        /// 测试
        /// </summary>
        Debug,
        /// <summary>
        /// 信息
        /// </summary>
        Info,
        /// <summary>
        /// 警告
        /// </summary>
        Warn,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// 事故
        /// </summary>
        Fatal
    }

}
