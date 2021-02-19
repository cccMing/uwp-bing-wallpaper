using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;

namespace CommonUtil
{
    /// <summary>
    /// 系统设置
    /// </summary>
    public class AppSettings
    {
        public static AppSettings Current { get; }
        static AppSettings()
        {
            Current = new AppSettings();
        }

        public ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;

        /// <summary>
        /// 版本
        /// </summary>
        public string Version
        {
            get
            {
                var ver = Package.Current.Id.Version;
                return $"{ver.Major}.{ver.Minor}.{ver.Build}";//.{ver.Revision}
            }
        }

        public int Theme
        {
            get => GetSettingValue<int>("Theme", (int)ElementTheme.Default);
            set => SetSettingValue("Theme", value);
        }

        public string Language
        {
            get => GetSettingValue<string>("CurrentLanguage", Windows.System.UserProfile.GlobalizationPreferences.Languages.FirstOrDefault());
            set => SetSettingValue("CurrentLanguage", value);
        }

        /// <summary>
        /// 是否启用通知
        /// </summary>
        public bool IsEnableToast
        {
            get => GetSettingValue<bool>("IsEnableToast", true);
            set => SetSettingValue("IsEnableToast", value);
        }

        /// <summary>
        /// 是否自动设置壁纸
        /// </summary>
        public bool IsAutoSetDestopWallpaper
        {
            get => GetSettingValue<bool>("IsAutoSetDestopWallpaper", false);
            set => SetSettingValue("IsAutoSetDestopWallpaper", value);
        }

        /// <summary>
        /// 最后一次通知日期
        /// </summary>
        public string ToastDate
        {
            get => GetSettingValue<string>("toastDate", "");
            set => SetSettingValue("toastDate", value);
        }

        /// <summary>
        /// 最后一次壁纸设置日期
        /// </summary>
        public string WallpaperSetDate
        {
            get => GetSettingValue<string>("WallpaperSetDate", "");
            set => SetSettingValue("WallpaperSetDate", value);
        }

        /// <summary>
        /// app最后一次打开时间
        /// </summary>
        public string LastOpenAppDate
        {
            get => GetSettingValue<string>(nameof(LastOpenAppDate), DateTime.MinValue.ToString("yyyyMMdd"));
            set => SetSettingValue(nameof(LastOpenAppDate), DateHelper.CurrentDateStr);
        }
        
        /// <summary>
        /// 读取系统设置值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private TResult GetSettingValue<TResult>(string key, TResult defaultValue)
        {
            try
            {
                if (!LocalSettings.Values.ContainsKey(key))
                {
                    LocalSettings.Values[key] = defaultValue;
                }
                return (TResult)LocalSettings.Values[key];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return defaultValue;
            }
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void SetSettingValue(string key, object value)
        {
            try
            {
                LocalSettings.Values[key] = value;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
