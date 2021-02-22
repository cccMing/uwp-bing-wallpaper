using Autofac;
using CommonUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UwpWallpaper.BingManager;
using UwpWallpaper.Pages;
using UwpWallpaper.Services.Navigation;
using UwpWallpaper.Util;
using UwpWallpaper.ViewModels;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UwpWallpaper
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {

        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

        }

        /// <summary>
        /// 设置语言
        /// </summary>
        private void LoadSettings()
        {

            #region 语言

            //用系统带来的默认语言，下面不要
            //var lang = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride;// = AppSettings.Current.Language;
            var langs = Windows.System.UserProfile.GlobalizationPreferences.Languages;
            if (langs.Count > 0)
            {
                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = langs[0];
            }

            #endregion

            #region 主题

            //this.RequestedTheme = AppSettings.Current.Theme;
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = (ElementTheme)AppSettings.Current.Theme;
            }

            #endregion
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            #region origin
#if false
            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                //加上这句订阅Frame的导航事件
                //rootFrame.Navigated += OnNavigated;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;


                //加上下面两句，订阅返回事件
                //SystemNavigationManager.GetForCurrentView().BackRequested += OnBackrequested;
                //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(PivotRoot), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }
            
#endif
            #endregion

            NLogger.InitNLogContext(); // set nlog config

            DatabaseManager.InitializeDatabase();

            RunBeforeSet.Setting();

            SetTitleBarStyle();

            InitWindow(skipWindowCreation: e.PrelaunchActivated);

            LoadSettings();
        }

        /// <summary>
        /// 这个的用处是在用户非直接激活应用的时候触发的，比如点击此应用发出的通知，会到这里来
        /// </summary>
        /// <param name="args"></param>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            //base.OnActivated(args);

            InitWindow(skipWindowCreation: false);

            if (args.Kind == ActivationKind.Protocol)
            {
                Window.Current.Activate();

                LoadSettings();
            }
        }

        private NavigationRoot _rootPage;
        private IContainer _container;
        private void InitWindow(bool skipWindowCreation = false)
        {
            var builder = new ContainerBuilder();
            _rootPage = Window.Current.Content as NavigationRoot;
            bool initApp = _rootPage == null && !skipWindowCreation;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (initApp)
            {
                _rootPage = new NavigationRoot();

                FrameAdapter adapter = new FrameAdapter(_rootPage.AppFrame);

                builder.RegisterInstance(adapter)
                    .AsImplementedInterfaces();

                //这个设为单例是因为在应用中第二次加载的时候第五张及以后的图片相应的文字消失了，暂时这么解决
                builder.RegisterType<TodayImageViewModel>();
                builder.RegisterType<GalleryViewModel>().SingleInstance();
                builder.RegisterType<SettingViewModel>();

                builder.RegisterType<NavigationService>()
                    .AsImplementedInterfaces()
                    .SingleInstance();

                _container = builder.Build();

                _rootPage.InitializeNavigationService(_container.Resolve<INavigationService>());

                //adapter.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = _rootPage;

                Window.Current.Activate();
            }
        }


        /// <summary>
        /// 标题颜色处理
        /// </summary>
        private void SetTitleBarStyle()
        {
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            // Set active window colors
            var uiSettings = new UISettings();
            uiSettings.ColorValuesChanged += System_UiSettings_ColorValuesChanged;
            titleBar.ForegroundColor = Windows.UI.Colors.White;
            titleBar.BackgroundColor = uiSettings.GetColorValue(UIColorType.Accent); //Color.FromArgb(0, 0, 120, 215); //#0078D7
            titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonBackgroundColor = uiSettings.GetColorValue(UIColorType.Accent); //Color.FromArgb(0, 0, 120, 215);
            titleBar.ButtonHoverForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonHoverBackgroundColor = uiSettings.GetColorValue(UIColorType.AccentLight1); //Color.FromArgb(0, 25, 133, 218);
            titleBar.ButtonPressedForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonPressedBackgroundColor = uiSettings.GetColorValue(UIColorType.AccentLight2); //Color.FromArgb(0, 51, 147, 223);

            // Set inactive window colors
            //titleBar.InactiveForegroundColor = Windows.UI.Colors.Gray;
            //titleBar.InactiveBackgroundColor = Windows.UI.Colors.AliceBlue;
            //titleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.Gray;
            //titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.SeaGreen;
        }

        private async void System_UiSettings_ColorValuesChanged(UISettings sender, object args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                    titleBar.BackgroundColor = sender.GetColorValue(UIColorType.Accent);
                    titleBar.ButtonBackgroundColor = sender.GetColorValue(UIColorType.Accent);
                    titleBar.ButtonHoverBackgroundColor = sender.GetColorValue(UIColorType.AccentLight1);
                    titleBar.ButtonPressedForegroundColor = sender.GetColorValue(UIColorType.Foreground);
                    titleBar.ButtonPressedBackgroundColor = sender.GetColorValue(UIColorType.AccentLight2);
                });
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
