using Polly;
using System;
using System.Threading.Tasks;

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
        public const string TODAYWALLPAPER_URL = "https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1";

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

                    return await policy.ExecuteAsync(async () => await httpclient.GetStringAsync(uri));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
