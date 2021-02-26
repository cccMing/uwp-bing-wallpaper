using Polly;
using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace CommonUtil
{
    public class DownloadHelper
    {
        /// <summary>
        /// 图片故事url地址
        /// </summary>
        public const string COVERSTORY_URL = "https://www.bing.com/cnhp/coverstory/"; //http://cn.bing.com/cnhp/coverstory/

        /// <summary>
        /// 今日图片数据
        /// </summary>
        public const string TODAYWALLPAPER_URL = "https://www.bing.com/HPImageArchive.aspx?n=1&idx=0&format=js";

        /// <summary>
        /// 获取今日图片信息str
        /// </summary>
        /// <returns></returns>
        public async static Task<string> GetTodayWallpaperAsync()
            => await HttpGetStringAsync(TODAYWALLPAPER_URL);

        /// <summary>
        /// 获取今日封面故事str
        /// </summary>
        /// <returns></returns>
        public async static Task<string> GetCoverstoryAsync()
            => await HttpGetStringAsync(COVERSTORY_URL);

        /// <summary>
        /// 以GET方式获取请求数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async static Task<string> HttpGetStringAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("download url cannot be null");
            }

            try
            {
                ULogger.Current.Log($"HttpGetStringAsync {url} {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

                //uwp中用这个httpclient，还有个system.net.http
                using (var httpclient = new Windows.Web.Http.HttpClient())
                {
                    var uri = new System.Uri(url);

                    // Retry multiple times, calling an action on each retry 
                    // with the current exception, retry count and context 
                    // provided to Execute()
                    var policy = Policy
                                 .Handle<Exception>()
                                 .RetryAsync(3, (exception, retryCount, context) =>
                                 {
                                     // do something 
                                     ULogger.Current.LogError($"Polly Policy Retry {retryCount}", exception);
                                 });

                    var result = await policy.ExecuteAsync(async () => await httpclient.GetStringAsync(uri));

                    ULogger.Current.Log($"HttpGetStringAsync End {url} {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

                    return result;
                }
            }
            catch (Exception ex)
            {
                ULogger.Current.LogError($"HttpGetStringAsync ex", ex);
                throw ex;
            }
        }

        /// <summary>
        /// http buffer get.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async static Task<IBuffer> HttpGetBufferAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("download url cannot be null");
            }

            try
            {
                ULogger.Current.Log($"HttpGetBufferAsync {url} {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");

                //uwp中用这个httpclient，还有个system.net.http
                using (var httpclient = new Windows.Web.Http.HttpClient())
                {
                    var uri = new System.Uri(url);

                    var policy = Policy.Timeout(8, onTimeout: (context, timespan, task) =>
                    {
                        ULogger.Current.Log($"Polly Policy Timeout {context.PolicyKey} at {context.OperationKey}: execution timed out after {timespan.TotalSeconds} seconds.");
                    });

                    var result = await policy.Execute(async () => await httpclient.GetBufferAsync(uri));
                    ULogger.Current.Log($"HttpGetBufferAsync End {url} {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                    return result;
                }
            }
            catch (Exception ex)
            {
                ULogger.Current.LogError($"HttpGetBufferAsync ex", ex);
                throw ex;
            }
        }
    }
}
