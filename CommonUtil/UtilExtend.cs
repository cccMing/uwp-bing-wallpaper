using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace CommonUtil
{
    public static class UtilExtend
    {
        /// <summary>
        /// 返回bingfolder的StorageFolder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static IAsyncOperation<StorageFolder> CreateBingdataFolderIfNotExist(this StorageFolder folder)
        {
            return folder.CreateFolderAsync(ConstantObj.BINGFOLDER, CreationCollisionOption.OpenIfExists);
        }

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
