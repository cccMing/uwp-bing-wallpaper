using CommonUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UwpWallpaper.Services.Navigation;
using UwpWallpaper.Util;
using UwpWallpaper.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UwpWallpaper.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TodayImage : Page, IPageWithViewModel<TodayImageViewModel>
    {
        public TodayImageViewModel ViewModel { get; set; }
        private string passid;//单个日期展示时传递过来的picid

        public TodayImage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ULogger.Current.Log($"TodayImage Page_Loaded {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            if (string.IsNullOrEmpty(passid))
                await ViewModel.GetDaysImageInfoAsync(15);
            else
                await ViewModel.GetDaysImageInfoAsync(passid);

            this.MyProgress.IsActive = false;

            SetDescriptionWidth((sender as TodayImage).ActualWidth);
            ULogger.Current.Log($"TodayImage Page_Loaded End {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
        }

        /// <summary>
        /// 跳转到这个页面时触发
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string s = (string)e.Parameter;
            if (!string.IsNullOrEmpty(s))
            {
                this.passid = s;
            }
        }

        /// <summary>
        /// 展示图片地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowMapButton_Click(object sender, RoutedEventArgs e)
        {
            this.ExtraSplitView.IsPaneOpen = !this.ExtraSplitView.IsPaneOpen;
            this.ExtraSplitView.OpenPaneLength = 600;

            try
            {
                var imageinfo = ((sender as Button)?.DataContext as ImageInfo);

                Windows.UI.Xaml.Controls.Maps.MapControl mapControl = new Windows.UI.Xaml.Controls.Maps.MapControl();

                Windows.UI.Xaml.Controls.TextBlock textBlock = new Windows.UI.Xaml.Controls.TextBlock();
                textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                textBlock.Margin = new Thickness(10);
                textBlock.FontSize = 18;

                textBlock.Text = imageinfo?.Attribute;
                var geopoint = imageinfo?.GPoint;
                SetMapLocation(mapControl, geopoint);

                ClearGrid();
                this.panGrid.Children.Add(textBlock);
                this.panGrid.Children.Add(mapControl);
                Grid.SetRow(mapControl, 1);
            }
            catch
            {
            }
        }

        private void ClearGrid() 
            => this.panGrid.Children.Clear();

        private void SetMapLocation(Windows.UI.Xaml.Controls.Maps.MapControl mapControl, Windows.Devices.Geolocation.Geopoint geopoint)
        {
            mapControl.Children.Clear();

            DependencyObject marker = new MarkerElement().GetCircleElement();
            mapControl.Children.Add(marker);
            Windows.UI.Xaml.Controls.Maps.MapControl.SetLocation(marker, geopoint);
            mapControl.Center = geopoint;
            mapControl.ZoomLevel = 1;
            //this.MyMap.StartContinuousZoom(1.7);
            //this.MyMap.ZoomLevelChanged += (Windows.UI.Xaml.Controls.Maps.MapControl mapsender, object args) =>
            //{
            //    if (mapsender.ZoomLevel > 9)
            //        mapsender.StopContinuousZoom();
            //};
        }

        /// <summary>
        /// 点击按钮显示今日壁纸信息，现在已经废弃，好像是bing官方接口19年3月后没有数据了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowExtraStory_Click(object sender, RoutedEventArgs e)
        {
            this.ExtraSplitView.IsPaneOpen = !this.ExtraSplitView.IsPaneOpen;
            this.ExtraSplitView.OpenPaneLength = 400;

            try
            {
                var imageinfo = ((sender as Button)?.DataContext as ImageInfo);

                string userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1";
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get,
                    new Uri($"https://cn.bing.com/cnhp/life?mkt=zh-cn&ensearch=0&d={imageinfo.ImgNo}"));
                httpRequestMessage.Headers.Append("User-Agent", userAgent);

                WebView webView = new WebView();
                webView.NavigateWithHttpRequestMessage(httpRequestMessage);
                Grid.SetRowSpan(webView, 2);

                ClearGrid();
                this.panGrid.Children.Add(webView);

                webView.NavigationCompleted += WebView_NavigationCompleted;
            }
            catch
            {
            }
        }

        private static string[] SetBodyOverFlowHiddenString = new string[] { @"function SetBodyOverFlowHidden() { document.body.style.overflowX = 'hidden'; } SetBodyOverFlowHidden();" };
        private async void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            await sender.InvokeScriptAsync("eval", SetBodyOverFlowHiddenString);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetDescriptionWidth(e.NewSize.Width);
        }

        private void SetDescriptionWidth(double newWidth)
        {
            foreach (var i in ViewModel.ImageInfos)
            {
                i.TextWidth = newWidth;
            }
        }

        int prevIndex;
        private void ImgFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0) return;

            int index = (sender as FlipView).SelectedIndex;
            FlipViewItem newSelectedItem;
            try
            {
                newSelectedItem = (this.ImgFlipView as ItemsControl).ContainerFromItem(e.AddedItems[0]) as FlipViewItem; //this.ImgFlipView.ItemContainerGenerator.ContainerFromItem(e.AddedItems[0]) as FlipViewItem;
            }
            catch (Exception)//要把viewmodel单例改掉，否者这边有异常--这个页面的viewmodel已经改成非单例，理论上不会进catch
            {
                return;
            }

            if (newSelectedItem == null) return;

            var image = FindElementByName<Image>(newSelectedItem, "TodayImage");//根据item找到image元素

            var tfGroup = image?.RenderTransform as TransformGroup;
            if (tfGroup == null) return;

            //只在第一次往后滚动的时候有缩放效果
            if (index >= prevIndex)
            {
                AnimationImage(tfGroup.Children.First());
                prevIndex = index;
            }
            else
            {
            }
        }

        #region 元素查找
        private T FindElementByName<T>(DependencyObject element, string sChildName) where T : FrameworkElement
        {
            T childElement = null;
            var nChildCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < nChildCount; i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child == null)
                    continue;

                if (child is T && child.Name.Equals(sChildName))
                {
                    childElement = (T)child;
                    break;
                }

                childElement = FindElementByName<T>(child, sChildName);

                if (childElement != null)
                    break;
            }
            return childElement;
        }

        private T FindElement<T>(DependencyObject element) where T : FrameworkElement
        {
            T childElement = null;
            var nChildCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < nChildCount; i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child == null)
                    continue;

                if (child is T)
                {
                    childElement = (T)child;
                    break;
                }

                childElement = FindElement<T>(child);

                if (childElement != null)
                    break;
            }
            return childElement;
        }

        #endregion

        /// <summary>
        /// 图片动画
        /// </summary>
        /// <param name="imgSacle"></param>
        private void AnimationImage(DependencyObject imgSacle)
        {
            Duration duration = new Duration(TimeSpan.FromSeconds(1.23));
            Storyboard sb = new Storyboard();

            DoubleAnimation anix = new DoubleAnimation
            {
                From = 1.1,
                To = 1,
                Duration = duration
            };
            DoubleAnimation aniy = new DoubleAnimation
            {
                From = 1.1,
                To = 1,
                Duration = duration
            };

            sb.Children.Add(anix);
            sb.Children.Add(aniy);

            Storyboard.SetTarget(anix, imgSacle);
            Storyboard.SetTarget(aniy, imgSacle);
            Storyboard.SetTargetProperty(anix, "ScaleX");//设置给定目标的动画和属性
            Storyboard.SetTargetProperty(aniy, "ScaleY");

            sb.Begin();

            //https://stackoverflow.com/questions/42959567/change-uwp-flipview
        }
    }
}
