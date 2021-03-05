using UwpWallpaper.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using SqliteManager;
using UwpWallpaper.Util;
using CommonUtil;
using SqliteManager.Models;

namespace UwpWallpaper.BingManager
{
    public class HttpManager
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date">时间，不写默认为今天</param>
        /// <returns></returns>
        public async static Task<WallpaperInfoPo> GetWallpaperInfoAsync(string date = "")
        {
            if (string.IsNullOrEmpty(date))
            {
                date = DateHelper.CurrentDateStr;
            }

            //先去数据库中查今天的数据是否已经存入
            var wallinfo = SqlQuery.GetDaysWallpaperInfo(date);
            if (wallinfo != null)
                return wallinfo;

            if (date == DateHelper.CurrentDateStr)
            {
                try
                {
                    //没有的话网络请求，一个是简介，一个是详情，都是获取今天的
                    string archive = await DownloadHelper.GetTodayWallpaperAsync();
#if FALSE // 现在这个数据没有了
                    string coverstory = await DownloadHelper.GetCoverstoryAsync();
#endif
                    return SqlQuery.SaveBingWallpaperInfo(archive, null);
                }
                catch (Exception ex)
                {
                    await new MessageHelper().ShowDialogAsync(LangResource.GetString("Tips"), LangResource.GetString("NetConnectError"));
                    ULogger.Current.LogError("HttpManager", ex);
                    return null;
                }
            }

            return null;
        }

        public async static Task<List<WallpaperInfoPo>> GetWallpaperInfosAsync(IList<string> dates)
        {
            if (dates.Contains(DateHelper.CurrentDateStr))
            {
                await GetWallpaperInfoAsync();
            }

            var imglist = SqlQuery.GetDaysWallpaperInfos(dates);

            return imglist;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imgdic">日期和url键值对</param>
        /// <returns></returns>
        public async static Task<Dictionary<string, BitmapImage>> GetBitmapImages(Dictionary<string, string> imgdic)
        {
            Dictionary<string, BitmapImage> bitmapImages = new Dictionary<string, BitmapImage>();
            foreach (var kvp in imgdic)
            {
                var img = await GetImageOrSave(kvp.Key, kvp.Value);
                if (img != null)
                {
                    bitmapImages.Add(kvp.Key, img);
                }
            }
            return bitmapImages;
        }

        /// <summary>
        /// 获取image数据
        /// </summary>
        /// <param name="dateNo"></param>
        /// <param name="dwnUrl"></param>
        /// <returns></returns>
        public async static Task<BitmapImage> GetImageOrSave(string dateNo, string dwnUrl)
        {
            string filepath = Path.Combine(UwpBing.PicFolderPath, ConcatFile(dateNo, FileEnum.JPG));
            //ULogger.Current.Log(filepath);
            if (File.Exists(filepath))
            {
                return new BitmapImage(new Uri(filepath));
            }

            return await DownloadImageFileAsync(dateNo, dwnUrl);
        }

        /// <summary>
        /// 网络请求image信息
        /// </summary>
        /// <param name="dateNo"></param>
        /// <param name="dwnUrl"></param>
        /// <returns></returns>
        private async static Task<BitmapImage> DownloadImageFileAsync(string dateNo, string dwnUrl)
        {
            UwpBing ub = new UwpBing();
            bool b = await ub.SavePicByBuffer(dateNo, dwnUrl);
            if (b)
            {
                return new BitmapImage(new Uri(Path.Combine(UwpBing.PicFolderPath, $"{dateNo}.jpg")));
            }
            return null;
        }

        private static string ConcatFile(string filename, FileEnum file)
        {
            switch (file)
            {
                case FileEnum.JSON:
                    return filename + ".me";
                case FileEnum.COVERSTORY:
                    return filename + ".story";
                case FileEnum.JPG:
                    return filename + ".jpg";
                default:
                    return ".error";
            }
        }
    }

    public enum FileEnum
    {
        JSON,
        COVERSTORY,
        JPG
    }

    public class ImageInfo : WallpaperInfoPo
    {
        public BitmapImage BitmapImage { get; set; }
    }
}
