using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace log4net
{
    public class Log4NetWrapper : MonoBehaviour
    {
        #region private const members

        private const string MAIN_LOG_REPOSTORY = "default";

        #endregion

        #region public members

        public string LogPath = "log";
        public LogLevel LogLevel = LogLevel.Debug;
        public LogConfig[] LogItems;

        #endregion

        #region life cycle

        private void Awake()
        {
            foreach (var item in LogItems.Concat(new LogConfig[] { new LogConfig() {  LogRepostory = MAIN_LOG_REPOSTORY, Level = LogLevel , Path = LogPath} }))
            {
                RollingFileAppender appender = new RollingFileAppender();
                appender.AppendToFile = true;
#if UNITY_EDITOR
                appender.File = string.Format("{0}/unity_run.log", item.Path);
#elif UNITY_STANDALONE_WIN
                appender.File = string.Format("{0}/unity_run.log", item.Path);
#elif UNITY_ANDROID
                appender.File = string.Format("{0}/{1}/unity_run.log", Application.persistentDataPath, item.Path);
#endif
                appender.MaxSizeRollBackups = 20;   // 支持20个文件
                appender.MaxFileSize = 20 * 1024 * 1024;//单件大小最大为20M
                appender.RollingStyle = RollingFileAppender.RollingMode.Composite;
                appender.StaticLogFileName = true;
                appender.ImmediateFlush = true;
                appender.LockingModel = new FileAppender.MinimalLock();
                appender.Name = "FileAppender";

                PatternLayout layout = new PatternLayout("[%d] [%-5p] [ThreadID: %t] [File: %F] [Line: %L] - %m%n");
                /*
                 *  [File: %F] [Line: %L]
                    %m(message)：输出的日志消息，如ILog.Debug(…)输出的一条消息 
                    %n(new line)：换行 
                    %d(datetime)：输出当前语句运行的时刻 
                    %r(run time)：输出程序从运行到执行到当前语句时消耗的毫秒数 
                    %t(thread id)：当前语句所在的线程ID 
                    %p(priority)：日志的当前优先级别，即DEBUG、INFO、WARN…等 
                    %c(class)：当前日志对象的名称，例如： 
                    %L(line)：输出语句所在的行号 
                    %F(file)：输出语句所在的文件名 
                    %-数字：表示该项的最小长度，如果不够，则用空格填充
                    如下配置输出时间、文件、行号、线程ID、日志级别、信息、换行
                */
                //layout.ConversionPattern = "[%d] [%-5p] [ThreadID: %t] [File: %F][Class：%c] [Line: %L] - %m%n";
                layout.Header = "------ 启动...... ------" + Environment.NewLine;
                layout.Footer = "------ 退出...... ------" + Environment.NewLine;
                appender.Layout = layout;
                appender.ActivateOptions();

                ILoggerRepository repository = LogManager.CreateRepository(item.LogRepostory);
                switch (item.Level)
                {
                    case LogLevel.Off:
                        repository.Threshold = Level.Off;
                        break;
                    case LogLevel.Debug:
                        repository.Threshold = Level.Debug;
                        break;
                    case LogLevel.Info:
                        repository.Threshold = Level.Info;
                        break;
                    case LogLevel.Error:
                        repository.Threshold = Level.Error;
                        break;
                    case LogLevel.Fatal:
                        repository.Threshold = Level.Fatal;
                        break;
                    case LogLevel.Warn:
                        repository.Threshold = Level.Warn;
                        break;
                    default:
                        repository.Threshold = Level.Info;
                        break;
                }

                BasicConfigurator.Configure(repository, appender);
            }

        }

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);// 永不消毁
        }


        #endregion

        #region public static functions

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="p_repository"></param>
        /// <returns></returns>
        public static ILog GetLog(string p_repository)
        {
            return LogManager.GetLogger(p_repository, "Default");
        }

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="p_repository"></param>
        /// <returns></returns>
        public static ILog GetLog()
        {
            return LogManager.GetLogger(MAIN_LOG_REPOSTORY, "Default");
        }

        #endregion
    }
}
