using UwpWallpaper.BingManager;
using UwpWallpaper.Models;
using SqliteManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
using UwpWallpaper.Util;
using UwpWallpaper.ViewModels;
using CommonUtil;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UwpWallpaper
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TodayWallpaper : Page
    {
        //private ObservableCollection<TodayDescription> wallpaperViews;
        private string picid = string.Empty;

        public TodayWallpaperViewModel ViewModel { get; set; }
        public TodayWallpaper()
        {
            this.InitializeComponent();

            //wallpaperViews = new ObservableCollection<TodayDescription>();
        }

        private TodayDescription _description = new TodayDescription();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.MyProgressRing.Visibility = Visibility.Visible;

            WallpaperInfo winfo;
            try
            {
                //页面加载完获取今日壁纸数据
                winfo = await HttpManager.GetWallpaperInfoAsync(picid);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                await new MessageDialog(LangResource.GetString("NetConnectError")).ShowAsync();
                return;
            }

            string url = winfo.PicUrl.GetFullDownloadPicUrl();
            BitmapImage bi = await HttpManager.GetImageOrSave(winfo.WallpaperNo, url);
            if (bi == null)
            {
                await new MessageDialog(LangResource.GetString("NetConnectError")).ShowAsync();
                return;
            }
            this.TodayImage.Source = bi;

            _description.MainDescribe = $"{winfo.Title} - {winfo.CopyRight}";
            _description.AllDescribe = $"{winfo.Title} - {winfo.CopyRight}{Environment.NewLine}" +
                                    $"{winfo.Description}";
            _description.Longitude = winfo.Longitude;
            _description.Latitude = winfo.Latitude;

            this.CoverStackPanel.Width = GetCoverWidth((sender as TodayWallpaper).ActualWidth, (sender as TodayWallpaper).ActualHeight);
            this.CopyrightTextBlock.Text = _description.MainDescribe;

            ToolTip toolTip = new ToolTip();//地图按钮的提示：地点
            toolTip.Content = winfo.Attribute;
            ToolTipService.SetToolTip(this.ShowMapButton, toolTip);

            if (winfo.Latitude == 0 || winfo.Longitude == 0)
            {
                this.ShowMapButton.Visibility = Visibility.Collapsed;
            }

            this.MyProgressRing.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 获取图片下简介的宽度
        /// </summary>
        /// <param name="pagewidth">页面宽度</param>
        /// <param name="pageheight">页面高度</param>
        /// <returns></returns>
        private double GetCoverWidth(double pagewidth, double pageheight)
        {
            double coverwidth = 500;
            if (pagewidth > 0 && pageheight > 0)//这个页面的宽度
            {
                double whp = 1920 / 1080.0;//图片宽高比
                coverwidth = (pagewidth / pageheight) > whp ? whp * pageheight : pagewidth;
            }

            return coverwidth;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            GetPasserInfo(e);
        }

        /// <summary>
        /// 别的页面跳转过来获取数据
        /// </summary>
        private void GetPasserInfo(NavigationEventArgs e)
        {
            string s = (string)e.Parameter;
            if (!string.IsNullOrEmpty(s))
            {
                this.picid = s;
            }
        }


        private void ShowDetailButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ShowDetailButton.Content.ToString() == "\xE70D")
            {
                this.ShowDetailButton.Content = "\xE70E";
                this.CopyrightTextBlock.Text = _description.MainDescribe;
            }
            else
            {
                this.ShowDetailButton.Content = "\xE70D";
                this.CopyrightTextBlock.Text = _description.AllDescribe;
            }
            //RelativePanel.SetAlignTopWith(this.ShowDetailButton, this.CoverStackPanel);
            //TextBlock textBlock = new TextBlock();
            //textBlock.TextWrapping = TextWrapping.Wrap;
            //textBlock.Text = "dsffdffff";
            //textBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 65, 105, 225));
            //RelativePanel.SetAlignRightWith(textBlock, this.TodayImage);
            //RelativePanel.SetBelow(textBlock, this.ShowDetailButton);
            //textBlock.Width = 100;
            ////Thickness margin;
            //textBlock.Margin = new Thickness(5);
            //this.MyRelatviePanel.Children.Add(textBlock);

        }

        private void ShowMapButton_Click(object sender, RoutedEventArgs e)
        {
            this.MyRelatviePanel.Visibility = Visibility.Collapsed;
            this.TodayMap.Visibility = Visibility.Visible;


            DependencyObject marker = new MarkerElement().GetCircleElement();
            this.TodayMap.Children.Add(marker);
            Windows.UI.Xaml.Controls.Maps.MapControl.SetLocation(marker, _description.gpoint);
            this.TodayMap.Center = _description.gpoint;
            this.TodayMap.ZoomLevel = 2;
            this.TodayMap.StartContinuousZoom(1.7);
            this.TodayMap.ZoomLevelChanged += (Windows.UI.Xaml.Controls.Maps.MapControl mapsender, object args) =>
            {
                if (mapsender.ZoomLevel > 9)
                    mapsender.StopContinuousZoom();
            };
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.CoverStackPanel.Width = e.NewSize.Width; //this.TodayImage.ActualWidth;
            Debug.WriteLine(e.NewSize.Width);
        }

        private void BackPicButton_Click(object sender, RoutedEventArgs e)
        {
            this.TodayMap.Visibility = Visibility.Collapsed;
            //this.TodayMap = null;
            this.MyRelatviePanel.Visibility = Visibility.Visible;
        }

        private void TodayImage_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

    public class TodayDescription
    {
        public BitmapImage Image { get; set; }
        public string Copyright { get; set; }

        public string MainDescribe { get; set; }
        public string AllDescribe { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public Windows.Devices.Geolocation.Geopoint gpoint
        {
            get
            {
                Windows.Devices.Geolocation.BasicGeoposition basicposition;
                basicposition.Longitude = this.Longitude;
                basicposition.Latitude = this.Latitude;
                basicposition.Altitude = 0;
                return new Windows.Devices.Geolocation.Geopoint(basicposition);
            }
        }
    }

    public class MarkerElement
    {
        public Canvas GetCircleElement()
        {

            Canvas marker = new Canvas();
            Ellipse outer = new Ellipse() { Width = 25, Height = 25 };
            outer.Fill = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
            outer.Margin = new Thickness(-12.5, -12.5, 0, 0);
            Ellipse inner = new Ellipse() { Width = 20, Height = 20 };
            inner.Fill = new SolidColorBrush(Colors.Black);
            inner.Margin = new Thickness(-10, -10, 0, 0);
            Ellipse core = new Ellipse() { Width = 10, Height = 10 };
            core.Fill = new SolidColorBrush(Colors.White);
            core.Margin = new Thickness(-5, -5, 0, 0);
            marker.Children.Add(outer);
            marker.Children.Add(inner);
            marker.Children.Add(core);
            return marker;
        }
    }

}
