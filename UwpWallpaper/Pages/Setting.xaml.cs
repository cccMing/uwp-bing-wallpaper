using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UwpWallpaper.Services.Navigation;
using UwpWallpaper.Util;
using UwpWallpaper.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UwpWallpaper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Setting : Page, IPageWithViewModel<SettingViewModel>
    {

        public SettingViewModel ViewModel { get; set; }

        public Setting()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await ViewModel.CalcAppStorageAsync();
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    this.StorageProgressRing.Visibility = Visibility.Collapsed;
                });
            });
        }

        /// <summary>
        /// show flyout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RateBtn_Click(object sender, RoutedEventArgs e)
        {
#if !DEBUG
            //_navigationService.NavigateToTestAsync();
            //_navigationService.NavigateToDebugWindow();
#else
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
#endif
        }

        private async void ReviewApp_Button_Click(object sender, RoutedEventArgs e)
        {
            //https://docs.microsoft.com/zh-cn/windows/uwp/monetize/request-ratings-and-reviews

            //使用跳转商店方式评论
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9NW3XBW0D1MX"));
            //if (await ReateApp())
            //{

            //}
        }

        /// <summary>
        /// 内嵌在App中的评论页面，目前没有用到
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ReateApp()
        {
            StoreSendRequestResult result = await StoreRequestHelper.SendRequestAsync(
                    StoreContext.GetDefault(), 16, String.Empty);

            if (result.ExtendedError == null)
            {
                JObject jsonObject = JObject.Parse(result.Response);
                if (jsonObject.SelectToken("status").ToString() == "success")
                {
                    // The customer rated or reviewed the app.
                    return true;
                }
            }

            // There was an error with the request, or the customer chose not to
            // rate or review the app.
            return false;
        }
    }
}
