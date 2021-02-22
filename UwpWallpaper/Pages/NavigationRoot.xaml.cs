using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UwpWallpaper.Services.Navigation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CommonUtil;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UwpWallpaper.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NavigationRoot : Page
    {
        public NavigationRoot()
        {
            this.InitializeComponent();
        }

        public Frame AppFrame => this.appFrame;

        private INavigationService _navigationService;
        public void InitializeNavigationService(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ULogger.Current.Log($"NavigationRoot Navigation Page is Loaded");
            await _navigationService.NavigateToTodayImageAsync();

            await ActiveBackgroundTask();
            ActiveNotificationBackground();
        }

        private void NavigationBtn_Click(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;

            if (frameworkElement != null)
                switch (frameworkElement.Tag.ToString())
                {
                    case "Today":
                        _navigationService.NavigateToTodayImageAsync();
                        break;
                    case "Gallery":
                        _navigationService.NavigateToGalleryAsync();
                        break;
                    case "Setting":
                        _navigationService.NavigateToSettingsAsync();
                        break;
                    default:
                        break;
                }
        }


        /// <summary>
        /// 激活后台任务
        /// </summary>
        private async Task<bool> ActiveBackgroundTask()
        {
            // 检查后台是否启用
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status == BackgroundAccessStatus.Unspecified)
            {
                await new MessageDialog("后台已被锁定").ShowAsync();
            }
            // 设置builder
            bool isToastRegistered = false;
            string AlertToastTaskName = "BingWallpaper_DownloadTask";
            isToastRegistered = BackgroundTaskRegistration.AllTasks.Any(p => p.Value.Name == AlertToastTaskName);
            if (!isToastRegistered)
            {
                var builder = new BackgroundTaskBuilder
                {
                    Name = AlertToastTaskName,
                    TaskEntryPoint = "BackgroundTasks.MyBackgroundTask",
                    IsNetworkRequested = true
                };
                TimeTrigger trigger = new TimeTrigger(16, false);
                builder.SetTrigger(trigger);
                BackgroundTaskRegistration task = builder.Register();
                ULogger.Current.Log($"NavigationRoot {builder.Name}已经注册成功");
            }
            return true;
        }

        /// <summary>
        /// 激活后台任务（主要是发送Windows通知时按钮的处理）
        /// </summary>
        private void ActiveNotificationBackground()
        {
            var taskRegistered = false;
            var notificationTaskName = "BingWallpaper_NotificationTask";
            taskRegistered = BackgroundTaskRegistration.AllTasks.Any(q => q.Value.Name == notificationTaskName);

            if (!taskRegistered)
            {
                var builder = new BackgroundTaskBuilder();

                builder.Name = notificationTaskName;
                builder.TaskEntryPoint = "NotificationBackgroundTasks.NotifyTask";
                builder.SetTrigger(new ToastNotificationActionTrigger());

                BackgroundTaskRegistration registration = builder.Register();
            }
        }

        /// <summary>
        /// show flyout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RateBtn_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            //_navigationService.NavigateToTestAsync();
            _navigationService.NavigateToDebugWindow();
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
        /// 内嵌在App中的评论页面
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
