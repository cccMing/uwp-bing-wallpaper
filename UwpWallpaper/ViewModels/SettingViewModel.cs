using CommonUtil;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UwpWallpaper.Util;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace UwpWallpaper.ViewModels
{
    public class SettingViewModel:ObservableObject
    {
        public SettingViewModel()
        {
            _theme =(ElementThemeExtended)AppSettings.Current.Theme;
            
        }

        public async void CalcAppStorage()
        {
            var res = await new StorageHelper().GetAppDataStorageSize();
            //Thread.Sleep(3000);
            _dataStorage = res.ToString("f2");
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                NotifyPropertyChanged(nameof(DataStorage));
            });
        }

        public string AppVersion
        {
            get
            {
                return AppSettings.Current.Version;
            }
        }

        private ElementThemeExtended _theme;
        public ElementThemeExtended UserTheme
        {
            get => _theme;
            set => Set<ElementThemeExtended>(ref _theme, value);
        }

        private string _dataStorage="--";
        /// <summary>
        /// 数据存储量 Mb
        /// </summary>
        public string DataStorage
        {
            get => _dataStorage + "Mb";
            set => Set<string>(ref _dataStorage, value);
        }

        public bool ToastToggleIsOn
        {
            get => AppSettings.Current.IsEnableToast;
        }

        public bool AutoSetDestopWallpeperIsOn
        {
            get => AppSettings.Current.IsAutoSetDestopWallpaper;
        }

        public void RadioBtn_Click(object sender, RoutedEventArgs e)
        {
            var param = (sender as RadioButton)?.CommandParameter;

            if (param != null)
            {
                if (Window.Current.Content is FrameworkElement frameworkElement)
                {
                    frameworkElement.RequestedTheme = (ElementTheme)param;
                    AppSettings.Current.Theme = (int)param;
                }
            }
        }

        public void ToastToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                AppSettings.Current.IsEnableToast = toggleSwitch.IsOn;
            }
        }

        public void AutoSetDestopWallpaperToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                AppSettings.Current.IsAutoSetDestopWallpaper = toggleSwitch.IsOn;
            }
        }

        public async Task ClearCache_Click()
        {
            var dia = await new MessageHelper().ShowDialogAsync(LangResource.GetString("Tips"), LangResource.GetString("ConfirmDelete")
                                                            , "Ok", "Cancel");
            if (dia == true)
            {
                try
                {
                    await new StorageHelper().DeleteAppCache();
                    await Task.Run(() =>
                    {
                        CalcAppStorage();
                    });
                }
                catch (Exception ex)
                {
                    await new MessageHelper().ShowDialogAsync("异常", ex.Message);
                }
            }
        }
    }

    /// <summary>
    /// Defines the type of theme
    /// </summary>
    public enum ElementThemeExtended
    {
        Default = ElementTheme.Default,
        Light = ElementTheme.Light,
        Dark = ElementTheme.Dark,
        Custom = 1000
    }
}
