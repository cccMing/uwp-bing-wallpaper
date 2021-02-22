﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace CommonUtil
{
    public class UwpBing
    {
        /// <summary>
        /// 获取本地应用程序数据存储区中的根文件夹。 文件夹已备份到云。
        /// </summary>
        public static StorageFolder Folder
            => ApplicationData.Current.LocalFolder;

        /// <summary>
        /// 图片文件存放路径xxx\LocalState\bingdata
        /// </summary>
        public static string CurrentStorgePath
            => Path.Combine(UwpBing.Folder.Path, ConstantObj.BINGFOLDER);

        /// <summary>
        /// 以buffer的形式写入图片信息
        /// </summary>
        /// <param name="picNo"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<bool> SavePicByBuffer(string picNo, string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

#if false
            if (url.Contains("1920x1080"))
            {
                await SavePicByBuffer($"{picNo}-1920x1200", url.Replace("1920x1080", "1920x1200"));
                await SavePicByBuffer($"{picNo}-1080x1920", url.Replace("1920x1080", "1080x1920"));
            }

            if (await IsFileExist(Path.Combine(UwpBing.CurrentStorgePath, $"{picNo}.jpg"))) return true;

#endif
            #region Picture Save
            StorageFolder saveFolder = await Folder.CreateBingdataFolderIfNotExist();
            StorageFile saveFile = await saveFolder.CreateFileAsync($"{picNo}.jpg", CreationCollisionOption.OpenIfExists);
            try
            {
                string downloadUrl = url.GetFullDownloadPicUrl();
                Uri uri = new Uri(downloadUrl);

                using (Windows.Web.Http.HttpClient http = new Windows.Web.Http.HttpClient())
                {
                    IBuffer buffer = await http.GetBufferAsync(uri);
                    if (buffer.Length == 0)
                    {
                        throw new Exception($"get pic buffer empty uri:{uri}");
                    }
                    await FileIO.WriteBufferAsync(saveFile, buffer);
                }
            }
            catch (Exception ex)
            {
                await saveFile.DeleteAsync();
                ULogger.Current.LogError("UwpBing WallpaperDownload", ex);
                return false;
            }
            return true;
            #endregion
        }

        /// <summary>
        /// 判断文件存在并且文件里面有数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true 存在</returns>
        public static bool IsFileExist(string path)
        {
            if (File.Exists(path))
            {
                if (new FileInfo(path).Length > 0)//文件存在并且文件里面有数据
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断图片存在并且文件里面有数据
        /// </summary>
        /// <param name="picName"></param>
        /// <param name="filePath">成功情况下的文件路径</param>
        /// <returns>图片路径</returns>
        public static bool IsPicExist(string picName, out string filePath)
        {
            if (!picName.EndsWith(".jpg") && int.TryParse(picName, out var _))
            {
                picName += ".jpg";
            }

            filePath = Path.Combine(CurrentStorgePath, picName);
            if (IsFileExist(filePath))
            {
                return true;
            }

            filePath = null;
            return false;
        }
    }
}
