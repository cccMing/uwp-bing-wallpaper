using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtil
{
    public class DownloadHelper
    {
        /// <summary>
        /// 获取今日图片信息str
        /// </summary>
        /// <returns></returns>
        public async static Task<string> GetTodayWallpaperAsync()
            => await HttpGetStringAsync(ConstantObj.TODAYWALLPAPERURL);

        /// <summary>
        /// 获取今日封面故事str
        /// </summary>
        /// <returns></returns>
        public async static Task<string> GetCoverstoryAsync()
            => await HttpGetStringAsync(ConstantObj.COVERSTORYURL);

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
