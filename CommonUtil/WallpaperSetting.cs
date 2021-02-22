using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.UserProfile;

namespace CommonUtil
{
    public class WallpaperSetting
    {
        /// <summary>
        /// 设置壁纸
        /// </summary>
        /// <param name="imgId"></param>
        /// <param name="eu"></param>
        /// <returns></returns>
        public static async Task<bool> SetWallpaper(string imgId, BackgroundEnum eu)
        {
            if (string.IsNullOrEmpty(imgId))
            {
                return false;
            }

            string filePath;
            if (!UwpBing.IsPicExist(imgId, out filePath))
            {
                return false;
            }

            //获取文件
            StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
            //设置背景
            UserProfilePersonalizationSettings setting = UserProfilePersonalizationSettings.Current;

            if (eu == BackgroundEnum.Destop)
            {
                return await setting.TrySetWallpaperImageAsync(file);
            }
            else
            {
                return await setting.TrySetLockScreenImageAsync(file);
            }
        }
    }

    /// <summary>
    /// 壁纸类型
    /// </summary>
    public enum BackgroundEnum
    {
        Destop,
        LockScreen
    }
}
