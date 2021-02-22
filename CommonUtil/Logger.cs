using MetroLog;
using Microsoft.Services.Store.Engagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtil
{
    public class ULogger
    {
        public static ULogger Current = new ULogger();

        public void Log(string info, LogLevel logLevel = LogLevel.Info, Exception ex = null)
        {
            ILogger log;
#if !DEBUG
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

    public class MertoLogger : ILogger
    {
        private static Lazy<MetroLog.ILogger> Logger = new Lazy<MetroLog.ILogger>(
            () =>
            {
                var logManager = LogManagerFactory.CreateLogManager();
                var logger = logManager.GetLogger("UWPBing");
                return logger;
            });

        public void LogError(string eventName, Exception ex)
        {
            Logger.Value.Error($"{eventName} -- {ex}");
        }

        public void LogInfo(string str)
        {
            Logger.Value.Error(str);
        }
    }

    public class NLogger : ILogger
    {
        public static void InitNLogContext()
        {
            var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            NLog.GlobalDiagnosticsContext.Set("LogPath", storageFolder.Path + "\\");
        }

        private static Lazy<NLog.Logger> Logger = new Lazy<NLog.Logger>(
            () => NLog.LogManager.GetCurrentClassLogger());

        public void LogError(string eventName, Exception ex)
        {
            Logger.Value.Error(ex, eventName);
        }

        public void LogInfo(string str)
        {
            Logger.Value.Info(str);
        }
    }
}
