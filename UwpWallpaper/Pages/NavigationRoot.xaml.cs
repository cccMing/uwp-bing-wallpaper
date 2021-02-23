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
using Windows.UI.ViewManagement;

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
        /// fullScreen click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullScreenBtn_Click(object sender, RoutedEventArgs e)
        {
            var view = ApplicationView.GetForCurrentView();

            view.VisibleBoundsChanged += (ApplicationView viewSender, object args) =>
            {
                if (!viewSender.IsFullScreenMode)
                { // 全屏退出时，使导航条可见
                    this.NaviStack.Visibility = Visibility.Visible;
                }
            };

            var isFullScreen = false;

            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
            }
            else
            {
                isFullScreen = view.TryEnterFullScreenMode();
            }

            if (isFullScreen)
            {
                this.NaviStack.Visibility = Visibility.Collapsed;
            }
        }
    }
}
