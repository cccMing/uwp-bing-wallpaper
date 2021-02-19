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

        public void Log(string className, string info, LogLevel logLevel = LogLevel.Info)
        {
            ILogger log;
#if !DEBUG
            log = new StoreLogger();
#else
            log = new MertoLogger();
#endif
            if (logLevel == LogLevel.Info)
            {
                log.LogInfo($"{className}:{info}");
            }
            else
            {
                log.LogError($"{className}:{info}");
            }
        }

        public void LogError(string className, string info)
            => Log(className, info, LogLevel.Error);
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
        void LogError(string eventName);

        void LogInfo(string str);
    }

    /// <summary>
    /// store日志记录，正式环境异常会到dashboard
    /// </summary>
    public class StoreLogger : ILogger
    {
        public void LogError(string eventName)
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
        public void LogError(string eventName)
        {
            MetroLog.LogManagerFactory.CreateLogManager().GetLogger("UWPBing").Error(eventName);
        }

        public void LogInfo(string str)
        {
            MetroLog.LogManagerFactory.CreateLogManager().GetLogger("UWPBing").Error(str);
        }
    }
}
