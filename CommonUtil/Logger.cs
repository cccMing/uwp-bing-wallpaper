using Microsoft.Services.Store.Engagement;
using System;

namespace CommonUtil
{
    public class ULogger
    {
        public static ULogger Current = new ULogger();

        public void Log(string info, LogLevel logLevel = LogLevel.Info, Exception ex = null)
        {
            ILogger log;
#if FALSE
            log = new StoreLogger();
#else
            log = new NLogger();
#endif
            if (logLevel == LogLevel.Info)
            {
                log.LogInfo($"{info}");
            }
            else
            {
                log.LogError($"{info}", ex);
            }
        }

        public void LogError(string info, Exception ex)
            => Log(info, LogLevel.Error, ex);
    }

    /// <summary>
    /// 日志记录类别
    /// </summary>
    public enum LogLevel
    {
        Info,
        Error
    }

    public interface ILogger
    {
        void LogError(string eventName, Exception ex);

        void LogInfo(string str);
    }

    /// <summary>
    /// store日志记录，正式环境异常会到dashboard
    /// </summary>
    public class StoreLogger : ILogger
    {
        public void LogError(string eventName, Exception ex)
        {
            StoreServicesCustomEventLogger logger = StoreServicesCustomEventLogger.GetDefault();
            logger.Log(eventName);
        }

        public void LogInfo(string str)
        {
            //throw new NotImplementedException();
        }
    }

    public class NLogger : ILogger
    {
        static NLogger()
        {
            InitNLogContext();
        }

        /// <summary>
        /// 使用代码配置的方式，原因：MyBackgroundTask 记录日志需要，用配置方式有问题
        /// </summary>
        private static void InitNLogContext()
        {
            //var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            //NLog.GlobalDiagnosticsContext.Set("LogPath", storageFolder.Path + "\\");

            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { 
                FileName = Windows.Storage.ApplicationData.Current.LocalFolder.Path + "/nlog/log.${date:format=yyyy-MM-dd}.txt" ,
                CreateDirs = true,
                AutoFlush=true,
                Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss.fff}|${level}|${message}|${exception:format=tostring}",
                ConcurrentWrites = false,
            };

            // Rules for mapping loggers to targets            
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logfile);

            // Apply config           
            NLog.LogManager.Configuration = config;
        }

        private static Lazy<NLog.Logger> Logger = new Lazy<NLog.Logger>(
            () => NLog.LogManager.GetCurrentClassLogger());

        static void Error(string eventName, Exception ex)
        {
            Logger.Value.Error(ex, eventName);
        }

        static void Info(string str)
        {
            Logger.Value.Info(str);
        }

        public void LogError(string eventName, Exception ex)
        {
            Error(eventName, ex);
        }

        public void LogInfo(string str)
        {
            Info(str);
        }
    }
}
