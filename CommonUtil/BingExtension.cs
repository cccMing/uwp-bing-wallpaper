using System;

namespace CommonUtil
{
    public static class BingExtension
    {
        /// <summary>
        /// 获取下载图片链接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetFullDownloadPicUrl(this string url)
        {
            if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return "https://www.bing.com" + url;
            }
            return url;
        }
    }
}
